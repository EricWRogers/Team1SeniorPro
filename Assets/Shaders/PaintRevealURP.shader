Shader "URP/PaintReveal"
{
    Properties
    {
        _BaseTex("Base (Color) Texture", 2D) = "white" {}
        _OverlayTex("Overlay Texture (optional)", 2D) = "white" {}
        _OverlayColor("Overlay Color", Color) = (1,1,1,1) // used if overlay tex is just white
        _Mask_Texture("Mask (White=Unpainted, Black=Painted)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" "RenderPipeline"="UniversalPipeline" }
        LOD 100

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_BaseTex);       SAMPLER(sampler_BaseTex);
            TEXTURE2D(_OverlayTex);    SAMPLER(sampler_OverlayTex);
            TEXTURE2D(_Mask_Texture);  SAMPLER(sampler_Mask_Texture);

            CBUFFER_START(UnityPerMaterial)
            float4 _BaseTex_ST;
            float4 _OverlayTex_ST;
            float4 _Mask_Texture_ST;
            float4 _OverlayColor;
            CBUFFER_END

            struct Attributes {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };
            struct Varyings {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
            };

            Varyings vert (Attributes IN) {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;
                half4 baseC  = SAMPLE_TEXTURE2D(_BaseTex, sampler_BaseTex,     uv);
                half4 overT  = SAMPLE_TEXTURE2D(_OverlayTex, sampler_OverlayTex, uv);
                half4 maskC  = SAMPLE_TEXTURE2D(_Mask_Texture, sampler_Mask_Texture, uv);
                // If overlay texture is just white, tint by OverlayColor
                half4 overlay = overT * _OverlayColor;
                // mask.r = 1 -> show overlay (white); mask.r = 0 -> show base (color)
                half t = 1.0h - maskC.r; // 0 = overlay, 1 = base
                return lerp(overlay, baseC, t);
            }
            ENDHLSL
        }
    }
}