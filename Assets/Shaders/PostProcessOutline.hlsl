#pragma once
//#include "EdgeDetect.hlsl"

struct Attributes
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
};

struct VertexOutput
{
    float2 uv : TEXCOORD0;
    float4 vertex : SV_POSITION;
    float4 sp : TEXCOORD1;
};

sampler2D _MainTex;
UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
float4 _MainTex_ST;

float4 _OutlineColor;
float _Thickness;
float _DepthStrength;
float _DepthThickness;
float _DepthThreshold;


VertexOutput vert(Attributes v)
{
    VertexOutput o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
    o.sp = ComputeScreenPos(o.vertex);
    return o;
}

float4 FragmentShader(VertexOutput vary) : SV_Target
{
    /*
    // sample the texture
    float4 base = tex2D(_MainTex, vary.uv);
    float2 depth_uv = vary.sp.xy / vary.sp.w;
    float sobel = DepthSobel(_CameraDepthTexture, depth_uv, _Thickness);
    float stepped = smoothstep(0.0, _DepthThreshold, sobel);
    stepped = pow(stepped, _DepthThickness) * _DepthStrength;
    //return (float)tex2D(_CameraDepthTexture, vary.uv);
    return lerp(base, _OutlineColor, stepped);
    */
    return tex2D(_CameraDepthTexture, vary.sp.xy / vary.sp.w);

}

