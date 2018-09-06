using Microsoft.DirectX;

namespace AdvancedFramework
{
    public class Camera
    {
        public Vector2 position;
        public Vector2 screenSize;

        public Camera(int x, int y, int screenX, int screenY)
        {
            position.X = x;
            position.Y = y;
            screenSize.X = screenX;
            screenSize.Y = screenY;
        }
        
        public void CenterOn(float x, float y)
        {
        	position.X = x - screenSize.X / 2;
        	position.Y = y - screenSize.Y / 2;
        }
        
        public Vector2 Position
        {
            get { return position; }
        }
    }
}
