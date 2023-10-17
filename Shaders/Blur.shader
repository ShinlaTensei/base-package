Shader "Custom/BlurShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurSize ("Blur Size", Range(0.0, 10.0)) = 2.0
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        
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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MainTex;
            float _BlurSize;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 color = tex2D(_MainTex, i.uv);
                
                // Apply blur effect using a box filter
                float2 texelSize = 1.0 / _ScreenParams.xy;
                float2 offset = _BlurSize * texelSize;
                
                color += tex2D(_MainTex, i.uv + float2(-offset.x, -offset.y));
                color += tex2D(_MainTex, i.uv + float2(-offset.x, offset.y));
                color += tex2D(_MainTex, i.uv + float2(offset.x, offset.y));
                color += tex2D(_MainTex, i.uv + float2(offset.x, -offset.y));
                
                color /= 5.0; // Normalize the blurred color
                
                return color;
            }
            
            ENDCG
        }
    }
}
