//
// Textures and Samplers
//

// An override to override the dot product of normals with 1.0, to achieve solid lighting
// when wireframe is on
float WireframeNormalOverride : register(c0);


sampler2D DaySampler : register(s0);
sampler2D NightSampler : register(s1);
sampler2D NightLightsSampler : register(s2);
sampler2D NormalSampler : register(s3);
sampler2D MaskSampler : register(s4);


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

static const float AmbientIntensity = 1.0f;
static const float3 AmbientColor = float3(0,0,0);
static const float3 LampColor = float3(1,1,1); //float3(1, 0.77, 0.47);
static const float DiffuseIntensity = 1.2f;
static const float SpecularIntensity = 0.25f;
static const float SpecularPower = 10.0f;
static const float LandBumpFactor = 4.0f;
static const float WaterBumpFactor = 2.0f;

//
// Pixel Shader
//
float4 main(VertexShaderOutput input) : COLOR 
{
	// Sample textures
    float3 daySample = tex2D(DaySampler, input.UV).rgb;
    float3 nightSample = tex2D(NightSampler, input.UV).rgb;	
	float3 lightSample = tex2D(NightLightsSampler, input.UV).rgb;
	float3 maskSample = tex2D(MaskSampler, input.UV).rgb;
	float2 normalSample = tex2D(NormalSampler, input.UV).rg;
	
	// Normalize vertex normals
    float3 Nn = normalize(input.WorldNormal);
    float3 Tn = normalize(input.WorldTangent);
    float3 Bn = normalize(input.WorldBinormal);

    // Apply bump map
    float2 landPerturbation = LandBumpFactor * (normalSample - (0.5).xx) * (1.0 - maskSample.r);
    float2 waterPerturbation = WaterBumpFactor * (normalSample - (0.5).xx) * maskSample.r;
    float2 perturbation = landPerturbation + waterPerturbation;
    Nn += (perturbation.r * Tn + perturbation.g * Bn);
    Nn = normalize(Nn);
    //Nn = (2 * normalSample) - 1.0;

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
    float3 specular = pow(litVec.z, 1.5) * saturate(maskSample.r + 0.3) * LampColor;
    float3 day = saturate(litVec.y) * LampColor * daySample;
    float nightDiffuse = saturate(-ldn-0.1); //saturate(1 - litVec.y);
    float3 night = nightDiffuse * LampColor * nightSample + nightDiffuse * LampColor * lightSample; 

    // Final color calculation
    float3 result = ambient + day + night + specular;

    //result += saturate(4*(-ldn-0.1)) * nightSample;	

    return float4(result.rgb, 1.0);
}