using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cloo;
using System.Runtime.InteropServices;

namespace FastRecognizedImages
{
	class GPU
	{
		public ComputeContextPropertyList Properties { get; set; }
		public ComputeContext Context { get; set; }
		public List<ComputeDevice> Devs { get; set; }
		ComputeCommandQueue Queue { get; set; }
		private string codeGetPower;
		ComputeProgram prog = null;

		private GPU()
		{
			codeGetPower = GetFileSource("CL.c");
		
		}
        public float GetPowerNeuron(float[] w1, float[] w2)
		{
			float[] res = new float[1];

			ComputeKernel kernelVecSum = prog.CreateKernel("GetPowerNeuron");
			ComputeBuffer<float> bufV1 = new ComputeBuffer<float>(Context, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.UseHostPointer, w1);
			ComputeBuffer<float> bufV2 = new ComputeBuffer<float>(Context, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.UseHostPointer, w2);
			ComputeBuffer<float> bufV3 = new ComputeBuffer<float>(Context, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.UseHostPointer, res);

			kernelVecSum.SetMemoryArgument(0, bufV1);
			kernelVecSum.SetMemoryArgument(1, bufV2);
			kernelVecSum.SetMemoryArgument(2, bufV3);


			ComputeCommandQueue Queue = new ComputeCommandQueue(Context, Cloo.ComputePlatform.Platforms[1].Devices[0], Cloo.ComputeCommandQueueFlags.None);
			Queue.Execute(kernelVecSum, null, new long[] { w1.Length }, null, null);

			float[] arrC = new float[res.Length];
			GCHandle arrCHandle = GCHandle.Alloc(arrC, GCHandleType.Pinned);
			Queue.Read<float>(bufV3, true, 0, res.Length, arrCHandle.AddrOfPinnedObject(), null);

			return arrC[0];
		}
		public string GetFileSource(string fileName)
		{
			string r = "";
			using (var stream = new System.IO.FileStream(fileName, System.IO.FileMode.Open))
			{
				var reader = new System.IO.StreamReader(stream);
				r = reader.ReadToEnd();
			}
				return r;
		}
		private static GPU instance;
		public static GPU GetInstance()
		{
			if (instance == null)
			{
				instance = new GPU();
				instance.Setup(1);
			}
			return instance;
		}
		public void Setup(int idPlatform)
		{
			Properties = new ComputeContextPropertyList(ComputePlatform.Platforms[idPlatform]);
			Context = new ComputeContext(ComputeDeviceTypes.All, Properties, null, IntPtr.Zero);
			ComputeCommandQueue Queue = new ComputeCommandQueue(Context, Cloo.ComputePlatform.Platforms[1].Devices[0], Cloo.ComputeCommandQueueFlags.None);

			Devs = new List<ComputeDevice>();
			Devs.Add(ComputePlatform.Platforms[1].Devices[0]);

			try
			{

				prog = new ComputeProgram(Context, codeGetPower);
				prog.Build(Devs, "", null, IntPtr.Zero);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}
	}
}
