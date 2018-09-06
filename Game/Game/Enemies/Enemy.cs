using System.Drawing;

using Microsoft.DirectX;
using Direct3D = Microsoft.DirectX.Direct3D;
using DirectSound = Microsoft.DirectX.DirectSound;
using DirectInput = Microsoft.DirectX.DirectInput;

using AdvancedFramework;

namespace Game
{
	public abstract class Enemy : Sprite
	{
		public int life;
		public int speed;
		public bool canMove = true;
		
		protected Timer timer;
		protected Player player;
		
		public Enemy(Direct3D.Texture texture, Player player) : base(texture)
		{
			timer = new Timer();
			this.player = player;
		}
		
		public override void Update(float deltaTime)
		{			
			position += velocity * deltaTime;
        	
        	if (canMove)
        	{	        	
	        	if (velocity.X < 0f)
	        		invertX = false;
	        	else if (velocity.X > 0f)
	        		invertX = true;
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
			
			base.Update(deltaTime);
		}
		
		public void Die()
		{
			color = Color.Red;	
		}
	}
}
