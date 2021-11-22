#pragma once
#include <mutex>
#include <deque>

namespace rt_soft_autumn_school {

	template<typename T>
	class UnboundedMpmcQueue
	{
	public:

		// produce  data to the worker thread
		void put(T task)
		{
			std::unique_lock<std::mutex> lock(m_isBusy);
			m_buffer.push_back(std::move(task));
			m_notEmpty.notify_all();
		}

		// consume data to the worker thread
		T get()
		{
			std::unique_lock<std::mutex> lock(m_isBusy);
		
			while (m_buffer.empty())
				m_notEmpty.wait(lock);//wait for pushing task

			return take_locked();
		}

	private:
		T take_locked()
		{
			T front = std::move(m_buffer.front());
			m_buffer.pop_front();

			return front;
		}


		std::deque<T> m_buffer;
		std::condition_variable m_notEmpty;
		std::mutex m_isBusy;
	};
}