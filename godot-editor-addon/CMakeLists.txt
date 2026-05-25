cmake_minimum_required(VERSION 3.25)

project(Game)

file(GLOB_RECURSE GAME_SOURCES "${CMAKE_CURRENT_SOURCE_DIR}/addons/rider-plugin/*")

add_custom_target(Game SOURCES ${GAME_SOURCES}
        main.tscn
        project.godot
    )

add_subdirectory(${CMAKE_CURRENT_SOURCE_DIR}/native/rider-plugin SYSTEM)

# Prevent Godot from importing build artifacts as project resources
file(TOUCH "${CMAKE_BINARY_DIR}/.gdignore")

