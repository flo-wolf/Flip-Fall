// simple "dissolving" shader by genericuser (radware.wordpress.com)
// clips materials, using an image as guidance.
// use clouds or random noise as the slice guide for best results.
Shader "Custom/Stencil/Dissolve"
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Texture (RGB)", 2D) = "white" {}
		_SliceGuide("Slice Guide (RGB)", 2D) = "white" {}
		_SliceAmount("Slice Amount", Range(0.0, 1.0)) = 0.5
		_Scale("Texture Scale", Float) = 1.0
	}

		SubShader
		{
			Tags { "RenderType" = "Transparent" "Queue" = "Geometry-1" "IgnoreProjector" = "True" }
			ZWrite Off
			ZTest Less
			Lighting Off
			LOD 200
			Blend One OneMinusSrcAlpha

			Stencil
		{
			Ref 1
			Comp always
			Pass replace
		}

			CGPROGRAM

			//if you're not planning on using shadows, remove "addshadow" for better performance
			#pragma surface surf Lambert alpha:blend

			fixed4 _Color;
			sampler2D _MainTex;
			sampler2D _SliceGuide;
			float _SliceAmount;
			float _Scale;

			struct Input
			{
				fixed4 _Color;
				float2 uv_MainTex;
				float2 uv_SliceGuide;
				float _SliceAmount;

				// used for creating worldposition textures
				float3 worldNormal;
				float3 worldPos;
			};

			void surf(Input IN, inout SurfaceOutput o)
			{
				float2 UV;
				fixed4 c;

				if (abs(IN.worldNormal.x) > 0.5)
				{
					UV = IN.worldPos.yz; // side
					c = tex2D(_SliceGuide, UV* _Scale); // use WALLSIDE texture
				}
				else if (abs(IN.worldNormal.z) > 0.5)
				{
					UV = IN.worldPos.xy; // front
					c = tex2D(_SliceGuide, UV* _Scale); // use WALL texture
				}
				else
				{
					UV = IN.worldPos.xz; // top
					c = tex2D(_SliceGuide, UV* _Scale); // use FLR texture
				}

				o.Emission = _Color.rgb;
				o.Alpha = _Color.a;

				clip(c - _SliceAmount);

				// use albedo if lighting/reflection is neccessary

				//o.Alpha = tex2D(_MainTex, IN.uv_MainTex).a;
				//o.Alpha = tex2D(_MainTex, IN.uv_MainTex).a * (1 - _SliceAmount);
			}

			ENDCG
		}
			Fallback "VertexLit"
}