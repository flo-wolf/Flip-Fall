Shader "Custom/WorldUVDiffuse" {
	Properties
	{
		_MainTex("Base", 2D) = "white" {}
	_BumpMap("Normalmap", 2D) = "bump" {}
	_Scale("Scale", float) = 1.0
	}

		SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 400

		CGPROGRAM
#pragma surface surf Lambert

	struct Input
	{
		float4 color : COLOR;
		float2 uv_MainTex;
		float2 uv_BumpMap;
		float3 worldPos;
		float3 worldNormal; INTERNAL_DATA
	};

	sampler2D _MainTex;
	sampler2D _BumpMap;
	fixed4 _Color;

	half _Scale;

	void surf(Input IN, inout SurfaceOutput o)
	{
		float3 correctWorldNormal = WorldNormalVector(IN, float3(0, 0, 1));
		float2 uv = IN.worldPos.zx;

		if (abs(correctWorldNormal.x) > 0.5) uv = IN.worldPos.zy;
		if (abs(correctWorldNormal.z) > 0.5) uv = IN.worldPos.xy;

		uv.x *= _Scale;
		uv.y *= _Scale;

		fixed4 tex = tex2D(_MainTex, uv);
		o.Albedo = IN.color * tex.rgb;
		o.Normal = UnpackNormal(tex2D(_BumpMap, uv));
	}

	ENDCG
	}
		FallBack "Bumped Specular"
}