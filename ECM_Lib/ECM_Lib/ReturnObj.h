#pragma once
#include <string>
class ReturnObj
{
	int errnum = 0;
	char errstr[20];
public:
	ReturnObj(int errnum, char errstr[20])
	{
		this->errnum = errnum;
		strcpy_s(this->errstr, errstr);
	}
	~ReturnObj();
};

