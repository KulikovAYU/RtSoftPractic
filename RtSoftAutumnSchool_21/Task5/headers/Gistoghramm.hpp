#pragma once
#include "Constants.hpp"
#include "Picture.hpp"
#include <iosfwd>


namespace rt_soft_autumn_school {
	class Gistoghramm {

	public:
		//void CalcPixel(uint8_t pxVal){
		//
		//	//get cell in gist
		//	std::atomic<size_t> spaceCell = (static_cast<size_t>(pxVal) / ((MAX_PIXEL_VAL / MAX_GIST_SIZE)));
		//	//take and inc cell value
		//	std::atomic<size_t>& val = m_spacing[spaceCell];
		//	++val;
		//}

		size_t& operator[](size_t cell) {
			return m_spacing[cell];
		}

		void FromMessage(const Message& msg);

		friend std::wofstream& operator<< (std::wofstream& out, const Gistoghramm& gist);

	private:
		size_t m_spacing[MAX_GIST_SIZE] = {0}; //spacings 0..24, 25..49, 50..74 etc
	};


	std::wofstream& operator<< (std::wofstream& out, const Gistoghramm& gist);


}