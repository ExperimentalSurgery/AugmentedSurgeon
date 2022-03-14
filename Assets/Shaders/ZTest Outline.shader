// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/ZTest Outline Shader"
{
    Properties
    {
        _NormalColor("Normal Color", Color) = (1, 1, 1, 1)
        _ZTestColor("ZTest Color", Color) = (1, 1, 1, 1)
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineWidth ("Outline width", Range (0.0, 1.0)) = .005
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
             
            struct appdata 
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
             
            struct v2f
            {
                float4 pos : POSITION;
            };
             
            uniform float _OutlineWidth;
            uniform float4 _OutlineColor;
             
            v2f vert(appdata v)
            {
                v2f o;
                 
                float3 norm = normalize(v.normal);
                v.vertex.xyz += v.normal * _OutlineWidth;
                o.pos = UnityObjectToClipPos(v.vertex);
                 
                return o;
            }
             
            half4 frag(v2f i) : COLOR
            {
                return _OutlineColor;
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
             
            struct appdata 
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
             
            struct v2f
            {
                float4 pos : POSITION;
            };
 
            uniform float4 _ZTestColor;
             
            v2f vert(appdata v)
            {
                v2f o;
                 
                float3 norm = normalize(v.normal);
                o.pos = UnityObjectToClipPos(v.vertex);
                 
                return o;
            }
             
            half4 frag(v2f i) : COLOR
            {
                return _ZTestColor;
            }
            ENDCG
        }
    }
}