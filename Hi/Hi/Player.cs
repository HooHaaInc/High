﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using TileEngine;

namespace Hi
{
    public class Player : GameObject
    {
        private Vector2 fallSpeed = new Vector2(0, 15);
        private float moveScale = 180.0f;
        private Vector2 lastMove;
        private bool dead = false;
        public int drugCount = 0;
        public bool drugged = false;
        private int score = 0;
        private int livesRemaining = 3;
        public int inyecciones = 5;
        public int drugStatus = 0;
        KeyboardState lastState;
        public bool paused = false;

        public bool Dead
        {
            get { return dead; }
        }

        public int Score
        {
            get { return score; }
            set { score = value; }
        }

        public int LivesRemaining
        {
            get { return livesRemaining; }
            set { livesRemaining = value; }
        }


        #region Constructor
        public Player(ContentManager content)
        {
            animations.Add("idle", new AnimationStrip(content.Load<Texture2D>(@"Textures\Sprites\Player\Idle"), 48, "idle"));

            animations["idle"].LoopAnimation = true;
            animations["idle"].setSignal(2);

            animations.Add("run", new AnimationStrip(content.Load<Texture2D>(@"Textures\Sprites\Player\Run"), 48, "run"));
            animations["run"].LoopAnimation = true;
            animations["run"].FrameLength = 0.1f;
            animations["run"].setSignal(11);

            animations.Add("jump",
                new AnimationStrip(
                    content.Load<Texture2D>(@"Textures\Sprites\Player\Jump"),
                    48,
                    "jump"));
            animations["jump"].LoopAnimation = false;
            animations["jump"].FrameLength = 0.08f;
            animations["jump"].NextAnimation = "idle";

            animations["jump"].setSignal(4);
            animations["jump"].setSignal(7);
            //  animations["jump"].setSignal(8);

            animations.Add("die",
                new AnimationStrip(
                    content.Load<Texture2D>(@"Textures\Sprites\Player\Die"),
                    48,
                    "die"));
            animations["die"].LoopAnimation = false;
            animations["die"].setSignal(15);
            animations["die"].FrameLength = .1f;
            frameWidth = 48;
            frameHeight = 48;
            CollisionRectangle = new Rectangle(9, 1, 23, 46);

            drawDepth = 0.825f;

            enabled = true;
            codeBasedBlocks = false;
            PlayAnimation("idle");
        }
        #endregion

        #region Public Methods
        public override void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();
            //if (keyState.IsKeyDown(Keys.Escape) && lastState.IsKeyUp(Keys.Escape) ) paused = !paused;
          //  if (paused)
            {
            //    lastState = keyState;
           //     return;
            }
            if (!Dead)
            {
                string newAnimation = "idle";

                velocity = new Vector2(0, velocity.Y);
                GamePadState gamePad = GamePad.GetState(PlayerIndex.One);
                if (keyState.IsKeyDown(Keys.Q) && lastState.IsKeyUp(Keys.Q)) Clean();
                if (keyState.IsKeyDown(Keys.Left) ||
                    keyState.IsKeyDown(Keys.A))
                {
                    flipped = false;
                    newAnimation = "run";
                    velocity = new Vector2(-moveScale, velocity.Y);
                }

                if (keyState.IsKeyDown(Keys.Right) ||
                    keyState.IsKeyDown(Keys.D))
                {
                    flipped = true;
                    newAnimation = "run";
                    velocity = new Vector2(moveScale, velocity.Y);
                }

                if (keyState.IsKeyDown(Keys.Space) ||
                    (gamePad.Buttons.A == ButtonState.Pressed))
                {
                    if (onGround)
                    {
                        Jump();
                        newAnimation = "jump";
                    }
                }

                if (keyState.IsKeyDown(Keys.Up) ||
                    keyState.IsKeyDown(Keys.W))
                {
                    checkLevelTransition();
                }


                if (currentAnimation == "jump")
                {
                    newAnimation = "jump";
                }

                if (currentAnimation == "die")
                {
                    newAnimation = "die";
                }

                if (keyState.IsKeyDown(Keys.E) && lastState.IsKeyUp(Keys.E))
                {
                    Drug();
                    if (dead)
                    {
                        newAnimation = "die";
                    }
                }

                if (newAnimation != currentAnimation)
                {
                    PlayAnimation(newAnimation);
                }
                lastState = keyState;
            }
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            velocity += fallSpeed * 60 * elapsed;

