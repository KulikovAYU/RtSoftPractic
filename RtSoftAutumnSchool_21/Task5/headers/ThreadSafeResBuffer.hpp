#pragma once
#include <mutex>
#include <deque>
#include <future>

namespace rt_soft_autumn_school {

	template<typename T>
	class TsResBuffer {

	public:
		void Push(std::future<T> futureRes) {

			std::unique_lock<std::mutex> lock(m_sync);
			tasks.emplace_back(std::move(futureRes));
			m_hasMessage.notify_all();
		}

		std::future<T> Pop() {
		
			std::unique_lock<std::mutex> lock(m_sync);

			while (tasks.empty())
				m_hasMessage.wait(lock);

			std::future<T> val = std::move(tasks.front());
			tasks.pop_front();
			return val;
		}

		bool IsEmpty(){
			std::unique_lock<std::mutex> lock(m_sync);
			return tasks.empty();
		}

	private:
		std::condition_variable m_hasMessage;
		std::mutex m_sync;

		std::deque <std::future<T>> tasks;
	};
}