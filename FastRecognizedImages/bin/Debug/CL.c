
__kernel void GetPowerNeuron(__global float * v1, __global float * v2, __global float * r)
{
	int i = get_global_id(0);

	r[0] += (v1[i] / 10) * (v2[i] / 10);
}

