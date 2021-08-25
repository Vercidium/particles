# Particles
Optimised particle system, written in C#, rendered with OpenGL and open sourced from the Vercidium Engine.

## Overview

This repository updates 3D particles and writes to shared GPU memory across multiple threads.

The explanation of this particle system can be found in [this blog post](https://vercidium.com/blog/opengl-particle-systems/).

## Dependencies

This repository uses mock OpenGL bindings. [OpenGL.Net](https://github.com/luca-piccioni/OpenGL.Net) can be installed through NuGet as an alternative.

## In Practice
Particles in Sector's Edge are small cubes that are created from explosions, block destruction, projectile trails, player damage, rain and snow. See the particle system in action on our [Steam page](https://sectorsedge.com/s/izl4).
