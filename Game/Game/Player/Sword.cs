using System.Drawing;

using Microsoft.DirectX;
using Direct3D = Microsoft.DirectX.Direct3D;

using AdvancedFramework;

namespace Game
{
	public class Sword : Sprite
	{			
		public Transformable transform;
		public int damage;
		
		private Transformable parent;
		
		public Sword(Direct3D.Texture texture, Transformable parent) : base(texture)
		{
			Rect = new Rectangle(908 - 64, 168-12, 40, 84+12);
			transform = new Transformable();
			
			this.parent = parent;
			
			transform.position.X = 30f;
			transform.position.Y = 32f;
			
			damage = 5;
			anchor.Y = 1f;
		}
		
		public override void Update(float deltaTime)
		{
			Sum(parent, transform);
			base.Update(deltaTime);
		}
	}
}
