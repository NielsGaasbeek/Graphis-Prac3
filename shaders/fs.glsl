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
	//the shader first calculates all needed information for shading
	vec3 LightDirection = (lightPos.xyz-worldPos.xyz);
	float dist = LightDirection.length();
	float attenuation = 1.0f / (dist * dist);
	LightDirection = normalize(LightDirection);

	vec3 ViewDirection = normalize(cameraPos.xyz - worldPos.xyz);
	vec3 ReflectedViewDirection = normalize(-reflect(ViewDirection, normal.xyz));

	//sets the colors used for the shading calculations
	vec3 lightColor = vec3(10,10,10);
	vec3 materialColor = texture(pixels, uv).xyz;

	//standard diffuse-color calculations based on NDotL-shading and distance attenuation
	vec3 diffuseColor = materialColor * ( max( 0.0f, dot( normal.xyz, LightDirection))) * lightColor * attenuation;

	//calculations for the specular part of the shading
	vec3 speculrColor;
	if (dot(normal.xyz, LightDirection) < 0.0f)		//if-statement makes sure no highlight appears on the wrong side of the object
	{
		speculrColor = vec3(0.0f,0.0f,0.0f);	
	}
	else	//standard calculation for specular component based on information from the lecture-slides
	{
		float alpha = 4.0f;
		speculrColor = materialColor * ( pow( max( 0.0f, dot( LightDirection, ReflectedViewDirection)), alpha)) * lightColor * attenuation;		
	}

	//outputColor is then set to the sum of all components
	outputColor = vec4( (ambientColor + diffuseColor + speculrColor), 1) ; 
}