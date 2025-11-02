Shader "Custom/DissolveShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _DissolveAmount ("Dissolve Amount", Range(0,1)) = 0
        _DissolveWidth ("Dissolve Width", Range(0,0.2)) = 0.1
        _DissolveColor ("Dissolve Color", Color) = (1,0.5,0,1)
        _DissolveEmission ("Dissolve Emission", Float) = 2
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry"}
        LOD 100
        Cull Off

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
                float2 uvNoise : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;
            float _DissolveAmount;
            float _DissolveWidth;
            float4 _DissolveColor;
            float _DissolveEmission;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uvNoise = TRANSFORM_TEX(v.uv, _NoiseTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample main texture
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // Sample noise texture for dissolve pattern
                float noise = tex2D(_NoiseTex, i.uvNoise).r;
                
                // Dissolve logic
                float dissolve = step(noise, _DissolveAmount);
                
                // Edge detection
                float edge = step(noise, _DissolveAmount + _DissolveWidth) - dissolve;
                
                // Combine colors
                fixed4 finalColor = col;
                finalColor.rgb = lerp(finalColor.rgb, _DissolveColor.rgb * _DissolveEmission, edge);
                finalColor.a = dissolve;
                
                return finalColor;
            }
            ENDCG
        }
    }
    
FallBack "Diffuse"
}