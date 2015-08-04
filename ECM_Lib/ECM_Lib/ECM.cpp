#include <stdio.h>
#include <stdlib.h>
#include <string>
#include <string.h>
#include "ReturnObj.h"

using namespace std;


#define ecc_uint8 unsigned char
#define ecc_uint16 unsigned short
#define ecc_uint32 unsigned


static ecc_uint8 ecc_f_lut[256];
static ecc_uint8 ecc_b_lut[256];
static ecc_uint32 edc_lut[256];




ReturnObj* ECM(char* Source, char* Dest);
ReturnObj* UnECM(char* Source, char* Dest);

FILE *fin, *fout;


typedef void(__stdcall * ProgressCallback)(int);
ProgressCallback progFunction;

extern "C"
{
	
	__declspec(dllexport) ReturnObj* ConvertToECM(char* Source, char* Dest,  ProgressCallback Progression)
	{
		progFunction = Progression;
		return ECM(Source, Dest);
	}

	__declspec(dllexport) ReturnObj* ConvertToIMG(char* Source, char* Dest, ProgressCallback Progression)
	{
		progFunction = Progression;
		return UnECM(Source, Dest);
	}

	__declspec(dllexport) void TryCloseFinStream()
	{
		try
		{
			fclose(fin);
		}
		catch (...) {}
	}
	__declspec(dllexport) void TryCloseFoutStream()
	{
		try
		{
			fclose(fout);
		}
		catch (...) {}
	}
	__declspec(dllexport) void TryCloseFileStream()
	{
		TryCloseFinStream();
		TryCloseFoutStream();
	}

	__declspec(dllexport) int GetIntErr(ReturnObj* p)
	{
		return p->errnum;
	}
	__declspec(dllexport) const char* GetStringErr(ReturnObj* p)
	{
		return p->errstr;
	}
}



static void eccedc_init(void) {
	ecc_uint32 i, j, edc;
	for (i = 0; i < 256; i++) {
		j = (i << 1) ^ (i & 0x80 ? 0x11D : 0);
		ecc_f_lut[i] = j;
		ecc_b_lut[i ^ j] = i;
		edc = i;
		for (j = 0; j < 8; j++) edc = (edc >> 1) ^ (edc & 1 ? 0xD8018001 : 0);
		edc_lut[i] = edc;
	}
}


ecc_uint32 edc_computeblock(ecc_uint32  edc,const ecc_uint8  *src,ecc_uint16  size) {
	while (size--) edc = (edc >> 8) ^ edc_lut[(edc ^ (*src++)) & 0xFF];
	return edc;
}
void edc_computeblock_unECM(const ecc_uint8  *src,ecc_uint16  size,ecc_uint8  *dest) {
	ecc_uint32 edc = edc_computeblock(0, src, size);
	dest[0] = (edc >> 0) & 0xFF;
	dest[1] = (edc >> 8) & 0xFF;
	dest[2] = (edc >> 16) & 0xFF;
	dest[3] = (edc >> 24) & 0xFF;
}

static void ecc_computeblock_UnECM(ecc_uint8 *src,ecc_uint32 major_count,ecc_uint32 minor_count,ecc_uint32 major_mult,ecc_uint32 minor_inc,ecc_uint8 *dest) {
	ecc_uint32 size = major_count * minor_count;
	ecc_uint32 major, minor;
	for (major = 0; major < major_count; major++) {
		ecc_uint32 index = (major >> 1) * major_mult + (major & 1);
		ecc_uint8 ecc_a = 0;
		ecc_uint8 ecc_b = 0;
		for (minor = 0; minor < minor_count; minor++) {
			ecc_uint8 temp = src[index];
			index += minor_inc;
			if (index >= size) index -= size;
			ecc_a ^= temp;
			ecc_b ^= temp;
			ecc_a = ecc_f_lut[ecc_a];
		}
		ecc_a = ecc_b_lut[ecc_f_lut[ecc_a] ^ ecc_b];
		dest[major] = ecc_a;
		dest[major + major_count] = ecc_a ^ ecc_b;
	}

}
static void ecc_generate_UnECM(ecc_uint8 *sector, int zeroaddress)
{
	ecc_uint8 address[4], i;

	if (zeroaddress) for (i = 0; i < 4; i++) {
		address[i] = sector[12 + i];
		sector[12 + i] = 0;
	}
	ecc_computeblock_UnECM(sector + 0xC, 86, 24, 2, 86, sector + 0x81C);

	ecc_computeblock_UnECM(sector + 0xC, 52, 43, 86, 88, sector + 0x8C8);

	if (zeroaddress) for (i = 0; i < 4; i++) sector[12 + i] = address[i];
}
void eccedc_generate(ecc_uint8 *sector, int type) {
	ecc_uint32 i;
	switch (type) {
	case 1:

		edc_computeblock_unECM(sector + 0x00, 0x810, sector + 0x810);

		for (i = 0; i < 8; i++) sector[0x814 + i] = 0;

		ecc_generate_UnECM(sector, 0);
		break;
	case 2:

		edc_computeblock_unECM(sector + 0x10, 0x808, sector + 0x818);

		ecc_generate_UnECM(sector, 1);
		break;
	case 3:

		edc_computeblock_unECM(sector + 0x10, 0x91C, sector + 0x92C);
		break;
	}
}



