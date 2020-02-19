// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Hardlight" {
	Properties{
		_MainTex("RGB Texture", 2D) = "white" {}
		_Colour("Hardlight Colour", Color) = (0.14,0.39,0.71,1)
		_Alpha("Hardlight Opacity",Range(0,1)) = 0.5
		_Speed("Motion Speed (Gr/Bl)",Range(0,10)) = 3
		_Wobble("Wobble Magnitude (Gr/Bl)", Range(0,10)) = 0.4
	}

	SubShader{
		LOD 250
		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" "ForceNoShadowCasting" = "True" }
		Blend SrcAlpha OneMinusSrcAlpha
		Pass{
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "unitycg.cginc"

				sampler2D _MainTex;
				float4 _MainTex_ST;
				float4 _Colour;
				half _Alpha;
				half _Speed;
				half _Wobble;


				struct VertInput {
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct VertOutput {
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
				};

				VertOutput vert(VertInput i) {
					VertOutput o;
					o.pos = UnityObjectToClipPos(i.vertex);
					o.uv = TRANSFORM_TEX(i.uv,_MainTex);
					return o;
				}

				half4 frag(VertOutput i) : SV_Target{
					float4 tex;
					tex = float4(_Colour.rgb*0.9, 0);

					tex += tex2D(_MainTex, i.uv).r * float4(_Colour.rgb*1.3, tex2D(_MainTex, i.uv).r*6);
						
					if (tex2D(_MainTex, i.uv).r <= 0.8) {
						tex += tex2D(_MainTex, i.uv + float2(0.01*(i.uv.y / 20 + _Time[1] * _Speed), 0.01*cos(_Speed * _Time[1] * _Wobble))).g * float4(_Colour.rgb*0.5, 0.15);
						tex += tex2D(_MainTex, i.uv + float2(0.01*(i.uv.y / 20 + _Time[1] * _Speed*0.5), 0.01*sin(_Speed*0.3 * _Time[1] * _Wobble))).b * float4(_Colour.rgb*0.5, 0.1);
					}

					tex.a += 0.25;
					tex.a *= _Alpha * 2 * (1 + sin(_Time[1] * 2) / 5);
						
					return tex;
				}

			ENDCG
		}
	}
	FallBack "Diffuse"
}
