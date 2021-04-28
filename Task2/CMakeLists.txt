cmake_minimum_required(VERSION 3.9)

set(GITHUB_PARSER_PROJECT_NAME RtSoft_GithubParser)
project(${GITHUB_PARSER_PROJECT_NAME})

find_package(cpprestsdk REQUIRED)

#set subdir
include_directories(headers)
include_directories(src)

#headers
set (HEADER_FILES 
headers/GithubParser.h
headers/base64_decoder.h
)

#src files
set (SRC_FILES 
src/GithubParser.cpp
src/base64_decoder.cpp
src/main.cpp)

#set startup sln
set_property(DIRECTORY ${CMAKE_CURRENT_SOURCE_DIR} PROPERTY  VS_STARTUP_PROJECT ${GITHUB_PARSER_PROJECT_NAME})

#off codevt_error
add_compile_definitions(_SILENCE_CXX17_CODECVT_HEADER_DEPRECATION_WARNING)

set(CMAKE_CXX_FLAGS_DEBUG "${CMAKE_CXX_FLAGS_DEBUG} -D DEBUG")




add_executable(${GITHUB_PARSER_PROJECT_NAME} 
               ${HEADER_FILES}
               ${SRC_FILES})
target_link_libraries(${GITHUB_PARSER_PROJECT_NAME} PRIVATE cpprestsdk::cpprest)
target_compile_features(${GITHUB_PARSER_PROJECT_NAME} PUBLIC cxx_std_17)
set_target_properties(${GITHUB_PARSER_PROJECT_NAME} PROPERTIES RUNTIME_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin/)