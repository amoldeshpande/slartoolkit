//
// Constants from app code
//

// transform object vertices to world-space
float4x4 WorldMatrix : register(c0);

// transform object normals, tangents, & binormals to world-space
float4x4 WorldInverseTransposeMatrix : register(c4);

// transform object vertices to view space and project them in perspective
float4x4 WorldViewProjectionMatrix : register(c8);

// provide tranform from "view" or "eye" coords back to world-space
float4x4 ViewInverseMatrix : register(c12);

// total seconds counter for animations
float TotalSeconds : register(c16);

// Light position
float4 LightPos : register(c17);

//
// Vertex data from application
//
struct VertexData 
{
    float3 Position	: POSITION;
    float3 Normal	: NORMAL;
    float2 UV		: TEXCOORD0;
};

//
// Data passed from vertex shader to pixel shader
//
struct VertexShaderOutput 
{
    float4 Position	     : POSITION;
    float2 UV		     : TEXCOORD0;
    // The following values are passed in "World" coordinates since
    //   it tends to be the most flexible and easy for handling
    //   reflections, sky lighting, and other "global" effects.
    float3 LightVector	 : TEXCOORD1;
    float3 WorldNormal	 : TEXCOORD2;
    float3 WorldTangent	 : TEXCOORD3;
    float3 WorldBinormal : TEXCOORD4;
    float3 WorldView	 : TEXCOORD5;
};

// Rotates around the Y-axis by angle
float4x4 RotateY(float A) 
{
	float s = sin(A);
	float c = cos(A);
	return float4x4(c,0,s,0,
					0,1,0,0,
					-s,0,c,0,
					0,0,0,1);
}

static const float RotationSpeed = -0.2;

//
// Vertex shader with rotation
//
VertexShaderOutput atmosphereVS(VertexData input, float atmosphereSize) 
{	
	VertexShaderOutput output = (VertexShaderOutput)0;

	// Rotation
	float4x4 rotationMatrix = RotateY(TotalSeconds * RotationSpeed * 1.1);	

	// Texture
	output.UV = input.UV;

	// Normal
	float4 rotatedNormal = float4(input.Normal, 1.0);
	rotatedNormal = mul(rotatedNormal, rotationMatrix);
	output.WorldNormal = mul(rotatedNormal, WorldInverseTransposeMatrix).xyz;

	// Position
	float4 rotatedPosition = float4(input.Position, 1.0);	
	rotatedPosition = mul(rotatedPosition, rotationMatrix);
	output.Position = mul(rotatedPosition, WorldViewProjectionMatrix) + (mul(atmosphereSize, mul(rotatedNormal, WorldViewProjectionMatrix)));
		
	// Transform the rotated vertex to world coordinates in order to derive the world view
    float3 worldPosition = mul(rotatedPosition, WorldMatrix).xyz;		// world coordinates
    output.WorldView = normalize(ViewInverseMatrix[3].xyz - worldPosition);	// obj coords
	
	// LightDir
	output.LightVector = -normalize(worldPosition.xyz - LightPos.xyz);

	return output;
}
