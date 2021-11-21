#pragma once
#include <Picture.hpp>
#include <random>


namespace rt_soft_autumn_school {
	//helps to generate random picture
	Picture GetRandPict();

	/// <summary>
	/// devide src image on chunks
	/// </summary>
	/// <param name="pSrcPict">Source picture</param>
	/// <param name="cntPxlsInChunk">count pixels in required chunk</param>
	/// <returns></returns>
	std::vector<ImageField> GetPixelChunks(const Picture& pSrcPict, size_t cntPxlsInChunk);


	Message GenMessage();

	std::vector<Interval> SplitMessage(const Message& srcMsg, size_t cntMsgChunks);
	
	
	
}
