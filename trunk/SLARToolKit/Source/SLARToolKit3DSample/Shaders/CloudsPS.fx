//
// Textures and Samplers
//

sampler CloudSampler : register(s0);

/* data passed from vertex shader to pixel shader */
struct VertexShaderOutput {
    float4 Position	     : POSITION;
    float2 UV		     : TEXCOORD0;
    // The following values are passed in "World" coordinates since
    // it tends to be the most flexible and easy for handling
    // reflections, sky lighting, and other "global" effects.
    float3 LightVec	     : TEXCOORD1;
    float3 WorldNormal	 : TEXCOORD2;
    float3 WorldTangent	 : TEXCOORD3;
    float3 WorldBinormal : TEXCOORD4;
    float3 WorldView	 : TEXCOORD5;
};

static const float3 LightDir = float3(2.0f, -0.5f, 0.2f); // Lamp light direction
static const float CloudSpeed = 0.18;
static const float3 AmbientColor = float3(0.5, 0.5, 0.5);
static const float4 LampColor = float4(1, 1, 1, 1);

//
// Pixel Shader
//
float4 main(VertexShaderOutput input) : COLOR 
{
	float3 Ln = normalize(-LightDir).xyz;
	float3 Nn = normalize(input.WorldNormal);
	float ldn = dot(Ln, Nn);
	float diffuse = max(saturate(ldn), 0.1);
		
	float4 clouds = tex2D(CloudSampler, input.UV).rgba;	
	float4 cloudsN = clouds;
		
	clouds *= LampColor;
	clouds = float4(clouds.rgb * diffuse, clouds.a);
	
	cloudsN = float4(clouds.rgb * saturate(0.25 - diffuse), 0);
	float4 color = clouds;// + cloudsN;
	
	return color;
}