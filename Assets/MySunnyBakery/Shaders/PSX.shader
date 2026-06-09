Shader "Custom/PSX"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _VertexJitter ("Vertex Jitter", Float) = 160.0
        _ColorDepth ("Color Depth", Float) = 15.0
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull Mode", Float) = 2
    }

    SubShader
    {
        Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest" "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }
            Cull [_Cull]

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float _VertexJitter;
                float _ColorDepth;
                float _Cutoff;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float3 uv_affine    : TEXCOORD0;
                half3  lighting     : TEXCOORD1;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                float3 normalWS   = TransformObjectToWorldNormal(IN.normalOS);
                float4 clipPos = TransformWorldToHClip(positionWS);

                float4 jitteredPos = clipPos;
                jitteredPos.xyz /= jitteredPos.w;
                jitteredPos.xy = floor(jitteredPos.xy * _VertexJitter) / _VertexJitter;
                jitteredPos.xyz *= jitteredPos.w;

                OUT.positionHCS = jitteredPos;

                float2 uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.uv_affine = float3(uv * clipPos.w, clipPos.w);

                Light mainLight = GetMainLight();
                half3 ambient = SampleSH(normalWS);
                half diff = saturate(dot(normalWS, mainLight.direction));

                half3 ramp = diff > 0.5 ? 1.0 : 0.6;
                OUT.lighting = (mainLight.color * ramp) + ambient;

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 affineUV = IN.uv_affine.xy / IN.uv_affine.z;
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, affineUV);

                clip(col.a - _Cutoff);

                col.rgb *= IN.lighting;
                col.rgb = floor(col.rgb * _ColorDepth) / _ColorDepth;

                return col;
            }
            ENDHLSL
        }
    }
}
