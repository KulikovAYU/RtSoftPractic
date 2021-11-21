#include "Utils.hpp"
#include "HistCalc.hpp"
#include <Task.hpp>
#include "FileWriter.hpp"
#include "ThreadPool.hpp"
#include "GenTimer.hpp"
#include <deque>
#include "ThreadSafeResBuffer.hpp"



//https://stackoverflow.com/questions/12620186/futures-vs-promises
int main(){

	using namespace rt_soft_autumn_school;

	
	TsResBuffer<Gistoghramm> buffer;

	//std::deque <std::promise<Gistoghramm>> tasks;

	Timer tmr(15);
	//std::condition_variable hasMessage;
	//std::mutex sync;

	std::atomic<bool> isFinished = false;
	auto producer = std::thread([&]
		{
			while (tmr.IsWorking())
			{
				//gen message
				auto msg = GenMessage();

				//gen calc gistogramm
				auto future = ThreadPool::Instance().
					Spawn([&msg]() {
					Gistoghramm gst;
					gst.FromMessage(msg);
					return gst; });

				//tasks.push_back(std::move())
			/*	auto promise = std::promise<Gistoghramm>();
				promise.set_value(future.get());*/

				buffer.Push(future.get());
				
			/*	std::unique_lock lock(sync);
				tasks.emplace_back(std::move(promise));
				hasMessage.notify_all();*/
			}

			isFinished = true;
		});



	auto consumer = std::thread([&]
		{
			//Timer selfTmr(10);
			while (true)
			{
				if (!buffer.IsEmpty())
				{
					auto promise = std::move(buffer.Pop());
					auto future = promise.get_future();
					auto result = future.get();
					//ThreadPool::Instance().GetFutures();
					//write to file
					FileWriter::Write(result);


					if (isFinished)
					{
						while (!buffer.IsEmpty())
						{
							auto promise = std::move(buffer.Pop());
							auto future = promise.get_future();
							auto result = future.get();
							//ThreadPool::Instance().GetFutures();
							//write to file
							FileWriter::Write(result);
							
						}
						return;
					}
				}
				
				
			//	selfTmr.Reset(10);
			}
			
		});

	producer.join();
	consumer.join();

	std::cout << "finished" << std::endl;
	return 0;
}