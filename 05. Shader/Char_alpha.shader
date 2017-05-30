Shader "Custom/Char"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_WarpTex("warptex)", 2D) = "white" {}
		_BumpTex("NormalMap", 2D) = "bump"{}
		_SpecTex("Specular Map", 2D) = "white" {}
		_SpecPower("Specular Multi", Range(1,5)) = 1
		_SpecPowRange("SpecPow Range", Range(1,50)) = 1
		_SpecularColor("Specular Color", Color) = (1,1,1,1)
		_emit("emitt" , Range(0,1)) = 0.5
		_upperColor("upperColor" , color) = (0,0,0,0)
		_upperPower("UpperrimPower", Range(1,10)) = 1
		_LightDir("CustomLight", Vector) = (0,0,0,0)

		_rimpower("림 파워", float) = 1
		_RimColor("림 컬러", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "IgnoreProjector" = "true" }
		LOD 200
		cull off

		CGPROGRAM
#pragma surface surf Lambertjp   fullforwardshadows     
#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BumpTex;
		sampler2D _WarpTex;
		sampler2D _SpecTex;

		float _rimpower;
		float4 _RimColor;
		float _emit;
		float4 _upperColor;
		float _upperPower;
		float4 _LightDir;
	
		float4 _SpecularColor;
		float _SpecChannel;
		float _SpecPower;
		float _SpecPowRange;
		//float _SpecPowRange;

		struct Input
		{
			float2 uv_MainTex;
			float2 uv_BumpTex;
			float2 uv_SpecTex;
			float3 worldNormal;
			float3 viewDir;
			INTERNAL_DATA
		};
	
		fixed4 _Color;

		void surf(Input IN, inout SurfaceOutput o)
		{
			//normal
			fixed4 n = tex2D(_BumpTex, IN.uv_BumpTex);
			o.Normal = UnpackNormal(tex2D(_BumpTex,IN.uv_BumpTex));
			
			//tex
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			//SpecTex
			fixed4 SpecMap = tex2D(_SpecTex, IN.uv_SpecTex);
			_SpecChannel = SpecMap.r;
	
			float3 rimCol = float3 (0,0,0);
			float rim = abs(dot(IN.viewDir, o.Normal));
			rim = pow(1 - rim,_rimpower);
			rimCol = rim * _RimColor;
	

			float3 worldNor = WorldNormalVector(IN,o.Normal);
			float3 upperColor = (pow(saturate(worldNor.y),_upperPower)) * _upperColor.rgb;
	
			o.Albedo = c.rgb;
			//o.Emission = saturate(c.rgb + rimCol)* _emit + upperColor;
			o.Emission = saturate(c.rgb * rimCol)* 5 + upperColor;
			
			o.Alpha = c.a;
		}

		float4 LightingLambertjp(SurfaceOutput s, float3 lightDir, float3 viewDir, float atten)
		{
			float NdotL = dot(normalize(lightDir), normalize(s.Normal));
			NdotL = NdotL * 0.5 + 0.5;
	
			float4 warp = tex2D(_WarpTex, float2(NdotL, 0.5));
	
			float3 reflection = normalize(reflect(_LightDir, s.Normal));
			float spec = saturate(dot(reflection, -viewDir));
			spec = pow(spec, _SpecPowRange) * _SpecChannel * _SpecPower;
	
			float4 final = float4(1, 1, 1, 1);
			final.rgb = NdotL * warp *s.Albedo *_LightColor0.rgb * atten + (spec * _SpecularColor);
	
			return (spec * _SpecularColor);
		}
	ENDCG
	}
	FallBack "Diffuse"
}