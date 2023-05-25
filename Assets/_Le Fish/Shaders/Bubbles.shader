Shader "Unlit/Bubbles"
{
    Properties
    {
        _Test ("Test", Range(0, 1)) = 0
        _Color ("Color", Color) = (1, 1, 1, 1)

        [Header(Bubble Properties)]
        _BubbleColor ("Color", Color) = (1, 1, 1, 1)
        _Size ("Size", Range(0, 1)) = 0.5
        _BubbleThickness ("Thickness", Range(0, 1)) = 0.5
        _Speed ("Speed", Vector) = (0, 0, 0)

        [Header(Outline)]
        _OutlineColor ("Color", Color) = (1, 1, 1, 1)
        _OutlineThickness ("Thickness", Range(0, 1)) = 0.5
        
        [Header(Noise Properties)]
        _NoiseScale ("Noise Scale", Float) = 10
        _NoisePeriod ("Noise Period", Float) = 1
        [KeywordEnum(Classic, Simplex)] _NoiseType("Noise Type", Int) = 0
        [Toggle(GRADIENT)] _Gradient("Gradient", Int) = 0
    }

    SubShader
    {
        Tags {"RenderType"="Opeque"}
        LOD 100

        Pass // Outline
        {
            Cull Front

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Common.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float3 normal : NORMAL;
                float4 vertexObject : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            float _Test;
            float _OutlineThickness;
            float4 _OutlineColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertexObject = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex + normalize(v.normal) * _OutlineThickness);
                o.normal = v.normal;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            
            ENDCG
        }
        
        Pass // Bubbles
        {
            CGPROGRAM
            // Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members r)
            // #pragma exclude_renderers d3d11
            #pragma vertex vert alpha
            #pragma fragment frag alpha
            #pragma multi_compile _NOISETYPE_CLASSIC _NOISETYPE_SIMPLEX
            #pragma multi_compile _ GRADIENT

            #include "UnityCG.cginc"
            #include "Common.hlsl"
            #include "Packages/jp.keijiro.noiseshader/Shader/ClassicNoise2D.hlsl"
            #include "Packages/jp.keijiro.noiseshader/Shader/ClassicNoise3D.hlsl"
            #include "Packages/jp.keijiro.noiseshader/Shader/SimplexNoise2D.hlsl"
            #include "Packages/jp.keijiro.noiseshader/Shader/SimplexNoise3D.hlsl"
            
            #if defined(_NOISETYPE_SIMPLEX)

                #if defined(GRADIENT)
                    #define NOISE_FUNC(coord, period) SimplexNoiseGrad(coord)
                #else
                    #define NOISE_FUNC(coord, period) SimplexNoise(coord)
                #endif

            #elif defined(_NOISETYPE_CLASSIC)

                #if defined(GRADIENT)
                    #define NOISE_FUNC(coord, period) ClassicNoise(coord)
                #else
                    #define NOISE_FUNC(coord, period) PeriodicNoise(coord, period)
                #endif

            #endif

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 vertexObject : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };
            
            float _Test;
            float4 _Color;

            // Bubbles
            float _BubbleThickness, _Size;
            float4 _BubbleColor, _Speed;

            // Noise
            float _NoiseScale, _NoisePeriod;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertexObject = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = v.normal;
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 vertexWorld = mul(i.vertexObject, unity_ObjectToWorld);
                float3 position = vertexWorld + _Speed * _Time;
                float r = iLerp(-1, 1, NOISE_FUNC(float3(position * _NoiseScale), _NoisePeriod));

                float bubbles = iLerp(_Size, 1, max(_Size, r));
                
                float bubblesMask = r >= _Size;
                float4 bubbleOutline = map(_BubbleThickness, 1, 1 - bubbles);
                bubbleOutline = lerp(_BubbleColor, color().white, bubbleOutline);
                bubbleOutline = bubblesMask * bubbleOutline;

                float4 col = bubbleOutline + _Color * (1 - bubblesMask);
                return col;
            }
            ENDCG
        }
    }
}
