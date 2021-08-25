#version 330

out vec4 gColor;

flat in vec4 colour;

void main()
{
    gColor = colour;
}