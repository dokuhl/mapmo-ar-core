Shader "Custom/Clip" {
	Properties{
		_MainTex("Texture", 2D) = "white" {}
		_BumpMap("Bumpmap", 2D) = "bump" {}
		_Cutoff("Cutoff", float) = 100
	}
	SubShader{
		Tags{ "RenderType" = "Opaque" }
		Cull Off
		
			
		CGPROGRAM
		#pragma surface surf Lambert

		half4 LightingSimpleLambert(SurfaceOutput s, half3 lightDir, half atten) {
			half NdotL = dot(s.Normal, lightDir);
			half4 c; 
			c.rgb = s.Albedo *_LightColor0.rgb * (NdotL * atten);
			c.a = 0.5;
			return c;
		}

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float3 worldPos;
		};
		sampler2D _MainTex;
		sampler2D _BumpMap;
		float _Cutoff;
		void surf(Input IN, inout SurfaceOutput o) {
			clip((IN.worldPos.x*IN.worldPos.x + IN.worldPos.z*IN.worldPos.z < _Cutoff * _Cutoff) ? 1 : -1);
			o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
			o.Alpha = 0.8;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			//o.uv = TRANSFORM_TEX(IN.uv, _MainTex);
		}
		ENDCG
		

			
	}
	Fallback "Diffuse"
}