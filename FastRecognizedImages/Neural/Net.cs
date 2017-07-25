using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastRecognizedImages.Neural
{
	public class Net
	{
		public List<Neuron> Neurons { get; set; }
		public Net()
		{
			Neurons = new List<Neuron>();
		}
		public void AddNeuron(string date, float[] input)
		{
			
			Neurons.Add(new Neuron(date, input));
		}
		public Neuron Recognize(float[] input)
		{
			int index = 0;

			for (int i = 0; i < Neurons.Count; i++)
			{
				if (Neurons[i].GetPower(input) > Neurons[index].Power)
					index = i;
			}
			Neurons.ForEach(n => n.Power = 0);

			return Neurons[index];
		}
	}
}
