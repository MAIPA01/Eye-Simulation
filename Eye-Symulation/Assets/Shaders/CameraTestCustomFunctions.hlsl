#ifndef CAMERATESTCUSTOMFUNCTIONS_INCLUDED
#define CAMERATESTCUSTOMFUNCTIONS_INCLUDED

float2 ClampVec(float2 value, float minValue, float maxValue) {
	return float2(clamp(value.x, minValue, maxValue), clamp(value.y, minValue, maxValue));
}

void RayTracingForEachR_float(float L, float d, float D, float3 P, float3 O, UnityTexture2D Texture, out float3 color) {
	color = float3(0.f, 0.f, 0.f);
	for (float Ry = -1.f; Ry <= 1.f; Ry += 0.1f) {
		for (float Rx = -1.f; Rx <= 1.f; Rx += 0.1f) {
			float3 R = float3(Rx, Ry, 0.f);
			if (Ry * Ry + Rx * Rx > 1.f) {
				R = float3(sqrt(1.f - Ry * Ry), Ry, 0.f);
			}

			float3 RO = O - R;
			float3 PR = R - P;
			float3 RB = ((D + d) * RO) / d + (D * PR) / L;

			float3 B = RB + R;
            float2 uv = -B.xy;
			uv *= L / D;
			uv = ClampVec(uv, 0.f, 1.f);

			color += SAMPLE_TEXTURE2D_LOD(Texture.tex, Texture.samplerstate, B.xy / 100000000.f, 0).xyz;
		}
	}
	color /= 400.f;
    //color += SAMPLE_TEXTURE2D_LOD(Texture.tex, Texture.samplerstate, P.xy, 0).xyz;
}

#endif