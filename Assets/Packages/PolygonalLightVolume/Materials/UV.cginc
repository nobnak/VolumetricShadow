#ifndef __UV_CGINC__
#define __UV_CGINC__



float2 UVAtBottom(float2 uv) {
	float2 uvb = uv;
	if (_ProjectionParams.x < 0)
		uvb.y = 1.0 - uvb.y;
	return uvb;
}



#endif
