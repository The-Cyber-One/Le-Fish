Shader "Custom/Bubbles"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)

        [Header(Bubble Properties)]
        _BubbleColor ("Color", Color) = (1, 1, 1, 1)
        _NoiseScale ("Bubble Scale", Float) = 100
        _Size ("Size", Range(0, 1)) = 0.5
        _BubbleThickness ("Thickness", Range(0, 1)) = 0.5
        _Speed ("Speed", Vector) = (0, 0, 0)

        [Header(Outline)]
        _OutlineColor ("Color", Color) = (1, 1, 1, 1)
        _OutlineThickness ("Thickness", Range(0, 1)) = 0.5
        
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
            #pragma vertex vert alpha
            #pragma fragment frag alpha

            #include "UnityCG.cginc"
            #include "Common.hlsl"
            #include "FastNoiseLite.hlsl"

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
            
            float4 _Color;

            // Bubbles
            float _BubbleThickness, _Size;
            float4 _BubbleColor, _Speed;

            // Noise
            float _NoiseScale;

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
                fnl_state noise = fnlCreateState();
                noise.noise_type = FNL_NOISE_PERLIN;

                float3 vertexWorld = mul(i.vertexObject, unity_ObjectToWorld);
                float3 position = vertexWorld + _Speed * _Time;
                position *= _NoiseScale;
                float noiseValue = iLerp(-1, 1, fnlGetNoise3D(noise, position.x, position.y, position.z));

                float bubbles = iLerp(_Size, 1, max(_Size, noiseValue));
                
                float bubblesMask = noiseValue >= _Size;
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
