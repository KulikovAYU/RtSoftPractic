cmake_minimum_required(VERSION 3.9)

set (CMAKE_CXX_STANDARD 20)

set(This LeakFreeList_tests)

set(Sources
	LeakFreeList_tests.cpp
)

include_directories("${PROJECT_SOURCE_DIR}/headers")

add_executable(${This} ${Sources})

target_link_libraries(${This} PUBLIC
gtest_main
RtSoft_leak_free_list
)

add_test(
NAME ${This}
COMMAND ${This}
)