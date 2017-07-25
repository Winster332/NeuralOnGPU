using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FastRecognizedImages.UI
{
	/// <summary>
	/// Interaction logic for DrawingLayout.xaml
	/// </summary>
	public partial class DrawingLayout : UserControl
	{
		public DrawingLayout()
		{
			InitializeComponent();
		}
		public void GetImage()
		{
			RenderTargetBitmap rtb = new RenderTargetBitmap((int)canvas.RenderSize.Width,
	(int)canvas.RenderSize.Height, 96d, 96d, System.Windows.Media.PixelFormats.Default);
			rtb.Render(canvas);

			var crop = new CroppedBitmap(rtb, new Int32Rect(0, 0, (int)canvas.RenderSize.Width,
				(int)canvas.RenderSize.Height));



			var img = CreateResizedImage(crop, 50, 50, 0);
			int stride = img.PixelWidth * 4;
			int size = img.PixelHeight * stride;
			byte[] pixels = new byte[size];
			img.CopyPixels(pixels, stride, 0);

			BitmapEncoder pngEncoder = new PngBitmapEncoder();
			pngEncoder.Frames.Add(BitmapFrame.Create(img));

			using (var fs = System.IO.File.OpenWrite("logo.png"))
			{
				pngEncoder.Save(fs);
			}
		}
		private static BitmapFrame CreateResizedImage(ImageSource source, int width, int height, int margin)
		{
			var rect = new Rect(margin, margin, width - margin * 2, height - margin * 2);

			var group = new DrawingGroup();
			RenderOptions.SetBitmapScalingMode(group, BitmapScalingMode.HighQuality);
			group.Children.Add(new ImageDrawing(source, rect));

			var drawingVisual = new DrawingVisual();
			using (var drawingContext = drawingVisual.RenderOpen())
				drawingContext.DrawDrawing(group);

			var resizedImage = new RenderTargetBitmap(
				width, height,         // Resized dimensions
				96, 96,                // Default DPI values
				PixelFormats.Default); // Default pixel format
			resizedImage.Render(drawingVisual);

			return BitmapFrame.Create(resizedImage);
		}
		public float[] GetInput()
		{
			RenderTargetBitmap rtb = new RenderTargetBitmap((int)canvas.RenderSize.Width,
	   (int)canvas.RenderSize.Height, 96d, 96d, System.Windows.Media.PixelFormats.Default);
			rtb.Render(canvas);

			var crop = new CroppedBitmap(rtb, new Int32Rect(0, 0, (int)canvas.RenderSize.Width,
				(int)canvas.RenderSize.Height));



			var img = CreateResizedImage(crop, 300, 250, 0);
			int stride = img.PixelWidth * 4;
			int size = img.PixelHeight * stride;
			byte[] pixels = new byte[size];
			img.CopyPixels(pixels, stride, 0);

			//BitmapEncoder pngEncoder = new PngBitmapEncoder();
			//pngEncoder.Frames.Add(BitmapFrame.Create(img));

			//using (var fs = System.IO.File.OpenWrite("logo.png"))
			//{
			//	pngEncoder.Save(fs);
			//}

			return Normalize(pixels);
		}
		private float[] Normalize(byte[] bytes)
		{
			float[] result = new float[bytes.Length / 4];
			int index = 0;

			for (int i = 0; i < bytes.Length; i += 4)
			{
				byte r = bytes[i];
				byte g = bytes[i + 1];
				byte b = bytes[i + 2];
				byte a = bytes[i + 3];

				result[index] = r + g + b;

				if (result[index] != 765)
					result[index] = 0.08f;
				else result[index] = -0.02f;

				index++;
            }

			return result;
		}
		public void Clear()
		{
			canvas.Children.Clear();
		}
		private Point prev;
		private SolidColorBrush color = new SolidColorBrush(Colors.White);
		private bool isPaint = false;

		private const int SIZE = 4;
		private const int SHIFT = SIZE / 2;

		private void OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (isPaint) return;
			isPaint = true;
			prev = Mouse.GetPosition(canvas);
			var dot = new Ellipse { Width = SIZE, Height = SIZE, Fill = color };
			dot.SetValue(Canvas.LeftProperty, prev.X - SHIFT);
			dot.SetValue(Canvas.TopProperty, prev.Y - SHIFT);
			canvas.Children.Add(dot);
		}

		private void OnMouseUp(object sender, MouseButtonEventArgs e)
		{
			isPaint = false;
		}

		private void OnMouseMove(object sender, MouseEventArgs e)
		{
			if (!isPaint) return;
			var point = Mouse.GetPosition(canvas);
			var line = new Line
			{
				Stroke = color,
				StrokeThickness = SIZE,
				X1 = prev.X,
				Y1 = prev.Y,
				X2 = point.X,
				Y2 = point.Y,
				StrokeStartLineCap = PenLineCap.Round,
				StrokeEndLineCap = PenLineCap.Round
			};
			prev = point;
			canvas.Children.Add(line);
		}

		private void button_Click(object sender, RoutedEventArgs e)
		{
			Clear();
		}
	}
}
