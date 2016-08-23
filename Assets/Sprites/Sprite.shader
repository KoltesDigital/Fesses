Shader "Custom/Sprite"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Direction("Direction", Vector) = (0, 0, 0, 0)
		_Displacement("Displacement", Vector) = (0, 0, 0, 0)
	}
	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}
		
		AlphaTest NotEqual 0.0
		Pass
		{
			Lighting Off
			Fog { Mode Off }
			Blend One OneMinusSrcAlpha


			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct vertexInput {
				float4 pos : POSITION;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
			};

			struct fragmentInput {
				float4 pos : SV_POSITION;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
			};

			uniform sampler2D _MainTex;
			uniform float4 _Direction;
			uniform float4 _Displacement;

			fragmentInput vert(vertexInput i) {
				float4 pos = mul(_Object2World, i.pos);
				fragmentInput o;
				float d = pow(dot(_Direction, pos), 2);
				pos += d * _Displacement;
				o.pos = mul(UNITY_MATRIX_VP, pos);
				o.uv = i.uv;
				o.color = i.color;
				return o;
			}

			fixed4 frag (fragmentInput i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv) * i.color;
				return col;
			}
			ENDCG
		}
	}
}
