using System.Drawing;

using Microsoft.DirectX;
using Direct3D = Microsoft.DirectX.Direct3D;
using DirectSound = Microsoft.DirectX.DirectSound;
using DirectInput = Microsoft.DirectX.DirectInput;

using AdvancedFramework;

public class Scenary : Sprite
{
	public Scenary(Direct3D.Texture texture) : base(texture)
	{
		this.texture = texture;
		position = new Vector2(0f, 0f);
		Rect = new Rectangle(0, 0, 64, 64);
	}
	
	public override void Draw(Direct3D.Sprite renderer, Camera camera)
	{	
		RectX = 12*64;
		RectY = 4*64;
		RectWidth = 4*64;
		RectHeight = 3*64;
		
		for (int i = 0; i < 4; i++)
		{
			position.X = (i*width) + 32;
			position.Y = 32;
			base.Draw(renderer, camera);
		}
		
		RectHeight = 64;
		RectWidth = 64;
		RectX = 128;
		RectY = 192;
		
		for (int i = 0; i < AdvancedFramework.Game.screenWidth / width; i++)
		{
			for (int j = 3; j < AdvancedFramework.Game.screenHeight / height; j++)
			{
				position.X = i * width + width/2;
				position.Y = j * height + height/2;
				base.Draw(renderer, camera);
			}
		}
	}
}
