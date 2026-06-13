Shader "Custom/PSX_Bakeable"
{
    Properties
    {
        _MainTex ("Texture 1", 2D) = "white" {}
        _SecondTex ("Texture 2", 2D) = "white" {}
        _ThirdTex ("Texture 3", 2D) = "white" {}
        _NoiseScale ("Noise Scale", Float) = 10.0
        _Progress ("Progress", Range(0, 2)) = 0.0
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
            TEXTURE2D(_SecondTex);
            SAMPLER(sampler_SecondTex);
            TEXTURE2D(_ThirdTex);
            SAMPLER(sampler_ThirdTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float _NoiseScale;
                float _Progress;
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

            float PseudoRandom(float2 co)
            {
                return frac(sin(dot(co, float2(12.9898, 78.233))) * 43758.5453);
            }

            float ValueNoise(float2 uv)
            {
                float2 i = floor(uv);
                float2 f = frac(uv);

                float a = PseudoRandom(i);
                float b = PseudoRandom(i + float2(1.0, 0.0));
                float c = PseudoRandom(i + float2(0.0, 1.0));
                float d = PseudoRandom(i + float2(1.0, 1.0));

                float2 u = f * f * (3.0 - 2.0 * f);

                return lerp(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
            }

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
                
                float2 texSize;
                _MainTex.GetDimensions(texSize.x, texSize.y);
                float2 pixelUV = floor(affineUV * texSize);

                float noise = ValueNoise((pixelUV / texSize) * _NoiseScale);

                half4 col1 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, affineUV);
                half4 col2 = SAMPLE_TEXTURE2D(_SecondTex, sampler_SecondTex, affineUV);
                half4 col3 = SAMPLE_TEXTURE2D(_ThirdTex, sampler_ThirdTex, affineUV);

                float step1 = step(noise, _Progress);
                float step2 = step(noise, _Progress - 1.0);

                half4 col = lerp(col1, col2, step1);
                col = lerp(col, col3, step2);
                
                clip(col.a - _Cutoff);

                col.rgb *= IN.lighting;
                col.rgb = floor(col.rgb * _ColorDepth) / _ColorDepth;
                
                return col;
            }
            ENDHLSL
        }
    }
}
