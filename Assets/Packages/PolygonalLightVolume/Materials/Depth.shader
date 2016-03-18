Shader "Unlit/Depth" {
	Properties {
		_Gain ("Gain", Float) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		ColorMask RGB

		CGINCLUDE
		#pragma target 5.0

		#include "UnityCG.cginc"
            #include "Assets/Packages/PolygonalLightVolume/Materials/UV.cginc"

		struct appdata {
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
		};

		struct v2f { 
			float4 vertex : SV_POSITION;
			float4 uv : TEXCOORD0;
		};

		float _Gain;

        sampler2D _CameraDepthTexture;

		v2f vert (appdata v) {
			v2f o;
			o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
			o.uv = float4(v.uv, UVAtBottom(v.uv));
			return o;
		}

		float4 frag (v2f i) : SV_Target {
            float zfar = LinearEyeDepth(tex2D(_CameraDepthTexture, i.uv.zw).x);
            return zfar * _Gain;
		}
		ENDCG

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}
}
