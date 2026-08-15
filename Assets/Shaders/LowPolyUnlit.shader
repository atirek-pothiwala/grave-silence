Shader "GraveSilence/LowPolyUnlit"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.5, 0.5, 0.5, 1)
        _Flatness ("Flat Shading", Range(0, 1)) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Name "Unlit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

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
                float _Flatness;
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

            float3 FlatNormal(float3 positionWS)
            {
                float3 dx = ddx(positionWS);
                float3 dy = ddy(positionWS);
                return normalize(cross(dx, dy));
            }

            half4 frag(Varyings input) : SV_Target
            {
                float3 normal = normalize(lerp(input.normalWS, FlatNormal(input.positionWS), _Flatness));
                float3 lightDir = normalize(float3(0.3, 1.0, 0.2));
                float shade = saturate(dot(normal, lightDir) * 0.6 + 0.4);
                float3 col = _BaseColor.rgb * input.color.rgb * shade;
                return half4(col, 1);
            }
            ENDHLSL
        }
    }
}
