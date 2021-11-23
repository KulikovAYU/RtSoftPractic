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
		//  thread pool returns future, and future
		//  is emplaced into thread safe buffer which contains futures for writer thread
		//3. consumer - takes(wait) futures from thread safe buffer and write into file
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

					auto task = [&msg]() { BarChart gst; gst.FromMessage(msg); return gst; };
					auto future = ThreadPool::Instance().Spawn(task);
					threadSafeBuffer.Push(std::move(future));
				}

				isFinished = true;
			});


		auto consumer = std::thread([&]
			{
				while (true){

					while(!threadSafeBuffer.IsEmpty() && !isFinished){

						auto future = threadSafeBuffer.Pop();
						auto result = future.get(); //wait
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