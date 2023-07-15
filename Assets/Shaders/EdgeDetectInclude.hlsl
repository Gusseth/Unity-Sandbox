#pragma once

static float ConvolutionXMatrix[9] = 
{
    1,  0,  -1,
    2,  0,  -2,
    1,  0,  -1
};

static float ConvolutionYMatrix[9] =
{
     1,   2,  1,
     0,   0,  0,
    -1,  -2,  -1
};

static float2 Samples[9] =
{
     float2(-1, 1), float2(0, 1),   float2(1, 1),
     float2(-1, 0), float2(0, 0),   float2(1, 0),
     float2(-1, -1),float2(0, -1),  float2(1, -1)
};

/*
float DepthSobel(sampler2D _CameraDepthTexture, float2 uv, float thickness) {
    float2 sobel = 0;
    [unroll]
    for (int i = 0; i < 9; i++) {
        float depth = tex2D(_CameraDepthTexture, uv + Samples[i] * thickness);
        sobel += depth * float2(ConvolutionXMatrix[i], ConvolutionYMatrix[i]);
    }
    return length(sobel);
}
*/

void DepthSobel_float(float2 UV, float thickness, out float Out) {
    float2 sobel = 0;
    [unroll]
    for (int i = 0; i < 9; i++) {
        float depth = SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV + Samples[i] * thickness);
        sobel += depth * float2(ConvolutionXMatrix[i], ConvolutionYMatrix[i]);
    }
    Out = length(sobel);
}

