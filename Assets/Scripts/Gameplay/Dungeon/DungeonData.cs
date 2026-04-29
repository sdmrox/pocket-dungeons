using System.Collections.Generic;
using UnityEngine;

namespace PocketDungeons.Gameplay.Dungeon
{
    public enum TileType
    {
        Wall,
        Floor,
        Corridor,
        Entrance,
        Exit,
        Door
    }

    public class DungeonData
    {
        public TileType[,] Tiles;
        public List<RectInt> Rooms;
        public List<RectInt> Corridors;
        public int Width;
        public int Height;
        public Vector2Int Entrance;
        public Vector2Int Exit;
        public int Seed;
        public int FloorDepth;

        public bool IsWalkable(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height) return false;
            return Tiles[x, y] != TileType.Wall;
        }

        public bool IsRoom(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height) return false;
            return Tiles[x, y] == TileType.Floor;
        }
    }
}
