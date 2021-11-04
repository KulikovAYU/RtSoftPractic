cmake_minimum_required(VERSION 3.9)

set(SHARED_PTR_PROJECT_NAME RtSoft_Simple_shared_pointer)
project(${SHARED_PTR_PROJECT_NAME})

set(gtest_force_shared_crt ON CACHE BOOL "" FORCE)
add_subdirectory(vendors/googletest)

#set subdir
include_directories(headers)

#headers
set (HEADER_FILES 
headers/simple_shared_pointr_impl.hpp
)
set(sources src/main.cpp)

include_directories("${PROJECT_SOURCE_DIR}/headers"
 "googletest/googletest/include/")


add_library(${PROJECT_NAME} STATIC
               ${sources}
               ${headers})
               
add_subdirectory(Simple_shared_pointer_tests)

target_link_libraries(gtest)