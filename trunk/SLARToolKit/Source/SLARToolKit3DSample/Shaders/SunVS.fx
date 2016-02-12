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

// Data passed from vertex shader to pixel shader
struct VertexShaderOutput 
{
    float4 Position	     : POSITION;
    float2 UV		     : TEXCOORD0;
    float2 UV1		     : TEXCOORD1;
    float2 UV2		     : TEXCOORD2;
    float3 LightVector	 : TEXCOORD3;
    float3 WorldNormal	 : TEXCOORD4;
    float3 WorldView	 : TEXCOORD5;
    float4 WorldPos	     : TEXCOORD6;
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
static const float RotationSpeed1 =  0.005;
static const float RotationSpeed2 = -0.01;

//
// Vertex shader with rotation
//
VertexShaderOutput main(VertexData input) 
{
    VertexShaderOutput output;

	// Output texture coordinates and lighting
    output.UV = input.UV.xy;
    output.UV1 = input.UV;
	output.UV1.x += RotationSpeed1 * TotalSeconds;
    output.UV2 = input.UV; 
	output.UV2.x += RotationSpeed2 * TotalSeconds;

    // rotation matrix created here
    float4x4 rotation = RotateY(TotalSeconds * RotationSpeed);
	
	// normally this would be passed in, derive default
	float3 normal = input.Normal;
	
	// Rotate normals
	normal = mul(float4(normal,0), rotation).xyz;

	// Normals are transformed with the inverse transpose instead of the 
	// standard world matrix to eliminate scaling.
    output.WorldNormal = mul(float4(normal,0), WorldInverseTransposeMatrix).xyz;

	// Get the vertex position
    float4 position = float4(input.Position.xyz, 1.0);	// object coordinates

	// Rotate the vertex, project, and output it
    position = mul(position, rotation);
    output.Position = mul(position, WorldViewProjectionMatrix);	// screen clipspace coords
	output.WorldPos = output.Position;

	// Transform the rotated vertex to world coordinates in order to derive the world view
    float4 worldPosition = mul(position, WorldMatrix);		// world coordinates
    output.WorldView = normalize(ViewInverseMatrix[3].xyz - worldPosition);	// obj coords

	// Light
	output.LightVector = normalize(worldPosition.xyz - LightPos.xyz);

    return output;
}