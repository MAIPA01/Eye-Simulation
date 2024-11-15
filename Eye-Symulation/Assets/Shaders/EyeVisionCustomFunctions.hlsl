#ifndef CAMERATESTCUSTOMFUNCTIONS_INCLUDED
#define CAMERATESTCUSTOMFUNCTIONS_INCLUDED

float Remap(float value, float2 InMinMax, float2 OutMinMax)
{
    float value01 = (value - InMinMax.x) / (InMinMax.y - InMinMax.x); // value from 0 to 1
    return value01 * (OutMinMax.y - OutMinMax.x) + OutMinMax.x; // value from outMin to outMax
}

float2 Remap(float2 value, float2 InMinMax, float2 OutMinMax)
{
    return float2(Remap(value.x, InMinMax, OutMinMax), Remap(value.y, InMinMax, OutMinMax));
}

void RayTracingForEachR_float(float eyeLength /* L */, float sfera /* ds */, float cylinder /* cs */, float os /* os */, float objectDistance /* D */, float focusDistance, float3 retinaPoint /* P */, float3 lenCenter /* O */, float distantFactor, float angleFactor, float lenRadius, UnityTexture2D Texture, out float3 color) {	    
    float pi = 3.14159265f;
    float invObjectDistance = 1.f / objectDistance;
    float invEyeLength = 1.f / eyeLength;
    
    if (sfera > 0)
        sfera += 1;
    
    float normalFocalLength = eyeLength / (1.f - eyeLength * sfera);
    float astigmatismFocalLength = eyeLength / (1.f - eyeLength * (sfera + cylinder));
    float cosA = cos(os);
    float cosAsqr = cosA * cosA;
    float sinA = sin(os);
    float sinAsqr = sinA * sinA;
    float invR1sqr = 1.f / (astigmatismFocalLength * astigmatismFocalLength);
    float invR2sqr = 1.f / (normalFocalLength * normalFocalLength);
    float A = cosAsqr * invR1sqr + sinAsqr * invR2sqr;
    float B = -2.f * sinA * cosA * (invR1sqr + invR2sqr);
    float C = sinAsqr * invR1sqr + cosAsqr * invR2sqr;
    
    color = float3(0.f, 0.f, 0.f);
    for (uint distantIndex = 0u; distantIndex < distantFactor; ++distantIndex)
    {
        float sampleDistance = float(distantIndex) / distantFactor; // r
        for (uint angleIndex = 0u; angleIndex < angleFactor; ++angleIndex)
        {
            float sampleAngle = (2.f * pi * (float(angleIndex) + sampleDistance)) / angleFactor; // fi
            float2 v = float2(cos(sampleAngle), sin(sampleAngle));
            float3 lenSamplePoint = float3(v.xy, 0.f) * sampleDistance * lenRadius; // R

            float focalLength = 1.f / sqrt(v.x * v.x * A + v.x * v.y * B + v.y * v.y * C);
            if (focalLength > eyeLength)
                lenSamplePoint *= invObjectDistance * 2.f;
            else if (focalLength < eyeLength)
                lenSamplePoint *= 0.4f;
            
			float3 RO = lenCenter - lenSamplePoint; // O - R
            float3 PR = lenSamplePoint - retinaPoint; // R - P
            float3 RB = ((objectDistance + focalLength) * RO) / focalLength + (objectDistance * PR) * invEyeLength;

            float3 B = RB + retinaPoint;
            float2 uv = -B.xy;
            uv *= eyeLength * invObjectDistance;
            uv = Remap(uv, float2(-1.f, 1.f), float2(0.f, 1.f));
            uv = clamp(uv, 0.f, float2((Texture.texelSize.z - 1.f) / Texture.texelSize.z, (Texture.texelSize.w - 1.f) / Texture.texelSize.w)); // From 0 to 1 in all directions

			color += SAMPLE_TEXTURE2D_LOD(Texture.tex, Texture.samplerstate, uv.xy, 0).xyz;
        }
	}
    color /= distantFactor * angleFactor;
}

void EyeSimulation_float(float defaultFocalLength /* dl */, float sfera /* ds */, float cylinder /* cs */, float os /* os */, float objectDistance /* Do */, float focusDistance /* Df */, float2 uv /* uv */, float3 lenCenter /* O */, float distantFactor, float angleFactor, float lenRadius, UnityTexture2D Texture, out float3 color)
{
    float pi = 3.14159265f;
    float eyeLength = defaultFocalLength / (1.f + defaultFocalLength * sfera);
    
    float3 retinaPoint = float3(uv.xy, -eyeLength);
    float invObjectDistance = 1.f / objectDistance;
    float invEyeLength = 1.f / eyeLength;
    
    float normalFocalLength = eyeLength * focusDistance / (focusDistance - eyeLength); //eyeLength / (1.f + eyeLength * sfera);
    float astigmatismFocalLength = 0.f;
    float cosA = 0.f;
    float cosAsqr = 0.f;
    float sinA = 0.f;
    float sinAsqr = 0.f;
    float invR1sqr = 0.f;
    float invR2sqr = 0.f;
    /*if (cylinder != 0.f)
    {
        astigmatismFocalLength = eyeLength / (1.f + eyeLength * (sfera + cylinder));
        cosA = cos(os);
        cosAsqr = cosA * cosA;
        sinA = sin(os);
        sinAsqr = sinA * sinA;
        invR1sqr = 1.f / (astigmatismFocalLength * astigmatismFocalLength);
        invR2sqr = 1.f / (normalFocalLength * normalFocalLength);
    }*/
    
    color = float3(0.f, 0.f, 0.f);
    for (uint distantIndex = 0u; distantIndex < distantFactor; ++distantIndex)
    {
        float sampleDistance = float(distantIndex) / distantFactor; // r
        for (uint angleIndex = 0u; angleIndex < angleFactor; ++angleIndex)
        {
            float sampleAngle = (2.f * pi * (float(angleIndex) + sampleDistance)) / angleFactor; // fi
            float3 lenSamplePoint = float3(cos(sampleAngle), sin(sampleAngle), 0.f) * sampleDistance * lenRadius; // R

            float focalLength = normalFocalLength;
            /*if (cylinder != 0.f)
            {
                float2 v = float2(sin(sampleAngle), cos(sampleAngle));
                float2 vsqr = float2(v.x * v.x, v.y * v.y);
                focalLength = 1.f / sqrt(vsqr.x * (cosAsqr * invR1sqr + sinAsqr * invR2sqr) - 2.f * v.x * v.y * sinA * cosA * (invR1sqr + invR2sqr) + vsqr.y * (sinAsqr * invR1sqr + cosAsqr * invR2sqr));
            }*/
            
            float3 RO = lenCenter - lenSamplePoint; // O - R
            float3 PR = lenSamplePoint - retinaPoint; // R - P
            float3 RB = ((objectDistance + focalLength) * RO) / focalLength + (objectDistance * PR) * invEyeLength;

            float3 B = RB + retinaPoint;
            float2 uv = -B.xy;
            uv *= eyeLength * invObjectDistance;
            uv = Remap(uv, float2(-1.f, 1.f), float2(0.f, 1.f));
            uv = clamp(uv, 0.f, float2((Texture.texelSize.z - 1.f) / Texture.texelSize.z, (Texture.texelSize.w - 1.f) / Texture.texelSize.w)); // From 0 to 1 in all directions

            color += SAMPLE_TEXTURE2D_LOD(Texture.tex, Texture.samplerstate, uv.xy, 0).xyz;
        }
    }
    color /= distantFactor * angleFactor;
}

#endif