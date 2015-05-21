using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TileEngine
{
	[Serializable]
	public class MapSquare
	{
		#region Declarations
		public int[] LayerTiles = new int[3];
		public string CodeValue = "";
		public bool Passable = true;
		#endregion

		public static MapSquare Neutral{
			get{ return new MapSquare (
					TileMap.skyTile,
					0,
					0,
					"",
					true);
			}
		}

		#region Constructor
		public MapSquare (
			int background,
			int interactive,
			int foreground,
			string code,
			bool passable)
		{
			LayerTiles [0] = background;
			LayerTiles [1] = interactive;
			LayerTiles [2] = foreground;
			CodeValue = code;
			Passable = passable;
		}
		#endregion

		#region Public Methods
		public void TogglePassable(){
			Passable = !Passable;
		}

		public void Copy(MapSquare mapSquare){
			LayerTiles [0] = mapSquare.LayerTiles[0];
			LayerTiles [1] = mapSquare.LayerTiles[1];
			LayerTiles [2] = mapSquare.LayerTiles[2];
			CodeValue = mapSquare.CodeValue;
			Passable = mapSquare.Passable;
		}
		#endregion
	}
}

