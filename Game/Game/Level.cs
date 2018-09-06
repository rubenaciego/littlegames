using System;
using System.Drawing;
using System.Collections.Generic;

using Microsoft.DirectX;
using Direct3D = Microsoft.DirectX.Direct3D;
using DirectSound = Microsoft.DirectX.DirectSound;

using AdvancedFramework;

namespace Game
{
	public class Level
	{		
		private Player player;
		private bool attackButtonPressed;
		private Powerup powerup;
		private Timer timer;
		private DirectSound.SecondaryBuffer mainTheme;
		private TextRenderer textRenderer;
		private TextRenderer pressEnterText;
		private bool soundPlayed = false;
		private DirectSound.SecondaryBuffer hitSound;
		private DirectSound.SecondaryBuffer itemSound;
		
		public bool started = false;
		public Scenary scenary;
		public byte winOrLost = 0;
		public static Random random = new Random();
		public static Direct3D.Device graphics;
		public static DirectSound.Device sound;
		public static Direct3D.Texture mainTexture;
		public static Direct3D.Texture bombTexture;
		public static Direct3D.Texture[] explosionTextures;
		public static List<Enemy> enemies;
		public static List<Enemy> tempEnemies;
		
		public Vector2 InputVector
		{
			get { return player.velocity; }
			set
			{
				if (player.canMove)
				{
					player.velocity = value;
					player.velocity.Normalize();
					player.velocity *= player.speed;
				}
			}
		}
		
		public Level(DirectSound.Device sound, Direct3D.Texture texture, Direct3D.Texture bombTexture, Direct3D.Texture[] explosionTextures)
		{	
			Level.sound = sound;
			
			enemies = new List<Enemy>();
			tempEnemies = new List<Enemy>(1);
			timer = new Timer();
			
			textRenderer = new TextRenderer(graphics, 100f, FontStyle.Regular, "Bahnschrift",
			                                new Point(150, 200), Color.Green);
			
			pressEnterText = new TextRenderer(graphics, 20f, FontStyle.Regular, "Bahnschrift",
			                                  new Point(320, 400), Color.Yellow);
			
			Level.bombTexture = bombTexture;
			Level.explosionTextures = explosionTextures;
			Level.mainTexture = texture;
			
			mainTheme = new DirectSound.SecondaryBuffer("Sound/Boss-ZeldaALTTP.wav", sound);
			hitSound = new DirectSound.SecondaryBuffer("Sound/8BitHit.wav", Level.sound);
			itemSound = new DirectSound.SecondaryBuffer("Sound/TLoZItem.wav", Level.sound);
		}
		
		~Level()
		{
			GameObject.gameObjects.Clear();
		}
		
		public void Start()
		{
			player = new Player(Level.mainTexture);
			enemies.Add(new Dormin(Level.mainTexture, player));
			enemies[0].position.X = random.Next(500, AdvancedFramework.Game.screenWidth - 20);
			enemies[0].position.Y = random.Next(20, AdvancedFramework.Game.screenHeight - 20);
			
			timer.Reset();
			
			mainTheme.Play(0, DirectSound.BufferPlayFlags.Looping);
			
			started = true;
		}
		
		public static void AddEnemy(Enemy enemy)
		{
			tempEnemies.Add(enemy);
		}
		
		public void UpdateElements(float deltaTime)
		{
			if (winOrLost == 0)
			{
				if (enemies[0].life <= 0)
				{
					deltaTime /= 2f;
					winOrLost = 1;
				}
				else if (player.life <= 0)
				{
					deltaTime /= 2f;
					winOrLost = 2;
				}
			}
			else
				deltaTime /= 2f;
			
			if (timer.ElapsedNoReset() >= 10f && powerup == null)
			{
				powerup = new Powerup(Level.mainTexture);
				powerup.position.X = random.Next(200, AdvancedFramework.Game.screenWidth - 64);
				powerup.position.Y = random.Next(64, AdvancedFramework.Game.screenHeight - 64);
				timer.Reset();
			}
			
			CheckCollisions();
			
			foreach (GameObject i in GameObject.gameObjects)
			{
				if (i.Active)
					i.Update(deltaTime);
			}
			
			for (int i = 0; i < Level.enemies.Count; i++)
			{
				if (!enemies[i].Active)
				{
					GameObject.gameObjects.Remove(enemies[i]);
					enemies.RemoveAt(i);
					i--;
				}
			}
		}
		
