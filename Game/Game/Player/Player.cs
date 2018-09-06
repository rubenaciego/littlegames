using System.Drawing;

using Microsoft.DirectX;
using Direct3D = Microsoft.DirectX.Direct3D;
using DirectSound = Microsoft.DirectX.DirectSound;
using DirectInput = Microsoft.DirectX.DirectInput;

using AdvancedFramework;

namespace Game
{
	public class Player : Sprite
	{
		public int life = 100;
		public int speed = 200;
        public bool canMove = true;
        public bool attack = false;
        public Sword sword = null;
        
        private DirectSound.SecondaryBuffer swordSwing;
		private Timer timer = null;
		private TextRenderer textRenderer = null;
        
        public Player(Direct3D.Texture texture) : base(texture)
        {	
        	Rect = new Rectangle(960 - 128-64, 944, 60, 80);
        	sword = new Sword(texture, this);
        	timer = new Timer();
        	textRenderer = new AdvancedFramework.TextRenderer(Level.graphics, 30f,
			                   FontStyle.Regular, "Bahnschrift", new Point(10, 60), Color.White);
        	
        	swordSwing = new DirectSound.SecondaryBuffer("Sound/ALTTPSword.wav", Level.sound);
        	
        	position.X = 100f;
        	position.Y = 200f;
        }
        
        public override void Update(float deltaTime)
        {
        	if (life <= 0)
        	{
        		life = 0;
        		attack = false;
        		canMove = false;
        		return;
        	}
        	
        	position += velocity * deltaTime;
        	
        	if (OutOfScreen(0, 64))
				position -= velocity * deltaTime;
        	
        	if (canMove)
        	{	        	
	        	if (velocity.X < 0f)
	        		invertX = true;
	        	else if (velocity.X > 0f)
	        		invertX = false;
        	}
        	else
        	{   
        		timer.Paused = false;
        		
        		if (timer.ElapsedNoReset() > 0.25f)
        		{
        			canMove = true;
        			
        			timer.Reset();
        			timer.Paused = true;
        			
        			velocity = Vector2.Empty;
        			color = Color.White;
        		}
        	}
        	
        	if (attack)
        	{
        		swordSwing.Play(0, DirectSound.BufferPlayFlags.Default);
        		
        		if (sword.invertX)
        		{    
        			if (sword.transform.angle < -90f || sword.transform.angle > 0f)
        			{
        				sword.transform.angle = 0f;
        				attack = false;
        			}
        			else
        			{
        				sword.transform.angle -= 600f * deltaTime;
        			}
        		}
        		else
        		{     
        			if (sword.transform.angle > 90f || sword.transform.angle < 0f)
        			{
        				sword.transform.angle = 0f;
        				attack = false;
        			}
        			else
        			{
        				sword.transform.angle += 600f * deltaTime;
        			}
        		}
        	}
        	
        	base.Update(deltaTime);
        }
        
		public override void Draw(Direct3D.Sprite renderer, Camera camera)
		{	
			sword.Draw(renderer, camera);
			base.Draw(renderer, camera);
		}
		
		public override void UpdateUI(Direct3D.Sprite sprite)
		{
			sprite.Transform = Matrix.Translation(0f, 0f, 0f);
			textRenderer.Render("Life: " + life, sprite);
		}
	}
}
