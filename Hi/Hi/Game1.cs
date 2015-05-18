using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TileEngine;
//using BmFont;

namespace Hi
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
		#region Declarations
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Player player;
        SpriteFont pericles8;
        Vector2 scorePosition = new Vector2(20, 10);
        enum GameState { TitleScreen, Playing, PlayerDead, GameOver, Drugged, Help, Paused };
		enum MenuOptions { Play = 0, Instructions, Exit };
        GameState gameState = GameState.TitleScreen;
		MenuOptions menuOptions = MenuOptions.Play;
        Vector2 gameOverPosition = new Vector2(350, 300);
        Vector2 livesPosition = new Vector2(20, 30);
        Vector2 inyeccionPosition = new Vector2(20, 20);
		Vector2[] menuPositions = new Vector2[] {
			new Vector2(350, 300),
			new Vector2(350, 320),
			new Vector2(350, 340)
		};
        int helpIndex = 0;
        Texture2D titleScreen;
        float deathTimer = 0.0f;
        float deathDelay = 5.0f;
		//BitFont myFont;
		KeyboardState lastState;
        GameState lastGameState;
		#endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            this.graphics.PreferredBackBufferWidth = 800;
            this.graphics.PreferredBackBufferHeight = 500;
            this.graphics.ApplyChanges();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {

            spriteBatch = new SpriteBatch(GraphicsDevice);
            TileMap.Initialize(Content.Load<Texture2D>(@"Textures\PlatformTiles"));
			try{
	            TileMap.spriteFont = Content.Load<SpriteFont>(@"Fonts\Pericles8");
	            pericles8 = Content.Load<SpriteFont>(@"Fonts\Pericles8");
			}catch{
				//myFont = new BitFont (Content);
			}
            titleScreen = Content.Load<Texture2D>(@"Textures\TitleScreen");
            Camera.WorldRectangle = new Rectangle(0, 0, TileMap.MapWidth * TileMap.TileHeight, TileMap.MapHeight *
            TileMap.TileWidth);
            Camera.Position = Vector2.Zero;
            Camera.ViewPortWidth = 800;
            Camera.ViewPortHeight = 600;
            player = new Player(Content);
            LevelManager.Initialize(Content, player);
            /*
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            TileMap.Initialize(Content.Load<Texture2D>(@"Textures\PlatformTiles"));
            pericles8 = Content.Load<SpriteFont>(@"Fonts\Pericles8");
            player = new Player(Content);
           // player.WorldLocation = new Vector2(350, 300);
            Camera.WorldRectangle = new Rectangle(0, 0, TileMap.MapWidth * TileMap.TileWidth, TileMap.MapHeight * TileMap.TileHeight);
            Camera.Position = Vector2.Zero;
            Camera.ViewPortWidth = 800;
            Camera.ViewPortHeight = 600;
            LevelManager.Initialize(Content, player);
            LevelManager.LoadLevel(0);
            // TODO: use this.Content to load your game content here*/
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            // Allows the game to exit
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back ==ButtonState.Pressed)
            //this.Exit();
            KeyboardState keyState = Keyboard.GetState();
            GamePadState gamepadState = GamePad.GetState(PlayerIndex.One);

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (gameState == GameState.Paused) {
                if (keyState.IsKeyDown(Keys.Escape) && lastState.IsKeyUp(Keys.Escape))
                {
                    gameState = lastGameState;
                    lastState = keyState;
                    base.Update(gameTime);
                    return;
                }
            }
            if (gameState == GameState.TitleScreen){
				if (keyState.IsKeyDown (Keys.Enter) || gamepadState.Buttons.A == ButtonState.Pressed) {
					switch (menuOptions) {
					case MenuOptions.Play:
						StartNewGame ();
						gameState = GameState.Playing;
						break;
					case MenuOptions.Instructions:
						 gameState = GameState.Help;
                         helpIndex = 0;
						break;
					case MenuOptions.Exit:
						Exit ();
						break;
					}
				} else if (keyState.IsKeyDown (Keys.Down) && lastState.IsKeyUp(Keys.Down) || 
				           keyState.IsKeyDown (Keys.S) && lastState.IsKeyUp(Keys.S)) {
					if (menuOptions != MenuOptions.Exit)
						++menuOptions;
				}else if(keyState.IsKeyDown (Keys.Up) && lastState.IsKeyUp(Keys.Up)
				         || keyState.IsKeyDown (Keys.W) && lastState.IsKeyUp(Keys.W))
					if(menuOptions != MenuOptions.Play) --menuOptions;
            }
            if (gameState == GameState.Help) {
                if (keyState.IsKeyDown(Keys.D) && lastState.IsKeyUp(Keys.D) ||
                    keyState.IsKeyDown(Keys.Right) && lastState.IsKeyUp(Keys.Right) )
                {
                    helpIndex++;
                    if (helpIndex == 3) gameState = GameState.TitleScreen;
                   
                }
                if (keyState.IsKeyDown(Keys.A) && lastState.IsKeyUp(Keys.A) ||
                    keyState.IsKeyDown(Keys.Left) && lastState.IsKeyUp(Keys.Left) )
                {
                    helpIndex--;
                    if (helpIndex == -1) gameState = GameState.TitleScreen;

                }
                 if (keyState.IsKeyDown(Keys.Escape) && lastState.IsKeyUp(Keys.Escape)){
                    gameState = GameState.TitleScreen;
                 }
            
            }
            if (gameState == GameState.Playing || gameState == GameState.Drugged){
				player.Update(gameTime);
				LevelManager.Update(gameTime, gameState == GameState.Drugged);
                if (player.Dead){
                    if (player.LivesRemaining > 0){
                        gameState = GameState.PlayerDead;
                        deathTimer = 0.0f;
                    }else{
                        gameState = GameState.GameOver;
                        deathTimer = 0.0f;
                    }
                }
				if (gameState == GameState.Playing && player.drugged)
					gameState = GameState.Drugged;
				else if (gameState == GameState.Drugged && !player.drugged)
					gameState = GameState.Playing;
                if (keyState.IsKeyDown(Keys.Escape) && lastState.IsKeyUp(Keys.Escape))
                {
                    lastGameState = gameState;
                    gameState = GameState.Paused;
                }
            }
            if (gameState == GameState.PlayerDead){
				player.Update(gameTime);
				LevelManager.Update(gameTime, gameState == GameState.Drugged);
                deathTimer += elapsed;
                if (deathTimer > deathDelay){
                    player.WorldLocation = Vector2.Zero;
                    LevelManager.ReloadLevel();
                    player.Revive();
                    gameState = GameState.Playing;
                }
            }
            if (gameState == GameState.GameOver){
                deathTimer += elapsed;
                if (deathTimer > deathDelay){
                    gameState = GameState.TitleScreen;
                }
            }
			lastState = keyState;

            base.Update(gameTime);
            /*
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            player.Update(gameTime);
            player.Update(gameTime);
            LevelManager.Update(gameTime);
            base.Update(gameTime);*/
            //lastState = keyState;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(
            SpriteSortMode.BackToFront,
            BlendState.AlphaBlend);
            if (gameState == GameState.TitleScreen)
            {
                //spriteBatch.Draw(titleScreen, Vector2.Zero, Color.White);
				if (pericles8 != null) {
					spriteBatch.DrawString (
						pericles8,
						"JUGAR         ",
						menuPositions[0],
						Color.Red);

					//spriteBatch.DrawString(pericles8, "Current green" + TileMap.currentGreen.ToString(), new Vector2(20, 70), Color.White);
					//spriteBatch.DrawString(pericles8, "Color: " + TileMap.currentColor.ToString(), new Vector2(20, 50), Color.White);
					spriteBatch.DrawString (
						pericles8, 
						"INSTRUCCIONES           ", 
						menuPositions[1], 
						Color.Red);
					spriteBatch.DrawString (
						pericles8,
						"SALIR                ",
						menuPositions[2],
						Color.Red);
					spriteBatch.DrawString (
						pericles8,
						">",
						menuPositions[(int)menuOptions] - new Vector2 (10, 0),
						Color.Red);
                    spriteBatch.Draw(titleScreen, Vector2.Zero, Color.White);
				} else {
                    /*
					myFont.DrawText (spriteBatch, menuPositions [0], "Jugar");
					myFont.DrawText (spriteBatch, menuPositions [1], "Instrucciones");
					myFont.DrawText (spriteBatch, menuPositions [2], "Salir");
					myFont.DrawText (spriteBatch, menuPositions [(int)menuOptions] - new Vector2 (10, 0), ">");
                     */
				}
            }
            if (gameState == GameState.Help) { 
                
                switch(helpIndex){

                    case (0): {
                        spriteBatch.Draw(Content.Load<Texture2D>(@"Textures\HelpMenus\HelpMenu2"), Vector2.Zero, Color.White);
                        break;
                    }
                    case (1):
                        {
                            spriteBatch.Draw(Content.Load<Texture2D>(@"Textures\HelpMenus\HelpMenu1"), Vector2.Zero, Color.White);
                            break;
                    }
                    case (2):
                        {
                            spriteBatch.Draw(Content.Load<Texture2D>(@"Textures\HelpMenus\HelpMenu1"), Vector2.Zero, Color.White);
                            break;
                    }

                }
            }
            if ((gameState == GameState.Playing) ||
                (gameState == GameState.Drugged) ||
            (gameState == GameState.PlayerDead) ||
            (gameState == GameState.GameOver) ||
                gameState == GameState.Paused)
            {
                TileMap.Draw(spriteBatch,player.drugged);
				player.Draw(spriteBatch);
				LevelManager.Draw(spriteBatch);
				if(pericles8 != null){
	                spriteBatch.DrawString(
	                pericles8,
	                "Drogas: " + player.drugCount.ToString(),
	                scorePosition,
	                Color.White);

	                //spriteBatch.DrawString(pericles8, "Current green" + TileMap.currentGreen.ToString(), new Vector2(20, 70), Color.White);
	                //spriteBatch.DrawString(pericles8, "Color: " + TileMap.currentColor.ToString(), new Vector2(20, 50), Color.White);
	                spriteBatch.DrawString(pericles8, "Inyecciones: " + player.inyecciones.ToString(),inyeccionPosition, Color.White);
	                spriteBatch.DrawString(
	                pericles8,
	                "Vidas:  " + player.LivesRemaining.ToString(),
	                livesPosition,
	                Color.White);
				}else{
					//myFont.DrawText (spriteBatch, scorePosition, "Drogas: " + player.drogas.ToString ());
					//myFont.DrawText (spriteBatch, inyeccionPosition, "Inyecciones: " + player.inyecciones.ToString ());
					//myFont.DrawText (spriteBatch, livesPosition, "Vidas: " + player.LivesRemaining.ToString ());
				}
                
                spriteBatch.Draw(Content.Load<Texture2D>(@"Textures\greenTexture"),new Rectangle(600,10,180,20),Color.White);
                spriteBatch.Draw(Content.Load<Texture2D>(@"Textures\redTexture"),
                    new Rectangle(600, 10, (int)(player.drugStatus * 1.8), 20),
                    Color.White);
                //spriteBatch.Draw(Content.Load<Texture2D>(@"Textures\redTexture"), 
                 //   new Rectangle(600, 10, (int) (player.drugStatus * 1.8), 20),
                 //   Color.White);

            }

           /* if(gameState == GameState.Drugged){
                TileMap.Draw(spriteBatch,player.drugged);
                player.Draw(spriteBatch);
                LevelManager.Draw(spriteBatch);
                if(pericles8 != null){
					spriteBatch.DrawString(
	                pericles8,
	                "Drogas: " + player.drugCount.ToString(),
	                scorePosition,
	                Color.White);
	                spriteBatch.DrawString(pericles8, "Inyecciones: " + player.inyecciones.ToString(), inyeccionPosition, Color.White);
	                spriteBatch.DrawString(
	                pericles8,
	                "Vidas:  " + player.LivesRemaining.ToString(),
	                livesPosition,
	                Color.White);
				}else {
					//myFont.DrawText (spriteBatch, scorePosition, "Drogas: " + player.drogas.ToString ());
					//myFont.DrawText (spriteBatch, inyeccionPosition, "Inyecciones: " + player.inyecciones.ToString ());
					//myFont.DrawText (spriteBatch, livesPosition, "Vidas: " + player.LivesRemaining.ToString ());
				}
                spriteBatch.Draw(Content.Load<Texture2D>(@"Textures\redTexture"),
                    new Rectangle(600, 10, (int)(player.drugStatus * 1.8), 20),
                    Color.White);
                spriteBatch.Draw(Content.Load<Texture2D>(@"Textures\greenTexture"), new Rectangle(600, 10, 180, 20), Color.White);
            }
            */
            if (gameState == GameState.PlayerDead)
            {
            }
            if (gameState == GameState.GameOver)
            {
				try{
	                spriteBatch.DrawString(
	                pericles8,
	                "G A M E O V E R !",
	                gameOverPosition,
	                Color.White);
				}catch{}
            }
            if (gameState == GameState.Paused) {
                spriteBatch.Draw(Content.Load<Texture2D>(@"Textures\greenTexture"), new Rectangle(0, 0,
                    this.graphics.PreferredBackBufferWidth,
                    this.graphics.PreferredBackBufferHeight), Color.Black * 0.5f);
                spriteBatch.DrawString(pericles8, "PAUSA", new Vector2(370, 140), Color.White);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

        #region Helper Methods
        private void StartNewGame()
        {
            player.Revive();
            player.LivesRemaining = 3;
            player.WorldLocation = Vector2.Zero;
            LevelManager.LoadLevel(0);
        }
        #endregion


    }
}