static int ecc_computeblock_ECM(ecc_uint8 *src,ecc_uint32 major_count,ecc_uint32 minor_count,ecc_uint32 major_mult,ecc_uint32 minor_inc,ecc_uint8 *dest) {
	ecc_uint32 size = major_count * minor_count;
	ecc_uint32 major, minor;
	for (major = 0; major < major_count; major++) {
		ecc_uint32 index = (major >> 1) * major_mult + (major & 1);
		ecc_uint8 ecc_a = 0;
		ecc_uint8 ecc_b = 0;
		for (minor = 0; minor < minor_count; minor++) {
			ecc_uint8 temp = src[index];
			index += minor_inc;
			if (index >= size) index -= size;
			ecc_a ^= temp;
			ecc_b ^= temp;
			ecc_a = ecc_f_lut[ecc_a];
		}
		ecc_a = ecc_b_lut[ecc_f_lut[ecc_a] ^ ecc_b];
		if (dest[major] != (ecc_a)) return 0;
		if (dest[major + major_count] != (ecc_a ^ ecc_b)) return 0;
	}
	return 1;
}


static int ecc_generate_ECM(ecc_uint8 *sector,int zeroaddress,ecc_uint8 *dest) 
{
	int r;
	ecc_uint8 address[4], i;
	if (zeroaddress) for (i = 0; i < 4; i++) {
		address[i] = sector[12 + i];
		sector[12 + i] = 0;
	}
	if (!(ecc_computeblock_ECM(sector + 0xC, 86, 24, 2, 86, dest + 0x81C - 0x81C))) {
		if (zeroaddress) for (i = 0; i < 4; i++) sector[12 + i] = address[i];
		return 0;
	}
	r = ecc_computeblock_ECM(sector + 0xC, 52, 43, 86, 88, dest + 0x8C8 - 0x81C);
	if (zeroaddress) for (i = 0; i < 4; i++) sector[12 + i] = address[i];
	return r;
}

