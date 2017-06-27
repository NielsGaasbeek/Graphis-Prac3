#version 330

// shader input
in vec2 P;						// fragment position in screen space
in vec2 uv;						// interpolated texture coordinates
uniform sampler2D pixels;		// input texture (1st pass render target)

// shader output
out vec3 outputColor;

void main()
{
	// retrieve input pixel
	outputColor = texture( pixels, uv ).rgb;

	float dx = P.x - 0.5, dy = P.y - 0.5;
	float distance = -sqrt( dx * dx + dy * dy ) * 0.5f;
	//calculate distance to screen.
	//add this distance to the outputColor to make pixels further away darker.
	outputColor += distance;
	
	float aberration = 5.0f;
    // retrieve input pixel and apply chromatic aberration
    outputColor.x = texture(pixels, vec2(uv.x, uv.y)).x;
    outputColor.y = texture(pixels, vec2(uv.x + 0.002 * aberration, uv.y)).y;
    outputColor.z = texture(pixels, vec2(uv.x, uv.y + 0.002 * aberration)).z;

}

// EOF