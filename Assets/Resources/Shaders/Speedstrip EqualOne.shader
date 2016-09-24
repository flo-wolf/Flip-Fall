// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/Attractor EqualOne" {
	Properties{
		// color settings
		_Color("Default Color, not in radius (RGBA)", Color) = (0.7, 1, 1, 1)
		_AttractedColor("Attracted Color (RGBA)", Color) = (0.7, 1, 1, 1)
		_ColorMulti("Attracted Color Multiplier", float) = 1

		// updated when player collides with an Attractor
		_AttractorCenter("Attractor Center", Vector) = (-1, -1, -1, -1)
		_AttractorRadius("Attractor Radius", float) = 100
		_PlayerDistance("Player distance to Attractor", float) = 500
	}

		SubShader{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" "DisableBatching" = "True" }
		LOD 2000
		Cull Off
			ZWrite Off
			Lighting Off

		Stencil
	{
		Ref 1
		Comp equal
		Pass keep
	}

		CGPROGRAM
#pragma surface surf Lambert vertex:vert alpha
#pragma target 3.0

	struct Input {
		float customDist;
		fixed4 color;
		float3 worldPos;
		float3 worldNormal;

		//float4 vertex : POSITION;
		//float2 uv : TEXCOORD0;
	};

	float4 _Color;
	float4 _AttractedColor;
	float4 _AttractionCenter;
	float _PlayerDistance;
	float _AttractorRadius;
	float _ColorMulti;

	void vert(inout appdata_full v, out Input o)
	{
		//_Color = float4(0.7, 0.7, 0.7, 1);
		o.customDist = distance(_AttractionCenter.xyz, v.vertex.xyz);
		UNITY_INITIALIZE_OUTPUT(Input, o);
		o.color = _Color;

		//o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
		//// Gets the xy position of the vertex in worldspace.
		//float2 worldXY = mul(unity_ObjectToWorld, v.vertex).xy;

		//// Use the worldspace coords instead of the mesh's UVs.
		//o.uv = TRANSFORM_TEX(worldXY, _AttractorTex);
	}

	void surf(Input IN, inout SurfaceOutput o)
	{
		o.Albedo = _Color.rgb;
		o.Emission = _Color;
		o.Alpha = 1;

		float2 uv = IN.worldPos.xy;

		//if (IN.worldPos == _AttractionCenter)
		//	uv

		if (_PlayerDistance < _AttractorRadius)
		{
			//o.Alpha = (1 - (_EffectDistance / _EffectRadius)) + _AttractorColor.a;
			//o.Emission = _AttractorColor.rgb
			o.Albedo = _Color.rgb + ((1 - (_PlayerDistance / _AttractorRadius)) * _AttractedColor.rgb) * _ColorMulti;
			o.Emission = _Color.rgb + ((1 - (_PlayerDistance / _AttractorRadius)) * _AttractedColor.rgb) * _ColorMulti;
			//o.Alpha = 0 + ((1 - (_PlayerDistance / _AttractorRadius)) * _AttractedColor.a) * _ColorMulti;
		}
		else
		{
			//o.Alpha = o.Alpha = _MoveZoneColor.a;
		}
	}

ENDCG
	}
		Fallback "Transparent/Diffuse"
}