int check_type(unsigned char *sector, int canbetype1) {
	int canbetype2 = 1;
	int canbetype3 = 1;
	ecc_uint32 myedc;
	/* Check for mode 1 */
	if (canbetype1) {
		if (
			(sector[0x00] != 0x00) ||
			(sector[0x01] != 0xFF) ||
			(sector[0x02] != 0xFF) ||
			(sector[0x03] != 0xFF) ||
			(sector[0x04] != 0xFF) ||
			(sector[0x05] != 0xFF) ||
			(sector[0x06] != 0xFF) ||
			(sector[0x07] != 0xFF) ||
			(sector[0x08] != 0xFF) ||
			(sector[0x09] != 0xFF) ||
			(sector[0x0A] != 0xFF) ||
			(sector[0x0B] != 0x00) ||
			(sector[0x0F] != 0x01) ||
			(sector[0x814] != 0x00) ||
			(sector[0x815] != 0x00) ||
			(sector[0x816] != 0x00) ||
			(sector[0x817] != 0x00) ||
			(sector[0x818] != 0x00) ||
			(sector[0x819] != 0x00) ||
			(sector[0x81A] != 0x00) ||
			(sector[0x81B] != 0x00)
			) {
			canbetype1 = 0;
		}
	}
	/* Check for mode 2 */
	if (
		(sector[0x0] != sector[0x4]) ||
		(sector[0x1] != sector[0x5]) ||
		(sector[0x2] != sector[0x6]) ||
		(sector[0x3] != sector[0x7])
		) {
		canbetype2 = 0;
		canbetype3 = 0;
		if (!canbetype1) return 0;
	}

	/* Check EDC */
	myedc = edc_computeblock(0, sector, 0x808);
	if (canbetype2) if (
		(sector[0x808] != ((myedc >> 0) & 0xFF)) ||
		(sector[0x809] != ((myedc >> 8) & 0xFF)) ||
		(sector[0x80A] != ((myedc >> 16) & 0xFF)) ||
		(sector[0x80B] != ((myedc >> 24) & 0xFF))
		) {
		canbetype2 = 0;
	}
	myedc = edc_computeblock(myedc, sector + 0x808, 8);
	if (canbetype1) if (
		(sector[0x810] != ((myedc >> 0) & 0xFF)) ||
		(sector[0x811] != ((myedc >> 8) & 0xFF)) ||
		(sector[0x812] != ((myedc >> 16) & 0xFF)) ||
		(sector[0x813] != ((myedc >> 24) & 0xFF))
		) {
		canbetype1 = 0;
	}
	myedc = edc_computeblock(myedc, sector + 0x810, 0x10C);
	if (canbetype3) if (
		(sector[0x91C] != ((myedc >> 0) & 0xFF)) ||
		(sector[0x91D] != ((myedc >> 8) & 0xFF)) ||
		(sector[0x91E] != ((myedc >> 16) & 0xFF)) ||
		(sector[0x91F] != ((myedc >> 24) & 0xFF))
		) {
		canbetype3 = 0;
	}
	if (canbetype1) { if (!(ecc_generate_ECM(sector, 0, sector + 0x81C))) { canbetype1 = 0; } }
	if (canbetype2) { if (!(ecc_generate_ECM(sector - 0x10, 1, sector + 0x80C))) { canbetype2 = 0; } }
	if (canbetype1) return 1;
	if (canbetype2) return 2;
	if (canbetype3) return 3;
	return 0;
}


void write_type_count(FILE *out,unsigned type,unsigned count) {
	count--;
	fputc(((count >= 32) << 7) | ((count & 31) << 2) | type, out);
	count >>= 5;
	while (count) {
		fputc(((count >= 128) << 7) | (count & 127), out);
		count >>= 7;
	}
}



unsigned mycounter;
unsigned mycounter_analyze;
unsigned mycounter_encode;
unsigned mycounter_total;


void resetcounter(unsigned total) {
	mycounter = 0;
	mycounter_analyze = 0;
	mycounter_encode = 0;
	mycounter_total = total;
}
void setcounter(unsigned n) {
	if ((n >> 20) != (mycounter >> 20)) {
		unsigned a = (n + 64) / 128;
		unsigned d = (mycounter_total + 64) / 128;
		if (!d) d = 1;
		if (progFunction)
			progFunction((int)((100 * a) / d));
	}
	mycounter = n;
}
void setcounter_analyze(unsigned n) {
	if ((n >> 20) != (mycounter_analyze >> 20)) {
		unsigned a = (n + 64) / 128;
		unsigned e = (mycounter_encode + 64) / 128;
		unsigned d = (mycounter_total + 64) / 128;
		if (!d) d = 1;


		if (progFunction)
			progFunction((int)((100 * e) / d));


	}
	mycounter_analyze = n;
}
void setcounter_encode(unsigned n) {
	if ((n >> 20) != (mycounter_encode >> 20)) {
		unsigned a = (mycounter_analyze + 64) / 128;
		unsigned e = (n + 64) / 128;
		unsigned d = (mycounter_total + 64) / 128;
		if (!d) d = 1;


		if (progFunction)
			progFunction((int)((100 * e) / d));


	}
	mycounter_encode = n;
}


unsigned in_flush(unsigned edc,unsigned type,unsigned count,FILE *in,FILE *out) {
	unsigned char buf[2352];
	write_type_count(out, type, count);
	if (!type) {
		while (count) {
			unsigned b = count;
			if (b > 2352) b = 2352;
			fread(buf, 1, b, in);
			edc = edc_computeblock(edc, buf, b);
			fwrite(buf, 1, b, out);
			count -= b;
			setcounter_encode(ftell(in));
		}
		return edc;
	}
	while (count--) {
		switch (type) {
		case 1:
			fread(buf, 1, 2352, in);
			edc = edc_computeblock(edc, buf, 2352);
			fwrite(buf + 0x00C, 1, 0x003, out);
			fwrite(buf + 0x010, 1, 0x800, out);
			setcounter_encode(ftell(in));
			break;
		case 2:
			fread(buf, 1, 2336, in);
			edc = edc_computeblock(edc, buf, 2336);
			fwrite(buf + 0x004, 1, 0x804, out);
			setcounter_encode(ftell(in));
			break;
		case 3:
			fread(buf, 1, 2336, in);
			edc = edc_computeblock(edc, buf, 2336);
			fwrite(buf + 0x004, 1, 0x918, out);
			setcounter_encode(ftell(in));
			break;
		}
	}
	return edc;
}

