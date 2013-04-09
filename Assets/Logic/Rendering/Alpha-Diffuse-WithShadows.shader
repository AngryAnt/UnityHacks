// Based on the shader generated from Alpha-Diffuse.shader and
// on the ShadowCollector pass from Normal-VertexLit.shader.
Shader "Transparent/Diffuse with shadows" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Foo ("Foo", Float) = 1
	}
	SubShader {
		Tags { "Queue" = "Geometry+1" "RenderType"="Opaque" }
		LOD 200
		
		Alphatest Greater 0 ZWrite Off ColorMask RGB
	
		Pass {
			Name "FORWARD"
			Tags { "LightMode" = "ForwardBase" }
			Blend SrcAlpha OneMinusSrcAlpha

CGPROGRAM
#pragma vertex vert_surf
#pragma fragment frag_surf
#pragma fragmentoption ARB_precision_hint_fastest
#pragma multi_compile_fwdbase
#include "HLSLSupport.cginc"
#include "UnityShaderVariables.cginc"
#define UNITY_PASS_FORWARDBASE
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal
#line 1
#line 11

//#pragma surface surf Lambert
//#pragma only_renderers d3d11
#pragma debug

sampler2D _MainTex;
fixed4 _Color;
float _Foo;

struct Input {
	float2 uv_MainTex;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
	o.Albedo = c.rgb;
	o.Alpha = c.a;
}
#ifdef LIGHTMAP_OFF
struct v2f_surf {
  float4 pos : SV_POSITION;
  float2 pack0 : TEXCOORD0;
  fixed3 normal : TEXCOORD1;
  fixed3 vlight : TEXCOORD2;
  LIGHTING_COORDS(3,4)
};
#endif
#ifndef LIGHTMAP_OFF
struct v2f_surf {
  float4 pos : SV_POSITION;
  float2 pack0 : TEXCOORD0;
  float2 lmap : TEXCOORD1;
  LIGHTING_COORDS(2,3)
};
#endif
#ifndef LIGHTMAP_OFF
float4 unity_LightmapST;
#endif
float4 _MainTex_ST;
v2f_surf vert_surf (appdata_full v) {
  v2f_surf o;
  o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
  o.pack0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
  #ifndef LIGHTMAP_OFF
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #endif
  float3 worldN = mul((float3x3)_Object2World, SCALED_NORMAL);
  #ifdef LIGHTMAP_OFF
  o.normal = worldN;
  #endif
  #ifdef LIGHTMAP_OFF
  float3 shlight = ShadeSH9 (float4(worldN,1.0));
  o.vlight = shlight;
  #ifdef VERTEXLIGHT_ON
  float3 worldPos = mul(_Object2World, v.vertex).xyz;
  o.vlight += Shade4PointLights (
    unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
    unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
    unity_4LightAtten0, worldPos, worldN );
  #endif // VERTEXLIGHT_ON
  #endif // LIGHTMAP_OFF
  TRANSFER_VERTEX_TO_FRAGMENT(o);
  return o;
}
#ifndef LIGHTMAP_OFF
sampler2D unity_Lightmap;
#ifndef DIRLIGHTMAP_OFF
sampler2D unity_LightmapInd;
#endif
#endif
fixed4 frag_surf (v2f_surf IN) : COLOR {
	Input surfIN;
	surfIN.uv_MainTex = IN.pack0.xy;
	#ifdef SHADER_API_D3D11
	SurfaceOutput o = (SurfaceOutput)0;
	#else
	SurfaceOutput o;
	#endif
	o.Albedo = 0.0;
	o.Emission = 0.0;
	o.Specular = 0.0;
	o.Alpha = 0.0;
	o.Gloss = 0.0;
	#ifdef LIGHTMAP_OFF
	o.Normal = IN.normal;
	#endif
	surf (surfIN, o);
	fixed atten = LIGHT_ATTENUATION(IN);
	fixed4 c = 0;
	#ifdef LIGHTMAP_OFF
	c = LightingLambert (o, _WorldSpaceLightPos0.xyz, atten);
	#endif // LIGHTMAP_OFF
	#ifdef LIGHTMAP_OFF
	c.rgb += o.Albedo * IN.vlight;
	#endif // LIGHTMAP_OFF
	#ifndef LIGHTMAP_OFF
	#ifdef DIRLIGHTMAP_OFF
	fixed4 lmtex = tex2D(unity_Lightmap, IN.lmap.xy);
	fixed3 lm = DecodeLightmap (lmtex);
	#else
	fixed4 lmtex = tex2D(unity_Lightmap, IN.lmap.xy);
	fixed4 lmIndTex = tex2D(unity_LightmapInd, IN.lmap.xy);
	half3 lm = LightingLambert_DirLightmap(o, lmtex, lmIndTex, 0).rgb;
	#endif
	#ifdef SHADOWS_SCREEN
	#if defined(SHADER_API_GLES) && defined(SHADER_API_MOBILE)
	c.rgb += o.Albedo * min(lm, atten*2);
	#else
	c.rgb += o.Albedo * max(min(lm,(atten*2)*lmtex.rgb), lm*atten);
	#endif
	#else // SHADOWS_SCREEN
	c.rgb += o.Albedo * lm;
	#endif // SHADOWS_SCREEN
	c.a = o.Alpha;
	#endif // LIGHTMAP_OFF
	c.a = o.Alpha;
	// Trying things out:
	//c.a = lerp(0.5, o.Alpha, atten);
	//c.a = o.Alpha;// + lerp (1, 0.0, atten);
	return c;
}

ENDCG
	} // Pass
	
	// Pass to render object as a shadow collector
	Pass {
		Name "ShadowCollector"
		Tags { "LightMode" = "ShadowCollector" }
		
		Fog {Mode Off}
		// Was: ZWrite On ZTest LEqual
		ZWrite Off ZTest LEqual
		
		// Take the lowest shadowing value
		Blend One One
		BlendOp Min

CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#pragma multi_compile_shadowcollector

#define SHADOW_COLLECTOR_PASS
#include "UnityCG.cginc"

// Was:
// res.x = saturate(shadow + shadowFade);
// Play around with Main Color's alpha to see the result
#define COMPUTE_SHADOW_COLLECTOR_SHADOW_CUSTOM(i, weights, shadowFade) \
	float4 coord = float4(i._ShadowCoord0 * weights[0] + i._ShadowCoord1 * weights[1] + i._ShadowCoord2 * weights[2] + i._ShadowCoord3 * weights[3], 1); \
	SAMPLE_SHADOW_COLLECTOR_SHADOW(coord) \
	float4 res; \
	res.x = lerp (1, saturate(shadow + shadowFade), _Color.a); \
	res.y = 1.0; \
	res.zw = EncodeFloatRG (1 - i._WorldPosViewZ.w * _ProjectionParams.w); \
	return res;
	
#if defined (SHADOWS_SPLIT_SPHERES)
#define SHADOW_COLLECTOR_FRAGMENT_CUSTOM(i) \
	float3 fromCenter0 = i._WorldPosViewZ.xyz - unity_ShadowSplitSpheres[0].xyz; \
	float3 fromCenter1 = i._WorldPosViewZ.xyz - unity_ShadowSplitSpheres[1].xyz; \
	float3 fromCenter2 = i._WorldPosViewZ.xyz - unity_ShadowSplitSpheres[2].xyz; \
	float3 fromCenter3 = i._WorldPosViewZ.xyz - unity_ShadowSplitSpheres[3].xyz; \
	float4 distances2 = float4(dot(fromCenter0,fromCenter0), dot(fromCenter1,fromCenter1), dot(fromCenter2,fromCenter2), dot(fromCenter3,fromCenter3)); \
	float4 cascadeWeights = float4(distances2 < unity_ShadowSplitSqRadii); \
	cascadeWeights.yzw = saturate(cascadeWeights.yzw - cascadeWeights.xyz); \
	float sphereDist = distance(i._WorldPosViewZ.xyz, unity_ShadowFadeCenterAndType.xyz); \
	float shadowFade = saturate(sphereDist * _LightShadowData.z + _LightShadowData.w); \
	COMPUTE_SHADOW_COLLECTOR_SHADOW_CUSTOM(i, cascadeWeights, shadowFade)
#else
#define SHADOW_COLLECTOR_FRAGMENT_CUSTOM(i) \
	float4 viewZ = i._WorldPosViewZ.w; \
	float4 zNear = float4( viewZ >= _LightSplitsNear ); \
	float4 zFar = float4( viewZ < _LightSplitsFar ); \
	float4 cascadeWeights = zNear * zFar; \
	float shadowFade = saturate(i._WorldPosViewZ.w * _LightShadowData.z + _LightShadowData.w); \
	COMPUTE_SHADOW_COLLECTOR_SHADOW_CUSTOM(i, cascadeWeights, shadowFade)
#endif

fixed4 _Color;

struct appdata {
	float4 vertex : POSITION;
};

struct v2f {
	V2F_SHADOW_COLLECTOR;
};

v2f vert (appdata v)
{
	v2f o;
	TRANSFER_SHADOW_COLLECTOR(o)
	return o;
}

fixed4 frag (v2f i) : COLOR
{
	SHADOW_COLLECTOR_FRAGMENT_CUSTOM(i)
}
ENDCG

	} // Pass
} // Subshader

Fallback "VertexLit"
} // Shader
