attribute vec3 coord3d;
attribute vec3 v_normal;
attribute vec4 v_color;
attribute vec2 texcoord;
attribute vec2 marktexcoord;

varying vec4 f_position;
varying vec4 color;
varying vec3 varyingNormalDirection;
varying vec2 f_texcoord;
varying vec2 f_marktexcoord;

uniform mat4 m_transform;
uniform mat4 m_view;
uniform mat4 m_view_inv;
uniform mat4 m_projection;
uniform mat3 m_normal;

void main(void)
{
	f_position = m_transform * vec4(coord3d, 1.0);
	varyingNormalDirection = normalize(m_normal * v_normal);
	color = v_color;
	f_texcoord = texcoord;
	f_marktexcoord = marktexcoord;

	mat4 mvp = m_projection * m_view * m_transform;
	gl_Position = mvp * vec4(coord3d, 1.0);
}