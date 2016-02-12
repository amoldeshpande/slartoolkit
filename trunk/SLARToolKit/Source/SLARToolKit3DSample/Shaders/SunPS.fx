//
// Textures and Samplers
//

// An override to override the dot product of normals with 1.0, to achieve solid lighting
// when wireframe is on
float WireframeNormalOverride : register(c0);

sampler TexSampler : register(s0);
sampler Turbulence1Sampler : register(s1);
sampler Turbulence2Sampler : register(s2);
sampler GradientSampler : register(s3);

/* data passed from vertex shader to pixel shader */
struct VertexShaderOutput {
    float4 Position	     : POSITION;
    float2 UV		     : TEXCOORD0;
    float2 UV1		     : TEXCOORD1;
    float2 UV2		     : TEXCOORD2;
    float3 LightVector	 : TEXCOORD3;
    float3 WorldNormal	 : TEXCOORD4;
    float3 WorldView	 : TEXCOORD5;
    float4 WorldPos	     : TEXCOORD6;
};

static const float AmbientIntensity = 0.02f;
static const float3 AmbientColor = float3(1,1,1);
static const float3 LightColor = float3(1,1,1); //float3(1, 0.77, 0.47);
static const float DiffuseIntensity = 1.2f;
static const float SpecularIntensity = 0.5f;
static const float SpecularPower = 10.0f;

float ComputeLookup(float2 uv1, float2 uv2)
{
    // Sample turbulence textures
    float turb1Sample = tex2D(Turbulence1Sampler, uv1).r;
    float turb2Sample = tex2D(Turbulence2Sampler, uv2).r;
    
	// Add both
    float i = (turb1Sample + turb2Sample) - 1;
	i = clamp(i, 0, 1);
	return i;
}

//
// Pixel Shader
//
float4 main(VertexShaderOutput input) : COLOR 
{
	// Sample textures
    float3 tex = tex2D(TexSampler, input.UV).rgb;
	
	// Add turbulence
	// Sample turbulence textures
	float2 uv1 = input.UV1;
    float2 uv2 = input.UV2;
	float i = ComputeLookup(uv1, uv2);
	        
	// Lookup gradient color
	tex += tex2D(GradientSampler, float2(i, 0));
	
	// Calculate lighting coefficient vector (ambient, diffuse, specular, 1)
    float3 Nn = normalize(input.WorldNormal);
    float3 Vn = input.WorldView;
    float3 Ln = input.LightVector;
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