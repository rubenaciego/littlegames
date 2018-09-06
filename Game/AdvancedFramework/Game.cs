using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using Microsoft.DirectX;
using Direct3D = Microsoft.DirectX.Direct3D;
using DirectSound = Microsoft.DirectX.DirectSound;
using DirectInput = Microsoft.DirectX.DirectInput;

using Game;

namespace AdvancedFramework
{
	public class Game : Form
	{
		public static string gameTitle = "Legend of the Dungeon";
		public static int screenWidth = 1024;
		public static int screenHeight = 576;
		public static bool windowed = true;
		public static bool paused = false;
		
		private static Timer gameTimer = null;
		private static bool graphicsLost = false;
		
		private Direct3D.Device graphics = null;
		private DirectSound.Device sound = null;
		private DirectInput.Device keyboard = null;
		private DirectInput.Device mouse = null;
		private DirectInput.Device controller = null;
		
		private Sprite pressAnyKey;
		private TextRenderer menuRenderer;
		private TextRenderer helpText;
		private Direct3D.Sprite spriteRenderer = null;
		private Direct3D.Texture texture;
		private Direct3D.Texture bombTexture;
		private Direct3D.Texture[] explosionTextures;
		private Sprite menuBackground;
		private DirectSound.SecondaryBuffer menuTheme;
		private Camera camera = null;
		private float deltaTime = 0f;
		private Vector2 movement = Vector2.Empty;
		private bool introMenu = true;
		private Scenary scenary;
		private Level level;
		private float timeQuantity;
		
		public Game()
		{
			ClientSize = new Size(screenWidth, screenHeight);
			Text = gameTitle;
			this.MaximizeBox = false;
			
			this.CenterToScreen();
			gameTimer = new Timer();
			explosionTextures = new Direct3D.Texture[11];
		}

		public void InitializeGraphics()
		{
			// Setup the parameters
			Direct3D.PresentParameters p = new Direct3D.PresentParameters();
			p.SwapEffect = Direct3D.SwapEffect.Discard;
			p.PresentationInterval = Direct3D.PresentInterval.One;

			if (windowed == true)
			{
				p.Windowed = true;
			}
			else
			{
				// Setup a fullscreen display device
				p.Windowed = false;
				p.BackBufferCount = 1;
				p.BackBufferFormat = Direct3D.Manager.Adapters[0].CurrentDisplayMode.Format;
				p.BackBufferWidth =  Direct3D.Manager.Adapters[0].CurrentDisplayMode.Width;
				p.BackBufferHeight = Direct3D.Manager.Adapters[0].CurrentDisplayMode.Height;
			}

			// Create a new device:
			graphics = new Direct3D.Device(0, Direct3D.DeviceType.Hardware, this,
			                               Direct3D.CreateFlags.HardwareVertexProcessing, p);

			// Setup the event handlers for the device
			graphics.DeviceLost += new EventHandler(this.InvalidateDeviceObjects);
			graphics.DeviceReset += new EventHandler(this.RestoreDeviceObjects);
			graphics.Disposing += new EventHandler(this.DeleteDeviceObjects);
			graphics.DeviceResizing += new CancelEventHandler(this.EnvironmentResizing);

			// Setup various drawing options
			graphics.RenderState.CullMode = Direct3D.Cull.None;
			graphics.RenderState.AlphaBlendEnable = true;
			graphics.RenderState.AlphaBlendOperation = Direct3D.BlendOperation.Add;
			graphics.RenderState.DestinationBlend = Direct3D.Blend.InvSourceAlpha;
			graphics.RenderState.SourceBlend = Direct3D.Blend.SourceAlpha;
		}

		public void InitializeSound()
		{
			// Setup a device
			sound = new DirectSound.Device();
			sound.SetCooperativeLevel(this, DirectSound.CooperativeLevel.Normal);
		}

