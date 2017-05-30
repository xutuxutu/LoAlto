Shader "Custom/BackgroundShader" 
{
	Properties 
	{
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BumpTex ("NormalMap", 2D) = "bump" {}
		_SpecularColor("SpecularColor", Color) = (1,1,1,1)
		_SpecularPower("SpecularPower", Range(0, 300)) = 1
		_UpperColor("UpperColor", Color) = (1,1,1,1)
		_UpperPower("UpperPower", Range(0, 50)) = 1
	}
	SubShader 
		{

		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Custom
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BumpTex;

		float _AddNormal;
		fixed4 _SpecularColor;
		float _SpecularPower;

		float4 _UpperColor;
		float _UpperPower;

		struct Input 
		{
			float2 uv_MainTex;
			float2 uv_BumpTex;
			float3 viewDir;
			float3 worldNormal;
			INTERNAL_DATA
		};

		void surf (Input IN, inout SurfaceOutput o) 
		{
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Normal = UnpackNormal(tex2D(_BumpTex, IN.uv_BumpTex));
			o.Alpha = c.a;

			float spec = saturate(dot(o.Normal, IN.viewDir));
			spec = pow(spec, _SpecularPower);
			
			float3 upperColor = (pow(saturate(IN.worldNormal.y), _UpperPower)) * _UpperColor.rgb;
			o.Albedo = c.rgb + spec * _SpecularColor;
		}

		float4 LightingCustom(SurfaceOutput s, float3 lightDir, float3 viewDir, float atten)
		{
			return float4(0, 0, 0, 0);
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
