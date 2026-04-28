using UnityEngine;

namespace PocketDungeons.Data
{
    [CreateAssetMenu(fileName = "NewBiome", menuName = "Pocket Dungeons/Data/Biome")]
    public class BiomeData : ScriptableObject
    {
        [Header("Identity")]
        public string DisplayName;
        [TextArea(1, 2)]
        public string Description;

        [Header("Visuals")]
        public Color PrimaryColor;
        public Color SecondaryColor;
        public Color AccentColor;
        public Color BackgroundColor;

        [Header("Tilesets")]
        public Sprite FloorTile;
        public Sprite WallTile;
        public Sprite DoorTile;
        public Sprite[] DecorationTiles;

        [Header("Enemies")]
        public EnemyData[] AvailableEnemies;
        [Range(1, 20)]
        public int MinEnemiesPerRoom = 2;
        [Range(1, 20)]
        public int MaxEnemiesPerRoom = 6;

        [Header("Music")]
        public AudioClip ExplorationMusic;
        public AudioClip CombatMusic;
        public AudioClip BossMusic;

        [Header("Floor Range")]
        [Tooltip("First floor this biome can appear on")]
        public int MinFloor = 1;
        [Tooltip("Last floor this biome appears on (0 = unlimited)")]
        public int MaxFloor = 10;
    }
}
