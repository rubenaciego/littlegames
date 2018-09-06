using System;
using System.Drawing;

using Microsoft.DirectX;
using Direct3D = Microsoft.DirectX.Direct3D;

using Game;

namespace AdvancedFramework
{
	public class Sprite : GameObject
    {
		public Color color;
		
        protected Direct3D.Texture texture;
        protected Rectangle rect;

        public Sprite(Direct3D.Texture texture)
        {
            this.Texture = texture;
            color = Color.White;
        }
        
        public Direct3D.Texture Texture
        {
            get { return texture; }
            set 
            {
                texture = value;
                
                Rect = new Rectangle(0, 0, texture.GetLevelDescription(0).Width,  texture.GetLevelDescription(0).Height);
            }
        }
        
        public Rectangle Rect
        {
        	set
        	{
        		rect = value;
        		
        		width = rect.Width;
        		height = rect.Height;
        	}
        }
        
        public int RectX
        {
        	set { rect.X = value; }
        }
        
        public int RectY
        {
        	set { rect.Y = value; }
        }
        
        public int RectWidth
        {
        	set
        	{
        		rect.Width = value;
        		width = rect.Width;
        	}
        }
        
        public int RectHeight
        {
        	set
        	{
        		rect.Height = value;
        		height = rect.Height;
        	}
        }
        
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }
        
        public float CenteredX
        {
        	get { return position.X + scaledAnchor.X; }
        }
        
        public float CenteredY
        {
        	get { return position.Y + scaledAnchor.Y; }
        }
        
        public virtual void Draw(Direct3D.Sprite renderer, Camera camera)
        {           	
        	Vector2 pos = position - camera.Position;
            
        	renderer.Transform = Matrix.Scaling((invertX? -1f : 1f) * scale.X, (invertY? -1f : 1f) * scale.Y, 0f) * Matrix.RotationZ(angle * (float)(Math.PI / 180d)) *
        		Matrix.Translation(Convert.ToInt32(pos.X), Convert.ToInt32(pos.Y), 0f);
        	renderer.Draw(texture, rect, new Vector3(scaledAnchor.X, scaledAnchor.Y, 0f), Vector3.Empty, color);
        }
        
        public virtual void UpdateUI(Direct3D.Sprite sprite)
        {
        	
        }
    }
}
