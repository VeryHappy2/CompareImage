using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Runtime.InteropServices;

namespace CompareImg
{
    internal class Program
    {
		private static Rgba32 invalidPixel = new() { A = 255, B = 255, R = 255, G = 0};
        static void Main(string[] args)
        {
            if (args.Length <= 1 || string.IsNullOrEmpty(args[0]) || string.IsNullOrEmpty(args[1]))
			{
				Console.Error.WriteLine("There is no path. Example: CompareImg.exe \"C:\\img1.png\" \"C:\\img2.png\"");
				return;
			}

			using var inputImg1 = Image.Load<Rgba32>(args[0]);
			using var inputImg2 = Image.Load<Rgba32>(args[1]);

			if (inputImg1.Size != inputImg2.Size)
			{
				Console.Error.WriteLine("Size both of images must be the same");
				return;
			}

			CheckPixels(inputImg1, inputImg2, 30);

			SaveOutputFile(ref args, inputImg1);
		}

		private static void SaveOutputFile(ref string[] args, Image<Rgba32> img1)
		{
			if (args.Length >= 3 && !string.IsNullOrEmpty(args[2]))
			{
				if (Directory.Exists(Directory.GetParent(args[2]).FullName))
				{
					img1.Save(args[2]);
					Console.WriteLine("The file was saved with the path: " + args[2]);
				}
				else
					Console.WriteLine(args[2] + " - such directory doesn't exist");
			}
			else
			{
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				{
					int i = 0;
					while (File.Exists($"C:\\changed_img{i}.png"))
						i++;

					string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"changed_img{i}.png");
					img1.Save(path);
					Console.WriteLine("The file was saved with the path: " + path);
				}
				else
					Console.WriteLine("There is no path to save the output file.");
			}
		}

		private static void CheckPixels(Image<Rgba32> img1, Image<Rgba32> img2, int allowedRange)
		{
			for (int y = 0; y < img1.Height; y++)
			{
				for (int x = 0; x < img1.Width; x++)
				{
					Rgba32 pixel1 = img1[x, y];
					Rgba32 pixel2 = img2[x, y];
					int diffR = pixel1.R - pixel2.R;
					int diffG = pixel1.G - pixel2.G;
					int diffB = pixel1.B - pixel2.B;
					int diffA = pixel1.A - pixel2.A;

					bool r = Math.Abs(diffR) > allowedRange;
					bool g = Math.Abs(diffG) > allowedRange;
					bool b = Math.Abs(diffB) > allowedRange;
					bool a = Math.Abs(diffA) > allowedRange;

					if (r || g || b || a)
					{
						Console.Error.WriteLine($"X: {x}, Y: {y} pixels are different." +
							$"\nPixel of the first img: R = {pixel1.R}, G = {pixel1.G}, B = {pixel1.B}, A = {pixel1.A}");
						pixel1 = invalidPixel;
						img1[x, y] = pixel1;
					}
				}
			}
		}
    }
}
