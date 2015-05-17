﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using TileEngine;

namespace Hi
{
    public class GameObject
    {
        #region Declarations

        protected Vector2 worldLocation;
        public Vector2 velocity;
        protected int frameWidth;
        protected int frameHeight;
        protected bool enabled;
        protected bool flipped = false;
        public bool onGround;
        protected Rectangle collisionRectangle;
        protected int collideWidth;
        protected int collideHeight;
        protected bool codeBasedBlocks = true;
        protected float drawDepth = 0.85f;
        protected Dictionary<string, AnimationStrip> animations = new Dictionary<string, AnimationStrip>();
        protected string currentAnimation;
		protected Vector2 autoMove;
		protected Vector2 defaultLocation;
		protected Texture2D rekt = null;
        protected enum ObjectStates { Normal, Drugged };
        protected ObjectStates objectState = ObjectStates.Normal;

        #endregion

        #region Properties
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }
        public Vector2 WorldLocation
        {
            get { return worldLocation; }
            set { worldLocation = value; }
        }
        public Vector2 WorldCenter
        {
            get { return new Vector2((int)worldLocation.X + (int)(frameWidth / 2), (int)worldLocation.Y + (int)(frameHeight / 2)); }
        }
        public Rectangle WorldRectangle
        {
            get { return new Rectangle((int)worldLocation.X, (int)worldLocation.Y, frameWidth, frameHeight); }
        }
        public Rectangle CollisionRectangle
        {
            get
            {
                return new Rectangle((int)worldLocation.X + collisionRectangle.X, (int)WorldRectangle.Y + collisionRectangle.Y,
                                     collisionRectangle.Width, collisionRectangle.Height);
            }
            set { collisionRectangle = value; }
        }

		public Vector2 AutoMove{
			get{ return autoMove; }
			set{ autoMove = value; }
		}

        #endregion

        #region Helper Methods
        protected void updateAnimation(GameTime gameTime)
        {
            if (animations.ContainsKey(currentAnimation))
            {
                if (animations[currentAnimation].FinishedPlaying) PlayAnimation(animations[currentAnimation].NextAnimation);
                else animations[currentAnimation].Update(gameTime);
            }
        }
        #endregion

        #region Public Methods
		protected GameObject(){
		}

		protected GameObject(ContentManager content){
			rekt = content.Load <Texture2D> (@"rect");
		}

