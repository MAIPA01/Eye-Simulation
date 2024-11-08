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

void RayTracingForEachR_float(float eyeLength /* L */, float sfera /* ds */, float objectDistance /* D */, float3 retinaPoint /* P */, float3 lenCenter /* O */, float distantFactor, float angleFactor, float lenRadius, UnityTexture2D Texture, out float3 color) {	    
    float pi = 3.14159265f;
    float invObjectDistance = 1.f / objectDistance;
    float invEyeLength = 1.f / eyeLength;
    float focalLength = eyeLength / (1.f + eyeLength * sfera);
    float invFocalLength = 1.f / focalLength;
    
    color = float3(0.f, 0.f, 0.f);
    for (uint distantIndex = 0u; distantIndex < distantFactor; ++distantIndex)
    {
        float r = float(distantIndex) / distantFactor;
        for (uint angleIndex = 0u; angleIndex < angleFactor; ++angleIndex)
        {
            float fi = (2.f * pi * (float(angleIndex) + r)) / angleFactor;
        
            float3 lenSamplePoint = float3(r * cos(fi), r * sin(fi), 0.f) * lenRadius; // R

			float3 RO = lenCenter - lenSamplePoint; // O - R
            float3 PR = lenSamplePoint - retinaPoint; // R - P
            float3 RB = ((objectDistance + focalLength) * RO) * invFocalLength + (objectDistance * PR) * invEyeLength;

            float3 B = RB + retinaPoint;
            float2 uv = -B.xy;
            uv *= eyeLength * invObjectDistance;
            uv = Remap(uv, float2(-1.f, 1.f), float2(0.f, 1.f));
            uv = clamp(uv, 0.f, float2((Texture.texelSize.z - 1.f) / Texture.texelSize.z, (Texture.texelSize.w - 1.f) / Texture.texelSize.w)); // From 0 to 1 in all directions

			color += SAMPLE_TEXTURE2D_LOD(Texture.tex, Texture.samplerstate, uv.xy, 0).xyz;
        }
	}
    color /= distantFactor * angleFactor;
    //color += SAMPLE_TEXTURE2D_LOD(Texture.tex, Texture.samplerstate, P.xy, 0).xyz;
}

#endif