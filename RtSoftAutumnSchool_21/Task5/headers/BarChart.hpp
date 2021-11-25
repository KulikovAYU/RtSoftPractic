#pragma once
#include "Constants.hpp"
#include <iosfwd>


namespace rt_soft_autumn_school {

	struct Message;

	class BarChart {

	public:
		void FromMessage(const Message& msg);

		friend std::ofstream& operator<< (std::ofstream& out, const BarChart& gist);

	private:
		size_t m_spacing[MAX_BAR_CHART_SIZE] = { 0 }; //spacings 0..24, 25..49, 50..74 etc
	};


	std::ofstream& operator<< (std::ofstream& out, const BarChart& gist);
}