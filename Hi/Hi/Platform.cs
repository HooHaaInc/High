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
		private static int UP_DANGER = 1;
		private static int DOWN_DANGER = 2;
		private static int LEFT_DANGER = 4;
		private static int RIGHT_DANGER = 8;

		#region Declarations
		List<GameObject> aboveThings = new List<GameObject>();
		int danger;
		Vector2 limit;
		#endregion

		#region Constructor
		public Platform (Texture2D texture, Rectangle rect, Vector2 movement, Vector2 limits, int danger = 0)
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
		}

		public Platform(ContentManager content, int type){
            danger = 1;
			//switch(type){
				///cases
			//}
		}
		#endregion

		#region Public Methods
		public override void Update (GameTime gameTime)
		{
			if (!enabled)
				return;
			float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            updateAnimation(gameTime);
			if (velocity.Y == 0 && velocity.X == 0)
				return;
			Vector2 moveAmount = (limit.Y - limit.X)/2*(new Vector2((float)Math.Sin (velocity.X*elapsed), (float)Math.Sin (velocity.Y*elapsed)));
			Vector2 newPosition = worldLocation + moveAmount;
			for (int i=0; i<aboveThings.Count; ++i)
				aboveThings [i].WorldLocation += moveAmount;
			newPosition = new Vector2 (
				MathHelper.Clamp (newPosition.X, 0, Camera.WorldRectangle.Width - frameWidth),
				MathHelper.Clamp (newPosition.Y, 2 * (-TileMap.TileHeight), Camera.WorldRectangle.Height - frameHeight));
			worldLocation = newPosition;
            base.Update(gameTime);

		}

		public void CollisionTest(Player player, GameTime gameTime){
			if (aboveThings.Contains (player)) return;
			aboveThings.Add (player);
			float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
			Vector2 before = player.WorldLocation - player.velocity * elapsed;
			if (before.Y + player.CollisionRectangle.Height < WorldLocation.Y)
				player.WorldLocation = new Vector2 (
					player.WorldLocation.X,
					WorldLocation.Y - player.CollisionRectangle.Height);
			else if (before.X < WorldLocation.X)
				player.WorldLocation = new Vector2 (
					WorldLocation.X - player.CollisionRectangle.Width,
					player.WorldLocation.Y);
			else if (before.X > worldLocation.X)
				player.WorldLocation = new Vector2 (
					WorldLocation.X + CollisionRectangle.Width,
					player.WorldLocation.Y);
			else
				player.WorldLocation = new Vector2 (
					player.WorldLocation.X,
					WorldLocation.Y + CollisionRectangle.Height);

            player.setOnGround(true);
			
		}
		#endregion
	}
}

