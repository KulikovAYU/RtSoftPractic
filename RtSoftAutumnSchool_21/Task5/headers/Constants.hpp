#pragma once
#include <stdint.h>
#include <utility>
#include <vector>

namespace rt_soft_autumn_school {

	struct Interval {
		size_t m_min;
		size_t m_max;
	};

	static const size_t MAX_BAR_CHART_SIZE = 10;

	static const size_t MAX_SIZE = 255;
}