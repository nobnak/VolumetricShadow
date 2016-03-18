Shader "Hidden/EyeDepthBaker" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader { 
		Cull Off ZWrite Off ZTest Always

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
            #include "Assets/Packages/PolygonalLightVolume/Materials/UV.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float4 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v) {
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = float4(v.uv, UVAtBottom(v.uv));
				return o;
			}
			
			sampler2D _MainTex;
			float4x4 _LightVolumeMat;

			fixed4 frag (v2f i) : SV_Target {
				float zn = tex2D(_MainTex, i.uv.xy).r;
				float ze = 1.0 / mul(_LightVolumeMat, float4(0, 0, zn, 1)).z;
				return ze;
			}
			ENDCG
		}
	}
}
