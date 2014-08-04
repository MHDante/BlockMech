//Shader "Custom/FirstShader" {
//	Properties {
//		_MainTex ("Base (RGB)", 2D) = "white" {}
//		//_Increment ("Increment", Float) = 4f
//	}
//	SubShader {
//		Tags { "RenderType"="Opaque" }
//		//LOD 200
//		ZWrite Off
//		//Blend SrcAlpha OneMinusSrcAlpha
//		//Blend One One
//		CGPROGRAM
//		#pragma surface surf Lambert
//
//		sampler2D _MainTex;
//		//float _Increment;
//
//		struct Input {
//			float2 uv_MainTex;
//			//float3 worldPos;
//		};
//
//		void surf (Input IN, inout SurfaceOutput o) {
//			//half4 c = tex2D (_MainTex, IN.uv_MainTex);
//			//float brightness = 1f;
//			//float increment = 4f;
//			
//				o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
//				return;
//			//if (fmod(IN.worldPos.x, increment) < 0.1f || fmod(IN.worldPos.y, increment) < 0.1f)
//				//o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
//				//o.Albedo = half4(brightness,brightness,brightness,1);//c.rgb;
//			//else
//			//{
//				//o.Albedo = c.rgb;//half4(0,0,0,0);
//				//o.Alpha = 0;//c.a;
//			//}
//		}
//		ENDCG
//	} 
//	FallBack "Diffuse"
//}
Shader "Custom/Grid"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		_gridWith ("gridWith", Float) = 16
		_gridHeight ("gridHeight", Float) = 12
		_blockSize ("blockSize", Float) = 4
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct appdata_t members worldPos)
#pragma exclude_renderers d3d11 xbox360
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile DUMMY PIXELSNAP_ON
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
				float4 worldpos;
			};
			
			fixed4 _Color;
			float _gridWith;
			float _gridHeight;
			float _blockSize;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				float4 wat = IN.vertex;
				OUT.vertex = mul(UNITY_MATRIX_MVP, wat);
				
				OUT.worldpos = mul(_Object2World, IN.vertex);

				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif
				return OUT;
			}

			sampler2D _MainTex;

			float colFactor(float alpha, float value)
			{
				return alpha * fmod(value + _Time.w, 1f);
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				float increment = 4f;
				float4 wp = IN.worldpos;
				fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
				if (wp.x < 0 || wp.y < 0 || wp.x > _gridWith * _blockSize || wp.y > _gridHeight * _blockSize)
				{
					c.rgba *= 0;
					return c;
				}
				else if (fmod(wp.x, increment) < 0.2f)
				{
					//c.r = fmod(wp.y / 100f + wp.x + _Time.x, 1f);
					//c.g = fmod(2 *wp.y/ 100f + wp.x + _Time.x, 1f);
					//c.b = fmod(3 *wp.y/ 100f - wp.x + _Time.x, 1f);
					//c.rgb *= fmod(c.rgb + wp.x + _Time.x, 1f);
					//c.rgb *= c.a * (_SinTime.w * 0.2f) + 0.5f;
					//c.rgb *= c.a * fmod(wp.y + _Time.w, 1f);
					c.rgb *= colFactor(c.a, wp.y);
					return c;
				}
				else if (fmod(wp.y, increment) < 0.2f)
				{
					//c.r = fmod(wp.x / 100f * wp.y + _Time.x, 1f);
					//c.g = fmod(2 *wp.x/ 100f + wp.y + _Time.x, 1f);
					//c.b = fmod(3 *wp.x/ 100f - wp.y + _Time.x, 1f);
					//c.rgb *= fmod(c.rgb + wp.y + _Time.x, 1f);
					//c.rgb *= c.a * (_SinTime.w * 0.2f) + 0.5f;
					//c.rgb *= c.a * fmod(wp.x + _Time.w, 1f);
					c.rgb *= colFactor(c.a, wp.x);
					return c;
				}
				else
				{
					c.rgba *= 0;
					return c;
				}
				
			}
		ENDCG
		}
	}
}
