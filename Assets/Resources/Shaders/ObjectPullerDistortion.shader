// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/SkyReflection"
{
	SubShader
	{
		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

	struct v2f {
		half3 worldRefl : TEXCOORD0;
		float4 pos : SV_POSITION;
	};

	v2f vert(float4 vertex : POSITION, float3 normal : NORMAL)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(vertex);
		// compute world space position of the vertex
		float3 worldPos = mul(unity_ObjectToWorld, vertex).xyz;
		// compute world space view direction
		float3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
		// world space normal
		float3 worldNormal = UnityObjectToWorldNormal(normal);
		// world space reflection vector
		o.worldRefl = reflect(-worldViewDir, worldNormal);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		// sample the default reflection cubemap, using the reflection vector
		half4 skyData = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, i.worldRefl);
		// decode cubemap data into actual color
		half3 skyColor = DecodeHDR(skyData, unity_SpecCube0_HDR);
		// output it!
		fixed4 c = 0;
		c.rgb = skyColor;
		return c;
	}
		ENDCG
	}
	}
}