		public void InitializeInput()
		{
			// Setup the keyboard
			keyboard = new DirectInput.Device(DirectInput.SystemGuid.Keyboard);
			keyboard.SetCooperativeLevel(this,
			                             DirectInput.CooperativeLevelFlags.Background |
			                             DirectInput.CooperativeLevelFlags.NonExclusive);
			keyboard.Acquire();

			// Setup the mouse
			mouse = new DirectInput.Device(DirectInput.SystemGuid.Mouse);
			mouse.SetCooperativeLevel(this,
			                          DirectInput.CooperativeLevelFlags.Background |
			                          DirectInput.CooperativeLevelFlags.NonExclusive);
			mouse.Acquire();
			
			// Setup the controller
			foreach (DirectInput.DeviceInstance i in
			         DirectInput.Manager.GetDevices(DirectInput.DeviceClass.GameControl,
			                                        DirectInput.EnumDevicesFlags.AttachedOnly))
			{
				controller = new DirectInput.Device(i.InstanceGuid);
				controller.SetCooperativeLevel(this, DirectInput.CooperativeLevelFlags.Background |
				                               DirectInput.CooperativeLevelFlags.NonExclusive);
				controller.SetDataFormat(DirectInput.DeviceDataFormat.Joystick);
				controller.Acquire();
				
				foreach (DirectInput.DeviceObjectInstance inst in controller.Objects)
				{
					if ((inst.ObjectId & (int)DirectInput.DeviceObjectTypeFlags.Axis) != 0)
					{
						controller.Properties.SetRange(DirectInput.ParameterHow.ById, inst.ObjectId, new DirectInput.InputRange(-10000, 10000));
						controller.Properties.SetDeadZone(DirectInput.ParameterHow.ById, inst.ObjectId, 1500);
					}
				}
			}
		}
		
		public void InitializeResources()
		{
			Level.graphics = graphics;
			
			menuBackground = new Sprite(Direct3D.TextureLoader.FromFile(graphics,
			                                                            "Textures/Menu.png", 0, 0, 0, 0, Direct3D.Format.Unknown,
			                                                            Direct3D.Pool.Managed, Direct3D.Filter.Point,
			                                                            Direct3D.Filter.Point, 0));
			
			texture = Direct3D.TextureLoader.FromFile(graphics,
			                                          "Textures/Sprite.png", 0, 0, 0, 0, Direct3D.Format.Unknown,
			                                          Direct3D.Pool.Managed, Direct3D.Filter.Point,
			                                          Direct3D.Filter.Point, 0);
			
			bombTexture = Direct3D.TextureLoader.FromFile(graphics,
			              "Textures/Bomb.png", 0, 0, 0, 0, Direct3D.Format.Unknown,
			              Direct3D.Pool.Managed, Direct3D.Filter.Point, Direct3D.Filter.Point, 0);
			
			for (int i = 0; i < explosionTextures.Length; i++)
			{
				explosionTextures[i] = Direct3D.TextureLoader.FromFile(graphics,
				                       "Textures/Explosion/frame" + i + ".png", 0, 0, 0, 0, Direct3D.Format.Unknown,
				                        Direct3D.Pool.Managed, Direct3D.Filter.Point, Direct3D.Filter.Point, 0);
			}
			
			pressAnyKey = new Sprite(Direct3D.TextureLoader.FromFile(graphics,
				                       "Textures/PressAnyKey.png", 0, 0, 0, 0, Direct3D.Format.Unknown,
				                       Direct3D.Pool.Managed, Direct3D.Filter.Point, Direct3D.Filter.Point, 0));
			pressAnyKey.position = new Vector2(300,450);
			pressAnyKey.scale = new Vector2(0.5f, 0.5f);
			
			spriteRenderer = new Direct3D.Sprite(graphics);
			camera = new Camera(0, 0, Width, Height);
			scenary = new Scenary(texture);
			
			menuTheme = new DirectSound.SecondaryBuffer("Sound/Menu.wav", sound);
			menuBackground.position = new Vector2(screenWidth / 2 - 370, screenHeight / 2 - 200);
			menuBackground.scale = new Vector2(0.75f, 0.75f);
			
			menuRenderer = new TextRenderer(graphics, 60, FontStyle.Regular, "Hylia Serif Beta",
			               new Point(340, 200), Color.Gold);
			
			helpText = new TextRenderer(graphics, 20, FontStyle.Italic, "Arial",
			                            new Point(50, 500), Color.Gray);
			
			level = new Level(sound, texture, bombTexture, explosionTextures);
			
			menuTheme.Play(0, DirectSound.BufferPlayFlags.Default);
		}
		
		protected virtual void InvalidateDeviceObjects(object sender, EventArgs e)
		{
		}
		
		protected virtual void RestoreDeviceObjects(object sender, EventArgs e)
		{
		}
		
		protected virtual void DeleteDeviceObjects(object sender, EventArgs e)
		{
		}
		
		protected virtual void EnvironmentResizing(object sender, CancelEventArgs e)
		{
			e.Cancel = true;
		}
		
		protected virtual void ProcessFrame()
		{
			// Process the game only while it's not paused
			if (!paused)
			{
				// Get the amount of time that passed since last frame
				deltaTime = gameTimer.Elapsed();
				
				if (!introMenu)
				{
					if (!level.started)
					{
						level.Start();
						level.scenary = scenary;
					}
					
					level.UpdateElements(deltaTime);
				}
				else
					timeQuantity += deltaTime;
			}
			else
			{
				System.Threading.Thread.Sleep(1);
			}
		}
		
