using System.Drawing;

using Microsoft.DirectX;
using Direct3D = Microsoft.DirectX.Direct3D;
using DirectSound = Microsoft.DirectX.DirectSound;
using DirectInput = Microsoft.DirectX.DirectInput;

using AdvancedFramework;

namespace Game
{
	public class Bomb : Sprite
	{		
		private Direct3D.Texture textureDefault;
		private DirectSound.SecondaryBuffer explosionSound;
		private float explodingTime = 0f;
		private float time = 0f;
		private int spriteNum = 0;
		public bool exploded = false;
		public bool endExplosion = false;
		
		public Bomb(Direct3D.Texture texture) : base(texture)
		{
			explosionSound = new DirectSound.SecondaryBuffer("Sound/8BitExplosion.wav", Level.sound);
			Rect = new Rectangle(15 * 64, 3* 64 , 64, 64);
			this.textureDefault = texture;
		}
		
		private void Animate()
		{
			if (time > 0.5f && !exploded)
			{
				if (textureDefault == base.texture)
				{
					base.Texture = Level.bombTexture;
					position.X -= 1f;
				}
				else
				{
					base.Texture = textureDefault;
					Rect = new Rectangle(15 * 64, 3* 64 , 64, 64);
					position.X += 1f;
				}
				
				time = 0f;
			}
			else if (exploded && !endExplosion)
			{				
				if (explodingTime >= 0.06f)
				{
					scale.X = 0.5f;
					scale.Y = 0.5f;
					
					explodingTime = 0f;
					base.Texture = Level.explosionTextures[spriteNum++];
					
					if (spriteNum > 10)
						endExplosion = true;
				}
			}
		}
		
		public override void Update(float deltaTime)
		{
			time += deltaTime;
			explodingTime += deltaTime;
			
			if (explodingTime >= 3f)
			{
				explosionSound.Play(0, DirectSound.BufferPlayFlags.Default);
				exploded = true;
				explodingTime = 0f;
			}
			
			Animate();
			
			base.Update(deltaTime);
		}
	}
}
