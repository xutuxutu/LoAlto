Shader "Custom/Char_2"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_BumpTex("NormalMap", 2D) = "bump"{}
		_SpecTex("Specular Map", 2D) = "white" {}
		_Brightnees("Brightnees", Range(0.1,5)) = 1
		[Toggle] _isSpec("existSpecular", Float) = 0
		
		_SpecularColor("Specular Color", Color) = (1,1,1,1)
		_SpecMulti("SpecPower", Range(1,100)) = 1
		_SpecPow("SpecRange", Range(30,300)) = 1

		_ViewSpecularColor("View Specular Color", Color) = (1,1,1,1)
		_ViewSpecMulti("View Specular Multi", Range(0.1,5)) = 1
		_ViewSpecPowRange("View SpecPow Range", Range(30,300)) = 10

		_upperColor("upperColor" , color) = (0,0,0,0)
		_upperPower("upperPower", Range(1,30)) = 1

		_rimpower("RimPower", float) = 1
		_RimColor("RimColor", Color) = (1,1,1,1)
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
		sampler2D _SpecTex;

		float _Brightnees;
		float _rimpower;
		float4 _RimColor;
		float4 _upperColor;
		float _upperPower;
	
		float _isSpec;
		float _SpecPow;
		float4 _SpecularColor;
		float4 _ViewSpecularColor;
		float _ViewSpecMulti;
		float _SpecChannel;
		float _SpecMulti;
		float _ViewSpecPowRange;

		float3 worldNor;
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
			//texture
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Normal = UnpackNormal(tex2D(_BumpTex,IN.uv_BumpTex));
			fixed4 SpecMap = tex2D(_SpecTex, IN.uv_SpecTex);

			o.Albedo = c.rgb * _Brightnees;
			o.Alpha = c.a;
			_SpecChannel = SpecMap.r;
			worldNor = WorldNormalVector(IN, o.Normal);
		}

		float4 LightingLambertjp(SurfaceOutput s, float3 lightDir, float3 viewDir, float atten)
		{
			float3 halfVec = normalize(s.Normal + viewDir);
			float NdotL = saturate(dot(s.Normal, lightDir));
			float spec = dot(halfVec, viewDir);
			spec = pow(spec, _SpecPow);

			float rim = dot(viewDir, s.Normal);
			float viewSpec = saturate(rim);

			rim = 1 - abs(rim);
			rim = pow(rim, _rimpower);

			viewSpec = pow(viewSpec, _ViewSpecPowRange);
	
			float3 upperColor = (pow(saturate(worldNor.y), _upperPower)) * _upperColor.rgb;

			float4 final = float4(0, 0, 0, 0);
			final.rgb = s.Albedo * NdotL * _LightColor0 * atten;
			final.rgb += saturate(s.Albedo.rgb * rim * _RimColor) * 5 + upperColor;
			if (_isSpec == 1)
			{
				final.rgb += viewSpec * _ViewSpecularColor * _SpecChannel * _ViewSpecMulti;
				final.rgb += spec * _SpecularColor * _SpecChannel * _SpecMulti;
			}
			return final;
		}
	ENDCG
	}
	FallBack "Diffuse"
}