using System;
using System.Drawing;

using Microsoft.DirectX;
using Direct3D = Microsoft.DirectX.Direct3D;

namespace AdvancedFramework
{	
	public class TextRenderer
	{
		public Direct3D.Font font;
		public Point location;
		public Color color;
		
		public TextRenderer(Direct3D.Device device, float size, FontStyle style,
		                    string fontname, Point location, Color color)
		{
			font = new Direct3D.Font(device, new Font(fontname, size, style));
			
			this.location = location;
			this.color = color;
		}
		
		public void Render(string text, Direct3D.Sprite sprite)
		{
			font.DrawText(sprite, text, location, color);
		}
	}
}
