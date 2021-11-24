#pragma once
#include "UnboundedMpmcQueue.hpp"
#include <functional>
#include <future>

namespace rt_soft_autumn_school {

	using Task = std::function<void()>;

	class ThreadPool
	{
	public:
		static ThreadPool& Instance() {
			static ThreadPool static_thread_pool;
			return static_thread_pool;
		}

		template<typename MyTask, typename...Args>
		auto Spawn(MyTask&& tsk, Args&&... args)-> std::future<decltype(tsk(args...))> {
			
			//this idea takes from: https://github.com/mtrebi/thread-pool/blob/master/README.md
			// Create a function with bounded
			//parameters ready to execute
			std::function<decltype(tsk(args...))()> func = std::bind(std::forward<MyTask>(tsk), std::forward<Args>(args)...);

			// Encapsulate it into a shared ptr
			//in order to be able to copy construct / assign
			auto taskPtr = std::make_shared<std::packaged_task<decltype(tsk(args...))()>>(func);

			// Wrap packaged task into void function
			std::function<void()> wrapperFunc = [taskPtr]() {(*taskPtr)(); };

			// Enqueue generic wrapper function
			m_tasksQueue.put(wrapperFunc);
			
			// Return future from promise
			return taskPtr->get_future();
		}

		
	private:
		// reserve 1 thread as an external
		explicit ThreadPool(size_t nThreadsCnt = (std::thread::hardware_concurrency() - 1)) : m_continueWorkFlag(true){
			CreateAndRunWorkers(nThreadsCnt);
		}


		~ThreadPool(){
			m_continueWorkFlag.store(false);
			JoinAllWorkers();
		}

		void CreateAndRunWorkers(size_t nThreadsCnt){
			m_threadPool.reserve(nThreadsCnt);

			for (size_t i = 0; i < nThreadsCnt; ++i)
				m_threadPool.emplace_back([this]() {WorkerRoutine(); });
		}


		void WorkerRoutine(){
			while (m_continueWorkFlag)
			{
				auto task = std::move(m_tasksQueue.get());
				task();
			}
		}

		void JoinAllWorkers(){
			for (auto& worker : m_threadPool)
				worker.join();
		}

		std::vector<std::thread> m_threadPool;
		UnboundedMpmcQueue<Task> m_tasksQueue;
		std::atomic<bool> m_continueWorkFlag;

	};
}