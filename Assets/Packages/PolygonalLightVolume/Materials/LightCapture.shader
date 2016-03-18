Shader "Hidden/LightCapture" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_Scale ("Scale", Float) = 1
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
			float _Scale;

			sampler2D _CameraDepthTexture;

			fixed4 frag (v2f i) : SV_Target {
				float dn = tex2D(_CameraDepthTexture, i.uv.zw).r;
				float d = Linear01Depth(dn);
				return d * _Scale;
			}
			ENDCG
		}
	}
}
