# Particles
Highly optimised particle system, written in C#, rendered with OpenGL and open sourced from the [Vercidium Engine](https://vercidium.com/blog).

## Overview

Some features of this particle system are:
- Fast batch particle creation
- Efficient memory management when creating, removing and updating particles
- Custom Matrix3x4 format for fast particle matrix operations on the GPU
- Writing to shared GPU memory across multiple threads

The explanation of this particle system can be found in [this blog post](https://vercidium.com/blog/opengl-particle-systems/).

## Dependencies

This repository uses mock OpenGL bindings. [OpenGL.Net](https://github.com/luca-piccioni/OpenGL.Net) can be installed through NuGet as an alternative.

## In Practice
This particle system is used in Sector's Edge to render block destruction particles projectile trails, rain and snow. See the particle system in action on the [Sector's Edge Steam page](https://sectorsedge.com/s/izl4).
