#region Using Statements
using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using TileEngine;
using BMFont;
using System.IO;

#endregion

namespace LevelEditor
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        BitFont font;
        KeyboardState lastState;
        Rectangle cursor = new Rectangle(0, 0, TileMap.TileWidth, TileMap.TileHeight);
        Texture2D rect;
        string currentString;
        enum EditorState { Map, CellCode, Options }
        EditorState editorState = EditorState.Map;
        string[] layer = new string[] { "BACKGROUND", "INTERACTIVE", "FOREGROUND", "CELLCODE" };
        string[] options = new string[] { "Load Map", "Save Map", "Exit" };
        int currentLayer = 1;
        int currentOption = 0;
        Vector2 optionsPosition = new Vector2(500, 200);
        int currentMap = 0;

        private Vector2 CursorPosition
        {
            get { return new Vector2(cursor.X, cursor.Y); }
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            //Content.RootDirectory = "LevelEditorContent";
            Content.RootDirectory = "Content";
            //graphics.IsFullScreen = true;
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
            base.Initialize();

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            TileMap.Initialize(Content.Load<Texture2D>("Textures/PlatformTiles"));
            font = new BitFont(Content);
            TileMap.spriteFont = font;
            TileMap.EditorMode = true;
            rect = Content.Load<Texture2D>("rect");
            Camera.WorldRectangle = new Rectangle(0, 0, TileMap.MapWidth * TileMap.TileHeight, TileMap.MapHeight *
                                                  TileMap.TileWidth);
            Camera.Position = Vector2.Zero;
            Camera.ViewPortWidth = 800;
            Camera.ViewPortHeight = 600;
            //TODO: use this.Content to load your game content here 
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();
            MapSquare prevtile;
            // For Mobile devices, this logic will close the Game when the Back button is pressed
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                Exit();
            }
            switch (editorState)
            {
                case EditorState.Map:
                    if (keyState.IsKeyDown(Keys.Up) && lastState.IsKeyUp(Keys.Up))
                    {
                        prevtile = TileMap.GetMapSquareAtPixel(new Vector2(cursor.X, cursor.Y));
                        if (cursor.Y > 0)
                            cursor.Y -= TileMap.TileHeight;
                        if (keyState.IsKeyDown(Keys.LeftControl) || keyState.IsKeyDown(Keys.RightControl))
                            TileMap.SetMapSquareAtCell(cursor.X / TileMap.TileWidth, cursor.Y / TileMap.TileHeight, prevtile);
                    }
                    else if (keyState.IsKeyDown(Keys.Down) && lastState.IsKeyUp(Keys.Down))
                    {
                        prevtile = TileMap.GetMapSquareAtPixel(new Vector2(cursor.X, cursor.Y));
                        if (cursor.Y < Camera.WorldRectangle.Height)
                            cursor.Y += TileMap.TileHeight;
                        if (keyState.IsKeyDown(Keys.LeftControl) || keyState.IsKeyDown(Keys.RightControl))
                            TileMap.SetMapSquareAtCell(cursor.X / TileMap.TileWidth, cursor.Y / TileMap.TileHeight, prevtile);
                    }
                    else if (keyState.IsKeyDown(Keys.Left) && lastState.IsKeyUp(Keys.Left))
                    {
                        prevtile = TileMap.GetMapSquareAtPixel(new Vector2(cursor.X, cursor.Y));
                        if (cursor.X > 0)
                            cursor.X -= TileMap.TileWidth;
                        if (keyState.IsKeyDown(Keys.LeftControl) || keyState.IsKeyDown(Keys.RightControl))
                            TileMap.SetMapSquareAtCell(cursor.X / TileMap.TileWidth, cursor.Y / TileMap.TileHeight, prevtile);
                    }
                    else if (keyState.IsKeyDown(Keys.Right) && lastState.IsKeyUp(Keys.Right))
                    {
                        prevtile = TileMap.GetMapSquareAtPixel(new Vector2(cursor.X, cursor.Y));
                        if (cursor.X < Camera.WorldRectangle.Width)
                            cursor.X += TileMap.TileWidth;
                        if (keyState.IsKeyDown(Keys.LeftControl) || keyState.IsKeyDown(Keys.RightControl))
                            TileMap.SetMapSquareAtCell(cursor.X / TileMap.TileWidth, cursor.Y / TileMap.TileHeight, prevtile);
                    }
                    else if (keyState.IsKeyDown(Keys.PageUp) && lastState.IsKeyUp(Keys.PageUp))
                    {
                        ++TileMap.GetMapSquareAtPixel(new Vector2(cursor.X, cursor.Y)).LayerTiles[currentLayer];
                    }
                    else if (keyState.IsKeyDown(Keys.PageDown) && lastState.IsKeyUp(Keys.PageDown))
                    {
                        --TileMap.GetMapSquareAtPixel(new Vector2(cursor.X, cursor.Y)).LayerTiles[currentLayer];
                    }
                    else if (keyState.IsKeyDown(Keys.Enter) && lastState.IsKeyUp(Keys.Enter))
                    {
                        editorState = EditorState.CellCode;
                        currentString = TileMap.GetMapSquareAtPixel(new Vector2(cursor.X, cursor.Y)).CodeValue;
                        TileMap.GetMapSquareAtPixel(new Vector2(cursor.X, cursor.Y)).CodeValue = "";
                        currentLayer = 3;
                    }
                    else if (keyState.IsKeyDown(Keys.Space) && lastState.IsKeyUp(Keys.Space))
                    {
                        TileMap.GetMapSquareAtPixel(new Vector2(cursor.X, cursor.Y)).TogglePassable();
                    }
                    else if (keyState.IsKeyDown(Keys.Delete) && lastState.IsKeyUp(Keys.Delete))
                    {
                        TileMap.SetMapSquareAtCell(cursor.X / TileMap.TileWidth, cursor.Y / TileMap.TileHeight, MapSquare.Neutral);
                    }
                    else if (keyState.IsKeyDown(Keys.Escape) && lastState.IsKeyUp(Keys.Escape))
                    {
                        editorState = EditorState.Options;
                    }
                    else if (keyState.IsKeyDown(Keys.Z) && lastState.IsKeyUp(Keys.Z))
                    {
                        currentLayer = 0;
                    }
                    else if (keyState.IsKeyDown(Keys.X) && lastState.IsKeyUp(Keys.X))
                    {
                        currentLayer = 1;
                    }
                    else if (keyState.IsKeyDown(Keys.C) && lastState.IsKeyUp(Keys.C))
                    {
                        currentLayer = 2;
                    }
                    repositionCamera();
                    break;
                case EditorState.CellCode:
                    if (keyState.IsKeyDown(Keys.Enter) && lastState.IsKeyUp(Keys.Enter))
                    {
                        editorState = EditorState.Map;
                        TileMap.GetMapSquareAtPixel(new Vector2(cursor.X, cursor.Y)).CodeValue = currentString;
                        currentLayer = 1;
                    } if (keyState.IsKeyDown(Keys.Back) && lastState.IsKeyUp(Keys.Back))
                    {
                        if (currentString.Length > 0) currentString = currentString.Substring(0, currentString.Length - 1);
                        break;
                    }
                    foreach (Keys key in keyState.GetPressedKeys())
                    {
                        bool repeated = false;
                        foreach (Keys kay in lastState.GetPressedKeys())
                        {
                            if (key == kay)
                            {
                                repeated = true;
                                break;
                            }
                        }
                        if (!repeated && key.ToString().Length == 1)
                            currentString += key.ToString();
                    }
                    break;
                case EditorState.Options:
                    if (keyState.IsKeyDown(Keys.Escape) && lastState.IsKeyUp(Keys.Escape))
                    {
                        editorState = EditorState.Map;
                    }
                    else if (keyState.IsKeyDown(Keys.Enter) && lastState.IsKeyUp(Keys.Enter))
                    {
                        switch (currentOption)
                        {
                            case 0:
                                LoadMap(currentMap);
                                editorState = EditorState.Map;
                                break;
                            case 1:
                                SaveMap(currentMap);
                                editorState = EditorState.Map;
                                break;
                            case 2:
                                Exit();
                                break;
                        }
                    }
                    else if (keyState.IsKeyDown(Keys.Down) && lastState.IsKeyUp(Keys.Down))
                    {
                        if (currentOption < 2)
                            ++currentOption;
                    }
                    else if (keyState.IsKeyDown(Keys.Up) && lastState.IsKeyUp(Keys.Up))
                    {
                        if (currentOption > 0)
                            --currentOption;
                    }
                    else if (keyState.IsKeyDown(Keys.PageUp) && lastState.IsKeyUp(Keys.PageUp))
                    {
                        if (currentMap < 999)
                            ++currentMap;
                        else
                            currentMap = 0;
                    }
                    else if (keyState.IsKeyDown(Keys.PageDown) && lastState.IsKeyUp(Keys.PageDown))
                    {
                        if (currentMap > 0)
                            --currentMap;
                        else
                            currentMap = 999;
                    }
                    break;
            }
            // TODO: Add your update logic here
            lastState = keyState;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(
                SpriteSortMode.BackToFront,
                BlendState.AlphaBlend);
            TileMap.Draw(spriteBatch, false);

            spriteBatch.Draw(
                rect,
                Camera.WorldToScreen(cursor),//cursor,
                Color.White * 0.4f);

            if (editorState == EditorState.CellCode)
            {
                spriteBatch.Draw(
                    rect,
                    new Rectangle(0, 0, 1024, 600),
                    Color.Black * 0.5f);

                font.DrawText(spriteBatch, Camera.WorldToScreen(CursorPosition), currentString);
            }
            if (editorState == EditorState.Options)
            {
                spriteBatch.Draw(
                    rect,
                    new Rectangle(0, 0, 1024, 600),
                    Color.Black * 0.5f);

                font.DrawText(spriteBatch, optionsPosition - Vector2.UnitY * 50, "MAP" + currentMap.ToString().PadLeft(3, '0'));
                font.DrawText(spriteBatch, optionsPosition, options[0]);
                font.DrawText(spriteBatch, optionsPosition + Vector2.UnitY * 20, options[1]);
                font.DrawText(spriteBatch, optionsPosition + Vector2.UnitY * 40, options[2]);
                font.DrawText(spriteBatch, optionsPosition + Vector2.UnitY * (20 * currentOption) - Vector2.UnitX * 10, ">");
            }
            else
                font.DrawText(spriteBatch, new Vector2(800, 0), layer[currentLayer]);

            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void repositionCamera()
        {
            Rectangle screenLoc = Camera.WorldToScreen(cursor);

            if (screenLoc.X > 500)
            {
                Camera.Move(new Vector2(screenLoc.X - 500, 0));
            }

            if (screenLoc.X < 200)
            {
                Camera.Move(new Vector2(screenLoc.X - 200, 0));
            }

            if (screenLoc.Y > 300)
            {
                Camera.Move(new Vector2(0, screenLoc.Y - 300));
            }

            if (screenLoc.Y < 200/*< 200*/)
            {
                Camera.Move(new Vector2(0, screenLoc.Y - 200));
            }
        }

        public void LoadMap(int levelNumber)
        {
            try
            {
                TileMap.LoadMap((System.IO.FileStream)TitleContainer.OpenStream(
                    @"LevelEditorContent/Maps/MAP" + levelNumber.ToString().PadLeft(3, '0') + ".MAP"));
            }
            catch
            {
                TileMap.ClearMap();
            }
        }

        public void SaveMap(int levelNumber)
        {
            TileMap.SaveMap(new FileStream(@"LevelEditorContent/Maps/MAP" + levelNumber.ToString().PadLeft(3, '0') + ".MAP",
                            FileMode.Create));
        }
    }
}

