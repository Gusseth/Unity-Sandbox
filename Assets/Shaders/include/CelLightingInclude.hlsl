#pragma once

/*
	This shader is a modified version of Ned Makes Games' Toon Shader:
	https://www.youtube.com/watch?v=RC91uxRTId8

	I excluded specular lighting here as I like the flatter shading more.
*/
void GetMainLightColour_float(out float3 Color) {
#ifdef SHADERGRAPH_PREVIEW
	Color = 1;
#else
	Color = GetMainLight(0).color;
#endif
}

void CalculateShadows_float(float3 objPosition_w,
	out float DistanceAttenuation, out float ShadowAttenuation) {
#ifdef SHADERGRAPH_PREVIEW
	DistanceAttenuation = 1;
	ShadowAttenuation = 1;
#else
	#ifdef SHADOWS_SCREEN
		float4 shadowPos = TransformWorldToHClip(objPosition_w);
		shadowPos = ComputeScreenPos(shadowPos);
	#else
		float4 shadowPos = TransformWorldToShadowCoord(objPosition_w);
	#endif
	Light m = GetMainLight(shadowPos);
	DistanceAttenuation = m.distanceAttenuation;
	ShadowAttenuation = m.shadowAttenuation;
#endif
}

void OtherLights_float(float3 objPosition_w, float3 objNormal_w,
	float mainLightDiffuse, float3 mainLightColor,
	float albedo,
	out float Diffuse, out float3 Color) {
	Diffuse = mainLightDiffuse;
	Color = mainLightColor * mainLightDiffuse;

#ifndef SHADERGRAPH_PREVIEW
	int n = GetAdditionalLightsCount();
	for (int i = 0; i < n; i++) {
		// Unity bloated feature rot bs: can't directly get additional lights like wtf
		int objectLightIndex = GetPerObjectLightIndex(i);
		Light light = GetAdditionalPerObjectLight(objectLightIndex, objPosition_w);
		float shadowAttenuation = AdditionalLightRealtimeShadow(objectLightIndex, objPosition_w);

		// Finally...
		float diffuse = saturate(dot(objNormal_w, light.direction)) * albedo;
		float attenuation = light.distanceAttenuation * shadowAttenuation;
		diffuse *= attenuation;

		Diffuse += diffuse;
		Color += light.color * diffuse;
	}
#endif
	if (Diffuse) {
		Color = Color / Diffuse;
	}
	else {
		Color = mainLightColor;
	}
}