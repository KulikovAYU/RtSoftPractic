#include <gtest/gtest.h>
#include <sml.hpp>
#include "StateMachine.hpp"

using namespace rt_soft_autumn_school;
using namespace boost::sml;

//state coverage
TEST(StateCoverage, StateMachineTestByTransitionTree_Test1)
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