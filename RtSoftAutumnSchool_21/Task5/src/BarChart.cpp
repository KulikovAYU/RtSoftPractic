#include "BarChart.hpp"
#include "Message.hpp"
#include <fstream>


namespace rt_soft_autumn_school {

	void BarChart::FromMessage(const Message& msg)
	{
		for (size_t i = 0; i < msg.GetSize(); ++i){
			//get cell in gist
			auto res = static_cast<size_t>(msg[i]);
			size_t spaceCell = (static_cast<size_t>(msg[i]) / ((MAX_SIZE / MAX_BAR_CHART_SIZE)));
			if (spaceCell >= MAX_BAR_CHART_SIZE)
				spaceCell = MAX_BAR_CHART_SIZE - 1;

			//take and inc cell value
			size_t& val = m_spacing[spaceCell];
			++val;
		}
	}

	std::wofstream& operator<<(std::wofstream& out, const BarChart& gist)
	{
		for (const auto& itm : gist.m_spacing)
			out << itm << "\t";

		out << "\n";

		return out;
	}
}