		protected virtual void Render()
		{
			if (graphics != null)
			{
				// Check to see if the device has been lost. If so, try to get it back
				if (graphicsLost)
				{
					try
					{
						graphics.TestCooperativeLevel();
					}
					catch (Direct3D.DeviceLostException)
					{
						// Device cannot be reaquired yet, just return
						return;
					}
					catch (Direct3D.DeviceNotResetException)
					{
						// Device has not been reset, but it can be reaquired now
						graphics.Reset(graphics.PresentationParameters);
					}
					graphicsLost = false;
				}

				try
				{
					graphics.Clear(Direct3D.ClearFlags.Target, Color.Black , 1.0f, 0);
					graphics.BeginScene();
					spriteRenderer.Begin(Direct3D.SpriteFlags.AlphaBlend);
					
					if (introMenu)
					{
						camera.position = new Vector2(32f, 32f);
						spriteRenderer.Transform = Matrix.Translation(0f, 0f, 0f);
						scenary.Draw(spriteRenderer, camera);
						
						spriteRenderer.Transform = Matrix.Translation(0f, 0f, 0f);
						menuBackground.Draw(spriteRenderer, camera);
						spriteRenderer.Transform = Matrix.Translation(0f, 0f, 0f);
						menuRenderer.Render("The Legend of the\n        Dungeon", spriteRenderer);
						spriteRenderer.Transform = Matrix.Translation(0f, 0f, 0f);
						helpText.Render("Use WASD to move and Space to attack!", spriteRenderer);
						
						if (timeQuantity > 0.4f)
						{
							pressAnyKey.Active = !pressAnyKey.Active;
							timeQuantity = 0f;
						}
						
						if (pressAnyKey.Active)
							pressAnyKey.Draw(spriteRenderer, camera);
	
						camera.position = new Vector2(0f, 0f);
					}
					else
						level.DrawElements(spriteRenderer, camera);
					
					spriteRenderer.End();
					graphics.EndScene();
					graphics.Present();
				}

				// Device has been lost, and it cannot be re-initialized yet
				catch (Direct3D.DeviceLostException)
				{
					graphicsLost = true;
				}
			}
		}
		
		public void Run()
		{
			// Reset the game timer
			gameTimer.Reset();

			// Loop while form is valid
			while (this.Created)
			{
				// Process one frame of the game
				ProcessFrame();

				// Render the current scene
				Render();

				// Handle all events
				Application.DoEvents();
			}
		}

		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);
		}
		
		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			
			if (e.KeyCode == System.Windows.Forms.Keys.W) { movement.Y = -1f; }
			if (e.KeyCode == System.Windows.Forms.Keys.S) { movement.Y = 1f; }
			if (e.KeyCode == System.Windows.Forms.Keys.A) { movement.X = -1f; }
			if (e.KeyCode == System.Windows.Forms.Keys.D) { movement.X = 1f; }
			if (e.KeyCode == System.Windows.Forms.Keys.Space && level.started) { level.AttackButton(true); }
			
			if (e.KeyCode == System.Windows.Forms.Keys.Enter && introMenu && !level.started)
			{
				introMenu = false;
				menuTheme.Stop();
			}
			
			if (level.started)
				level.InputVector = movement;
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);

			if (e.KeyCode == System.Windows.Forms.Keys.W) { movement.Y = 0f; }
			if (e.KeyCode == System.Windows.Forms.Keys.S) { movement.Y = 0f; }
			if (e.KeyCode == System.Windows.Forms.Keys.A) { movement.X = 0f; }
			if (e.KeyCode == System.Windows.Forms.Keys.D) { movement.X = 0f; }
			if (e.KeyCode == System.Windows.Forms.Keys.Space && level.started) { level.AttackButton(false); }
			
			if (level.started && e.KeyCode == System.Windows.Forms.Keys.Enter && level.winOrLost != 0)
			{
				/*level.StopMusic();
				level = new Level(sound, texture, bombTexture, explosionTextures);
				level.scenary = scenary;
				level.Start();*/
				System.Diagnostics.Process.Start(Application.ExecutablePath);
				this.Close();
				
				// Tanquem i obrim un altre cop l'aplicació perquè
				// de l'altre manera a vegades fa coses rares
			}
			
			if (level.started)
				level.InputVector = movement;
		}
		
		public static void Main()
		{
			Game game = new Game();
			game.InitializeGraphics();
			game.InitializeSound();
			game.InitializeInput();
			game.InitializeResources();

			game.Show();
			game.Run();
		}
	}
}
