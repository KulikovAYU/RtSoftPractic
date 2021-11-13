#include <gtest/gtest.h>
#include "ThreadSafeBuffer.hpp"


using namespace rt_soft_autumn_school;


struct StubObject {

	StubObject(size_t x_) : x{ x_ }
	{

	}
	StubObject(const StubObject& rhs) : x{ rhs.x }
	{
		int x = 1;
	}
	size_t x = 0;
};

//int main() {
//	
//	CircularThreadSafeBuffer<Test, 1> buffer;
//	buffer.Enqueue(Test());
//
//	CircularThreadSafeBuffer<Test, 1> buffer1(buffer);
//	CircularThreadSafeBuffer<Test, 1> buffer3(std::move(buffer1));
//
//
//	buffer.Emplace(10);
//
//	buffer.Dequeue();
//	buffer.Dequeue();
//	buffer.Get();
//}


TEST(SingleThread, Enqueue_single_test)
{
	CircularThreadSafeBuffer<StubObject, 1> buffer;
	const StubObject& obj = 10;
	buffer.Enqueue(obj);

	EXPECT_TRUE(buffer.Get().x == 10);
}

TEST(SingleThread, Enqueue_million_test)
{
	static constexpr size_t MAX_SIZE = 1000000;
	CircularThreadSafeBuffer<StubObject, MAX_SIZE> buffer;
	for (size_t i = 0; i < MAX_SIZE - 1; i++) 
	{
		buffer.Enqueue(StubObject(i));
	}


	for (size_t i = 0; i < MAX_SIZE - 1; i++)
		EXPECT_TRUE(buffer.Get().x == i);
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

	CircularThreadSafeBuffer<StubObject, 1> destBuffer(std::move(srcBuffer));
	destBuffer.Enqueue(StubObject{ 0 });

	EXPECT_TRUE(destBuffer.Get().x == 1000);
	EXPECT_TRUE(srcBuffer.Get().x == 0);
}


#if 0
//state coverage
TEST(StateCoverage, StateMachineStages_Test)
{
	BoostStateMachine deviceSm;

	deviceSm.process_event(StateMachine::EventOn{});
	EXPECT_TRUE(deviceSm.is("Starting"_s));

	deviceSm.process_event(StateMachine::EventReady{});
	EXPECT_TRUE(deviceSm.is("Running"_s));

	deviceSm.process_event(StateMachine::EventWarning{});
	EXPECT_TRUE(deviceSm.is("Error"_s));

	deviceSm.process_event(StateMachine::EventHitByHammer{});
	EXPECT_TRUE(deviceSm.is("Running"_s));

	deviceSm.process_event(StateMachine::EventOff{});
	EXPECT_TRUE(deviceSm.is("Stoped"_s));
}


//transition coverage
TEST(TransitionCoverage, StateMachineTestByTransitionTree_Test1)
{
	BoostStateMachine deviceSm;
	deviceSm.process_event(StateMachine::EventOn{});
	deviceSm.process_event(StateMachine::EventReady{});
	deviceSm.process_event(StateMachine::EventOff{});

	bool bIsStopped = deviceSm.is("Stoped"_s);
	EXPECT_TRUE(bIsStopped);
}

TEST(TransitionCoverage, StateMachineTestByTransitionTree_Test2)
{
	BoostStateMachine deviceSm;
	deviceSm.process_event(StateMachine::EventOn{});
	deviceSm.process_event(StateMachine::EventReady{});
	deviceSm.process_event(StateMachine::EventWarning{});
	deviceSm.process_event(StateMachine::EventHitByHammer{});
	deviceSm.process_event(StateMachine::EventOff{});

	EXPECT_TRUE(deviceSm.is("Stoped"_s));
}
#endif
