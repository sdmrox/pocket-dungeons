using UnityEngine;
using UnityEngine.Tilemaps;

namespace PocketDungeons.Gameplay.Dungeon
{
    /// <summary>
    /// Renders DungeonData to Unity Tilemaps.
    /// </summary>
    public class DungeonRenderer : MonoBehaviour
    {
        [Header("Tilemaps")]
        [SerializeField] private Tilemap _floorTilemap;
        [SerializeField] private Tilemap _wallTilemap;

        [Header("Tiles")]
        [SerializeField] private TileBase _floorTile;
        [SerializeField] private TileBase _wallTile;
        [SerializeField] private TileBase _corridorTile;
        [SerializeField] private TileBase _entranceTile;
        [SerializeField] private TileBase _exitTile;

        public void Render(DungeonData data)
        {
            Clear();

            for (int x = 0; x < data.Width; x++)
            {
                for (int y = 0; y < data.Height; y++)
                {
                    var pos = new Vector3Int(x, y, 0);

                    switch (data.Tiles[x, y])
                    {
                        case TileType.Wall:
                            _wallTilemap.SetTile(pos, _wallTile);
                            break;
                        case TileType.Floor:
                            _floorTilemap.SetTile(pos, _floorTile);
                            break;
                        case TileType.Corridor:
                            _floorTilemap.SetTile(pos, _corridorTile ?? _floorTile);
                            break;
                        case TileType.Entrance:
                            _floorTilemap.SetTile(pos, _entranceTile ?? _floorTile);
                            break;
                        case TileType.Exit:
                            _floorTilemap.SetTile(pos, _exitTile ?? _floorTile);
                            break;
                    }
                }
            }
        }

        public void Clear()
        {
            _floorTilemap.ClearAllTiles();
            _wallTilemap.ClearAllTiles();
        }
    }
}
