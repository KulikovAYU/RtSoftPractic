#pragma once
#include "Gistoghramm.hpp"


namespace rt_soft_autumn_school {

	/*class HistCalc {

	public:
		HistCalc(Picture* pSrcPict) : m_pSrcPict{ pSrcPict } {}

		void operator ()(ImageField fld) {
			const Interval& row = fld.m_horz;
			const Interval& col = fld.m_vert;
			Picture& fragment = (*m_pSrcPict);

			for (auto r = row.m_min; r < row.m_max; ++r)
			{
				for (auto c = col.m_min; c < col.m_max; ++c)
					m_gist.CalcPixel(fragment[r][c]);
			}
		}

		Gistoghramm* GetGistoghramm() noexcept { return &m_gist; }
		const Gistoghramm* GetGistoghramm() const noexcept { return &m_gist; }
	private:
		Picture* m_pSrcPict;
		Gistoghramm m_gist;
	};*/



	
}