unsigned char inputqueue[1048576 + 4];

ReturnObj* ecmify(FILE *in, FILE *out) {
	unsigned inedc = 0;
	int curtype = -1;
	int curtypecount = 0;
	int curtype_in_start = 0;
	int detecttype;
	int incheckpos = 0;
	int inbufferpos = 0;
	int intotallength;
	int inqueuestart = 0;
	int dataavail = 0;
	int typetally[4];
	fseek(in, 0, SEEK_END);
	intotallength = ftell(in);
	resetcounter(intotallength);
	typetally[0] = 0;
	typetally[1] = 0;
	typetally[2] = 0;
	typetally[3] = 0;
	fputc('E', out);
	fputc('C', out);
	fputc('M', out);
	fputc(0x00, out);
	for (;;) {

		if ((dataavail < 2352) && (dataavail < (intotallength - inbufferpos))) {
			int willread = intotallength - inbufferpos;
			if (willread >((sizeof(inputqueue) - 4) - dataavail)) willread = (sizeof(inputqueue) - 4) - dataavail;
			if (inqueuestart) {
				memmove(inputqueue + 4, inputqueue + 4 + inqueuestart, dataavail);
				inqueuestart = 0;
			}
			if (willread) {
				setcounter_analyze(inbufferpos);
				fseek(in, inbufferpos, SEEK_SET);
				fread(inputqueue + 4 + dataavail, 1, willread, in);
				inbufferpos += willread;
				dataavail += willread;
			}
		}
		if (dataavail <= 0) break;
		if (dataavail < 2336) {
			detecttype = 0;
		}
		else {
			detecttype = check_type(inputqueue + 4 + inqueuestart, dataavail >= 2352);
		}
		if (detecttype != curtype) {
			if (curtypecount) {
				fseek(in, curtype_in_start, SEEK_SET);
				typetally[curtype] += curtypecount;
				inedc = in_flush(inedc, curtype, curtypecount, in, out);
			}
			curtype = detecttype;
			curtype_in_start = incheckpos;
			curtypecount = 1;
		}
		else {
			curtypecount++;
		}
		switch (curtype) {
		case 0: incheckpos += 1; inqueuestart += 1; dataavail -= 1; break;
		case 1: incheckpos += 2352; inqueuestart += 2352; dataavail -= 2352; break;
		case 2: incheckpos += 2336; inqueuestart += 2336; dataavail -= 2336; break;
		case 3: incheckpos += 2336; inqueuestart += 2336; dataavail -= 2336; break;
		}
	}

	if (curtypecount) {
		fseek(in, curtype_in_start, SEEK_SET);
		typetally[curtype] += curtypecount;
		inedc = in_flush(inedc, curtype, curtypecount, in, out);
	}
	/* End-of-records indicator */
	write_type_count(out, 0, 0);
	/* Input file EDC */
	fputc((inedc >> 0) & 0xFF, out);
	fputc((inedc >> 8) & 0xFF, out);
	fputc((inedc >> 16) & 0xFF, out);
	fputc((inedc >> 24) & 0xFF, out);
	return new ReturnObj(0, "");
}

ReturnObj* ECM(char* Source, char* Dest) {
	
	char buff[10];
	eccedc_init();

	

	errno_t errorCodefin = fopen_s(&fin, Source,"rb");

	if (errorCodefin!=0) {
		string ss;
		ss.append("Input: err n ");
		_itoa_s(errorCodefin, buff, 2);
		ss.append(buff);
		return new ReturnObj(-1,ss.c_str());
	}
	errno_t errorCodefout = fopen_s(&fout, Dest, "wb");
	if (errorCodefout!=0) {
		string ss;
		ss.append("Output: err n ");
		_itoa_s(errorCodefin, buff, 2);
		ss.append(buff);

		TryCloseFileStream();
		return new ReturnObj(-1, ss.c_str());
	}

	ReturnObj* o =ecmify(fin, fout);

	TryCloseFileStream();
	progFunction = NULL;

	return o;
}


