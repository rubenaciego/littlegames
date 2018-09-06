using System.Drawing;

using Microsoft.DirectX;
using Direct3D = Microsoft.DirectX.Direct3D;
using DirectSound = Microsoft.DirectX.DirectSound;
using DirectInput = Microsoft.DirectX.DirectInput;

using AdvancedFramework;

namespace Game
{
	public class Powerup : Sprite
	{
		public byte improvement = 10;
		
		public Powerup(Direct3D.Texture texture) : base(texture)
		{
			Rect = new Rectangle(12 * 64, 11 * 64, 64, 64);
		}
	}
}
