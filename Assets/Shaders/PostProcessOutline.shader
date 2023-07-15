Shader "Unlit/PostProcessOutline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [HDR]
        _OutlineColor("Outline Color", Color) = (0,0,0,0)
        _Thickness("Thickness", float) = 0.1
        _ThicknessPersistence("Thickness Persistence", Range(0.0, 1.0)) = 0
        _DepthStrength("Depth Strength", float) = 0
        _DepthThickness("Depth Thickness", float) = 0
        _DepthThreshold("Depth Threshold", float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "RenderPipeline" = "UniversalPipeline"}
        LOD 100

        Pass
        {
            Name "Post-Processed Outline"

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment FragmentShader
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "PostProcessOutline.hlsl"

            ENDCG
        }
    }
}
