cmake_minimum_required(VERSION 3.9)

set (CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
add_compile_options("/std:c++latest")

set(BAR_CHART_PROJECT_NAME RtSoft_BarChart)
project(${BAR_CHART_PROJECT_NAME})


#headers
set (HEADER_FILES
headers/Application.hpp
headers/BasicLogic.hpp
headers/Message.hpp
headers/Utils.hpp
headers/BarChart.hpp
headers/Constants.hpp
headers/ThreadSafeResBuffer.hpp
)

set (THREAD_POOL_FILES
headers/ThreadPool/ThreadPool.hpp
headers/ThreadPool/UnboundedMpmcQueue.hpp
)

set(SRC_FILES
src/main.cpp
src/Application.cpp
src/Utils.cpp
src/BarChart.cpp
src/BasicLogic.cpp
)

#set startup sln
set_property(DIRECTORY ${CMAKE_CURRENT_SOURCE_DIR} PROPERTY  VS_STARTUP_PROJECT ${BAR_CHART_PROJECT_NAME})

#off codevt_error
add_compile_definitions(_SILENCE_CXX17_CODECVT_HEADER_DEPRECATION_WARNING)


include_directories("${PROJECT_SOURCE_DIR}/headers"
					"${PROJECT_SOURCE_DIR}/headers/ThreadPool")

add_executable(${PROJECT_NAME}
               ${SRC_FILES}
               ${HEADER_FILES}
			   ${THREAD_POOL_FILES}
			   )


#add filters
source_group(TREE ${CMAKE_CURRENT_SOURCE_DIR} FILES ${HEADER_FILES})
source_group(TREE ${CMAKE_CURRENT_SOURCE_DIR} FILES ${THREAD_POOL_FILES})


set_target_properties(${BAR_CHART_PROJECT_NAME} PROPERTIES RUNTIME_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin/)