using System.Drawing;
using System.Collections.Generic;

using Microsoft.DirectX;
using Direct3D = Microsoft.DirectX.Direct3D;
using DirectSound = Microsoft.DirectX.DirectSound;
using DirectInput = Microsoft.DirectX.DirectInput;

using AdvancedFramework;

namespace Game
{
	public class BadGuy : Enemy
	{
		private Timer dieTime;
		
		public BadGuy(Direct3D.Texture texture, Player player) : base(texture, player)
		{
			Rect = new Rectangle(384, 704, 128, 128);
			
			life = 20;
			speed = 700;
			scale *= 0.75f;
			
			dieTime = new Timer();
			dieTime.Paused = true;
		}
		
		public override void Draw(Microsoft.DirectX.Direct3D.Sprite renderer, Camera camera)
		{
			if (life <= 0)
			{
				if (dieTime.Paused)
					dieTime.Reset();
				else if (dieTime.ElapsedNoReset() >= 0.5f)
					Active = false;
				
				life = 0;
				Die();
				base.Draw(renderer, camera);
				return;
			}
			
			if (velocity == Vector2.Empty)
			{
				velocity = new Vector2(Level.random.Next(-10, 10), Level.random.Next(-10, 10));
				velocity.Normalize();
				velocity *= speed;
			}
			
			if (canMove)
			{
				if (position.X + width - scaledAnchor.X > AdvancedFramework.Game.screenWidth ||
				     position.X - scaledAnchor.X < 0)
				{
					velocity.X *= -1f;
					velocity.Y += Level.random.Next(-500, 500);
					velocity.Normalize();
					velocity.Multiply(speed);
				}
				
				else if (position.Y + height - scaledAnchor.Y > AdvancedFramework.Game.screenHeight ||
				    position.Y - scaledAnchor.Y < 64)
				{
					velocity.Y *= -1f;
					velocity.X += Level.random.Next(-500, 500);
					velocity.Normalize();
					velocity.Multiply(speed);
				}
			}
			
			#region OutOfBounds
			
			if (position.X + width - scaledAnchor.X > AdvancedFramework.Game.screenWidth)
				position.X = AdvancedFramework.Game.screenWidth + scaledAnchor.X - width;
			else if (position.X - scaledAnchor.X < 0)
				position.X = scaledAnchor.X;
			
			if (position.Y + height - scaledAnchor.Y > AdvancedFramework.Game.screenHeight)
				position.Y = AdvancedFramework.Game.screenHeight + scaledAnchor.Y - height;
			else if (position.Y - scaledAnchor.Y < 64)
				position.Y = scaledAnchor.Y + 64;
			
			#endregion
			
			base.Draw(renderer, camera);
		}
	}
}
