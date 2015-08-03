#pragma once
#include <string>
class ReturnObj
{
public:
	int errnum = 0;
	char errstr[20];

	ReturnObj(int errnum, const char* errstr)
	{
		this->errnum = errnum;
		strcpy_s(this->errstr, errstr);
	}
	~ReturnObj();
};

