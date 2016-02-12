//
// Textures and Samplers
//

float2 Size : register(c0);
float Brightness : register(c1);

sampler Turbulence1Sampler : register(s0);
sampler Turbulence2Sampler : register(s1);
sampler GradientSampler : register(s2);

/* data passed from vertex shader to pixel shader */
struct VertexShaderOutput {
    float4 Position	     : POSITION;
    float2 UV1		     : TEXCOORD0;
    float2 UV2		     : TEXCOORD1;
}
;
static float Weights[5][5] = {
		{ 0.003, 0.013, 0.022, 0.013, 0.003 },
		{ 0.013, 0.059, 0.097, 0.059, 0.013 },
		{ 0.022, 0.097, 0.159, 0.097, 0.022 },
		{ 0.013, 0.059, 0.097, 0.059, 0.013 },
		{ 0.003, 0.013, 0.022, 0.013, 0.003 }
	};

float ComputeLookup(float2 uv1, float2 uv2)
{
    // Sample turbulence textures
    float turb1Sample = tex2D(Turbulence1Sampler, uv1).r;
    float turb2Sample = tex2D(Turbulence2Sampler, uv2).r;
    // Add both
    float i = (turb1Sample + turb2Sample) - 1;
    return clamp(i, 0, 1);
}


//
// Pixel Shader
//
float4 main(VertexShaderOutput input) : COLOR 
{
    float4 result = 0;
    float2 uv1 = input.UV1;
    float2 uv2 = input.UV2;
    float2 off = 1.0 / Size;

    // Simple 5x5 gaussian blur
	for(int y = -2; y <= 2; y++)
	{
        for(int x = 2; x <= 2; x++)
		{
            uv1.x += off.x * x;
            uv1.y += off.y * y;
            uv2.x += off.x * x;
            uv2.y += off.y * y;
            
			// Sample turbulence textures
			float i = ComputeLookup(uv1, uv2);
            
			// Lookup gradient color
			float4 c = tex2D(GradientSampler, float2(i * 0.99, 0));
			
			// Blur
			result += c * Weights[x + 2][y + 2] * Brightness;
        } 
	}

	return clamp(result, 0, 1);
}