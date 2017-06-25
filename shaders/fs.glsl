#version 330
 
// shader input
in vec2 uv;						// interpolated texture coordinates
in vec4 normal;		
in vec4 worldPos;				// interpolated normal

uniform sampler2D pixels;		// texture sampler
uniform vec3 lightPos;
uniform vec3 ambientColor;
uniform vec3 cameraPos;

// shader output
out vec4 outputColor;


// fragment shader
void main()
{

	vec3 L = lightPos.xyz-worldPos.xyz;
	float dist = L.length();
	float attenuation = 1.0f / (dist * dist);
	L = normalize(L);

	vec3 V = normalize(cameraPos.xyz - worldPos.xyz);
	vec3 R = normalize(-reflect(V, normal.xyz));

	vec3 lightColor = vec3(10,10,10);
	vec3 materialColor = texture(pixels, uv).xyz;

	vec3 diffuseColor = materialColor * ( max( 0.0f, dot( normal.xyz, L))) * lightColor * attenuation;

	float alpha = 4.0f;
	vec3 speculrColor ;
	if (dot(normal.xyz, L) < 0.0f)
	{
		speculrColor = vec3(0f,0f,0f);	
	}
	else
	{
		speculrColor = materialColor * ( pow( max( 0.0f, dot( L, R)), alpha)) * lightColor * attenuation;		
	}


	//diffuseColor = vec3(0,0,0);
	//speculrColor = vec3(0,0,0);

	outputColor = vec4( (ambientColor + diffuseColor + speculrColor), 1) ; 
	
	//vec4(materialColor *  attenuation * lightColor + Phong, 1);
	//max(0.0f, dot(L,normal.xyz))* * attenuation
}