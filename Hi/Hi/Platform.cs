using System;

using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using TileEngine;

namespace Hi
{
	public class Platform : GameObject
	{
		public static int UP_DANGER = 1;
		public static int DOWN_DANGER = 2;
		public static int LEFT_DANGER = 4;
		public static int RIGHT_DANGER = 8;

		#region Declarations
		List<GameObject> aboveThings = new List<GameObject>();
		int danger;
		int limit;
		float elapsed = 0;
		Vector2 hk;
		#endregion

		#region Constructor
		public Platform (Texture2D texture, Rectangle rect, Vector2 movement, int limits, int danger = 0)
		{

			animations.Add ("plat", new AnimationStrip (texture, rect.Width, "plat"));
            animations["plat"].LoopAnimation = true;
            animations["plat"].FrameLength = 0.15f;
            animations["plat"].setSignal(20);
            drawDepth = 0.875f;
			collisionRectangle = new Rectangle(0, 0, rect.Width, rect.Height);
			velocity = movement;
			limit = limits;
			this.danger = danger;
			frameWidth = rect.Width;
			frameHeight = rect.Height;
			PlayAnimation ("plat");
            enabled = true;
			worldLocation = new Vector2 (TileMap.TileWidth * rect.X, TileMap.TileHeight * rect.Y);
			hk = new Vector2 (TileMap.TileWidth * rect.X, TileMap.TileHeight * rect.Y);
		}

		public Platform(ContentManager content, int x, int y, int type){
			worldLocation = new Vector2 (TileMap.TileWidth * x, TileMap.TileHeight * y);
			switch(type){
				default:
				animations.Add ("plat", new AnimationStrip(
					content.Load<Texture2D>(@"Textures\Sprites\ladrilloGris"), 64, "plat"));
				animations["plat"].LoopAnimation = true;
				animations["plat"].FrameLength = 0.15f;
				animations["plat"].setSignal(20);
				drawDepth = 0.875f;
				collisionRectangle = new Rectangle(0, 0, 64, 12);
				velocity = new Vector2(3, 0);
				limit = 64;
				this.danger = 0;
				frameWidth = 64;
				frameHeight = 12;
				PlayAnimation ("plat");
				enabled = true;
				hk = new Vector2 (TileMap.TileWidth * x, TileMap.TileHeight * y);
				break;
			}
		}
		#endregion

		#region Public Methods
		public override void Update (GameTime gameTime)
		{
			if (!enabled)
				return;
			float time = (float)gameTime.ElapsedGameTime.TotalSeconds;
			elapsed += time;
            updateAnimation(gameTime);
			if (velocity.Y == 0 && velocity.X == 0)
				return;
			Vector2 newPosition = hk + limit*(new Vector2 ((float)Math.Sin (velocity.X*elapsed), (float)Math.Sin (velocity.Y*elapsed)));
			newPosition = new Vector2 (
				MathHelper.Clamp (newPosition.X, 0, Camera.WorldRectangle.Width - frameWidth),
				MathHelper.Clamp (newPosition.Y, 2 * (-TileMap.TileHeight), Camera.WorldRectangle.Height - frameHeight));
			Vector2 moveAmount = newPosition - worldLocation;
			for (int i=aboveThings.Count-1; i>=0; --i) {
				//Console.Write (moveAmount);
				if (aboveThings [i].CollisionRectangle.Intersects (CollisionRectangle) && velocity.Y == 0)
					aboveThings [i].AutoMove = new Vector2(0, 4);
				else
					aboveThings [i].AutoMove = moveAmount; //autoCorrection(moveAmount, aboveThings[i].CollisionRectangle);
				aboveThings.RemoveAt (i);
			}
			worldLocation = newPosition;
            //base.Update(gameTime);

		}

		public bool ContainsPixel(Vector2 coord){
			return worldLocation.Y <= coord.Y && 
				worldLocation.Y + collisionRectangle.Height >= coord.Y &&
				worldLocation.X <= coord.X &&
				worldLocation.X + collisionRectangle.Width >= coord.X;
		}

		public void addAbove(GameObject go){
			for (int i=0; i<aboveThings.Count; ++i)
				if (aboveThings [i] == go)
					return;
			aboveThings.Add (go);
		}

		private Vector2 autoCorrection(Vector2 moveAmount, Rectangle rekt){
			Vector2 movement = Vector2.Zero;
			if (!rekt.Intersects (CollisionRectangle))
				return moveAmount;
			if(rekt.X + rekt.Width - worldLocation.X < collisionRectangle.Width){
				movement.X = rekt.X + rekt.Width - worldLocation.X;
			}else{
				movement.X = worldLocation.X + collisionRectangle.Width - rekt.X;
			}

			Console.Write (movement);
			return movement;


		}
		#endregion
	}
}

