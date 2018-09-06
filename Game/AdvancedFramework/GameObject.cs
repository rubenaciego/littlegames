using System.Collections.Generic;

namespace AdvancedFramework
{
	public abstract class GameObject : Transformable
	{
		private bool active;
		
		public static List<GameObject> gameObjects;
		
		public bool Active
		{
			get { return active; }
			set { active = value; }
		}
		
		static GameObject()
		{
			gameObjects = new List<GameObject>(20);
		}
		
		public GameObject()
		{
			active = true;
			gameObjects.Add(this);
		}
		
		~GameObject()
		{
			gameObjects.Remove(this);
		}
		
		public virtual void Update(float deltaTime)
		{
			CalculateScaledAnchor();
		}
	}
}
