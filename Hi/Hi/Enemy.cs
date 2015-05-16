using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TileEngine;

namespace Hi {
    public class Enemy : GameObject {
        private Vector2 fallSpeed = new Vector2(0, 20);
        private float walkSpeed = 60.0f;
        private bool facingLeft = true;
        public bool Dead = false;
		private bool killable;

		public bool Killable {
			get { return killable; }
		}

        #region Constructor
        public Enemy(ContentManager content, int cellX, int cellY, int type) {
			defaultLocation = new Vector2 (TileMap.TileWidth * cellX, TileMap.TileHeight * cellY);
			switch(type){
			case 1:
				animations.Add ("idle",
				                new AnimationStrip (
					content.Load<Texture2D> (
					@"Textures\Sprites\MonsterA\silla"),
					48,
					"idle"));
				animations ["idle"].LoopAnimation = true;
				animations ["idle"].setSignal (12);

				animations.Add ("run",
				                new AnimationStrip (
					content.Load<Texture2D> (
					@"Textures\Sprites\MonsterA\Run"),
					48,
					"run"));
				animations ["run"].FrameLength = 0.1f;
				animations ["run"].LoopAnimation = true;

				animations ["run"].setSignal (11);

				animations.Add ("die",
				                new AnimationStrip (
					content.Load<Texture2D> (
					@"Textures\Sprites\MonsterA\Die"),
					48,
					"die"));
				animations ["die"].LoopAnimation = false;

				animations ["die"].setSignal (14);

				frameWidth = 48;
				frameHeight = 48;
				CollisionRectangle = new Rectangle (9, 1, 25, 47);

				worldLocation = new Vector2 (
					cellX * TileMap.TileWidth,
					cellY * TileMap.TileHeight);

				enabled = true;
				killable = false;

				codeBasedBlocks = true;
				PlayAnimation ("run");
				break;
			case 3:
				animations.Add ("idle",
				                new AnimationStrip (
					content.Load<Texture2D> (
					@"Textures\Sprites\MonsterC\Idle"),
					48,
					"idle"));
				animations ["idle"].LoopAnimation = true;
				animations ["idle"].setSignal (12);

				animations.Add ("run",
				                new AnimationStrip (
					content.Load<Texture2D> (
					@"Textures\Sprites\MonsterC\Run"),
					48,
					"run"));
				animations ["run"].FrameLength = 0.1f;
				animations ["run"].LoopAnimation = true;

				animations ["run"].setSignal (11);

				animations.Add ("die",
				                new AnimationStrip (
					content.Load<Texture2D> (
					@"Textures\Sprites\MonsterC\Die"),
					48,
					"die"));
				animations ["die"].LoopAnimation = false;

				animations ["die"].setSignal (14);

				frameWidth = 48;
				frameHeight = 48;
				CollisionRectangle = new Rectangle (9, 1, 25, 47);

				worldLocation = new Vector2 (
					cellX * TileMap.TileWidth,
					cellY * TileMap.TileHeight);

				enabled = true;
				killable = true;

				codeBasedBlocks = true;
				PlayAnimation ("run");
				break;
			}
        }
        #endregion

        #region Public Methods
        public override void Update(GameTime gameTime, bool drugged) {
            Vector2 oldLocation = worldLocation;

            if (!Dead && drugged) {
                velocity = new Vector2(0, velocity.Y);

                Vector2 direction = new Vector2(1, 0);
                flipped = true;

                if (facingLeft) {
                    direction = new Vector2(-1, 0);
                    flipped = false;
                }

                direction *= walkSpeed;
                velocity += direction;
                velocity += fallSpeed;
            }

            base.Update(gameTime, drugged);
			if (!drugged) {
				currentAnimation = "idle";
				return;
			} else
				currentAnimation = Dead ? "die" : "run";
            if (!Dead) {
                if (oldLocation == worldLocation) {
                    facingLeft = !facingLeft;
                }
            }
            else {
                if (animations[currentAnimation].FinishedPlaying) {
                    enabled = false;
                }
            }

        }
        #endregion

    }
}
