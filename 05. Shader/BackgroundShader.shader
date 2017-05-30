Shader "Custom/BackgroundShader" 
{
	Properties 
	{
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
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
		
		float4 _Color;
		float _AddNormal;
		fixed4 _SpecularColor;
		float _SpecularPower;

		float4 _UpperColor;
		float _UpperPower;

		struct Input 
		{
			float2 uv_MainTex;
			float3 viewDir;
			float3 worldNormal;
		};

		void surf (Input IN, inout SurfaceOutput o) 
		{
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Alpha = c.a;

			float spec = saturate(dot(o.Normal, IN.viewDir));
			spec = pow(spec, _SpecularPower);
			
			float3 upperColor = (pow(saturate(IN.worldNormal.y), _UpperPower)) * _UpperColor.rgb;
			o.Albedo = c.rgb * _Color + spec * _SpecularColor + upperColor;
		}

		float4 LightingCustom(SurfaceOutput s, float3 lightDir, float3 viewDir, float atten)
		{
			float NdotL = saturate(dot(lightDir, s.Normal));

			float4 final = 0;
			final.rgb += NdotL * s.Albedo * atten * _LightColor0;

			return final;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
