#version 330
 
// shader input
in vec2 uv;						// interpolated texture coordinates
in vec4 normal;		
in vec4 worldPos;				// interpolated normal
uniform sampler2D pixels;		// texture sampler

// shader output
out vec4 outputColor;
uniform vec3 lightPos;

// fragment shader
void main()
{
	vec3 L = lightPos-worldPos.xyz;
	float dist = length(L);
	L = normalize(L);
	vec3 lightColor = vec3(100,100,100);
	vec3 materialColor = texture(pixels, uv).xyz;
	float attenuation = 1.0f / (dist * dist);
	outputColor = vec4(materialColor * max(0.0f, dot(L,normal.xyz))* attenuation * lightColor, 1);
}