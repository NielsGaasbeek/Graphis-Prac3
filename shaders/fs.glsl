#version 330
 
// shader input
in vec2 uv;						// interpolated texture coordinates
in vec4 normal;		
in vec4 worldPos;				// interpolated normal
in vec4 cPos;
uniform sampler2D pixels;		// texture sampler

// shader output
out vec4 outputColor;
uniform vec3 lightPos;
uniform vec3 ambientColor;

// fragment shader
void main()
{
	vec3 L = lightPos-worldPos.xyz;
	vec3 diffuseColor;
	vec3 speculrColor;

	float dist = length(L);
	float attenuation = 1.0f / (dist * dist);
	L = normalize(L);
	vec3 V = normalize(worldPos.xyz - cPos.xyz);
	vec3 R = normalize(reflect(V, normal.xyz));
	vec3 lightColor = vec3(1,1,1);
	vec3 materialColor = texture(pixels, uv).xyz;

	diffuseColor = materialColor * ( max( 0.0f, dot( normal.xyz,L))) * lightColor;

	float alpha = 10.0f;
	speculrColor = materialColor * ( pow( max( 0.0f, dot( L, R)), alpha)) * lightColor;

	outputColor = vec4( (ambientColor + diffuseColor + speculrColor), 1) ; 
	
	//vec4(materialColor *  attenuation * lightColor + Phong, 1);
	//max(0.0f, dot(L,normal.xyz))* * attenuation
}