        public void PlayAnimation(string name)
        {
            if (!(name == null) && animations.ContainsKey(name))
            {
                currentAnimation = name;
                animations[name].Play();
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            if (!enabled) return;
			Vector2 newPosition;
			if(objectState == ObjectStates.Normal){
				newPosition = defaultLocation;
				newPosition = new Vector2(MathHelper.Clamp(newPosition.X, 0, Camera.WorldRectangle.Width - frameWidth),
				                          MathHelper.Clamp(newPosition.Y, 2 * (-TileMap.TileHeight), Camera.WorldRectangle.Height - frameHeight));
				worldLocation = newPosition;
				return;
			}
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            updateAnimation(gameTime);
            if (velocity.Y != 0) onGround = false;

            Vector2 moveAmount = velocity * elapsed + autoMove;

            moveAmount = horizontalCollisionTest(moveAmount);
            moveAmount = verticalCollisionTest(moveAmount);
            newPosition = worldLocation + moveAmount;
            newPosition = new Vector2(MathHelper.Clamp(newPosition.X, 0, Camera.WorldRectangle.Width - frameWidth),
            MathHelper.Clamp(newPosition.Y, 2 * (-TileMap.TileHeight), Camera.WorldRectangle.Height - frameHeight));
            worldLocation = newPosition;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (!enabled) return;
            if (animations.ContainsKey(currentAnimation)){
                SpriteEffects effect = SpriteEffects.None;
                if (flipped)effect = SpriteEffects.FlipHorizontally;
                spriteBatch.Draw(animations[currentAnimation].Texture,Camera.WorldToScreen(WorldRectangle),
                animations[currentAnimation].FrameRectangle,Color.White, 0.0f, Vector2.Zero, effect, drawDepth);
            }

			//Debug c:
			if(rekt != null)
			spriteBatch.Draw (
				rekt,
				Camera.WorldToScreen (CollisionRectangle),
				Color.Green);
        }

        public void toggleDrugged() {
            if (objectState == ObjectStates.Normal) objectState = ObjectStates.Drugged;
            else objectState = ObjectStates.Normal;
        }
        #endregion
        #region Map-Based Collision Detection Methods
        protected Vector2 horizontalCollisionTest(Vector2 moveAmount)
        {
            if (moveAmount.X == 0) return moveAmount;
            Rectangle afterMoveRect = CollisionRectangle;
            afterMoveRect.Offset((int)moveAmount.X, 0);
            Vector2 corner1, corner2;
            if (moveAmount.X < 0)
            {
                corner1 = new Vector2(afterMoveRect.Left, afterMoveRect.Top + 1);
                corner2 = new Vector2(afterMoveRect.Left, afterMoveRect.Bottom - 1);
            }
            else
            {
                corner1 = new Vector2(afterMoveRect.Right, afterMoveRect.Top + 1);
                corner2 = new Vector2(afterMoveRect.Right, afterMoveRect.Bottom - 1);
            }
            Vector2 mapCell1 = TileMap.GetCellByPixel(corner1);
            Vector2 mapCell2 = TileMap.GetCellByPixel(corner2);
			//Platform platform1 = LevelManager.PlatformAt (corner1);
			//Platform platform2 = LevelManager.PlatformAt (corner2);
            if (!TileMap.CellIsPassable(mapCell1) || !TileMap.CellIsPassable(mapCell2) )//|| platform1 != null || platform2 != null)
            {
                moveAmount.X = 0;
                velocity.X = 0;
            }
            else if (codeBasedBlocks)
            {
                if (TileMap.CellCodeValue(mapCell1) == "BLOCK" || TileMap.CellCodeValue(mapCell2) == "BLOCK")
                {
                    moveAmount.X = 0;
                    velocity.X = 0;
                }
            }

			/*if (platform1 != null) {
				platform1.addAbove (this);
			} else if (platform2 != null)
				platform2.addAbove (this);
*/

            return moveAmount;
        }

        protected Vector2 verticalCollisionTest(Vector2 moveAmount)
        {
            if (moveAmount.Y == 0) return moveAmount;
            Rectangle afterMoveRect = CollisionRectangle;
            afterMoveRect.Offset((int)moveAmount.X, (int)moveAmount.Y);
            Vector2 corner1, corner2;
            if (moveAmount.Y < 0)
            {
                corner1 = new Vector2(afterMoveRect.Left + 1, afterMoveRect.Top);
                corner2 = new Vector2(afterMoveRect.Right - 1, afterMoveRect.Top);
            }
            else
            {
                corner1 = new Vector2(afterMoveRect.Left + 1, afterMoveRect.Bottom);
                corner2 = new Vector2(afterMoveRect.Right - 1, afterMoveRect.Bottom);
            }
            Vector2 mapCell1 = TileMap.GetCellByPixel(corner1);
            Vector2 mapCell2 = TileMap.GetCellByPixel(corner2);
			Platform platform1 = LevelManager.PlatformAt (corner1);
			Platform platform2 = LevelManager.PlatformAt (corner2);

			if (moveAmount.Y > 0) {
				if (platform1 != null) {
					platform1.addAbove (this);
					onGround = true;
					velocity.Y = 0;
					moveAmount.Y = 0;
				} else if (platform2 != null) {
					platform2.addAbove (this);
					onGround = true;
					velocity.Y = 0;
					moveAmount.Y = 0;
				}
				if(autoMove.Y != 0) moveAmount.Y = autoMove.Y;
			}

            if (!TileMap.CellIsPassable(mapCell1) || !TileMap.CellIsPassable(mapCell2) )//|| platform1 != null || platform2 != null)
            {
                if (moveAmount.Y > 0) onGround = true;
                moveAmount.Y = 0;
                velocity.Y = 0;
            }
            else if (codeBasedBlocks)
            {
                if (TileMap.CellCodeValue(mapCell1) == "BLOCK" || TileMap.CellCodeValue(mapCell2) == "BLOCK")
                {
                    if (moveAmount.Y > 0) onGround = true;
                    moveAmount.Y = 0;
                    velocity.Y = 0;
                }
            }
            return moveAmount;
        }


        #endregion
    }
            
}
