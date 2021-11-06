#include <sml.hpp>
#include <iostream>

namespace rt_soft_autumn_school {


	struct StateMachine;
	using BoostStateMachine = boost::sml::sm< StateMachine>;

	#define REGISTER_CLASS_TYPE(cls)  const char* ToString() const noexcept { return #cls; }

	struct StateMachine {

	public:

		#pragma region Events Block
		//Events
		struct EventOn {
			REGISTER_CLASS_TYPE(EventOn)
		};

		struct EventReady {
			REGISTER_CLASS_TYPE(EventReady)
		};

		struct EventWarning {
			REGISTER_CLASS_TYPE(EventWarning)
		};

		struct EventHitByHammer {
			REGISTER_CLASS_TYPE(EventHitByHammer)
		};

		struct EventOff {
			REGISTER_CLASS_TYPE(EventOff)
		};
		#pragma endregion


		#pragma region Actions Block
		//Actions
		struct ActionEventOn {
			void operator()(const auto& event) {
				std::cout << "State of the Device has moved to Starting by event->" << event.ToString() << std::endl;
			};
		};

		struct ActionEventReady {
			void operator()(const auto& event) {
				std::cout << "State of the Device has moved to Running by event->" << event.ToString() << std::endl;
			};
		};


		struct ActionEventWarning {
			void operator()(const auto& event) {
				std::cout << "Oops! State of the Device has moved to Error by event->" << event.ToString() << std::endl;
			};
		};

		struct ActionEventHitByHammer {
			void operator()(const auto& event) {
				std::cout << "State of the Device has moved to Running by event->" << event.ToString() << std::endl;
			};
		};

		struct ActionEventOff {
			void operator()(const auto& event) {
				std::cout << "State of the Device has moved to Stoped by event->" << event.ToString() << std::endl;
			};
		};

		#pragma endregion

		//in this block code i use some variants of action invokation:
		//such as:
		//- event<EventOn> / ActionEventOn {}  - if event "EventOn" has been invoked and state of machine was Stoped then invoke
		// functor of ActionEventOn
		//- "Running"_s + on_entry<EventReady> / ActionEventReady {} - if event "EventReady" has been invoked and state of
		//machine was Running, then invoke functor of ActionEventReady

		// Transition table
		auto operator()() const {
			using namespace boost::sml;
			return make_transition_table(
				*"Stoped"_s + event<EventOn> / ActionEventOn {} = "Starting"_s,
				"Starting"_s + event<EventReady> = "Running"_s,
				"Running"_s + on_entry<EventReady> / ActionEventReady {},
				"Running"_s + event<EventWarning> = "Error"_s,
				"Error"_s + on_entry<EventWarning> / ActionEventWarning {},
				"Error"_s + event<EventHitByHammer> = "Running"_s,
				"Running"_s + on_entry<EventHitByHammer> / ActionEventHitByHammer {},
				"Running"_s + event<EventOff> = "Stoped"_s,
				"Stoped"_s + on_entry<EventOff> / ActionEventOff {}
			);
		}
	};
}