cmake_minimum_required(VERSION 3.9)

set (CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
add_compile_options("/std:c++latest")


set(This ThreadSafeBuffer_tests)

set(Sources
	ThreadSafeBuffer_tests.cpp
)

include_directories("${PROJECT_SOURCE_DIR}/headers"
)


add_executable(${This} ${Sources})

target_link_libraries(${This} PUBLIC
gtest_main
RtSoft_ThreadSafeBuffer
)

add_test(
NAME ${This}
COMMAND ${This}
)