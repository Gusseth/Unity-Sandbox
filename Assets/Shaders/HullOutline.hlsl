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
	float4	positionCS:	SV_POSITION;	// same vertex positions but in clip/frag space
};

float _Thickness;
float4 _Color;


// Thank you to:
// https://www.videopoetics.com/tutorials/pixel-perfect-outline-shaders-unity/
VertexOutput vert(Attributes attr) {
	float4 position_clip = UnityObjectToClipPos(attr.position);	// transform vertices into clip space\

	// transform normal into clip space
	float3 normal_clip = mul((float3x3)UNITY_MATRIX_MVP, attr.normal.xyz);
	normal_clip = normalize(normal_clip);

	position_clip.xyz += normal_clip * _Thickness;	// We're pretty much "raising" the surface
													// by however long the thickness is
													// This is where the outline ends from the surface

	float2 offset = normalize(normal_clip.xy) / (_ScreenParams.xy * _Thickness * 2);
	position_clip.xy += offset;

	VertexOutput var;
	var.positionCS = position_clip;

	return var;
}

float4 frag(VertexOutput var) : SV_Target {
	return _Color;
}