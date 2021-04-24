cmake_minimum_required(VERSION 3.9)

#this is the solution name
PROJECT(RtSoft_task1)

set(PROJECT_NAME RtSoft_task1)

add_subdirectory(Server)
add_subdirectory(Client)

set_property(DIRECTORY ${CMAKE_CURRENT_SOURCE_DIR} PROPERTY  VS_STARTUP_PROJECT RtSoft_Server)