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

static const float RotationSpeed = 0.05;

//
// Vertex shader with rotation
//
VertexShaderOutput main(VertexData input) 
{
    VertexShaderOutput output;

	// Output texture coordinates
    output.UV = input.UV.xy;

    // rotation matrix created here
    float4x4 rotation = RotateY(TotalSeconds * RotationSpeed);
	
	// normally this would be passed in, derive default
	float3 normal = input.Normal;
	float3 tangent = cross(float3(0.0,1.0,0.0), normal);
	float3 binormal = cross(tangent, normal);
	
	// Rotate normals
	normal = mul(float4(normal,0), rotation).xyz;
	tangent = mul(float4(tangent,0), rotation).xyz;
	binormal = mul(float4(binormal,0), rotation).xyz;

	// Normals are transformed with the inverse transpose instead of the 
	// standard world matrix to eliminate scaling.
    output.WorldNormal = mul(float4(normal,0), WorldInverseTransposeMatrix).xyz;
    output.WorldTangent = mul(float4(tangent,0), WorldInverseTransposeMatrix).xyz;
    output.WorldBinormal = mul(float4(binormal,0), WorldInverseTransposeMatrix).xyz;

	// Get the vertex position
    float4 position = float4(input.Position.xyz, 1.0);	// object coordinates

	// Rotate the vertex, project, and output it
    position = mul(position, rotation);
    output.Position = mul(position, WorldViewProjectionMatrix);	// screen clipspace coords

	// Transform the rotated vertex to world coordinates in order to derive the world view
    float3 worldPosition = mul(position, WorldMatrix).xyz;		// world coordinates
    output.WorldView = normalize(ViewInverseMatrix[3].xyz - worldPosition);	// obj coords

	// Light Dir
	output.LightVector = -normalize(worldPosition.xyz - LightPos.xyz);

    return output;
}