using System.Drawing;
using System.Collections.Generic;

using Microsoft.DirectX;
using Direct3D = Microsoft.DirectX.Direct3D;
using DirectSound = Microsoft.DirectX.DirectSound;
using DirectInput = Microsoft.DirectX.DirectInput;

using AdvancedFramework;

namespace Game
{
	public class Dormin : Enemy
	{	
		private TextRenderer textRenderer;
		private List<Bomb> bombs;
		private Timer timer2;
		private Timer timer3;
		
		public Dormin(Direct3D.Texture texture, Player player) : base(texture, player)
		{
			Rect = new Rectangle(640, 704, 128, 128);
			
			life = 350;
			speed = 300;
			
			textRenderer = new AdvancedFramework.TextRenderer(Level.graphics, 30f,
			                   FontStyle.Regular, "Bahnschrift", new Point(10, 10), Color.White);
			
			bombs = new List<Bomb>(5);
			timer2 = new Timer();
			timer3 = new Timer();
			timer3.Reset();
			timer2.Reset();
		}
		
		public override void Update(float deltaTime)
		{
			// Intelligence and attack
			
			if (life <= 0)
			{
				life = 0;
				Die();
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
				if ((position.X + width - scaledAnchor.X > AdvancedFramework.Game.screenWidth ||
				   position.X - scaledAnchor.X < 0) || (position.Y + height - scaledAnchor.Y > AdvancedFramework.Game.screenHeight ||
				                                     position.Y - scaledAnchor.Y < 64))
				{
					if (player.life > 0)
						velocity = player.position - position;
					else if ((position.X + width - scaledAnchor.X > AdvancedFramework.Game.screenWidth ||
				  		 position.X - scaledAnchor.X < 0))
						velocity.X *= -1f;
					else if (position.Y + height - scaledAnchor.Y > AdvancedFramework.Game.screenHeight ||
				           position.Y - scaledAnchor.Y < 64)
						velocity.Y *= -1f;
					
					velocity.Normalize();
					velocity *= speed * 2f;
				}
			}
			
			float vel = velocity.Length();
			if (vel > speed)
			{
				velocity.Normalize();
				velocity *= vel - 5f;
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
			
			base.Update(deltaTime);
		}
		
		public override void Draw(Direct3D.Sprite renderer, Camera camera)
		{
			base.Draw(renderer, camera);
			
			if (life <= 0)
				return;
			
			if (timer2.ElapsedNoReset() >= 3f)
			{
				Bomb bomb = new Bomb(base.Texture);
				bomb.position = position;
				bombs.Add(bomb);
				timer2.Reset();
			}
			
			if (timer3.ElapsedNoReset() >= 10f && Level.enemies.Count < 5)
			{
				Enemy enemy = new BadGuy(Level.mainTexture, player);
				enemy.position = position;
				Level.AddEnemy(enemy);
				timer3.Reset();
			}
			
			for (int i = 0; i < bombs.Count; i++)
			{				
				bombs[i].Draw(renderer, camera);
				
				if (bombs[i].exploded)
				{					
					if ((player.position - bombs[i].position).Length() < 100f && player.canMove)
					{
						player.canMove = false;
						player.velocity = player.position - bombs[i].position;
						player.velocity.Normalize();
						player.velocity *= 500f;
						player.color = Color.Red;
						player.life -= 10;
					}
					
					foreach (Enemy enemy in Level.enemies)
					{
						if ((enemy.position - bombs[i].position).Length() < 100f && enemy.canMove)
						{
							enemy.canMove = false;
							enemy.velocity = position - bombs[i].position;
							enemy.velocity.Normalize();
							enemy.velocity *= 500f;
							enemy.color = Color.Red;
							enemy.life -= 10;
						}
					}
				}
				
				if (bombs[i].endExplosion)
				{
					gameObjects.Remove(bombs[i]);
					bombs.RemoveAt(i);
					i--;
				}
			}
		}
		
		public override void UpdateUI(Direct3D.Sprite sprite)
		{
			sprite.Transform = Matrix.Translation(0f, 0f, 0f);
			textRenderer.Render("Enemy: " + life, sprite);
		}
	}
}