ReturnObj* unecmify(FILE *in, FILE *out) {
	unsigned checkedc = 0;
	unsigned char sector[2352];
	unsigned type;
	unsigned num;
	fseek(in, 0, SEEK_END);
	resetcounter(ftell(in));
	fseek(in, 0, SEEK_SET);
	if (
		(fgetc(in) != 'E') ||
		(fgetc(in) != 'C') ||
		(fgetc(in) != 'M') ||
		(fgetc(in) != 0x00)
		) {
		goto corrupt;
	}
	for (;;) {
		int c = fgetc(in);
		int bits = 5;
		if (c == EOF) goto uneof;
		type = c & 3;
		num = (c >> 2) & 0x1F;
		while (c & 0x80) {
			c = fgetc(in);
			if (c == EOF) goto uneof;
			num |= ((unsigned)(c & 0x7F)) << bits;
			bits += 7;
		}
		if (num == 0xFFFFFFFF) break;
		num++;
		if (num >= 0x80000000) goto corrupt;
		if (!type) {
			while (num) {
				int b = num;
				if (b > 2352) b = 2352;
				if (fread(sector, 1, b, in) != b) goto uneof;
				checkedc = edc_computeblock(checkedc, sector, b);
				fwrite(sector, 1, b, out);
				num -= b;
				setcounter(ftell(in));
			}
		}
		else {
			while (num--) {
				memset(sector, 0, sizeof(sector));
				memset(sector + 1, 0xFF, 10);
				switch (type) {
				case 1:
					sector[0x0F] = 0x01;
					if (fread(sector + 0x00C, 1, 0x003, in) != 0x003) goto uneof;
					if (fread(sector + 0x010, 1, 0x800, in) != 0x800) goto uneof;
					eccedc_generate(sector, 1);
					checkedc = edc_computeblock(checkedc, sector, 2352);
					fwrite(sector, 2352, 1, out);
					setcounter(ftell(in));
					break;
				case 2:
					sector[0x0F] = 0x02;
					if (fread(sector + 0x014, 1, 0x804, in) != 0x804) goto uneof;
					sector[0x10] = sector[0x14];
					sector[0x11] = sector[0x15];
					sector[0x12] = sector[0x16];
					sector[0x13] = sector[0x17];
					eccedc_generate(sector, 2);
					checkedc = edc_computeblock(checkedc, sector + 0x10, 2336);
					fwrite(sector + 0x10, 2336, 1, out);
					setcounter(ftell(in));
					break;
				case 3:
					sector[0x0F] = 0x02;
					if (fread(sector + 0x014, 1, 0x918, in) != 0x918) goto uneof;
					sector[0x10] = sector[0x14];
					sector[0x11] = sector[0x15];
					sector[0x12] = sector[0x16];
					sector[0x13] = sector[0x17];
					eccedc_generate(sector, 3);
					checkedc = edc_computeblock(checkedc, sector + 0x10, 2336);
					fwrite(sector + 0x10, 2336, 1, out);
					setcounter(ftell(in));
					break;
				}
			}
		}
	}
	if (fread(sector, 1, 4, in) != 4) goto uneof;
	if ((sector[0] != ((checkedc >> 0) & 0xFF)) ||	(sector[1] != ((checkedc >> 8) & 0xFF)) ||	(sector[2] != ((checkedc >> 16) & 0xFF)) ||	(sector[3] != ((checkedc >> 24) & 0xFF))	) 
	{
		goto corrupt;
	}
	return new ReturnObj(0, "");
uneof:
	return new ReturnObj(-1, "EOF Inaspettata");
corrupt:
	return new ReturnObj(-1, "ECM file Corrotto");
}

ReturnObj* UnECM(char* Source, char* Dest) {


	eccedc_init();
	char buff[10];

	errno_t errorCodefin = fopen_s(&fin, Source, "rb");

	if (errorCodefin != 0) {
		string ss;
		ss.append("Input: err n ");
		_itoa_s(errorCodefin, buff, 2);
		ss.append(buff);
		return new ReturnObj(-1, ss.c_str());
	}


	errno_t errorCodefout = fopen_s(&fout, Dest, "wb");
	if (errorCodefout != 0) {
		string ss;
		ss.append("Output: err n ");
		_itoa_s(errorCodefin, buff, 2);
		ss.append(buff);

		TryCloseFileStream();
		return new ReturnObj(-1, ss.c_str());
	}

	ReturnObj* o=unecmify(fin, fout);

	TryCloseFileStream();
	progFunction = NULL;
	return o;
}
