Shader "Custom/CorrectDissolve"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _DissolveAmount ("Dissolve Amount", Range(0,1)) = 0
        _DissolveWidth ("Dissolve Width", Range(0,0.2)) = 0.1
        _DissolveColor ("Dissolve Color", Color) = (1,0.5,0,1)
        _DissolveEmission ("Dissolve Emission", Float) = 2
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                float2 texcoordNoise : TEXCOORD1;
            };

            fixed4 _Color;
            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;
            float _DissolveAmount;
            float _DissolveWidth;
            fixed4 _DissolveColor;
            float _DissolveEmission;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.texcoordNoise = TRANSFORM_TEX(IN.texcoord, _NoiseTex);
                OUT.color = IN.color * _Color;
                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap(OUT.vertex);
                #endif

                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                // Основная текстура спрайта
                fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
                
                // Текстура шума - используется ТОЛЬКО для dissolve
                fixed noise = tex2D(_NoiseTex, IN.texcoordNoise).r;
                
                // Dissolve логика
                float dissolve = step(noise, _DissolveAmount);
                
                // Edge detection - только когда dissolveAmount > 0
                float edge = 0;
                if (_DissolveAmount > 0)
                {
                    edge = step(noise, _DissolveAmount + _DissolveWidth) - dissolve;
                }
                
                // Применяем dissolve к альфа-каналу
                c.a *= (1.0 - dissolve);
                
                // Добавляем цвет края ТОЛЬКО если есть edge
                if (edge > 0)
                {
                    c.rgb = lerp(c.rgb, _DissolveColor.rgb * _DissolveEmission, edge);
                }
                
                // Premultiplied alpha
                c.rgb *= c.a;
                
                return c;
            }
            ENDCG
        }
    }
}