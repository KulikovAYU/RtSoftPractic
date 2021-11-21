#pragma once
#include <stdint.h>
#include <utility>
#include <vector>

namespace rt_soft_autumn_school {

	struct Interval{
		size_t m_min;
		size_t m_max;
	};

	struct ImageField{
		Interval m_horz;
		Interval m_vert;
	};


	#ifdef _DEBUG
	static const size_t WIDTH = 5;
	static const size_t HEIGHT = 5;
	#else
	static const size_t WIDTH = 255;
	static const size_t HEIGHT = 255;
	#endif // DEBUG


	static const size_t MAX_PIXEL_VAL = 255;
	static const size_t MAX_GIST_SIZE = 10;

	static const size_t MAX_SIZE = 255;
}