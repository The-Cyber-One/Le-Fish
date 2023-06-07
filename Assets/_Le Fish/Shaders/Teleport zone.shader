Shader "Custom/Teleport zone"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _Size ("Size", float) = 1
        _Transparancy ("Transparancy", Range(0, 1)) = 1
        _Outline ("Outline", Float) = 1
        _EmissionIntensity ("Emission Intensity", Range(0, 1)) = 1

        [Toggle] _UseTexture ("Use Texture", Int) = 0
        _OutlineTexture ("Outline texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="Transparent"}
        Cull Off
        ZWrite On
        Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM
        #pragma vertex vert
        #pragma surface surf Lambert alpha
        #pragma target 3.0

        fixed4 _Color;
        float _Transparancy, _Size;
        float _Outline;
        float _EmissionIntensity;
        bool _UseTexture;
        sampler2D _OutlineTexture;

        struct appdata
        {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float4 texcoord : TEXCOORD0;
            float4 outlinecoord : TEXCOORD1;
        };

        struct Input
        {
            float2 uv_MainTex;
            float2 myuv;
            float4 vertex;
            float4 worldVertex;
            float4 outlinecoord;
        };

        // Hexagon stuff: https://godotshaders.com/shader/hexagon-pattern/
        float is_in_hex(float hex_radius, float2 local_point) {
            float2 AXIS[3] = {
                float2(sqrt(3) * 0.5, 0.5),
                float2(0.0, 1.0),
                float2(-sqrt(3)*0.5, 0.5)
            };

            float max_r = 0.0;
            for (int i = 0; i < 3; i++) 
            {
                float r = dot(local_point, AXIS[i]);
                r /= (sqrt(3) * 0.5 * hex_radius);
                max_r = max(max_r, abs(r));
            }
            return max_r;
        }

        float snap_to_center(float local_coord, float hex_radius) {
            return float(floor((local_coord+hex_radius)/(2.0*hex_radius)))*2.0*hex_radius;
        }

        float2 calculate_local_center(float2 uv, float r) {
            float y_coord_1 = snap_to_center(uv.y, r);
            float x_coord_1 = snap_to_center(uv.x, r*sqrt(3));
            float2 point_1 = float2(x_coord_1, y_coord_1);
            
            float x_coord_2 = snap_to_center(uv.x - r*sqrt(3), r*sqrt(3));
            float y_coord_2 = snap_to_center(uv.y - r, r);
            float2 point_2 = float2(x_coord_2, y_coord_2) + float2(r*sqrt(3), r);
            
            if (length(uv - point_1) < length(uv - point_2)) {
                return point_1;
                } else {
                return point_2;
            }
        }

        void vert(inout appdata v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.vertex = v.vertex;
            o.worldVertex = mul(v.vertex, unity_ObjectToWorld);
            o.myuv = v.texcoord.xy;
            o.outlinecoord = v.outlinecoord;
        }

        void surf(in Input i, inout SurfaceOutput o)
        {
            float outline;
            if (_UseTexture)
            {
                outline = tex2D(_OutlineTexture, i.outlinecoord) > 0;
            }
            else
            {
                float2 center = i.vertex.xz;
                outline = abs(center.x) > _Outline || abs(center.y) > _Outline;
            }

            float2 uv = i.worldVertex.xz;// - 0.5;
            float r = _Size * sqrt(3) / 2;
            float2 local_center = calculate_local_center(uv, r);
            float2 local_coords = uv - local_center;
            float hexagonAnimation = abs(local_center.y) > (_SinTime.y * 2 + 1) / 2 * 3;
            float hexagon = is_in_hex(_Size, local_coords) > 0.9;
            float3 c = _Color.rgb;

            o.Albedo = c;
            o.Alpha = (!hexagon && !outline) * _Transparancy + outline * _Transparancy;
            o.Emission = _Color.rgb * _EmissionIntensity * (outline + hexagonAnimation);
        }
        ENDCG
    }
}