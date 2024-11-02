#ifndef CAMERATESTCUSTOMFUNCTIONS_INCLUDED
#define CAMERATESTCUSTOMFUNCTIONS_INCLUDED

void RayTracingForEachR_float(float L, float d, float D, float3 P, float3 O, UnityTexture2D Texture, out float3 color) {	
	color = float3(0.f, 0.f, 0.f);
	for (float Ry = -1.f; Ry <= 1.f; Ry += 0.1f) {
		for (float Rx = -1.f; Rx <= 1.f; Rx += 0.1f) {
			float3 R = float3(Rx, Ry, 0.f);
			if (length(R.xy) > 1.f) {
				R = float3(sqrt(1.f - Ry * Ry), Ry, 0.f);
			}

			float3 RO = O - R;
            float3 PR = R - P;
			float3 RB = ((D + d) * RO) / d + (D * PR) / L;

			float3 B = RB + R;
            float2 uv = -B.xy;
			uv *= L / D;
            uv = clamp(uv, 0.f, 1.f); // From -1 to 1 in all directions

			color += SAMPLE_TEXTURE2D_LOD(Texture.tex, Texture.samplerstate, uv.xy, 0).xyz;
            //color += float3(uv.xy, 0.f);
        }
	}
	color /= 400.f;
    //color += SAMPLE_TEXTURE2D_LOD(Texture.tex, Texture.samplerstate, P.xy, 0).xyz;
}

#endif