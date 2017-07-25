using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastRecognizedImages.Neural
{
	public class Neuron
	{
		public string Date { get; set; }
		public float[] Weight { get; set; }
		public float Power { get; set; }
		public Neuron(string date, float[] weight)
		{
			this.Date = date;
			this.Weight = weight;
		}
		public float GetPower(float[] input)
		{
			Power = GPU.GetInstance().GetPowerNeuron(Weight, input);
			return Power;
		}
	}
}
