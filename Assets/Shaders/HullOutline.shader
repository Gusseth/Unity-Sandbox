Shader "Unlit/HullOutline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [HDR]
        _Thickness("Thickness", float) = 1
        _ThicknessPersistence("Thickness Persistence", Range(0.0, 1.0)) = 0
        _Color("Outline Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline"}
        LOD 100

        Pass
        {
            Name "Hull Outline"
            Cull Front

            HLSLPROGRAM

            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma vertex vert
            #pragma fragment frag

            #include "HullOutline.hlsl"

            ENDHLSL
        }
    }
}
