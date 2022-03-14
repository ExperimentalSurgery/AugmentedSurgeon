// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/ZTest Shader"
{
    Properties
    {
        _NormalColor("Normal Color", Color) = (1, 1, 1, 1)
        _ZTestColor("ZTest Color", Color) = (0, 0, 0, 1)
    }
     
    SubShader
    {
        Pass
        {
            CGPROGRAM
             
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
             
            uniform float4 _NormalColor;
             
            struct appdata
            {
                float4 vertex : POSITION;
            };
             
            struct v2f
            {
                float4 pos : POSITION;
            };
             
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos( v.vertex);
                return o;
            }
             
            float4 frag(v2f i ) : COLOR
            {               
                return _NormalColor;
            }
            ENDCG
        }
         
        Pass
        {
            ZTest Greater
            CGPROGRAM
             
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
             
            uniform float4 _ZTestColor;
             
            struct appdata
            {
                float4 vertex : POSITION;
            };
             
            struct v2f
            {
                float4 pos : POSITION;
            };
             
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos( v.vertex);
                return o;
            }
             
            float4 frag(v2f i ) : COLOR
            {               
                return _ZTestColor;
            }
            ENDCG
        }
    }
}