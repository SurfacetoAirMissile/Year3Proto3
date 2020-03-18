Shader "Custom/hologram"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Pattern("Pattern", 2D) = "white" {}
		_SpeedX("Speed X", Float) = 0.0
		_SpeedY("Speed Y", Float) = 0.0
		_Value("Value",Range(0.0,128.0)) = 1.0

	}
		SubShader
		{
			Tags
		{
			"RenderType" = "Transparent" 
			"Queue" = "Transparent"
		}
			Blend One One
			LOD 100
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
					float4 vertex : SV_POSITION;
					half2 patternUV : TEXCOORD1;
					float4 color : COLOR;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				half _Value;
				sampler2D _Pattern;
				float4 _Pattern_ST;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					o.patternUV = TRANSFORM_TEX(v.uv, _Pattern);
					o.color = v.color;
					return o;
				}

				half _SpeedY;
				half _SpeedX;

				fixed4 frag(v2f i) : SV_Target
				{
					half4 col = tex2D(_MainTex, i.uv);
					half4 main = tex2D(_MainTex, i.patternUV);
					float2 offset = frac(_Time.y * float2(_SpeedX, _SpeedY));
					half4 pattern = tex2D(_Pattern, i.patternUV + offset);

					col.rgb *= i.color.rgb;
					col.a *= i.color.a;

					return col * pattern * _Value * i.color.a;
				}
				ENDCG
			}
		}
}
