using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;

namespace Sokoban
{
    public class MapData
    {
        [Title("Map Name")]
        [ShowInInspector, ReadOnly,]
        public string Name { get; private set; }

        [Title("Map Size")]
        [ShowInInspector, ReadOnly,]
        public int Rows { get; private set; }

        [ShowInInspector, ReadOnly,]
        public int Cols { get; private set; }

        [Title("Map Data")]
        [ShowInInspector, ReadOnly,]
        private readonly Dictionary<(int, int), string> _mapTiles = new();

        public void LoadMap(string filename)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Assets/Maps", filename);

            if (!File.Exists(path))
            {
                return;
            }

            using var sr = new StreamReader(path);

            ReadMapName(sr);

            ReadMapSize(sr);

            ReadMapTiles(sr);
        }

        public string GetMapTile(int row, int col)
        {
            return HasMapTile(row, col) ? _mapTiles[(row, col)] : " ";
        }

        private void ReadMapTiles(TextReader sr)
        {
            for (var row = 0; sr.Peek() >= 0; row++)
            {
                var line = sr.ReadLine();
                for (var col = 0; col < line?.Length; col++)
                {
                    var tile = line[col].ToString();

                    if (tile.Equals(" "))
                    {
                        continue;
                    }

                    _mapTiles[(row, col)] = tile;
                }
            }
        }

        private void ReadMapSize(TextReader sr)
        {
            var size = sr.ReadLine()?.Split("x");

            sr.ReadLine();
            if (size is null)
            {
                return;
            }

            Rows = int.Parse(size[1]);
            Cols = int.Parse(size[0]);
        }

        private void ReadMapName(TextReader sr)
        {
            Name = sr.ReadLine();
        }

        private bool HasMapTile(int row, int col)
        {
            return _mapTiles.ContainsKey((row, col));
        }
    }
}
