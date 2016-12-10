Shader "Custom/LevelObjects/Bouncer EqualOne"
{
	// blend in second color by interpolating between 0 and 1
	Properties
	{
		_Color("Untouched Tint", Color) = (1,1,1,1)
		_ColorTouch("Touching Tint", Color) = (1,1,1,1)
		_Blend("Is the Player on the Strip?",  Range(0, 1)) = 0.0

		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
	}

		SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Stencil
	{
		Ref 1
		Comp equal
		Pass keep
	}

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
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
				fixed4 color : COLOR;
				float2 texcoord  : TEXCOORD0;
			};

			fixed4 _Color;
			fixed4 _ColorTouch;
			float _Blend;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color *  lerp(_Color, _ColorTouch, _Blend);
				//OUT.color = IN.color * _Color * _Color2 * _BlendAmount;
				//OUT.color.rgb = IN.color.rgb * _Color.rgb * _Color2.rgb * _BlendAmount;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap(OUT.vertex);
				#endif

				return OUT;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = IN.color;
			//c.a = _Color.a;
			return c;
		}
	ENDCG
	}
	}
}