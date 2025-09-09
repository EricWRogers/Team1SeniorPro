Shader "Hidden/BrushStamp"
{
    Properties
    {
        _BrushTex ("Brush", 2D) = "white" {}
        _BrushColor ("Color", Color) = (1,0,0,1)
        _BrushUV ("BrushUV", Vector) = (0.5, 0.5, 0.1, 0.1)
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;   // previous paint
            sampler2D _BrushTex;
            float4 _BrushColor;
            float4 _BrushUV; // x=uv.x, y=uv.y, z=scaleX, w=scaleY

            fixed4 frag (v2f_img i) : SV_Target
            {
                // read existing paint
                fixed4 baseCol = tex2D(_MainTex, i.uv);

                // transform to brush space
                float2 local = (i.uv - _BrushUV.xy) / _BrushUV.z + 0.5;
                fixed4 brush = tex2D(_BrushTex, local);

                // colorize brush (grayscale * color)
                fixed4 stamp = _BrushColor * brush.r;

                // alpha blend
                return lerp(baseCol, stamp, stamp.a);
            }
            ENDCG
        }
    }
}
