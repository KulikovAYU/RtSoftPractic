cmake_minimum_required(VERSION 3.9)

set (CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
add_compile_options("/std:c++latest")

 
set(HISTOGRAMM_PROJECT_NAME RtSoft_Histoghramm)
project(${HISTOGRAMM_PROJECT_NAME})

set(gtest_force_shared_crt ON CACHE BOOL "" FORCE)
add_subdirectory(vendors/googletest)


#headers
set (HEADER_FILES
headers/HistCalc.hpp
headers/Picture.hpp
headers/Utils.hpp
headers/Gistoghramm.hpp
headers/Constants.hpp
headers/GenTimer.hpp
headers/ThreadSafeResBuffer.hpp
)

set (THREAD_POOL_FILES
headers/ThreadPool/ThreadPool.hpp
headers/ThreadPool/Task.hpp
headers/ThreadPool/UnboundedMpmcQueue.hpp
)


set (FILE_WRITER_FILES
headers/FileWriter/FileWriter.hpp
)

set(SRC_FILES src/main.cpp
src/Utils.cpp
src/FileWriter.cpp
src/Gistoghramm.cpp
)

#set subdir



include_directories("${PROJECT_SOURCE_DIR}/headers"
					"${PROJECT_SOURCE_DIR}/headers/ThreadPool"
					"${PROJECT_SOURCE_DIR}/headers/FileWriter"
					"googletest/googletest/include/")

add_executable(${PROJECT_NAME}
               ${SRC_FILES}
               ${HEADER_FILES}
			   ${THREAD_POOL_FILES}
			   ${FILE_WRITER_FILES}
			   )

# add_library(${PROJECT_NAME} STATIC
               # ${SRC_FILES}
               # ${HEADER_FILES})
               
# add_subdirectory(Histogramm_tests)

# target_link_libraries(gtest)

#add filters

source_group(TREE ${CMAKE_CURRENT_SOURCE_DIR} FILES ${HEADER_FILES})
source_group(TREE ${CMAKE_CURRENT_SOURCE_DIR} FILES ${THREAD_POOL_FILES})
source_group(TREE ${CMAKE_CURRENT_SOURCE_DIR} FILES ${FILE_WRITER_FILES})
