using System.Collections.Generic;
using UnityEngine;

namespace PocketDungeons.Gameplay.Dungeon
{
    /// <summary>
    /// Binary Space Partitioning dungeon generator.
    /// Pipeline: Seed → BSP Partition → Room Placement → Corridor Generation → Population → Validation
    /// </summary>
    public class BSPDungeonGenerator
    {
        private readonly int _width;
        private readonly int _height;
        private readonly int _minRoomSize;
        private readonly int _maxRoomSize;
        private readonly int _corridorWidth;
        private readonly int _maxDepth;

        public BSPDungeonGenerator(int width = 40, int height = 40, int minRoomSize = 6, int maxRoomSize = 12, int corridorWidth = 2, int maxDepth = 5)
        {
            _width = width;
            _height = height;
            _minRoomSize = minRoomSize;
            _maxRoomSize = maxRoomSize;
            _corridorWidth = corridorWidth;
            _maxDepth = maxDepth;
        }

        public DungeonData Generate(int seed, int floorDepth)
        {
            Random.InitState(seed);

            var tiles = new TileType[_width, _height];
            var rooms = new List<RectInt>();
            var corridors = new List<RectInt>();

            // Fill with walls
            for (int x = 0; x < _width; x++)
                for (int y = 0; y < _height; y++)
                    tiles[x, y] = TileType.Wall;

            // BSP partition
            var root = new BSPNode(new RectInt(1, 1, _width - 2, _height - 2));
            SplitNode(root, 0);

            // Create rooms in leaf nodes
            CollectRooms(root, rooms, tiles);

            // Connect rooms with corridors
            ConnectRooms(root, corridors, tiles);

            // Place entrance and exit
            var entrance = rooms[0].center;
            var exit = rooms[rooms.Count - 1].center;
            tiles[(int)entrance.x, (int)entrance.y] = TileType.Entrance;
            tiles[(int)exit.x, (int)exit.y] = TileType.Exit;

            return new DungeonData
            {
                Tiles = tiles,
                Rooms = rooms,
                Corridors = corridors,
                Width = _width,
                Height = _height,
                Entrance = new Vector2Int((int)entrance.x, (int)entrance.y),
                Exit = new Vector2Int((int)exit.x, (int)exit.y),
                Seed = seed,
                FloorDepth = floorDepth
            };
        }

        private void SplitNode(BSPNode node, int depth)
        {
            if (depth >= _maxDepth) return;
            if (node.Bounds.width < _minRoomSize * 2 + 2 && node.Bounds.height < _minRoomSize * 2 + 2) return;

            bool splitHorizontal;
            if (node.Bounds.width > node.Bounds.height * 1.25f)
                splitHorizontal = false;
            else if (node.Bounds.height > node.Bounds.width * 1.25f)
                splitHorizontal = true;
            else
                splitHorizontal = Random.value > 0.5f;

            if (splitHorizontal)
            {
                if (node.Bounds.height < _minRoomSize * 2 + 2) return;

                int splitY = Random.Range(
                    node.Bounds.y + _minRoomSize,
                    node.Bounds.y + node.Bounds.height - _minRoomSize);

                node.Left = new BSPNode(new RectInt(
                    node.Bounds.x, node.Bounds.y,
                    node.Bounds.width, splitY - node.Bounds.y));

                node.Right = new BSPNode(new RectInt(
                    node.Bounds.x, splitY,
                    node.Bounds.width, node.Bounds.y + node.Bounds.height - splitY));
            }
            else
            {
                if (node.Bounds.width < _minRoomSize * 2 + 2) return;

                int splitX = Random.Range(
                    node.Bounds.x + _minRoomSize,
                    node.Bounds.x + node.Bounds.width - _minRoomSize);

                node.Left = new BSPNode(new RectInt(
                    node.Bounds.x, node.Bounds.y,
                    splitX - node.Bounds.x, node.Bounds.height));

                node.Right = new BSPNode(new RectInt(
                    splitX, node.Bounds.y,
                    node.Bounds.x + node.Bounds.width - splitX, node.Bounds.height));
            }

            SplitNode(node.Left, depth + 1);
            SplitNode(node.Right, depth + 1);
        }

