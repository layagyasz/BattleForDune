#version 430 core

out vec4 out_color;

in vec4 vert_color;
in vec2 vert_tex_coord;
in vec3 vert_internal_coord;

void main()
{
    out_color = vert_color;
}