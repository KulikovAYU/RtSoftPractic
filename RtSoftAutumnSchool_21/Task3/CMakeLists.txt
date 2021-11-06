cmake_minimum_required(VERSION 3.9)

set (CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
add_compile_options("/std:c++latest")

 
set(STATE_MACHINE_PROJECT_NAME RtSoft_StateMachine)
project(${STATE_MACHINE_PROJECT_NAME})

set(gtest_force_shared_crt ON CACHE BOOL "" FORCE)
add_subdirectory(vendors/googletest)


#set subdir
include_directories(headers)

#headers
set (HEADER_FILES
headers/StateMachine.hpp
)
set(SRC_FILES src/main.cpp)

include_directories("${PROJECT_SOURCE_DIR}/headers"
 "googletest/googletest/include/"
 "${PROJECT_SOURCE_DIR}/vendors/sml/include/boost/"
 )

add_compile_definitions(__cpp_nontype_template_parameter_class)

add_library(${PROJECT_NAME} STATIC
               ${SRC_FILES}
               ${HEADER_FILES})
               
add_subdirectory(State_machine_tests)

target_link_libraries(gtest)