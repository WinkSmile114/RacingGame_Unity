Shader "Custom/SimpleSpriteURP"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _Tiling ("Tiling", Vector) = (1,1,0,0)
    }
    
    SubShader
    {
        Tags { "RenderType" = "Transparent" }
        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "UniversalForward" }
            
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_fog
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityPBSLighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            SamplerState samplerState;
            Texture2D _MainTex;
            float4 _Tiling;

            Varyings Vert (Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS);
                output.uv = input.uv * _Tiling.xy + _Tiling.zw;
                return output;
            }

            half4 Frag (Varyings input) : SV_Target
            {
                half4 color = _MainTex.Sample(samplerState, input.uv);
                return color;
            }
            ENDHLSL
        }
    }

    FallBack "Sprites/Default"
}