            repositionCamera();
            //base.Update(gameTime, true);

            if (!enabled) return;

            updateAnimation(gameTime);
            if (velocity.Y != 0) onGround = false;
            Vector2 moveAmount = velocity * elapsed + AutoMove;
            moveAmount = horizontalCollisionTest(moveAmount);
            moveAmount = verticalCollisionTest(moveAmount);
            AutoMove = Vector2.Zero;
            if (!dead)
            {
                if (!onGround)
                {
                    if (currentAnimation == "jump")
                    {
                        if (moveAmount.Y > 0 && animations["jump"].signalIndex == 0)
                        {
                            //animations["jump"].nextFrame();
                            animations["jump"].currentFrame = 7;
                            animations["jump"].signalIndex = 1;
                        }
                    }
                    else
                    {
                        if (moveAmount.Y > 0)
                        {
                            currentAnimation = "jump";
                            PlayAnimation("jump");
                            animations["jump"].currentFrame = 7;
                            animations["jump"].signalIndex = 1;
                        }
                    }
                }
                else
                {
                    if (currentAnimation == "jump") if (animations["jump"].signalIndex == 1) animations["jump"].nextFrame();
                }
            }
            Vector2 newPosition = worldLocation + moveAmount;
            newPosition = new Vector2(MathHelper.Clamp(newPosition.X, 0, Camera.WorldRectangle.Width - frameWidth),
            MathHelper.Clamp(newPosition.Y, 2 * (-TileMap.TileHeight), Camera.WorldRectangle.Height - frameHeight));
            worldLocation = newPosition;
            lastMove = moveAmount;
        }



        public void Clean()
        {
            if (!drugged) return;
            if (inyecciones > 0)
            {
                inyecciones--;
                LevelManager.toggleDrugged();
                drugged = false;
                drugStatus = 0;
            }
        }
        public void Jump()
        {
            velocity.Y = -450;
            animations["jump"].signalIndex = 0;
            PlayAnimation("jump");
        }

        public void Kill()
        {
            LivesRemaining--;
            velocity.X = 0;
            dead = true;
            PlayAnimation("die");
        }

        public void Drug()
        {
            if (!drugged && drugCount > 0)
            {
                LevelManager.toggleDrugged();
                drugged = true;
                drugCount--;
            }
        }
        public void Revive()
        {
            drugCount = 0;
            inyecciones = 5;
            PlayAnimation("idle");
            dead = false;
			drugged = false;
			drugCount = 0;
        }
        #endregion

        #region Helper Methods
        private void repositionCamera()
        {
            int screenLocX = (int)Camera.WorldToScreen(worldLocation).X;

            if (screenLocX > 500)
            {
                Camera.Move(new Vector2(screenLocX - 500, 0));
            }

            if (screenLocX < 200)
            {
                Camera.Move(new Vector2(screenLocX - 200, 0));
            }

            int screenLocY = (int)Camera.WorldToScreen(worldLocation).Y;

            if (screenLocY > 300)
            {
                Camera.Move(new Vector2(0, screenLocY - 300));
            }

            if (screenLocY < 200/*< 200*/)
            {
                Camera.Move(new Vector2(0, screenLocY - 200));
            }
        }

        private void checkLevelTransition()
        {
            Vector2 centerCell = TileMap.GetCellByPixel(WorldCenter);
            if (TileMap.CellCodeValue(centerCell) == ("EXIT"))
            {
                livesRemaining = 1;
                Kill();
            }
                /*string[] code = TileMap.CellCodeValue(centerCell).Split('_');

                if (code.Length != 4)
                    return;

                LevelManager.LoadLevel(int.Parse(code[1]));

                WorldLocation = new Vector2(
                    int.Parse(code[2]) * TileMap.TileWidth,
                    int.Parse(code[3]) * TileMap.TileHeight);

                LevelManager.RespawnLocation = WorldLocation;

                velocity = Vector2.Zero;*/
            //}
        }

        public void setOnGround(bool onGround)
        {
            this.onGround = onGround;
        }
        #endregion


    }
}
