#include <gtest/gtest.h>
#include "ThreadSafeBuffer.hpp"


using namespace rt_soft_autumn_school;


struct StubObject {

	StubObject(size_t x_) : x{ x_ }
	{}
	
	size_t x = 0;
};

TEST(SingleThread, Enqueue_single_test)
{
	CircularThreadSafeBuffer<StubObject, 1> buffer;
	const StubObject& obj = 10;
	buffer.Enqueue(obj);

	EXPECT_TRUE(buffer.Get().x == 10);
}

TEST(SingleThread, Enqueue_million_test)
{
	static constexpr size_t MAX_SIZE = 1000001;
	CircularThreadSafeBuffer<StubObject, MAX_SIZE> buffer;
	for (size_t i = 0; i < MAX_SIZE; i++) 
		buffer.Enqueue(StubObject(i));

	for (size_t i = 0; i < MAX_SIZE; i++)
		EXPECT_TRUE(buffer.Get().x == i);
}

TEST(SingleThread, Enqueue_circle_test)
{
	static constexpr size_t MAX_SIZE = 1;
	CircularThreadSafeBuffer<StubObject, MAX_SIZE> buffer;
	for (size_t i = 0; i < MAX_SIZE + 1; ++i)
		buffer.Enqueue(StubObject(i));

	
	EXPECT_TRUE(buffer.Get().x == 1);
}


TEST(SingleThread, Enqueue_copy_construct_test)
{
	CircularThreadSafeBuffer<StubObject, 1> srcBuffer;
	srcBuffer.Enqueue(StubObject{1000});

	CircularThreadSafeBuffer<StubObject, 1> destBuffer(srcBuffer);

	EXPECT_TRUE(destBuffer.Get().x == 1000);
}


TEST(SingleThread, Enqueue_move_construct_test)
{
	CircularThreadSafeBuffer<StubObject, 1> srcBuffer;
	srcBuffer.Enqueue(StubObject{ 1000 });

	CircularThreadSafeBuffer<StubObject, 1> destBuffer;
	destBuffer.Enqueue(StubObject{ 100 });

	srcBuffer = std::move(destBuffer);
	

	EXPECT_TRUE(destBuffer.Get().x == 1000);
	EXPECT_TRUE(srcBuffer.Get().x == 100);
}


TEST(SingleThread, Dequeue_test)
{
	constexpr size_t MAX_SIZE = 1000;
	CircularThreadSafeBuffer<StubObject, MAX_SIZE> srcBuffer;

	for (size_t i = 0; i < MAX_SIZE; ++i)
		srcBuffer.Emplace(i);


	for (size_t i = 0; i < MAX_SIZE; ++i)
		srcBuffer.Dequeue();

	SUCCEED();
}

TEST(SingleThread, CyclingReading_test)
{
	constexpr size_t MAX_SIZE = 10;
	CircularThreadSafeBuffer<StubObject, MAX_SIZE> srcBuffer;

	for (size_t i = 0; i < MAX_SIZE; ++i)
		srcBuffer.Emplace(i);

	using namespace std::chrono;
	auto start = high_resolution_clock::now();
	using namespace std::literals::chrono_literals;

	while (true)
	{
		const auto& val = srcBuffer.Get();
		auto end = high_resolution_clock::now();

		if (duration_cast<seconds>(end - start).count() > 10.0s.count())
			break;
	}

	SUCCEED();
}



//Deadlock Test
TEST(Concurrency, ReadWriteDeadlock_test)
{

	constexpr size_t MAX_SIZE = 1000;
	CircularThreadSafeBuffer<StubObject, MAX_SIZE> srcBuffer;

	std::thread reader([MAX_SIZE, &srcBuffer]() {

		for (size_t i = 0; i < MAX_SIZE; ++i)
		{
			const auto& item = srcBuffer.Get();
			std::this_thread::sleep_for(std::chrono::milliseconds(3));
		}

		});

	std::thread writer([MAX_SIZE, &srcBuffer]() {
		for (size_t i = 0 ; i < MAX_SIZE; ++i)
		{
			srcBuffer.Emplace(i);
			std::this_thread::sleep_for(std::chrono::milliseconds(1));
		}
		});

	reader.join();
	writer.join();

	SUCCEED();
}


TEST(Concurrency, Sleap_test)
{
	constexpr size_t MAX_SIZE = 1000;
	CircularThreadSafeBuffer<StubObject, MAX_SIZE> srcBuffer;

	for (size_t i = 0; i < MAX_SIZE; ++i)
		srcBuffer.Emplace(i);


	std::atomic<bool> bIsStoppedWrite = false;

	std::thread reader([&]() {

		while (!bIsStoppedWrite)
			const auto& item = srcBuffer.Get();
		});

	std::thread dequer([&]() {
		for (size_t i = 0; i < MAX_SIZE; ++i)
		{
			srcBuffer.Dequeue();
			std::this_thread::sleep_for(std::chrono::milliseconds(1));
		}

		std::this_thread::sleep_for(std::chrono::milliseconds(30));
		bIsStoppedWrite = true;

		//wake reader from pushing object (posion pill) for finishing test
		srcBuffer.Enqueue(StubObject{ 1 });
		});

	reader.join();
	dequer.join();

	SUCCEED();
}