#ifndef CAMERATESTCUSTOMFUNCTIONS_INCLUDED
#define CAMERATESTCUSTOMFUNCTIONS_INCLUDED

void RayTracingForEachR_float(float L, float d, float D, float3 P, float3 O, float distantFactor, float angleFactor, UnityTexture2D Texture, out float3 color) {	
    float pi = 3.14159265f;
    float invd = 1.f / d;
    float invD = 1.f / D;
    float invL = 1.f / L;
    
    color = float3(0.f, 0.f, 0.f);
    for (uint distantIndex = 0u; distantIndex < distantFactor; ++distantIndex)
    {
        for (uint angleIndex = 0u; angleIndex < angleFactor; ++angleIndex)
        {
            float r = float(distantIndex) / float(distantFactor);
            float fi = (2.f * pi * (float(angleIndex) + r)) / float(angleFactor);
        
            float3 R = float3(r * cos(fi), r * sin(fi), 0.f);

			float3 RO = O - R;
            float3 PR = R - P;
			float3 RB = ((D + d) * RO) * invd + (D * PR) * invL;

			float3 B = RB + R;
            float2 uv = -B.xy;
			uv *= L * invD;
            uv = clamp(uv, 0.f, float2((Texture.texelSize.z - 1.f) / Texture.texelSize.z, (Texture.texelSize.w - 1.f) / Texture.texelSize.w)); // From 0 to 1 in all directions

			color += SAMPLE_TEXTURE2D_LOD(Texture.tex, Texture.samplerstate, uv.xy, 0).xyz;
            //color += float3(uv.xy, 0.f);
        }
	}
    color /= distantFactor * angleFactor;
    //color += SAMPLE_TEXTURE2D_LOD(Texture.tex, Texture.samplerstate, P.xy, 0).xyz;
}

#endif