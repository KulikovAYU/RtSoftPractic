cmake_minimum_required(VERSION 3.9)

set (CMAKE_CXX_STANDARD 20)

set(This Simple_shared_pointer_tests)

set(Sources
	Simple_shared_pointer_tests.cpp
)

include_directories("${PROJECT_SOURCE_DIR}/headers")

add_executable(${This} ${Sources})

target_link_libraries(${This} PUBLIC
gtest_main
RtSoft_Simple_shared_pointer
)

add_test(
NAME ${This}
COMMAND ${This}
)