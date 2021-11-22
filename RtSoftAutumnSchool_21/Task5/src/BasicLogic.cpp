#include "BasicLogic.hpp"
#include "Utils.hpp"
#include "ThreadSafeResBuffer.hpp"
#include "BarChart.hpp"
#include "ThreadPool.hpp"
#include <iostream>


namespace rt_soft_autumn_school {


	void BasicLogic::Run()
	{

		//let's create threads:
		//1. producer - produces data during 60 sec
		//   and sends task to thread pool
		//2. thread pool - which working calculating BarChart.
		//   if task has been completed thread pool returns future, and result of future
		//   is emplaced into thread safe buffer which contains promises for writer thread
		//3. consumer - takes promises from thread safe buffer and write into file
		//If timer will stop we finish all tasks correctly


		const size_t MESSAGING_MODEL_TIME = 60; //sec
		
		Timer tmr(MESSAGING_MODEL_TIME);
		std::atomic<bool> isFinished = false;
		TsResBuffer<BarChart> threadSafeBuffer;

		auto producer = std::thread([&]
			{
				while (tmr.IsWorking())
				{
					//gen message
					auto msg = GenMessage();

					auto callback = [&msg]() { BarChart gst; gst.FromMessage(msg); return gst; };
					auto future = ThreadPool::Instance().Spawn(callback);
					threadSafeBuffer.Push(future.get());
				}

				isFinished = true;
			});


		auto consumer = std::thread([&]
			{
				while (true){

					while(!threadSafeBuffer.IsEmpty() && !isFinished){

						auto promise = std::move(threadSafeBuffer.Pop());
						auto future = promise.get_future();
						auto result = future.get();
						//write to file
						FileWriter::Write(result);
					}

					if (isFinished)
						return;
				}
			});

		producer.join();
		consumer.join();

		std::cout << "BarChart writing was finished" << std::endl;
	}

}