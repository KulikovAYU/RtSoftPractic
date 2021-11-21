cmake_minimum_required(VERSION 3.9)

set (CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
add_compile_options("/std:c++latest")


set(This Histogramm_tests)

set(Sources
	Histogramm_tests.cpp
)

include_directories("${PROJECT_SOURCE_DIR}/headers"
)


add_executable(${This} ${Sources})

target_link_libraries(${This} PUBLIC
gtest_main
RtSoft_Histoghramm
)

add_test(
NAME ${This}
COMMAND ${This}
)