Shader "Custom/Char_Specular_Named"
{
	Properties
	{
		_OutLineColor("OutLineColor", Color) = (1,1,1,1)
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_BumpTex("NormalMap", 2D) = "bump"{}
		_SpecTex("Specular Map", 2D) = "white" {}
		_Brightnees("Brightnees", Range(0.1,10)) = 1
		[Toggle] _isSpec("existSpecular", Float) = 0
		
		_SpecularColorR("Specular Color_Red", Color) = (1,1,1,1)
		_SpecPowR("SpecPow_Red", Range(40,500)) = 50
		_SpecMultiR("Specular Multi_Red", Range(0.1,30)) = 1

		_SpecularColorG("Specular Color_Green", Color) = (1,1,1,1)
		_SpecPowG("SpecPow_Green", Range(40,500)) = 50
		_SpecMultiG("Specular Multi_Green", Range(0.1,30)) = 1

		_ViewSpecularColor("View Specular Color", Color) = (1,1,1,1)
		_ViewSpecPow("View SpecPow Range", Range(30,300)) = 10
		_ViewSpecMulti("View Specular Multi", Range(0.1,30)) = 1

		_upperColor("upperColor" , color) = (0,0,0,0)
		_upperPower("upperPower", Range(1,30)) = 1

		_rimRange("RimRange", Range(1, 30)) = 1
		_rimpower("RimPower", Range(1, 150)) = 1
		_RimColor("RimColor", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "IgnoreProjector" = "true" }
		LOD 200


			cull front

			CGPROGRAM
#pragma surface surf Custom vertex:vert noambient
#pragma target 3.0

			sampler2D _MainTex;
		fixed4 _OutLineColor;

		void vert(inout appdata_full v)
		{
			//v.vertex.xyz += v.normal * 0.01;
		}

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutput o) { }

		float4 LightingCustom(SurfaceOutput s, float3 lightDir, float atten)
		{
			return _OutLineColor;
		}
		ENDCG

		cull off

		CGPROGRAM
#pragma surface surf Lambertjp   fullforwardshadows     
#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BumpTex;
		sampler2D _SpecTex;

		float _Brightnees;
		float _rimRange;
		float _rimpower;
		float4 _RimColor;
		float4 _upperColor;
		float _upperPower;
	
		float _isSpec;
		float _SpecPowR;
		float4 _SpecularColorR;
		float4 _ViewSpecularColor;
		float _ViewSpecMulti;
		float _SpecMultiR;
		float _ViewSpecPow;

		float _SpecPowG;
		float4 _SpecularColorG;
		float _SpecMultiG;

		float _SpecChannel_Red;
		float _SpecChannel_Green;

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
			_SpecChannel_Red = SpecMap.r;
			_SpecChannel_Green = SpecMap.g;
			worldNor = WorldNormalVector(IN, o.Normal);
		}

		float4 LightingLambertjp(SurfaceOutput s, float3 lightDir, float3 viewDir, float atten)
		{
			float3 halfVec = normalize(s.Normal + viewDir);
			float NdotV = dot(s.Normal, viewDir);
			float NdotH = dot(halfVec, viewDir);

			float rim = 1 - abs(NdotV);
			rim = pow(rim, _rimRange);

			float viewSpec = saturate(NdotV);
			viewSpec = pow(viewSpec, _ViewSpecPow);

			float spec_Red = pow(NdotH, _SpecPowR);
			float spec_Green = pow(NdotH, _SpecPowG);
	
			float3 upperColor = (pow(saturate(worldNor.y), _upperPower)) * _upperColor.rgb;

			float4 final = 0;
			final.rgb = rim * _RimColor * _rimpower + upperColor;
			if (_isSpec == 1)
			{
				final.rgb += viewSpec * _ViewSpecularColor * _ViewSpecMulti;
				final.rgb += spec_Green * _SpecularColorG * _SpecChannel_Green * _SpecMultiG + spec_Red * _SpecularColorR * _SpecChannel_Red * _SpecMultiR;
			}
			return final;
		}
	ENDCG
	}
	FallBack "Diffuse"
}