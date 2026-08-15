Shader "GraveSilence/LowPolyLit"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.5, 0.5, 0.5, 1)
        _ShadowColor ("Shadow Tint", Color) = (0.15, 0.1, 0.2, 1)
        _RimColor ("Rim Color", Color) = (0.4, 0.3, 0.6, 1)
        _RimPower ("Rim Power", Range(0.5, 8)) = 3
        _Flatness ("Flat Shading", Range(0, 1)) = 1
        _AmbientBoost ("Ambient Boost", Range(0, 1)) = 0.25
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float4 color : COLOR;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _ShadowColor;
                float4 _RimColor;
                float _RimPower;
                float _Flatness;
                float _AmbientBoost;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs pos = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs norm = GetVertexNormalInputs(input.normalOS);

                output.positionCS = pos.positionCS;
                output.positionWS = pos.positionWS;
                output.normalWS = norm.normalWS;
                output.color = input.color;
                return output;
            }

            float3 FlatNormal(float3 positionWS, float3 normalWS)
            {
                float3 dx = ddx(positionWS);
                float3 dy = ddy(positionWS);
                return normalize(cross(dx, dy));
            }

            half4 frag(Varyings input) : SV_Target
            {
                float3 normal = normalize(lerp(input.normalWS, FlatNormal(input.positionWS, input.normalWS), _Flatness));
                float3 lightDir = normalize(_MainLightPosition.xyz);
                float ndl = saturate(dot(normal, lightDir));

                float3 base = _BaseColor.rgb * input.color.rgb;
                float3 lit = lerp(_ShadowColor.rgb * base, base, ndl);
                lit += _AmbientBoost * base;

                float rim = pow(1.0 - saturate(dot(normal, normalize(_WorldSpaceCameraPos - input.positionWS))), _RimPower);
                lit += _RimColor.rgb * rim * 0.35;

                float shadow = MainLightRealtimeShadow(TransformWorldToShadowCoord(input.positionWS));
                lit *= lerp(0.65, 1.0, shadow);

                return half4(lit, 1);
            }
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Lit"
}
