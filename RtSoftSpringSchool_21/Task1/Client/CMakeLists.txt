cmake_minimum_required(VERSION 3.9)

set(SERVER_PROJECT_NAME RtSoft_Client)
project(${SERVER_PROJECT_NAME})

find_package(cpprestsdk REQUIRED)

add_executable(${SERVER_PROJECT_NAME} src/main.cpp)
target_link_libraries(${SERVER_PROJECT_NAME} PRIVATE cpprestsdk::cpprest)
target_compile_features(${SERVER_PROJECT_NAME} PUBLIC cxx_std_17)
set_target_properties(${SERVER_PROJECT_NAME} PROPERTIES RUNTIME_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin/)