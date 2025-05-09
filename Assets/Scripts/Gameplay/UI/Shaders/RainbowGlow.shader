Shader "Custom/RainbowBorderGlow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GlowIntensity ("Glow Intensity", Float) = 1.0
        _GlowSpeed ("Glow Speed", Float) = 1.0
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent" "Queue"="Transparent"
        }
        LOD 200

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _GlowIntensity;
            float _GlowSpeed;

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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 GetRainbowColor(float t)
            {
                float3 colors[12] = {
                    float3(1, 0, 0), // Red
                    float3(1, 0.5, 0), // Orange
                    float3(1, 1, 0), // Yellow
                    float3(0.5, 1, 0), // Chartreuse
                    float3(0, 1, 0), // Green
                    float3(0, 1, 0.5), // Spring Green
                    float3(0, 1, 1), // Cyan
                    float3(0, 0.5, 1), // Azure
                    float3(0, 0, 1), // Blue
                    float3(0.5, 0, 1), // Violet
                    float3(1, 0, 1), // Magenta
                    float3(1, 0, 0.5) // Rose
                };

                t = frac(t) * 11.0;
                int idx = (int)floor(t);
                float lerpT = t - idx;
                float3 color = lerp(colors[idx], colors[(idx + 1) % 12], lerpT);
                return float4(color, 1.0);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 texColor = tex2D(_MainTex, i.uv);
                float t = _Time.y * _GlowSpeed;
                float4 glowColor = GetRainbowColor(t);

                // Apply glow to the entire image
                float4 finalColor = texColor + glowColor * _GlowIntensity;
                finalColor.a = texColor.a;

                return finalColor;
            }
            ENDCG
        }
    }
}