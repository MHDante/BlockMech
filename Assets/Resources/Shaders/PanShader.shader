Shader "Custom/Pan"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
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

			float tri(float num, float mod)
			{
				//float a = mod - (abs(fmod(abs(num), (2 * mod)) - mod));
			    float a = fmod(abs(num), (2 * mod));
			    float b = a - mod;
			    float c = abs(b);
			    float d = mod - c;
			    return d;
			}
			
			fixed4 _Color;

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

			fixed4 frag(v2f IN) : SV_Target
			{
				float increment = 4f;

				half x = tri(IN.texcoord.x - _Time.x, 1f);
				//half x = 1f - (abs(fmod(abs(IN.texcoord.x - _Time.y), (2 * 1f)) - 1f));

				//half y = tri(IN.texcoord.y + _Time.y, 1f);
				half2 coord = half2(x,IN.texcoord.y);
				fixed4 c = tex2D(_MainTex, coord) * IN.color;
				return c;
				//if (fmod(IN.worldpos.x, increment) < 0.2f || fmod(IN.worldpos.y, increment) < 0.2f){
				//	c.rgb *= c.a;
				//	return c;
				//}
				//else{
				//c.rgba *= 0;
				//return c;
				//}

				/*
				float increment = 4f;
				half x = fmod(IN.texcoord.x + _Time.y, 1f);
				half y = fmod( + _Time.y, 1f);
				half2 coord = (IN.texcoord.x,IN.texcoord.y);
				fixed4 c = tex2D(_MainTex, coord) * IN.color;
				return c;
				*/
				
			}
		ENDCG
		}
	}
}
