Shader "Doors/Water"
{
    Properties
    {
        _MainColor ("Water Color", Color) = (0.2, 0.6, 0.8, 0.6)
        _WaveSpeed ("Wave Speed", Float) = 1.0
        _WaveScale ("Wave Scale", Float) = 1.0
    }
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent" 
            "Queue" = "Transparent" 
        }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };
            
            fixed4 _MainColor;
            float _WaveSpeed;
            float _WaveScale;
            
            v2f vert (appdata v)
            {
                v2f o;
                float wave = sin(_Time.y * _WaveSpeed + v.vertex.x * 5.0) * 
                           cos(_Time.y * _WaveSpeed * 0.8 + v.vertex.z * 5.0) * 
                           _WaveScale * 0.02;
                v.vertex.y += wave;
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = _MainColor;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
}