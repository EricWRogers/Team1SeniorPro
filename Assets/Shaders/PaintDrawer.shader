Shader "Custom/PaintDrawer"
{
    Properties
    {
        _GrayTex ("Grayscale Texture", 2D) = "white" {}
        _PaintTex ("Paint Mask", 2D) = "black" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _GrayTex;
            sampler2D _PaintTex;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // base grayscale detail
                float gray = tex2D(_GrayTex, i.uv).r;

                // dynamic paint layer (already colored by brush)
                float3 paintRGB = tex2D(_PaintTex, i.uv).rgb;

                // if no paint, lerp keeps grayscale
                float paintStrength = saturate(length(paintRGB)); // 0 = none, >0 = painted

                float3 tint = lerp(1.0.xxx, paintRGB, paintStrength);

                float3 finalCol =  tint;
                return float4(finalCol, 1);
            }
            ENDCG
        }
    }
}
