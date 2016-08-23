Shader "Custom/Tunnel" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_ColorR("R", Color) = (1, 1, 1, 1)
		_ColorG("G", Color) = (1, 1, 1, 1)
		_ColorB("B", Color) = (1, 1, 1, 1)
		_UVOffset("UV Offset", Vector) = (0, 0, 0, 0)
		_Direction("Direction", Vector) = (0, 0, 0, 0)
		_Displacement("Displacement", Vector) = (0, 0, 0, 0)
	}
	SubShader{
		Pass{
			CGPROGRAM
#pragma target 3.0
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

			struct vertexInput {
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct fragmentInput {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			uniform sampler2D _MainTex;
			uniform float3 _ColorR;
			uniform float3 _ColorG;
			uniform float3 _ColorB;
			uniform float2 _UVOffset;
			uniform float4 _Direction;
			uniform float4 _Displacement;

			fragmentInput vert(vertexInput i) {
				float4 pos = mul(_Object2World, i.pos);
				fragmentInput o;
				float d = pow(dot(_Direction, pos), 2);
				pos += d * _Displacement;
				o.pos = mul(UNITY_MATRIX_VP, pos);
				o.uv = i.uv + _UVOffset;
				return o;
			}

			fixed4 frag(fragmentInput i) : SV_Target{
				float4 text = tex2D(_MainTex, i.uv);
				float4 color = float4(0, 0, 0, 1);
				color.rgb = (text.r * _ColorR + text.g * _ColorG + text.b * _ColorB);
				return color;
			}

			ENDCG
		}
	}
}