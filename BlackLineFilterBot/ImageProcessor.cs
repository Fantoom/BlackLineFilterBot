using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace BlackLineFilterBot
{
	class ImageProcessor
	{
		public static Task<Bitmap> AddBlackLinesAsync(Bitmap bitmap)
		{
			return Task.Run(() =>
			{
				Bitmap result = new Bitmap(bitmap);
				for (int y = 0; y < bitmap.Height; y += 2)
				{
					for (int x = 0; x < result.Width; x++)
					{
						result.SetPixel(x, y, Color.FromArgb(255, 0, 0, 0));
					}
				}
				return result;
			});
		}

		public static Task<Bitmap> ResizeBitmap(Bitmap bmp, int width, int height)
		{
			return Task.Run(() =>
			{
				Bitmap result = new Bitmap(width, height);
				using (Graphics g = Graphics.FromImage(result))
				{
					g.DrawImage(bmp, 0, 0, width, height);
				}
				return result;
			});
		}
	}
}
