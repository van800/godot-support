# Using the same minimum as the godot-cpp project
cmake_minimum_required(VERSION 3.17)

# Silence unused variable warning when specified from toolchain
if(CMAKE_C_COMPILER)
endif()

# Set universal architecture for macOS builds (matches SCons default behavior)
if(APPLE AND NOT CMAKE_OSX_ARCHITECTURES)
    set(CMAKE_OSX_ARCHITECTURES "arm64;x86_64" CACHE STRING "Build architectures for macOS" FORCE)
endif()

set(LIBNAME "rider-gdextension") # "The name of the library"
set(GODOT_PROJECT_DIR "..")

# Make sure all the dependencies are satisfied
find_package(Python3 3.4 REQUIRED)
find_program(GIT git REQUIRED)

# Ensure godot-cpp submodule has been updated
if(NOT EXISTS "${CMAKE_CURRENT_LIST_DIR}/godot-cpp/src")
    message(NOTICE "godot-cpp bindings source not found")
    message(NOTICE "initializing/updating the godot-cpp submodule...")

    # update the c++ bindings submodule to populate it with the necessary source for the library
    execute_process(
        COMMAND git submodule update --init godot-cpp
        WORKING_DIRECTORY ${CMAKE_CURRENT_LIST_DIR}
        COMMAND_ERROR_IS_FATAL ANY
    )
endif()

set(GODOTCPP_TARGET "editor")

add_subdirectory(godot-cpp SYSTEM)

# Add godot-cpp's module path and include the exported functions.
# This is made available for documentation generation
set(CMAKE_MODULE_PATH "${CMAKE_MODULE_PATH};${godot-cpp_SOURCE_DIR}/cmake")
include(GodotCPPModule)

# The godot-cpp target has some of useful properties attached that can be retrieved like so.
get_target_property(GODOTCPP_SUFFIX godot::cpp GODOTCPP_SUFFIX)
get_target_property(GODOTCPP_PLATFORM godot::cpp GODOTCPP_PLATFORM)

# Remove .universal from suffix to match SCons behavior (SConstruct:54)
string(REPLACE ".universal" "" GODOTCPP_SUFFIX "${GODOTCPP_SUFFIX}")

# Now we can specify our own project which will inherit any global cmake properties or variables that have been defined.
project(godot-addon
    VERSION 1.0
    DESCRIPTION ""
    HOMEPAGE_URL "https://github.com/JetBrains/godot-support.git"
    LANGUAGES CXX
)

file(GLOB_RECURSE SOURCES "src/*.cpp" "src/*.h")
add_library(${LIBNAME} SHARED)
target_sources(${LIBNAME}
        PRIVATE
        ${SOURCES}
        ${GAME_SOURCES}
)

# Fetch a list of the xml files to use for documentation and add to our target
file(GLOB_RECURSE DOC_XML LIST_DIRECTORIES NO CONFIGURE_DEPENDS "${PROJECT_SOURCE_DIR}/doc_classes/*.xml")

# conditionally add doc data to compile output
if(DOC_XML)
    if(GODOTCPP_TARGET MATCHES "editor")
        target_doc_sources(${LIBNAME} ${DOC_XML})
    endif()
endif()

target_link_libraries(${LIBNAME} PRIVATE godot-cpp)

# Require at least C++17 for this target
set_property(TARGET ${LIBNAME} PROPERTY CXX_STANDARD 17)

set_target_properties(${LIBNAME}
    PROPERTIES
    # The generator expression here prevents msvc from adding a Debug or Release subdir.
    RUNTIME_OUTPUT_DIRECTORY "$<1:${PROJECT_SOURCE_DIR}/bin/${GODOTCPP_PLATFORM}>"

    OUTPUT_NAME "${LIBNAME}${GODOTCPP_SUFFIX}"
)

# Match SCons behavior: $SHLIBPREFIX is empty on Windows, "lib" elsewhere
if(NOT WIN32)
    set_target_properties(${LIBNAME} PROPERTIES PREFIX "lib")
endif()

set(GODOT_PROJECT_BINARY_DIR "${PROJECT_SOURCE_DIR}/${GODOT_PROJECT_DIR}/bin/${GODOTCPP_PLATFORM}")

add_custom_command(TARGET ${LIBNAME} POST_BUILD
    COMMAND ${CMAKE_COMMAND} -E echo "POST_BUILD: Copying $<TARGET_FILE:${LIBNAME}> to ${GODOT_PROJECT_BINARY_DIR}/$<TARGET_FILE_NAME:${LIBNAME}>"
    COMMAND ${CMAKE_COMMAND} -E copy "$<TARGET_FILE:${LIBNAME}>" "${GODOT_PROJECT_BINARY_DIR}/$<TARGET_FILE_NAME:${LIBNAME}>"
)
