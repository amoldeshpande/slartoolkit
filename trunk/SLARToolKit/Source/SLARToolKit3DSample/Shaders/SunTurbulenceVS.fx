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
    float2 UV1		     : TEXCOORD0;
    float2 UV2		     : TEXCOORD1;
};

static const float RotationSpeed1 =  0.005;
static const float RotationSpeed2 = -0.01;

//
// Vertex shader with rotation
//
VertexShaderOutput main(VertexData input) 
{
    VertexShaderOutput output;

	// Output texture coordinates
    output.UV1 = input.UV;
	output.UV1.x += RotationSpeed1 * TotalSeconds;
    output.UV2 = input.UV; 
	output.UV2.x += RotationSpeed2 * TotalSeconds;

	// Get the vertex position
    float4 position = float4(input.Position.xyz, 1.0);	// object coordinates

	// Project the vertex and output it
    output.Position = mul(position, WorldViewProjectionMatrix);	// screen clipspace coords

    return output;
}