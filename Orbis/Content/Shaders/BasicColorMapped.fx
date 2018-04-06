#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

uniform float4 FogColor;
uniform float FogEndDistance;
uniform float ColorInfluence;
uniform matrix WorldView;
uniform matrix Projection;
uniform Texture2D MainTexture;
uniform Texture2D ColorMapTexture;

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float4 UV : TEXCOORD0;
	float4 UV2: TEXCOORD1;
	float4 Color : COLOR0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 UV : TEXCOORD0;
	float4 UV2: TEXCOORD1;
	float4 Color : COLOR0;
	float FogFactor : BLENDWEIGHT0;
};

sampler DiffuseSampler = sampler_state
{
	Texture = <MainTexture>;
	mipFilter = LINEAR;
};

sampler ColorSampler = sampler_state
{
	Texture = <ColorMapTexture>;
	mipFilter = LINEAR;
};

float CalculateFog(float distance)
{
	float d = distance / FogEndDistance;
	return clamp(d, 0, 1);
}

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;
	float4 viewPos = mul(input.Position, WorldView);
	output.Position = mul(viewPos, Projection);
	output.Color = input.Color;
	output.UV = input.UV;
	output.UV2 = input.UV2;
	output.FogFactor = CalculateFog(length(viewPos));

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 diffuse = tex2D(DiffuseSampler, input.UV).rgba;
	float4 color = mul(float4(tex2D(ColorSampler, input.UV2).rgb, 0), ColorInfluence);
	return lerp(lerp(diffuse, input.Color, color), FogColor, input.FogFactor);
}

technique DefaultTechnique
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};