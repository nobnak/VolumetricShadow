Shader "Unlit/Airlight" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
		_Gain ("Gain", Float) = 1
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		//Tags { "RenderType"="Opaque" }
		ColorMask RGB

		CGINCLUDE
		#pragma target 5.0

		#include "UnityCG.cginc"

		// vertex (NDC Coord) : (-1, -1, 0) -> (1, 1, 0)
		// uv : (Depth Sampler) : (0, 0) -> (1, 1)
		struct appdata {
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
		};

		struct v2f { 
			float4 vertex : SV_POSITION;
			float2 uv : TEXCOORD0;
			float z : TEXCOORD1;
		};

		sampler2D _MainTex;
		float4 _MainTex_ST;
		float4 _Color;
		float _Gain;

		float4x4 _LightVolumeMat;
		float4x4 _ShadowCamToWorldMat;

		sampler2D _CameraDepthTexture;

		v2f vert (appdata v) {
			float2 uv = TRANSFORM_TEX(v.uv, _MainTex);

			float ze = tex2Dlod(_MainTex, float4(uv, 0, 0)).r;
			float3 lighteyePos = float3(mul(_LightVolumeMat, float4(v.vertex.xy, 0, 1)).xy, 1) * ze;
			lighteyePos = lerp(v.vertex.xyz, lighteyePos, v.vertex.z);

			float3 worldPos = mul(_ShadowCamToWorldMat, float4(lighteyePos, 1)).xyz;
			float3 eyePos = mul(UNITY_MATRIX_V, float4(worldPos, 1)).xyz;

			v2f o;
			o.vertex = mul(UNITY_MATRIX_P, float4(eyePos, 1));
			o.uv = 0.5 * (o.vertex.xy / o.vertex.w + 1.0);
			if (_ProjectionParams.x < 0)
				o.uv.y = 1.0 - o.uv.y;
			o.z = -eyePos.z;
			return o;
		}

		float4 frag (v2f i) : SV_Target {
			float depth = tex2D(_CameraDepthTexture, i.uv).x;
			float zeye = LinearEyeDepth(depth);
			float4 c = _Color;
			return c * (_Gain * max(i.z, 0));
		}
		ENDCG

		Pass {
			Cull Back
			ZTest Always ZWrite Off
			Blend One One
			BlendOp RevSub

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
		Pass {
			Cull Front
			ZTest Always ZWrite Off
			Blend One One
			BlendOp Add

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}
}
