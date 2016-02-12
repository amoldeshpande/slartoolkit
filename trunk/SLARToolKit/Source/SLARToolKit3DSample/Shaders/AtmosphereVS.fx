#include "AtmosphereVS.fxh"

//
// Vertex shader with rotation
//
VertexShaderOutput main(VertexData input) 
{	
	return atmosphereVS(input, 0.02f);
}
