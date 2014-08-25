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
Shader "Custom/Circular"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
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
			float tri(float num, float mod)
			{
				//float a = mod - (abs(fmod(abs(num), (2 * mod)) - mod));
			    float a = fmod(abs(num), (2 * mod));
			    float b = a - mod;
			    float c = abs(b);
			    float d = mod - c;
			    return d;
			}
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
			v2f vert(appdata_t IN)
			{
				v2f OUT;
				float4 wat = IN.vertex;
				OUT.vertex = mul(UNITY_MATRIX_MVP, wat);
				
				OUT.worldpos = mul(_Object2World, IN.vertex);

				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				return OUT;
			}

			sampler2D _MainTex;

			fixed4 frag(v2f IN) : SV_Target
			{
				float increment = 4f;
				float deadzone = 0.2f;
				float4 wp = IN.worldpos;
				fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
				
				//half2 center = float2(0.25,0.25);
				half random = frac(_Time);
				half2 center = float2(0.25, 0.5);
				half2 dir = IN.texcoord - center;
				half len = tri(length(dir) - _Time.x * 10f, 1f);
				//len = (fmod(IN.texcoord.x, 2 == 0) ? 1f : 0f;
				//len = a;
				//c.a = len / 1f;
				c.rgba = float4(len, len,len, 1f);
				return c;
			}
		ENDCG
		}
	}
}
