Shader "Unlit/Circle"
{
    Properties
    {
        _Color("Color", COLOR) = (1, 1, 1, 1)
        _Radius("Radius", FLOAT) = 1
        _Speed("Speed", FLOAT) = 1
    }
        SubShader
    {
        Tags { "RenderType" = "Opaque" }
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
                float4 objectVertex : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            float4 _Color;
            float _Radius, _Speed;

            v2f vert(appdata v)
            {
                v2f o;
                o.objectVertex = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float _distance = abs(distance(float2(0, 0), i.objectVertex.xy) - _Time * _Speed);
                return  (1-_distance % _Radius) * _Color;
            }
            ENDCG
        }
    }
}