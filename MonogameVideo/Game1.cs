using System;
using Furball.Engine.Engine.Graphics.Video;
using Kettu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Silk.NET.Core.Native;

namespace MonogameVideo;

public class Game1 : Game {
	private GraphicsDeviceManager _graphics;
	private SpriteBatch           _spriteBatch;

	public VideoDecoder VideoDecoder;
	public Texture2D    VideoTexture;

	public Game1() {
		_graphics             = new GraphicsDeviceManager(this);
		Content.RootDirectory = "Content";
		IsMouseVisible        = true;

		this.Window.AllowUserResizing = true;
	}

	protected override void Initialize() {
		// TODO: Add your initialization logic here

		Logger.AddLogger(new ConsoleLogger());
		Logger.StartLogging();

		this.VideoDecoder = new VideoDecoder(4);

		base.Initialize();
	}

	protected override void LoadContent() {
		this._spriteBatch = new SpriteBatch(this.GraphicsDevice);

		this.VideoDecoder.Load("video.mp4", HardwareDecoderType.Any);
		Console.WriteLine($"Using hardware codec type of {this.VideoDecoder.HwCodecType.ToHardwareDecoderType()}!");

		this._graphics.PreferredBackBufferWidth  = this.VideoDecoder.Width;
		this._graphics.PreferredBackBufferHeight = this.VideoDecoder.Height;
		this._graphics.ApplyChanges();

		this.VideoTexture = new Texture2D(this.GraphicsDevice, this.VideoDecoder.Width, this.VideoDecoder.Height, false, SurfaceFormat.Color);
	}

	protected override void Update(GameTime gameTime) {
		if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
			Exit();

		base.Update(gameTime);
	}

	protected override void OnExiting(object sender, EventArgs args) {
		base.OnExiting(sender, args);

		this.VideoDecoder.Dispose();

		Logger.StopLogging();
	}

	protected override void Draw(GameTime gameTime) {
		GraphicsDevice.Clear(Color.CornflowerBlue);

		byte[] frame = this.VideoDecoder.GetFrame((int)gameTime.TotalGameTime.TotalMilliseconds);

		if (frame != null) {
			this.VideoTexture.SetData(0, new Rectangle(0, 0, this.VideoTexture.Width, this.VideoTexture.Height), frame, 0, frame.Length);
		}

		this._spriteBatch.Begin();
		this._spriteBatch.Draw(this.VideoTexture, Vector2.Zero, null, Color.White, 0, Vector2.Zero, new Vector2((float)this.GraphicsDevice.Viewport.Height / this.VideoTexture.Height), SpriteEffects.None, 0);
		this._spriteBatch.End();

		base.Draw(gameTime);
	}
}
