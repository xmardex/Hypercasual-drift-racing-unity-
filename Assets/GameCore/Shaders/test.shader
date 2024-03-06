Shader "Unlit/Darkness"
{
	Properties
	{
		_Intensity ("Intensity", float) = 0.35
		_Offset ("Offset", Range(-1, 0)) = -0.15
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent-300" }

		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off

		Pass
		{
			Stencil
			{
				Ref 1
				ReadMask 1
				WriteMask 1
				Comp Always
				Pass Replace
				ZFail Replace
			}
			
			Cull Back

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 screenPos : TEXCOORD0;
				float3 wpos : TEXCOORD1;
			};
			
			sampler2D_float _CameraDepthTexture;
			float _Intensity, _Offset;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.wpos = mul(UNITY_MATRIX_M, v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, i.screenPos));
				
				float f = (depth - distance(_WorldSpaceCameraPos, i.wpos)) * _Intensity + _Offset;
				
				return half4(0, 0, 0, f);
			}
			ENDHLSL
		}
		Pass
		{
			Stencil
			{
				Ref 1
				ReadMask 1
				WriteMask 1
				Comp NotEqual
				Pass Zero
			}

			Cull Front
			ZTest Always
			
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 screenPos : TEXCOORD0;
				float3 wpos : TEXCOORD1;
			};

			sampler2D_float _CameraDepthTexture;
			float _Intensity, _Offset;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.wpos = mul(UNITY_MATRIX_M, v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float d1 = distance(_WorldSpaceCameraPos, i.wpos);
				float d2 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, i.screenPos));
				return half4(0, 0, 0, min(d1, d2) * _Intensity + _Offset);
			}
			ENDHLSL
		}
	}
}