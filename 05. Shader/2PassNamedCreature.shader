Shader "Custom/2PassNamedCreature" 
{
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
	}
	SubShader 
		{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		cull front
		
		CGPROGRAM
		#pragma surface surf Custom vertex:vert noambient
		#pragma target 3.0

		sampler2D _MainTex;
		fixed4 _Color;

		void vert(inout appdata_full v)
		{
			//v.vertex.xyz += v.normal * 0;
		}

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutput o) { }
		
		float4 LightingCustom(SurfaceOutput s, float3 lightDir, float atten)
		{
			return float4(1,0,0,0);
		}
		ENDCG
		
		cull back
		CGPROGRAM
		#pragma surface surf Custom
		#pragma target 3.0

		sampler2D _MainTex;
		fixed4 _Color;

		struct Input 
		{
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) 
		{
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}

		float4 LightingCustom(SurfaceOutput s, float3 lightDir, float3 viewDir, float atten)
		{
			float NdotV = dot(s.Normal, viewDir);

			float rim = 1 - abs(NdotV);
			rim = pow(rim, 10) * 10;

			float4 final = 0;
			final.rgb += rim;
			return final * float4(1,0,0,0);
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
