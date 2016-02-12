//
// Textures and Samplers
//

// An override to override the dot product of normals with 1.0, to achieve solid lighting
// when wireframe is on
float WireframeNormalOverride : register(c0);

sampler TexSampler : register(s0);
sampler NormalSampler : register(s1);


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

static const float AmbientIntensity = 0.02f;
static const float3 AmbientColor = float3(1,1,1);
static const float3 LightColor = float3(1,1,1); //float3(1, 0.77, 0.47);
static const float DiffuseIntensity = 1.2f;
static const float SpecularIntensity = 0.5f;
static const float SpecularPower = 10.0f;
static const float BumpFactor = 4.0f;

//
// Pixel Shader
//
float4 main(VertexShaderOutput input) : COLOR 
{
	// Sample textures
    float3 tex = tex2D(TexSampler, input.UV).rgb;
	float2 normalSample = tex2D(NormalSampler, input.UV).rg;

    // Apply bump map
    float2 bump = BumpFactor * (normalSample - (0.5).xx);
    float3 Nn = normalize(input.WorldNormal) + (bump.x * normalize(input.WorldTangent) + bump.y * normalize(input.WorldBinormal));
    Nn = normalize(Nn);

	// Calculate lighting coefficient vector (ambient, diffuse, specular, 1)
    float3 Vn = input.WorldView;
    float3 Ln = input.LightVec;
    float3 Hn = normalize(Vn + 2*Ln);
    float hdn = dot(Hn, Nn);
    float ldn = dot(Ln, Nn);
    hdn = WireframeNormalOverride + (1.0 - WireframeNormalOverride) * hdn;
    ldn = WireframeNormalOverride + (1.0 - WireframeNormalOverride) * ldn;
    float4 litVec = lit(ldn, hdn, SpecularPower);

    // Update intensities
    litVec.x *= AmbientIntensity;
    litVec.y *= DiffuseIntensity;
    litVec.z *= SpecularIntensity;

    // Apply lighting and coloring
    float3 ambient = litVec.x * AmbientColor;	
    float3 specular = pow(litVec.z, 1.5) * LightColor;
    float3 diffuse = saturate(litVec.y) * LightColor * tex;

    // Final color calculation
    float3 result = ambient + diffuse + specular;

    return float4(result.rgb, 1.0);
}