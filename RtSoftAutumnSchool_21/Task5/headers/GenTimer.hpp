#pragma once
#include <chrono>
#include <ctime>

namespace rt_soft_autumn_school {

	class Timer {

	public:
		Timer(size_t seconds) : m_stoppedSeconds{ seconds }
		{}

		void Start() {
			m_StartTime = std::chrono::steady_clock::now();
			m_bIsRunning = true;
		}

		void Stop(){
			m_bIsRunning = false;
		}

		bool IsWorking(){
			m_bIsRunning = m_stoppedSeconds >static_cast<size_t>(std::chrono::duration_cast<std::chrono::seconds>(std::chrono::steady_clock::now() - m_StartTime).count());

			return m_bIsRunning;
		}


		void Reset(size_t seconds) {
			m_stoppedSeconds = seconds;
			m_bIsRunning = true;
		}
	private:
		std::chrono::time_point<std::chrono::steady_clock> m_StartTime = std::chrono::steady_clock::now();


		size_t m_stoppedSeconds;
		bool m_bIsRunning = false;
	};


}