Shader "Custom/PSX_Terrain"
{
    Properties
    {
        [HideInInspector] _Control ("Control (RGBA)", 2D) = "red" {}
        [HideInInspector] _Splat3 ("Layer 3 (A)", 2D) = "white" {}
        [HideInInspector] _Splat2 ("Layer 2 (B)", 2D) = "white" {}
        [HideInInspector] _Splat1 ("Layer 1 (G)", 2D) = "white" {}
        [HideInInspector] _Splat0 ("Layer 0 (R)", 2D) = "white" {}
        
        _VertexJitter ("Vertex Jitter", Float) = 160.0
        _ColorDepth ("Color Depth", Float) = 15.0
        _AffineStrength ("Affine Strength", Range(0,1)) = 1.0
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" "RenderPipeline"="UniversalPipeline" "TerrainCompatible"="true" }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_Control); SAMPLER(sampler_Control);
            TEXTURE2D(_Splat0); SAMPLER(sampler_Splat0);
            TEXTURE2D(_Splat1); SAMPLER(sampler_Splat1);
            TEXTURE2D(_Splat2); SAMPLER(sampler_Splat2);
            TEXTURE2D(_Splat3); SAMPLER(sampler_Splat3);

            CBUFFER_START(UnityPerMaterial)
                float4 _Control_ST;
                float4 _Splat0_ST;
                float4 _Splat1_ST;
                float4 _Splat2_ST;
                float4 _Splat3_ST;
                float _VertexJitter;
                float _ColorDepth;
                float _AffineStrength;
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
                float2 uv_control   : TEXCOORD0;
                float4 uv_splat_01  : TEXCOORD1;
                float4 uv_splat_23  : TEXCOORD3;
                float3 lighting     : TEXCOORD5;
                float  affine_w     : TEXCOORD6;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(IN.normalOS);
                float4 clipPos = TransformWorldToHClip(positionWS);

                float4 jitteredPos = clipPos;
                jitteredPos.xyz /= (jitteredPos.w + 0.000001);
                jitteredPos.xy = floor(jitteredPos.xy * _VertexJitter) / _VertexJitter;
                jitteredPos.xyz *= jitteredPos.w;
                OUT.positionHCS = jitteredPos;

                float w = clipPos.w;
                float perspectiveFactor = lerp(1.0, w, _AffineStrength);
                
                OUT.affine_w = perspectiveFactor;
                OUT.uv_control = IN.uv * perspectiveFactor;
                OUT.uv_splat_01.xy = TRANSFORM_TEX(IN.uv, _Splat0) * perspectiveFactor;
                OUT.uv_splat_01.zw = TRANSFORM_TEX(IN.uv, _Splat1) * perspectiveFactor;
                OUT.uv_splat_23.xy = TRANSFORM_TEX(IN.uv, _Splat2) * perspectiveFactor;
                OUT.uv_splat_23.zw = TRANSFORM_TEX(IN.uv, _Splat3) * perspectiveFactor;

                Light mainLight = GetMainLight();
                half3 ambient = SampleSH(normalWS);
                half diff = saturate(dot(normalWS, mainLight.direction));
                half3 ramp = diff > 0.5 ? 1.0 : 0.6;
                OUT.lighting = (mainLight.color * ramp) + ambient;

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float invW = 1.0 / IN.affine_w;
                float2 controlUV = IN.uv_control * invW;
                half4 blend = SAMPLE_TEXTURE2D(_Control, sampler_Control, controlUV);

                half4 c0 = SAMPLE_TEXTURE2D(_Splat0, sampler_Splat0, IN.uv_splat_01.xy * invW);
                half4 c1 = SAMPLE_TEXTURE2D(_Splat1, sampler_Splat1, IN.uv_splat_01.zw * invW);
                half4 c2 = SAMPLE_TEXTURE2D(_Splat2, sampler_Splat2, IN.uv_splat_23.xy * invW);
                half4 c3 = SAMPLE_TEXTURE2D(_Splat3, sampler_Splat3, IN.uv_splat_23.zw * invW);

                half4 col = c0 * blend.r + c1 * blend.g + c2 * blend.b + c3 * blend.a;
                
                col.rgb *= IN.lighting;
                col.rgb = floor(col.rgb * _ColorDepth) / _ColorDepth;

                return col;
            }
            ENDHLSL
        }
    }
    Fallback "Universal Forward"
}