        private void CollectRooms(BSPNode node, List<RectInt> rooms, TileType[,] tiles)
        {
            if (node.Left == null && node.Right == null)
            {
                // Leaf node — create a room
                int roomW = Random.Range(_minRoomSize, Mathf.Min(_maxRoomSize, node.Bounds.width - 1) + 1);
                int roomH = Random.Range(_minRoomSize, Mathf.Min(_maxRoomSize, node.Bounds.height - 1) + 1);
                int roomX = Random.Range(node.Bounds.x, node.Bounds.x + node.Bounds.width - roomW);
                int roomY = Random.Range(node.Bounds.y, node.Bounds.y + node.Bounds.height - roomH);

                var room = new RectInt(roomX, roomY, roomW, roomH);
                node.Room = room;
                rooms.Add(room);

                // Carve floor tiles
                for (int x = room.x; x < room.x + room.width; x++)
                    for (int y = room.y; y < room.y + room.height; y++)
                        if (x >= 0 && x < _width && y >= 0 && y < _height)
                            tiles[x, y] = TileType.Floor;

                return;
            }

            if (node.Left != null) CollectRooms(node.Left, rooms, tiles);
            if (node.Right != null) CollectRooms(node.Right, rooms, tiles);

            // Propagate room references up
            if (node.Left != null) node.Room = node.Left.Room;
        }

        private void ConnectRooms(BSPNode node, List<RectInt> corridors, TileType[,] tiles)
        {
            if (node.Left == null || node.Right == null) return;

            ConnectRooms(node.Left, corridors, tiles);
            ConnectRooms(node.Right, corridors, tiles);

            var leftRoom = GetDeepestRoom(node.Left);
            var rightRoom = GetDeepestRoom(node.Right);

            if (leftRoom.HasValue && rightRoom.HasValue)
            {
                var start = new Vector2Int(
                    (int)leftRoom.Value.center.x,
                    (int)leftRoom.Value.center.y);
                var end = new Vector2Int(
                    (int)rightRoom.Value.center.x,
                    (int)rightRoom.Value.center.y);

                CarveCorridor(start, end, corridors, tiles);
            }
        }

        private void CarveCorridor(Vector2Int start, Vector2Int end, List<RectInt> corridors, TileType[,] tiles)
        {
            // L-shaped corridor: horizontal then vertical
            int x = start.x;
            int y = start.y;

            // Horizontal segment
            int dirX = end.x > x ? 1 : -1;
            while (x != end.x)
            {
                for (int w = 0; w < _corridorWidth; w++)
                {
                    int ty = y + w;
                    if (x >= 0 && x < _width && ty >= 0 && ty < _height)
                        if (tiles[x, ty] == TileType.Wall)
                            tiles[x, ty] = TileType.Corridor;
                }
                x += dirX;
            }

            // Vertical segment
            int dirY = end.y > y ? 1 : -1;
            while (y != end.y)
            {
                for (int w = 0; w < _corridorWidth; w++)
                {
                    int tx = x + w;
                    if (tx >= 0 && tx < _width && y >= 0 && y < _height)
                        if (tiles[tx, y] == TileType.Wall)
                            tiles[tx, y] = TileType.Corridor;
                }
                y += dirY;
            }
        }

        private RectInt? GetDeepestRoom(BSPNode node)
        {
            if (node == null) return null;
            if (node.Left == null && node.Right == null)
                return node.Room;

            var leftRoom = GetDeepestRoom(node.Left);
            var rightRoom = GetDeepestRoom(node.Right);

            return leftRoom ?? rightRoom;
        }
    }

    public class BSPNode
    {
        public RectInt Bounds;
        public BSPNode Left;
        public BSPNode Right;
        public RectInt Room;

        public BSPNode(RectInt bounds)
        {
            Bounds = bounds;
        }
    }
}