		public void DrawElements(Direct3D.Sprite spriteRenderer, Camera camera)
		{
			scenary.Draw(spriteRenderer, camera);
			player.Draw(spriteRenderer, camera);
			
			if (powerup != null)
				powerup.Draw(spriteRenderer, camera);
			
			for (int i = 0; i < enemies.Count; i++)
				enemies[i].Draw(spriteRenderer, camera);
			
			for (int i = 0; i < enemies.Count; i++)
				enemies[i].UpdateUI(spriteRenderer);
			
			player.UpdateUI(spriteRenderer);
			
			while (tempEnemies.Count > 1)
			{
				enemies.Add(tempEnemies[0]);
				tempEnemies.RemoveAt(0);
			}
			
			switch (winOrLost)
			{
				case 1:
					textRenderer.location.X = 130;
					textRenderer.color = Color.SpringGreen;
					spriteRenderer.Transform = Matrix.Translation(0f, 0f, 0f);
					textRenderer.Render("Has Guanyat!!", spriteRenderer);
					pressEnterText.Render("Polsa enter per tornar a començar", spriteRenderer);
					
					if (soundPlayed)
						break;
					
					mainTheme.Stop();
					mainTheme = new DirectSound.SecondaryBuffer("Sound/ALTTPGreatVictory.wav", sound);
					mainTheme.Play(0, DirectSound.BufferPlayFlags.Default);
					
					soundPlayed = true;
					break;
				case 2:
					textRenderer.color = Color.Red;
					spriteRenderer.Transform = Matrix.Translation(0f, 0f, 0f);
					textRenderer.Render("Has Perdut!!", spriteRenderer);
					pressEnterText.Render("Polsa enter per tornar a començar", spriteRenderer);
					
					if (soundPlayed)
						break;
					
					mainTheme.Stop();
					mainTheme = new DirectSound.SecondaryBuffer("Sound/TLoZGameOver.wav", sound);
					mainTheme.Play(0, DirectSound.BufferPlayFlags.Default);
					
					soundPlayed = true;
					break;
			}
		}
		
		public void AttackButton(bool pressed)
		{
			if (pressed)
			{
				if (!attackButtonPressed && player.life > 0)
				{
					player.attack = true;
					attackButtonPressed = true;
				}
			}
			else
			{
				attackButtonPressed = false;
			}
		}
		
		private void CheckCollisions()
		{
			foreach (Enemy enemy in enemies)
			{
				if (enemy.life > 0 &&player.CheckCollision(enemy, 20f, 20f, 20f, 20f) && player.canMove)
				{
					player.canMove = false;
					player.velocity = player.position - enemy.position;
					player.velocity.Normalize();
					player.velocity *= 500f;
					player.color = Color.Red;
					player.life -= 10;
					hitSound.SetCurrentPosition(0);
					hitSound.Play(0, DirectSound.BufferPlayFlags.Default);
				}
				
				if (player.attack && player.sword.CheckCollision(enemy, player.sword.invertX? -40f : 15f,
				    		player.sword.invertX? 15f : -40f, 10f, 0f) && enemy.canMove)
				{
					enemy.canMove = false;
					enemy.velocity = enemy.position - player.position;
					enemy.velocity.Normalize();
					enemy.velocity *= 500f;
					enemy.color = Color.Red;
					enemy.life -= player.sword.damage;
					hitSound.SetCurrentPosition(0);
					hitSound.Play(0, DirectSound.BufferPlayFlags.Default);
				}
				
				if (powerup != null && player.CheckCollision(powerup, 0f, 0f, 0f, 0f))
				{
					player.life += powerup.improvement;
					GameObject.gameObjects.Remove(powerup);
					powerup = null;
					itemSound.Play(0, DirectSound.BufferPlayFlags.Default);
				}
			}
		}
		
		public void StopMusic()
		{
			mainTheme.Stop();
			mainTheme = null;
		}
	}
}
