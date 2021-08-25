#version 330

uniform mat4 mvp;

layout (location = 0) in vec4 aPosition;
layout (location = 1) in vec4 aColour;
layout (location = 2) in mat4 aTransform;

flat out vec4 colour;

void main()
{
    // Precalculating vec4s is faster than accessing via aTransform[0][0]
    vec4 a0 = aTransform[0];
    vec4 a1 = aTransform[1];
    vec4 a2 = aTransform[2];

    vec4 b0 = mvp[0];
    vec4 b1 = mvp[1];
    vec4 b2 = mvp[2];
    vec4 b3 = mvp[3];

    vec3 v = aPosition.xyz;

    // Combined rotation, scale and translate with fewer multiplications and additions
    gl_Position = vec4((a0.x * b0.x)          * v.x + (a0.z * b0.x + a0.w * b1.x + a1.x * b2.x)               * v.y + (a1.y * b0.x + a1.w * b2.x)               * v.z + (a2.x * b0.x + a2.z * b2.x + b3.x),
                  (a0.x * b0.y + a0.y * b1.y) * v.x + (a0.z * b0.y + a0.w * b1.y + a1.x * b2.y) * v.y + (a1.y * b0.y + a1.z * b1.y + a1.w * b2.y) * v.z + (a2.x * b0.y + a2.y * b1.y + a2.z * b2.y + b3.y),
                  (a0.x * b0.z + a0.y * b1.z) * v.x + (a0.z * b0.z + a0.w * b1.z + a1.x * b2.z) * v.y + (a1.y * b0.z + a1.z * b1.z + a1.w * b2.z) * v.z + (a2.x * b0.z + a2.y * b1.z + a2.z * b2.z + b3.z),
                  (a0.x * b0.w + a0.y * b1.w) * v.x + (a0.z * b0.w + a0.w * b1.w + a1.x * b2.w) * v.y + (a1.y * b0.w + a1.z * b1.w + a1.w * b2.w) * v.z + (a2.x * b0.w + a2.y * b1.w + a2.z * b2.w + b3.w));
    

    // Particle colour copies cross directly
    colour = aColour;


    // Slightly darken the sides and underside of the particle
    int normal = int(aPosition.w);

    if (normal > 3)
        colour *= 0.8;
    else if (normal > 1)
        colour *= 0.9;
}