#version 330

uniform mat4 mvp;

layout (location = 0) in vec4 aPosition;
layout (location = 1) in vec4 aColour;
layout (location = 2) in mat4 aTransform;

flat out vec4 colour;

void main()
{
    gl_Position = mvp * aTransform * vec4(aPosition.xyz, 1.0);

    colour = aColour;

    int normal = int(aPosition.w);
	
	// Slightly darken the sides of the particle
    if (normal > 3)
        colour *= 0.8;
    else if (normal > 1)
        colour *= 0.9;
}