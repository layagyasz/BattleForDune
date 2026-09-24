#version 430 core

layout(location = 0) in vec3 in_position;  
layout(location = 1) in vec4 in_color;
layout(location = 2) in vec2 in_tex_coord;
layout(location = 3) in vec3 in_velocity;
layout(location = 4) in int in_birth_time;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;

uniform int global_time;
uniform float inv_lifetime;

out vec4 vert_color;
out vec2 vert_tex_coord;

void main(void)
{
	float age = float(global_time - in_birth_time);
	if (age * inv_lifetime > 1)
	{
		gl_Position = vec4(0);
	}
	else
	{
		gl_Position = vec4(0.001f * in_velocity * age + in_position, 1.0) * model * view * projection;
	}
	vert_color = in_color;
	vert_tex_coord = in_tex_coord;
}