#include "AtmospherePS.fxh"

//
// Pixel Shader
//
float4 main(VertexShaderOutput input) : COLOR 
{
	return atmospherePS(input, false);
}