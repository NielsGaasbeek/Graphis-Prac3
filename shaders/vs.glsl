#version 330
 
// shader input
in vec2 vUV;				// vertex uv coordinate
in vec3 vNormal;			// untransformed vertex normal
in vec3 vPosition;			// untransformed vertex position

// shader output
out vec4 normal;
out vec4 worldPos;			// transformed vertex normal
out vec2 uv;

//transforms			
uniform mat4 transform;
uniform mat4 toWorld;
 
// vertex shader
void main()
{
	// transform vertex using supplied matrix
	gl_Position = transform * vec4(vPosition, 1.0);
	worldPos = transform * vec4(vPosition, 1.0f);
	normal = transform * vec4(vNormal, 0.0f);

	// forward normal and uv coordinate; will be interpolated over triangle
	uv = vUV;
<<<<<<< HEAD
=======
	cPos = toWorld * vec4(0, -5, -20, 0);
>>>>>>> refs/remotes/origin/master
}