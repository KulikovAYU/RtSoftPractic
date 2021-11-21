#include "FileWriter.hpp"
#include <fstream>
#include <codecvt>
#include <iosfwd>
#include "Gistoghramm.hpp"


bool rt_soft_autumn_school::FileWriter::Write(const Gistoghramm& contentStream, const std::wstring& strFilePath /*= L"gistogramm.txt"*/)
{
	std::wofstream out(strFilePath, std::wofstream::app);
	out.imbue(std::locale(std::locale::empty(), new std::codecvt_utf8<wchar_t>));

	try
	{
		if (out.is_open())
		{
			out << contentStream;
			out.close();
			return true;
		}

	}
	catch (const std::exception&)
	{
		out.close();
	}
	return false;
}
