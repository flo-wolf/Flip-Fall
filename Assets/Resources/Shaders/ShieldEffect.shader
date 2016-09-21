Shader "Custom/ShieldEffect" {
	Properties{
		_Position("Collision", Vector) = (-1, -1, -1, -1)
		_MaxDistance("Effect Size", float) = 40
		_ShieldColor("Color (RGBA)", Color) = (0.7, 1, 1, 0)
		_EmissionColor("Emission color (RGBA)", Color) = (0.7, 1, 1, 0.01)
		_EffectDistance("Effect Distance", float) = 0
		_EffectRadius("Effect Radius", float) = 0
	}

		SubShader{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 2000
		Cull Off

		CGPROGRAM
#pragma surface surf Lambert vertex:vert alpha
#pragma target 3.0

	struct Input {
		float customDist;
	};

	float4 _Position;
	float _MaxDistance;
	float4 _ShieldColor;
	float4 _EmissionColor;
	float _EffectDistance;
	float _EffectRadius;

	float _Amount;

	void vert(inout appdata_full v, out Input o) {
		o.customDist = distance(_Position.xyz, v.vertex.xyz);
	}

	void surf(Input IN, inout SurfaceOutput o) {
		o.Albedo = _ShieldColor.rgb;
		o.Emission = _EmissionColor;

		if (_EffectDistance < _EffectRadius)
		{
			if (IN.customDist < _MaxDistance) {
				o.Alpha = (1 - (_EffectDistance / _EffectRadius)) - (IN.customDist / _MaxDistance) + _ShieldColor.a;
				if (o.Alpha < _ShieldColor.a) {
					o.Alpha = _ShieldColor.a;
				}
			}
			else {
				o.Alpha = _ShieldColor.a;
			}
		}
		else {
			o.Alpha = o.Alpha = _ShieldColor.a;
		}
	}

	ENDCG
	}
		Fallback "Transparent/Diffuse"
}