//
// Textures and Samplers
//

sampler AtmosphereSampler : register(s0);

/* data passed from vertex shader to pixel shader */
struct VertexShaderOutput {
    float4 Position	     : POSITION;
    float2 UV		     : TEXCOORD0;
    // The following values are passed in "World" coordinates since
    // it tends to be the most flexible and easy for handling
    // reflections, sky lighting, and other "global" effects.
    float3 LightVector   : TEXCOORD1;
    float3 WorldNormal	 : TEXCOORD2;
    float3 WorldTangent	 : TEXCOORD3;
    float3 WorldBinormal : TEXCOORD4;
    float3 WorldView	 : TEXCOORD5;
};

static const float DiffuseIntensity = 1.2f;
static const float SpecularIntensity = 1.0f;
static const float SpecularPower = 1.0f;

//
// Pixel Shader
//
float4 atmospherePS(VertexShaderOutput input, bool upper) : COLOR 
{
	// Light scatter
	float4 atmos = tex2D(AtmosphereSampler, input.UV);

	float3 Nn = normalize(input.WorldNormal);
	float3 Ln = input.LightVector;
	float3 Vn = input.WorldView;
    float3 Hn = normalize(Vn + Ln);

	// Calculate lighting coefficient vector (ambient, diffuse, specular, 1)
	float hdn = dot(Hn, Nn);
	float ldn = dot(Ln, Nn);
	float4 litVec = lit(ldn, hdn, SpecularPower);

	// Update intensities
	litVec.y *= DiffuseIntensity;
	litVec.z *= SpecularIntensity;

	float light = 0;
	if(!upper)
	{
		light = saturate(1 - litVec.y) * .125;
		atmos.a *= light;
	}
	else
	{
		//light = 1 - saturate(1.1 + hdn);
		//atmos *= light;

		light = 1 - saturate(0.8 + litVec.y);
		atmos *= light;
	}
	
	return atmos;
}