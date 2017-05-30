Shader "Custom/OutLine" {
	Properties 
	{
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_RimPower ("RimPower" , range(1,10)) = 3
		_RimColor ("RimColor" , color ) = (1,1,1,1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM

		#pragma surface surf RimKJG 

		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BumpTex;

		struct Input 
		{
			float2 uv_MainTex;
			float3 worldNormal;
			float3 viewDir;			
		};
		

			float _RimPower;
			float3 _RimColor;

		void surf (Input IN, inout SurfaceOutput o) {
		
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			
			/*
			float3 rimCol = float3 (0,0,0);
			float rim = dot(normalize(IN.viewDir),normalize(IN.worldNormal));
			rim = 1-rim;
			rim = pow(rim, _RimPower);
			rimCol = rim * _RimColor;
			o.Emission = rimCol * abs(sin(_Time.y)) * abs(sin(_Time.y));
			*/
			o.Albedo = c.rgb;
			o.Alpha = 1;
		}
		
		float4 LightingRimKJG(SurfaceOutput s , float3 lightDir, float3 viewDir, float atten){
		
			float NdotL = dot(normalize(lightDir), normalize(s.Normal));
			float VdotL = dot(normalize(viewDir), normalize(s.Normal));

			float4 returnValue;
			returnValue.rgb = float4(s.Albedo * _LightColor0.rgb * NdotL * atten, 1);		

			if (VdotL < 0.2)
			{
				float x = sin(_Time.y * 2) * 0.5 + 0.5;
				returnValue.rgb = lerp(returnValue.rgb, _RimColor, x);
			}
			return returnValue;
		}
		ENDCG
	} 

	FallBack "Diffuse"
}
