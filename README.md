# Particles
Optimised particle system, written in C#, rendered with OpenGL and open sourced from the Vercidium Engine.

## Overview

This repository contains C# code for creating and destroying particles, sending particle data to the GPU and rendering particles with OpenGL.

The explanation of this particle system can be found in  [this blog post](https://vercidium.com/blog/particle-optimisations/).

This particle system uses the voxel ray-marching algorithm found in [this repository](https://github.com/Vercidium/voxel-ray-marching).

The system is thread safe and can also buffer data to the GPU across multiple threads.

## Dependencies

This system uses [OpenGL.Net](https://github.com/luca-piccioni/OpenGL.Net) bindings for C#, which can be installed through NuGet.

## Benchmarks
This system is capable of updating, buffering and rendering 33,000 particles on a Ryzen 5 1600 CPU and GTX960 GPU before dropping below 60FPS, which equates to ~484 nanoseconds per particle.

## In Practice
Particles in Sector's Edge are small cubes that are created from explosions, block destruction, projectile trails, player damage, rain and other causes. See the particle system in action in our [latest video here]](https://www.youtube.com/watch?v=Dklpzrg1Zko).