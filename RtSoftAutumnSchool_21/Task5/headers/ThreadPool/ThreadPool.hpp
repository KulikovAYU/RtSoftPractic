#pragma once
#include "Task.hpp"
#include "UnboundedMpmcQueue.hpp"

namespace rt_soft_autumn_school {


	class ThreadPool {

	public:
		static ThreadPool& Instance() {
			static ThreadPool static_thread_pool;
			return static_thread_pool;
		}

		template<typename MyTask, typename...Args>
		static auto Spawn(MyTask&& tsk, Args&&... args)-> std::future<decltype(tsk(args...))> {
			//see https://github.com/mtrebi/thread-pool/blob/master/README.md

			auto& tp = Instance();

			// Create a function with bounded
			//parameters ready to execute
			std::function<decltype(tsk(args...))()> func = std::bind(std::forward<MyTask>(tsk), std::forward<Args>(args)...);

			// Encapsulate it into a shared ptr
			//in order to be able to copy construct / assign
			auto taskPtr = std::make_shared<std::packaged_task<decltype(tsk(args...))()>>(func);

			// Wrap packaged task into void function
			std::function<void()> wrapperFunc = [taskPtr]() {(*taskPtr)(); };

			// Enqueue generic wrapper function
			tp.tasks_queue_.put(wrapperFunc);
			
			tp.m_cvIsBusy.notify_all();

			// Return future from promise
			return taskPtr->get_future();
		}

		
	private:
		// reserve 1 thread as an external
		explicit ThreadPool(size_t nThreadsCnt = (std::thread::hardware_concurrency() - 1)) : continue_work_flag_{ true }
		{
			CreateAndRunWorkers(nThreadsCnt);
		}


		~ThreadPool()
		{
			continue_work_flag_.store(false);
			JoinAllWorkers();
		}

		void CreateAndRunWorkers(size_t nThreadsCnt)
		{
			thread_pool_.reserve(nThreadsCnt);

			for (size_t i = 0; i < nThreadsCnt; ++i)
				thread_pool_.emplace_back([this]() {WorkerRoutine(); });
		}


		void WorkerRoutine()
		{
			while (continue_work_flag_)
			{
				auto task = std::move(tasks_queue_.get());
				task();
			}
		}

		void JoinAllWorkers()
		{
			for (auto& worker : thread_pool_)
				worker.join();
		}

		std::vector<std::thread> thread_pool_;
		UnboundedMpmcQueue<Task> tasks_queue_;
		std::atomic<bool> continue_work_flag_;

		std::mutex m_isBusy;
		std::condition_variable m_cvIsBusy;

	};
}