//
// Textures and Samplers
//

sampler TexSampler : register(s0);
sampler BumpSampler1 : register(s1);
sampler BumpSampler2 : register(s2);

static const float BumpDisplacmentFactor = 0.01;
static const float BaseDisplacmentFactor = 0.97;
static const float AmbientIntensity = 0.02f;
static const float3 AmbientColor = float3(1, 1, 1);
static const float3 LightColor = float3(0.8, 0.8, 1);
static const float DiffuseIntensity = 1.2f;
static const float SpecularIntensity = 0.5f;
static const float SpecularPower = 10.0f;

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

// Pixel Shader
float4 main(VertexShaderOutput input) : COLOR 
{
    // get bump texture
	float4 bumpTex1 = tex2D(BumpSampler1, input.UV1);
    float4 bumpTex2 = tex2D(BumpSampler2, input.UV2);
    float4 bumpTex = (bumpTex1 + bumpTex2) - 0.5;
    bumpTex = clamp(bumpTex, 0, 1);

    // displace texture coordinates
	float2 screenPos= input.WorldPos.xy / input.WorldPos.w;
    screenPos = (screenPos + 1.0) * 0.5;
    screenPos.y = 1.0 - screenPos.y;
    float2 uv = screenPos * BaseDisplacmentFactor + bumpTex.xy * BumpDisplacmentFactor;

	// Sample tex
	float4 tex = tex2D(TexSampler, uv);

    // Calculate lighting coefficient vector (ambient, diffuse, specular, 1)
    float3 Nn = normalize(input.WorldNormal);
    float3 Vn = input.WorldView;
    float3 Ln = input.LightVector;
    float3 Hn = normalize(Vn + 2*Ln);
    float hdn = dot(Hn, Nn);
    float ldn = dot(Ln, Nn);
    float4 litVec = lit(ldn, hdn, SpecularPower);

    // Update intensities
    litVec.x *= AmbientIntensity;
    litVec.y *= DiffuseIntensity;
    litVec.z *= SpecularIntensity;

    // Apply lighting and coloring
    float3 ambient = litVec.x * AmbientColor;
    float3 specular = pow(litVec.z, 1.5) * LightColor;
    float3 diffuse = saturate(litVec.y) * LightColor * tex.rgb;

    // Final color calculation
    float3 result = ambient + diffuse + specular;
    return float4(result.rgb, 1.0);
}
