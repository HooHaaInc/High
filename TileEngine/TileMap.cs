using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Storage;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TileEngine
{
	public static class TileMap
	{
		#region Declarations
		public const int TileWidth = 32;
		public const int TileHeight = 32;
		public const int MapWidth = 160;
		public static float rotation = 0.0f;
		public const int MapHeight = 12;
		public const int MapLayers = 3;
		private const int skyTile = 2;
		public static double frequency = 0.05f;
		private static bool positiveRotation = true;
		public static  int currentGreen = 0;                // Change me to private
		public static Color currentColor = Color.White;     // Change me to private
		static private MapSquare[,] mapCells = new MapSquare[MapWidth, MapHeight];
		public static bool EditorMode = false;
		public static SpriteFont spriteFont;
		static private Texture2D tileSheet;
		#endregion

		public static void Main(string[] args){

		}

		#region Initialization
		static public void Initialize(Texture2D tileTexture){
			tileSheet = tileTexture;
			for(int x=0; x<MapWidth; ++x){
				for(int y=0; y<MapHeight; ++y){
					for(int z=0; z<MapLayers; ++z){
						mapCells [x, y] = new MapSquare (skyTile, 0, 0, "", true);
					}
				}
			}
		}
		#endregion

		#region Information about Map Cells


		static public int GetCellByPixelX(int pixelX){
			return pixelX / TileWidth;
		}

		static public int GetCellByPixelY(int pixelY){
		return pixelY / TileHeight;
		}

		static public Vector2 GetCellByPixel(Vector2 pixelLocation){
			return new Vector2 (
				GetCellByPixelX ((int)pixelLocation.X),
			    GetCellByPixelY ((int)pixelLocation.Y));
		}

		static public Vector2 GetCellCenter(int squareX, int squareY){
			return new Vector2 (
				squareX * TileWidth + TileWidth / 2,
				squareY * TileHeight + TileHeight / 2);
		}

		static public Vector2 GetCellCenter(Vector2 square){
			return GetCellCenter (
				(int)square.X,
				(int)square.Y);
		}

		static public Rectangle CellWorldRectangle(int x, int y){
			return new Rectangle (
				x * TileWidth,
				y * TileHeight,
				TileWidth,
				TileHeight);
		}

		static public Rectangle CellWorldRectangle(Vector2 square){
			return CellWorldRectangle (
				(int)square.X,
				(int)square.Y);
		}

		static public Rectangle CellScreenRectangle(int x, int y){
			return Camera.Transform (CellWorldRectangle (x, y));
		}

		static public Rectangle CellScreenRectangle(Vector2 square){
			return CellScreenRectangle ((int)square.X, (int)square.Y);
		}

		static public bool CellIsPassable(int cellX, int cellY){
			MapSquare square = GetMapSquareAtCell (cellX, cellY);
			return square != null && square.Passable;
		}

		static public bool CellIsPassable(Vector2 cell){
			return CellIsPassable ((int)cell.X, (int)cell.Y);
		}

		static public bool CellIsPassableByPixel(Vector2 pixelLocation){
			return CellIsPassable (
				GetCellByPixelX ((int)pixelLocation.X),
				GetCellByPixelY ((int)pixelLocation.Y));
		}

		static public string CellCodeValue(int cellX, int cellY){
			MapSquare square = GetMapSquareAtCell (cellX, cellY);
			return square != null ? square.CodeValue : "";
		}

		static public string CellCodeValue(Vector2 cell ){
			return CellCodeValue ((int)cell.X, (int)cell.Y);
		}
		#endregion

		#region Information about MapSquare objects
		static public MapSquare GetMapSquareAtCell(int tileX, int tileY){
			if (tileX >= 0 && tileX < MapWidth && tileY >= 0 && tileY < MapHeight)
				return mapCells [tileX, tileY];
			else
				return null;
		}

		static public void SetMapSquareAtCell(int tileX, int tileY, MapSquare tile){
			if (tileX >= 0 && tileX < MapWidth && tileY >= 0 && tileY < MapHeight)
				mapCells [tileX, tileY] = tile;
		}

		static public void SetTileAtCell(
			int tileX, 
			int tileY,
			int layer, 
			int tileIndex){
			if (tileX >= 0 && tileX < MapWidth && tileY >= 0 && tileY < MapHeight)
				mapCells [tileX, tileY].LayerTiles [layer] = tileIndex;
		}

		static public MapSquare GetMapSquareAtPixel(int pixelX, int pixelY){
			return GetMapSquareAtCell (
				GetCellByPixelX (pixelX),
				GetCellByPixelY (pixelY));
		}

		static public MapSquare GetMapSquareAtPixel(Vector2 pixelLocation){
			return GetMapSquareAtPixel (
				(int)pixelLocation.X,
				(int)pixelLocation.Y);
		}
		#endregion

		#region Tile and Tile Sheet Handling
		public static int TilesPerRow{
			get{ return tileSheet.Width / TileWidth; }
		}

		public static Rectangle TileSourceRectangle(int tileIndex){
			return new Rectangle (
				(tileIndex % TilesPerRow) * TileWidth,
				(tileIndex / TilesPerRow) * TileHeight,
				TileWidth,
				TileHeight
			);
		}
		#endregion

		#region Drawing
		static public void Draw(SpriteBatch spriteBatch, bool drugged)
		{
			if (drugged)
			{
				if (rotation >= 0.5f) positiveRotation = false;
				if (rotation <= -0.5f) positiveRotation = true;
				if (positiveRotation) rotation += 0.01f;
				else rotation -= 0.01f;
				currentGreen++;
			}
			int startX = GetCellByPixelX((int)Camera.Position.X);
			int endX = GetCellByPixelX((int)Camera.Position.X + Camera.ViewPortWidth);
			int startY = GetCellByPixelY((int)Camera.Position.Y);
			int endY = GetCellByPixelY((int)Camera.Position.Y + Camera.ViewPortHeight);
			for (int x = startX; x <= endX; x++)
				for (int y = startY; y <= endY; y++){
				for (int z = 0; z < MapLayers; z++){
					if ((x >= 0) && (y >= 0) && (x < MapWidth) && (y < MapHeight)){
						if (!drugged) spriteBatch.Draw(tileSheet, CellScreenRectangle(x, y), TileSourceRectangle(mapCells[x, y].LayerTiles[z]),
						                               Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1f - ((float)z * 0.1f));
						else
						{
							if (currentGreen > 255) currentGreen = 0;

							/*currentColor = new Color(   (float)(Math.Sin(2.0f * Math.PI * ((float)currentGreen) / 255.0f - 1.0f) + 1.0f) * 127.0f,
                                                        (float)(Math.Sin(2.0f * Math.PI * currentGreen / 255.0f - 1.0f) + 1.0f) * 127.0f,
                                                        (float)(Math.Sin(2.0f * Math.PI * currentGreen / 255.0f - 1.0f) + 1.0f) * 127.0f);
                             */

							currentColor = new Color(
								//((int) ( (Math.Sin( currentGreen  * frequency + 0) * 127 )+ 128)),
								//((int) ( ( Math.Sin(currentGreen * frequency + 2) * 127 ) + 128)),
								//((int) ( ( Math.Sin(currentGreen * frequency + 4) * 127 ) + 128))
								(float)(Math.Sin(currentGreen*frequency + 0)),
								(float)(Math.Sin(currentGreen*frequency + 2)),
								(float)(Math.Sin(currentGreen*frequency + 4))
								);
							//currentColor = new Color(100, 255, currentGreen);
							spriteBatch.Draw(tileSheet, CellScreenRectangle(x, y), TileSourceRectangle(mapCells[x, y].LayerTiles[z]),
							                 currentColor, rotation, Vector2.Zero, SpriteEffects.None, 1f - ((float)z * 0.1f));
						}
					}
				}      
				if (EditorMode){
					DrawEditModeItems(spriteBatch, x, y);
				}
			}
		}

		public static void DrawEditModeItems(SpriteBatch spriteBatch, int x, int y){
			if (x < 0 || x >= MapWidth || y < 0 || y >= MapHeight)
				return;
			if(!CellIsPassable (x,y)){
				spriteBatch.Draw (
					tileSheet,
					CellScreenRectangle (x, y),
					TileSourceRectangle (1),
					new Color (255, 0, 0, 80),
					0.0f,
					Vector2.Zero,
					SpriteEffects.None,
					0.0f);
			}
			if(mapCells[x, y].CodeValue != ""){
				Rectangle screenRect = CellScreenRectangle (x, y);
				spriteBatch.DrawString (
					spriteFont,
					mapCells [x, y].CodeValue,
					new Vector2 (screenRect.X, screenRect.Y),
					Color.White,
					0.0f,
					Vector2.Zero,
					1.0f,
					SpriteEffects.None,
					0.0f);
			}
		}
		#endregion

		#region LOading and Saving Maps
		public static void LoadMap(FileStream fileStream){
			try{
				BinaryFormatter formater = new BinaryFormatter();
				mapCells = (MapSquare[,])formater.Deserialize (fileStream);
				fileStream.Close ();
			}catch{

			}
		}
		#endregion

		/*public class MapRow{
			public List<MapCell> Columns = new List<MapCell>();
		}

		public List<MapRow> Rows = new List<MapRow>();
		public int MapWidth = 50;
		public int MapHeight = 50;
		public TileMap ()
		{
			for (int i=0; i<MapHeight; ++i) {
				MapRow thisRow = new MapRow ();
				for (int j=0; j<MapWidth; ++j)
					thisRow.Columns.Add (new MapCell (0));
				Rows.Add (thisRow);
			}

			Rows [0].Columns [3].TileID = 3;
			Rows [0].Columns [4].TileID = 3;
			Rows [0].Columns [5].TileID = 1;
			Rows [0].Columns [6].TileID = 1;
			Rows [0].Columns [7].TileID = 1;

			Rows [1].Columns [3].TileID = 3;
			Rows [1].Columns [4].TileID = 1;
			Rows [1].Columns [5].TileID = 1;
			Rows [1].Columns [6].TileID = 1;
			Rows [1].Columns [7].TileID = 1;

			Rows [2].Columns [2].TileID = 3;
			Rows [2].Columns [3].TileID = 1;
			Rows [2].Columns [4].TileID = 1;
			Rows [2].Columns [5].TileID = 1;
			Rows [2].Columns [6].TileID = 1;
			Rows [2].Columns [7].TileID = 1;

			Rows [3].Columns [2].TileID = 3;
			Rows [3].Columns [3].TileID = 1;
			Rows [3].Columns [4].TileID = 1;
			Rows [3].Columns [5].TileID = 2;
			Rows [3].Columns [6].TileID = 2;
			Rows [3].Columns [7].TileID = 2;

			Rows [4].Columns [2].TileID = 3;
			Rows [4].Columns [3].TileID = 1;
			Rows [4].Columns [4].TileID = 1;
			Rows [4].Columns [5].TileID = 2;
			Rows [4].Columns [6].TileID = 2;
			Rows [4].Columns [7].TileID = 2;

			Rows [5].Columns [2].TileID = 3;
			Rows [5].Columns [3].TileID = 1;
			Rows [5].Columns [4].TileID = 1;
			Rows [5].Columns [5].TileID = 2;
			Rows [5].Columns [6].TileID = 2;
			Rows [5].Columns [7].TileID = 2;
		}*/
	}
}

