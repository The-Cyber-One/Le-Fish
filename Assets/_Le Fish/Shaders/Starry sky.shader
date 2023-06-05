Shader "Unlit/Starry sky"
{
    Properties
    {
        _ObjectSize ("Object Size", FLOAT) = 1

        _NebulaNoise ("Nebula Noise Scale", FLOAT) = 1
        _NebulaMin ("Nebula Min", RANGE(0.0, 1.0)) = 0
        _NebulaMax ("Nebula Max", RANGE(0.0, 1.0)) = 1
        _NebulaSpeed ("Nebula Speed", FLOAT) = 1
        _LineNoise ("Nebula Line Scale", FLOAT) = 1
        _LineMin ("Nebula Line Size", RANGE(0.0, 1.0)) = 0

        _StarNebulaDistance ("Distance between Nebula and Starrs", RANGE(0.0, 1.0)) = 0
        _StarNoise ("Star Noise Scale", FLOAT) = 1
        _StarSpeed ("Star Speed", FLOAT) = 1
        _StarDensity ("Star Density", RANGE(0.0, 1.0)) = 0

        [Gradient] _Gradient ("Nebula Gradient", 2D) = "white" {}

        [Header(Astronaut)]
        _Radius ("Radius", FLOAT) = 1
        _Astronaut ("Astronaut", 2D) = "white" {}
        _LetterA ("A", 2D) = "white" {}
        _LetterDistance ("Letter distance", FLOAT) = 0.05
        _AstronautSpeed ("Speed", FLOAT) = 1
        _LetterAmount ("Amount", INT) = 3
        _MovementSpace ("Movement Space", FLOAT) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Common.hlsl"
            #include "FastNoiseLite.hlsl"

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

            float _Radius, _AstronautSpeed, _LetterDistance;
            int _LetterAmount;
            Texture2D _Astronaut, _LetterA, _Gradient;
            float4 _Astronaut_ST, _LetterA_ST;
            SamplerState my_linear_clamp_sampler;
            SamplerState my_point_clamp_sampler;

            float _StarSpeed;
            float _NebulaSpeed;

            float _ObjectSize;
            float _NebulaNoise, _LineNoise, _StarNoise;
            float _NebulaMin, _NebulaMax, _LineMin;
            float _StarNebulaDistance;
            float _StarDensity;

            float _MovementSpace;

            // Shaders
            fixed4 astronautMover(v2f input, fnl_state noise)
            {
                float4 shape;
                for (int i = 0; i < _LetterAmount; i++)
                {
                    float scaledTime = (_Time + i * _LetterDistance) * _AstronautSpeed;
                    float x = fnlGetNoise2D(noise, scaledTime, scaledTime);
                    float y = fnlGetNoise2D(noise, scaledTime + 100, scaledTime + 100);
                    x = input.objectVertex.x - x * _MovementSpace;
                    y = input.objectVertex.z - y * _MovementSpace;
                    
//return float4(x,y,0,1);

                    float4 image;
                    if (i == _LetterAmount - 1){
                        image = _Astronaut.Sample(my_linear_clamp_sampler, float2(x,y) * ((_Radius * _LetterAmount)/i) + _Astronaut_ST.zw );
                        shape = image.a > 0 ? 0 : shape;
                    }
                    else {
                        image = _LetterA.Sample(my_linear_clamp_sampler, float2(x,y) * ((_Radius * _LetterAmount)/i) + _LetterA_ST.zw ).a;
                    }
                    shape += image * image.a;
                }
                return shape;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.objectVertex = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                fnl_state noise = fnlCreateState();
                noise.noise_type = FNL_NOISE_PERLIN;

                float3 vertexWorld = mul(i.objectVertex, unity_ObjectToWorld);

                float nebula = map(-1, 1, fnlGetNoise3D(noise, i.objectVertex.x * _NebulaNoise, _Time * _NebulaSpeed, i.objectVertex.z * _NebulaNoise));
                float lineValue = (i.objectVertex.x + i.objectVertex.z + _ObjectSize) / 2 * (1 / _ObjectSize);
                float lines = map(-1, 1, fnlGetNoise3D(noise, lineValue * _LineNoise, lineValue * _LineNoise, _Time));
                float stars = map(-1, 1, fnlGetNoise3D(noise, i.objectVertex.x * _StarNoise, _Time * _StarSpeed, i.objectVertex.z * _StarNoise));

                float4 col = 0;
                float4 nebulaMask = map(_NebulaMin, _NebulaMax, nebula) * map(_LineMin, 1, lines);
                col += nebulaMask * map(_Gradient, lineValue);
                col += map(_StarNebulaDistance, 1, 1 - nebulaMask) * map(_StarDensity, 1, stars * (stars > _StarDensity));
                col += astronautMover(i, noise);
                return col;
            }

            ENDCG
        }
    }
}
