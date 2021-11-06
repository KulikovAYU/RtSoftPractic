cmake_minimum_required(VERSION 3.9)

set (CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
add_compile_options("/std:c++latest")


set(This StateMachine_tests)

set(Sources
	StateMachine_tests.cpp
)

include_directories("${PROJECT_SOURCE_DIR}/headers"
 "${PROJECT_SOURCE_DIR}/vendors/sml/include/boost/")

add_compile_definitions(__cpp_nontype_template_parameter_class)

add_executable(${This} ${Sources})

target_link_libraries(${This} PUBLIC
gtest_main
RtSoft_StateMachine
)

add_test(
NAME ${This}
COMMAND ${This}
)