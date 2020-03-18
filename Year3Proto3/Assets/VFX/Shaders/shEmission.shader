Shader "Custom/Sparks"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Pattern("MultiplyPanner", 2D) = "white" {}
		_SpeedX("Hori Speed", Float) = 0.0
		_SpeedY("Vert Speed", Float) = 0.0
		_Value("Value",Range(0.0,128.0)) = 1.0
	}
		SubShader
		{
			Tags { "RenderType" = "Transparent" "Queue" = "Transparent"}
			LOD 100
			//Blend One One
			//Blend One OneMinusSrcAlpha
			Zwrite Off
			Cull off

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
					float4 color : COLOR;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					float uvPatt : TEXCOORD1;
					float4 vertex : SV_POSITION;
					float4 color : COLOR;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				float _Value;
				sampler2D _Pattern;
				float4 _Pattern_ST;

				//float4 _Color;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					o.uvPatt = TRANSFORM_TEX(v.uv, _Pattern);
					o.color = v.color;

					return o;
				}

				half _SpeedX;
				half _SpeedY;

				fixed4 frag(v2f i) : SV_Target
				{
					// sample the texture
					half4 col = tex2D(_MainTex, i.uv);
					half4 main = tex2D(_MainTex, i.uvPatt);
					float2 offset = frac(_Time.y * float2(_SpeedX, _SpeedY));
					half4 pattern = tex2D(_Pattern, i.uvPatt + offset);



					//col.rgb *= i.color.a;

					return col * pattern;
					//return col * _Value * pattern;

					//float uv = length(i.uv - .5);
					//return smoothstep(.5,0.4,uv)*smoothstep(.3,.4,uv)* _Value * i.color;
				}
				ENDCG
			}
		}
}