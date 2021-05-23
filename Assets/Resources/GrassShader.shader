Shader "Custom/GrassShader"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Main Texture", 2D) = "white" {}
		_AoTex("Color Map", 2D) = "white" {}
	}
		SubShader
	{
		Pass
		{
			CGPROGRAM

			#pragma vertex vertexFunc
			#pragma fragment fragmentFunc

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			#include "UnityCG.cginc"

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv0 : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
			};

			struct indata
			{
				float4 pos : POSITION;
				float2 uv0 : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
			};

			sampler2D _MainTex;
			sampler2D _AoTex;
			fixed4 _Color;

			v2f vertexFunc(indata IN)
			{
				v2f OUT;

				OUT.pos = UnityObjectToClipPos(IN.pos);
				OUT.uv0 = IN.uv0;
				OUT.uv1 = IN.uv1;

				return OUT;
			}

			fixed4 fragmentFunc(v2f IN) : SV_Target
			{
				fixed4 baseColor = tex2D(_MainTex, IN.uv0);
				return baseColor * tex2D(_AoTex, IN.uv1);
			}

			ENDCG
		}
	}
		FallBack "Diffuse"
}
