#version 430 core

out vec4 out_color;

in vec4 vert_color;
in vec2 vert_tex_coord;
in vec3 vert_internal_coord;
in vec3 vert_normal;
in vec2 vert_normal_tex_coord;
in vec2 vert_lighting_tex_coord;
in vec3 eye_normal;

layout(binding = 0) uniform sampler2D diffuse_texture;
layout(binding = 1) uniform sampler2D normal_texture;
layout(binding = 2) uniform sampler2D lighting_texture;

const vec3 light_normal = vec3(0, 0, -1);
const float light_intensity = 0.5f;
const float ambient = 0.2f;

vec4 quaternion_conjugate(vec4 q)
{ 
  return vec4(-q.x, -q.y, -q.z, q.w); 
}
  
vec4 quaternion_multiply(vec4 q1, vec4 q2)
{ 
  vec4 qr;
  qr.x = (q1.w * q2.x) + (q1.x * q2.w) + (q1.y * q2.z) - (q1.z * q2.y);
  qr.y = (q1.w * q2.y) - (q1.x * q2.z) + (q1.y * q2.w) + (q1.z * q2.x);
  qr.z = (q1.w * q2.z) + (q1.x * q2.y) - (q1.y * q2.x) + (q1.z * q2.w);
  qr.w = (q1.w * q2.w) - (q1.x * q2.x) - (q1.y * q2.y) - (q1.z * q2.z);
  return qr;
}

vec4 quaternion_rotate(vec3 v, vec4 q)
{
    return quaternion_multiply(quaternion_multiply(q, vec4(v, 0)), quaternion_conjugate(q));
}

vec3 combine_normals_quat(vec3 surface_normal, vec3 texture_normal)
{
    const float inv_sqrt_2 = 0.70710678f;
    return quaternion_rotate(
        surface_normal,
        vec4(
            inv_sqrt_2 * sqrt(1 - texture_normal.z) * normalize(vec3(-texture_normal.y, texture_normal.x, 0)), 
            inv_sqrt_2 * sqrt(1 + texture_normal.z))).xyz;
}

vec2 as_spherical(vec3 v)
{
    return vec2(atan(length(v.xz), v.y), atan(v.z, v.x));
}

vec3 as_cartesian(vec2 v)
{
    return vec3(sin(v.x) * cos(v.y), cos(v.x), sin(v.x) * sin(v.y));
}

vec3 combine_normals_tan(vec3 surface_normal, vec3 texture_normal)
{
    const float pi_over_2 = 1.57079632f;
    return as_cartesian(as_spherical(surface_normal) + as_spherical(texture_normal) - vec2(pi_over_2, pi_over_2));
}

void main()
{
    vec4 lighting = texture(lighting_texture, vert_lighting_tex_coord / textureSize(lighting_texture, 0));
    vec2 specular_params = lighting.xy;
    float luminance = lighting.z;
    float roughness = lighting.w;

    vec3 texture_normal = vec3(0, 0, 1);
    if (roughness > 0.00001)
    {
        texture_normal =  2 * texture(normal_texture, vert_normal_tex_coord / textureSize(normal_texture, 0)).rgb - 1;
        texture_normal = normalize(vec3(texture_normal.x * roughness, texture_normal.y * roughness, texture_normal.z));
    }
    vec3 normal = combine_normals_quat(normalize(vert_normal), texture_normal);

    float diffuse = light_intensity * max(0, dot(normal, light_normal));
    float specular = specular_params.x 
        * pow(max(0, dot(normal, normalize(light_normal + eye_normal))), specular_params.y);
    float ambient = ambient + luminance;

    vec4 diffuse_color = vert_color * texture(diffuse_texture, vert_tex_coord / textureSize(diffuse_texture, 0)); 
    out_color = vec4((diffuse + specular + ambient) * diffuse_color.rgb, 1);
}