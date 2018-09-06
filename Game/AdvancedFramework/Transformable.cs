using System;

using Microsoft.DirectX;

namespace AdvancedFramework
{
	public class Transformable
	{
		public Vector2 position;
		public Vector2 velocity;
		public Vector2 scale;
		public Vector2 anchor;
		public Vector2 scaledAnchor;
		public float angle;
		
		public int height;
		public int width;
		
		public bool invertX;
		public bool invertY;
		
		public Transformable()
		{
			anchor.X = 0.5f;
			anchor.Y = 0.5f;
			
			scale.X = 1f;
			scale.Y = 1f;
		}
		
		public void CalculateScaledAnchor()
		{
			scaledAnchor = anchor;
			scaledAnchor.X *= width;
			scaledAnchor.Y *= height;
		}
		
		public bool CheckCollision(Transformable collidable, float offsetLeft, float offsetRight, float offsetUp, float offsetDown)
		{	
			float maxX = position.X + width - scaledAnchor.X - offsetRight;
			float maxY = position.Y + height - scaledAnchor.Y - offsetDown;
			float minX = position.X - scaledAnchor.X + offsetLeft;
			float minY = position.Y - scaledAnchor.Y + offsetUp;
			
			float maxXColl = collidable.position.X + collidable.width - collidable.scaledAnchor.X;
			float maxYColl = collidable.position.Y + collidable.height - collidable.scaledAnchor.Y;
			float minXColl = collidable.position.X - collidable.scaledAnchor.X;
			float minYColl = collidable.position.Y - collidable.scaledAnchor.Y;
			
			return ((maxX > minXColl && maxX < maxXColl) || (minX < maxXColl && minX > minXColl) || (maxX > maxXColl && minX < minXColl))
				&& ((maxY > minYColl && maxY < maxYColl) || (minY < maxYColl && minY > minYColl) || (maxY > maxYColl && minY < minYColl));
		}
		
		public bool OutOfScreen(float offsetX, float offsetY)
		{
			return position.X + width - scaledAnchor.X > AdvancedFramework.Game.screenWidth ||
				   position.X - scaledAnchor.X < 0 + offsetX || position.Y - scaledAnchor.Y < 0 + offsetY ||
				   position.Y + height - scaledAnchor.Y > AdvancedFramework.Game.screenHeight;
		}
		
		public void Sum(Transformable parent, Transformable transform)
		{
			angle = parent.angle + transform.angle;
			velocity = parent.velocity + transform.velocity;
			
			invertX = parent.invertX != transform.invertX;
			invertY = parent.invertY != transform.invertY;
			
			position.X = parent.position.X + (invertX? -transform.position.X : transform.position.X);
			position.Y = parent.position.Y + (invertY? -transform.position.Y : transform.position.Y);
		}
	}
}
