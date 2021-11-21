#include "Utils.hpp"

namespace rt_soft_autumn_school {
	Picture GetRandPict()
	{
		Picture proxyPict;

		std::random_device device;
		std::mt19937  rng(device());

		std::uniform_int_distribution<size_t> uniform_dist(0, 255);

		//fill rand pixels
		for (int w = 0; w < WIDTH; ++w)
		{
			for (int h = 0; h < HEIGHT; ++h)
			{
				size_t value = uniform_dist(rng);
				proxyPict[w][h] = static_cast<uint8_t>(value);
			}
		}

		return proxyPict;
	}

	std::vector<rt_soft_autumn_school::ImageField> GetPixelChunks(const Picture& pSrcPict, size_t cntPxlsInChunk)
	{
		std::vector<ImageField> chunks;

		size_t nWidth = pSrcPict.GetW();
		size_t nHeight = pSrcPict.GetH();
		size_t step = cntPxlsInChunk - 1;

		for (size_t j = 0; j < nWidth; j += step + 1)
		{
			for (size_t i = 0; i < nHeight; ++i)
			{
				Interval itnV{ i, i };
				Interval itnH{ j, j };

				size_t iEnd = i + step;
				size_t jEnd = j + step;

				bool bIsLast = false;
				if (iEnd >= nHeight) {
					iEnd = nHeight - 1;
					bIsLast = true;
				}
				else {
					i += step;
				}

				if (jEnd >= nWidth)
					jEnd = nWidth - 1;

				itnV.m_max = iEnd;
				itnH.m_max = jEnd;

				chunks.push_back(ImageField{ itnV , itnH });

				if (bIsLast) {
					i = 0;
					break;
				}
			}
		}

		return chunks;
	}

	Message GenMessage(){
		Message msg;

		std::random_device device;
		std::mt19937  rng(device());

		std::uniform_int_distribution<size_t> uniform_dist(0, 255);

		//fill rand pixels
		for (size_t i = 0; i < MAX_SIZE; ++i)
			msg[i] = static_cast<uint8_t>(uniform_dist(rng));

		return msg;
	}

	std::vector<Interval> SplitMessage(const Message& srcMsg, size_t cntMsgChunks)
	{
		std::vector<Interval> interval;
		size_t nParts = (srcMsg.GetSize() % cntMsgChunks == 0) ?
			(srcMsg.GetSize() / cntMsgChunks) :
			(srcMsg.GetSize() / cntMsgChunks) + 1;

		interval.reserve(nParts);

		size_t nStart = 0;
		for (size_t i = nStart; i < srcMsg.GetSize(); ++i)
		{
			if (i != 0 && (i % cntMsgChunks == 0))
			{
				interval.emplace_back(nStart, i);
				nStart = i + 1;
			}

			if (i == srcMsg.GetSize() - 1 && (i % cntMsgChunks != 0))
			{
				interval.emplace_back(nStart, i);
				break;
			}
		}

		return interval;
	}

}

