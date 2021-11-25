#pragma once
#include <Message.hpp>
#include <chrono>
#include <ctime>
#include <string>

namespace rt_soft_autumn_school {

	class BarChart;


	class Timer final{

	public:
		Timer(size_t seconds);

		void Start();
		void Stop();
		bool IsWorking();
		void Reset(size_t seconds);

	private:
		std::chrono::time_point<std::chrono::steady_clock> m_StartTime = std::chrono::steady_clock::now();
		size_t m_stoppedSeconds;
		bool m_bIsRunning = false;
	};


	class FileWriter final {

	public:
		static bool Write(const BarChart& contentStream, const std::string& strFilePath = "BarChartResult.txt");
	};


	Message GenMessage();
	std::vector<Interval> SplitMessage(const Message& srcMsg, size_t cntMsgChunks);
}