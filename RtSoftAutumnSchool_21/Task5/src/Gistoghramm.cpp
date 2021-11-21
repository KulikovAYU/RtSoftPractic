#include "Gistoghramm.hpp"
#include <fstream>

void rt_soft_autumn_school::Gistoghramm::FromMessage(const Message& msg)
{
	for (size_t i = 0; i < msg.GetSize(); ++i)
	{
		//get cell in gist
		auto res = static_cast<size_t>(msg[i]);
		size_t spaceCell = (static_cast<size_t>(msg[i]) / ((MAX_PIXEL_VAL / MAX_GIST_SIZE)));
		if (spaceCell >= MAX_GIST_SIZE)
			spaceCell = MAX_GIST_SIZE - 1;

		//take and inc cell value
		size_t& val = m_spacing[spaceCell];
		++val;
	}
}

std::wofstream& rt_soft_autumn_school::operator<<(std::wofstream& out, const Gistoghramm& gist)
{
	for (const auto& itm : gist.m_spacing)
		out << itm << "\t";

	out << "\n";

	return out;
}
