#version 330 core

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTexCoord;

out vec2 frag_TexCoords;

uniform mat4 _model;
uniform mat4 _view;
uniform mat4 _projection;

void main() 
{
    frag_TexCoords = aTexCoord;
    gl_Position = _projection * _view * _model * vec4(aPosition, 1);
}