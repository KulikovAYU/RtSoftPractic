#pragma once
#include <iostream>

namespace rt_soft_autumn_school { class Gistoghramm; }



namespace rt_soft_autumn_school {

	class FileWriter {
	public:

		static bool Write(const Gistoghramm& contentStream, const std::wstring& strFilePath = L"gistogramm.txt");
	};



}