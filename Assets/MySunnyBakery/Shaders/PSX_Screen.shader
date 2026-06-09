Shader "Hidden/PSX_Screen"
{
    Properties
    {
        _Resolution ("Resolution", Vector) = (320, 240, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float4 _Resolution;

            half4 frag (Varyings input) : SV_Target
            {
                float2 pixelatedUV = floor(input.texcoord * _Resolution.xy) / _Resolution.xy;
                return SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, pixelatedUV);
            }
            ENDHLSL
        }
    }
}
