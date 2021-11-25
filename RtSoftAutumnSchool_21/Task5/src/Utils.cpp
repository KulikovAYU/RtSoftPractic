#include "Utils.hpp"
#include <random>
#include <fstream>
#include <codecvt>
#include <Constants.hpp>
#include "BarChart.hpp"

namespace rt_soft_autumn_school {

	Message GenMessage()
	{
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
				
				interval.emplace_back(Interval{nStart, i});
				nStart = i + 1;
			}

			if (i == srcMsg.GetSize() - 1 && (i % cntMsgChunks != 0))
			{
				interval.emplace_back(Interval{nStart, i});
				break;
			}
		}

		return interval;
	}

	Timer::Timer(size_t seconds) : m_stoppedSeconds{ seconds }
	{

	}

	void Timer::Start()
	{
		m_StartTime = std::chrono::steady_clock::now();
		m_bIsRunning = true;
	}

	void Timer::Stop()
	{
		m_bIsRunning = false;
	}

	bool Timer::IsWorking()
	{
		m_bIsRunning = m_stoppedSeconds > static_cast<size_t>(std::chrono::duration_cast<std::chrono::seconds>(std::chrono::steady_clock::now() - m_StartTime).count());

		return m_bIsRunning;
	}

	void Timer::Reset(size_t seconds)
	{
		m_stoppedSeconds = seconds;
		m_bIsRunning = true;
	}

	bool FileWriter::Write(const BarChart& contentStream, const std::string& strFilePath /*= L"BarChartResult.txt"*/)
	{
		std::ofstream out(strFilePath, std::wofstream::app);

		try
		{
			if (out.is_open())
			{
				out << contentStream;
				out.close();
				return true;
			}
		}
		catch (const std::exception&)
		{
			out.close();
		}
		return false;
	}
}