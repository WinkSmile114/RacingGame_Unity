//Custom shader developed by Gurbrinder Singh, 
//https://www.linkedin.com/in/gurbrinder-singh
//unity developer || hyper casual games developer || India
//kindly contact me for any kind of Unity projects to outsource
//gurbrindersinghs@gmail.com

Shader "Custom/HueShiftShader"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _HueShift ("Hue Shift", Range(0,1)) = 0.0
        _Saturation ("Saturation", Range(0,2)) = 1.0
        _Brightness ("Brightness", Range(0,2)) = 1.0
        _AlphaCutoff ("Alpha Cutoff", Range(0,1)) = 0.5
        _GlowColor ("Glow Color", Color) = (1,1,1,1)
        _GlowIntensity ("Glow Intensity", Range(0,1)) = 0.5
        _GlowSpread ("Glow Spread", Range(0,1)) = 0.2
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 200

        Pass
        {
            Name "SPRITE_RENDER"
            Tags {"LightMode" = "UniversalForward"}
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float _HueShift;
            float _Saturation;
            float _Brightness;
            float _AlphaCutoff;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed3 rgb2hsv(fixed3 c)
            {
                fixed4 K = fixed4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                fixed4 p = c.g < c.b ? fixed4(c.bg, K.wz) : fixed4(c.gb, K.xy);
                fixed4 q = c.r < p.x ? fixed4(p.xyw, c.r) : fixed4(c.r, p.yzx);
                fixed d = q.x - min(q.w, q.y);
                fixed e = 1.0e-10;
                fixed3 hsv = fixed3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
                return hsv;
            }

            fixed3 hsv2rgb(fixed3 c)
            {
                fixed4 K = fixed4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                fixed3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
                if (col.a < _AlphaCutoff)
                    discard;

                fixed3 hsv = rgb2hsv(col.rgb);
                hsv.x = frac(hsv.x + _HueShift);
                hsv.y *= _Saturation;
                hsv.z *= _Brightness;
                col.rgb = hsv2rgb(hsv);

                return col;
            }
            ENDCG
        }

        Pass
        {
            Name "GLOW"
            Tags {"LightMode" = "UniversalForward"}
            Blend SrcAlpha One
            Cull Off
            ZWrite Off
            CGPROGRAM
            #pragma vertex vertGlow
            #pragma fragment fragGlow
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _GlowColor;
            float _GlowIntensity;
            float _GlowSpread;

            v2f vertGlow (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex * float4(1 + _GlowSpread, 1 + _GlowSpread, 1, 1));
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 fragGlow (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
                fixed4 glow = _GlowColor * col.a * _GlowIntensity;
                return glow;
            }
            ENDCG
        }
    }
    FallBack "Sprites/Default"
}
