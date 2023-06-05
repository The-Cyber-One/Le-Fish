#ifndef Section
#define Section
#endif

#ifdef Section // Quick creations
// Int
int2 qint2(int value)
{
    return int2(value, value);
}

int3 qint3(int value)
{
    return int3(qint2(value), value);
}

int4 qint4(int value)
{
    return int4(qint2(value), qint2(value));
}

// Half
half2 qhalf2(half value)
{
    return half2(value, value);
}

half3 qhalf3(half value)
{
    return half3(value, value, value);
}

half4 qhalf4(half value)
{
    return half4(value, value, value, value);
}

// Float
float2 qfloat2(float value)
{
    return float2(value, value);
}

float3 qfloat3(float value)
{
    return float3(value, value, value);
}

float4 qfloat4(float value)
{
    return float4(value, value, value, value);
}

// Colors
struct _color
{
    float4 black;
    float4 red;
    float4 green;
    float4 yellow;
    float4 blue;
    float4 magenta;
    float4 cyan;
    float4 white;
};

_color color()
{
    _color c;
    c.black = float4(0, 0, 0, 1);
    c.red = float4(1, 0, 0, 1);
    c.green = float4(0, 1, 0, 1);
    c.yellow = float4(1, 1, 0, 1);
    c.blue = float4(0, 0, 1, 1);
    c.magenta = float4(1, 0, 1, 1);
    c.cyan = float4(0, 1, 1, 1);
    c.white = float4(1, 1, 1, 1);
    return c;
}
#endif

#ifdef Section // Other functions
float iLerp(float from, float to, float value)
{
    return (value - from) / (to - from);
}

float4 iLerp(float4 from, float4 to, float4 value)
{
    return (value - from) / (to - from);
}
#endif

#ifdef Section // Map
float map(float start, float end, float t, float startRange = 0, float endRange = 1)
{
    t = iLerp(start, end, t);
    return clamp(t, startRange, endRange);
}

float4 map(float4 start, float4 end, float t, float startRange = 0, float endRange = 1)
{
    t = iLerp(start, end, t);
    return clamp(t, startRange, endRange);
}

float4 map(Texture2D<float4> gradient, float t)
{
    uint width, _;
    gradient.GetDimensions(width, _);
    
    float4 a0 = gradient.Load(float3(0, 1, 0));
    int mode = a0[1]; // 0 = blend, 1 = fixed
    float t0 = a0[0];
    float4 c0 = gradient.Load(float3(0, 0, 0));
    float tEnd = gradient.Load(float3(width - 1, 1, 0))[0];
    float4 cEnd = gradient.Load(float3(width - 1, 0, 0));
    
    if (t < t0)
        return c0;
    else if (t > tEnd)
        return cEnd;
    
    if (width > 2)
    {
        for (int i = 0; i < width; i++)
        {
            t0 = gradient.Load(float3(i, 1, 0))[0];
            tEnd = gradient.Load(float3(i + 1, 1, 0))[0];
            if (t >= t0 && t < tEnd)
                break;
        }
        
        c0 = gradient.Load(float3(i, 0, 0));
        cEnd = gradient.Load(float3(i + 1, 0, 0));
    }
    
    t = iLerp(t0, tEnd, t);
    return mode ? cEnd : lerp(c0, cEnd, t);
}
#endif