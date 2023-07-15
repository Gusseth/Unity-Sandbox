#pragma once
#include "UnityCG.cginc"

// Passed from the mesh into the vertex shader
struct Attributes {
	float4 position:	POSITION;	// vertex positions in object space
	float4 normal :		NORMAL;		// normal vecs in object space
};

// Passed from the vertex shader to the fragment shader
// AKA Varying variables in GLSL
struct VertexOutput {
	float4	positionCS:	SV_POSITION;	// same vertex positions but in clip space
};

float _Thickness;
float _ThicknessPersistence;
float4 _Color;


// Thank you to:
// https://www.videopoetics.com/tutorials/pixel-perfect-outline-shaders-unity/
VertexOutput vert(Attributes attr) {
	float4 position_clip = UnityObjectToClipPos(attr.position);	// transform vertices into clip space
	float3 normal_clip = mul((float3x3)UNITY_MATRIX_MVP, attr.normal.xyz); // transform normal into clip space
	normal_clip = normalize(normal_clip);

	position_clip.xyz += normal_clip * _Thickness;	// We're pretty much "raising" the surface
													// by however long the thickness is.
													// This is where the outline ends from the surface

	// Since we're at the clip space, the properties of the camera affects position_clip.
	// We're correcting for aspect ratio here, otherwise the outlines "bulges" at the
	// widest axis (x, y) of the viewport.
	float2 correction = normalize(normal_clip.xy) * (position_clip.w * _ThicknessPersistence) / (_ScreenParams.xy * _Thickness * 2);
	position_clip.xy += correction;

	VertexOutput var;
	var.positionCS = position_clip;

	return var;
}

float4 frag(VertexOutput var) : SV_Target {
	return _Color;
}