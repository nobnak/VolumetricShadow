Shader "Unlit/Depth" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
		_ColorSub ("Sub Color", Color) = (1,1,1,1)
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
			float3 eyePos : TEXCOORD1;
		};

		sampler2D _MainTex;
		float4 _MainTex_ST;
		float4 _Color;
		float4 _ColorSub;
		float4x4 _NDCEyeMat;
		float4x4 _ShadowCamToWorldMat;

		v2f vert (appdata v) {
			float2 uvFromBottm = TRANSFORM_TEX(v.uv, _MainTex);

			float ze = tex2Dlod(_MainTex, float4(uvFromBottm, 0, 0)).r;
			float3 eyePos = float3(mul(_NDCEyeMat, float4(v.vertex.xy, 0, 1)).xy, 1) * ze;
			eyePos = lerp(v.vertex.xyz, eyePos, v.vertex.z);

			float3 worldPos = mul(_ShadowCamToWorldMat, float4(eyePos, 1)).xyz;

			v2f o;
			o.vertex = mul(UNITY_MATRIX_VP, float4(worldPos.xyz, 1));
			o.uv = uvFromBottm;
			o.eyePos = eyePos;
			return o;
		}
		
		ENDCG

		Pass {
			Cull Front
			ZTest LEqual ZWrite Off
			Blend SrcAlpha One
			BlendOp RevSub

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			fixed4 frag (v2f i) : SV_Target {
				return _Color;
			}
			ENDCG
		}
	}
}
