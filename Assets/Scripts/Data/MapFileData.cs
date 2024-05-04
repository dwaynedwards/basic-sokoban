using System;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;

namespace Sokoban
{
    [Serializable]
    public class MapFileData
    {
        #region Fields and Properties

        #region Public

        [ShowInInspector, ReadOnly]
        public int Rows { get; private set; }

        [ShowInInspector, ReadOnly]
        public int Cols { get; private set; }

        #endregion

        #region Private

        [ShowInInspector, ReadOnly]
        private string _name;

        private readonly Dictionary<(int, int), string> _mapTiles = new();

        #endregion

        #endregion

        #region Methods

        #region Public

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

        public void Reset()
        {
            _mapTiles.Clear();
        }

        public string GetMapTile(int row, int col)
        {
            return HasMapTile(row, col) ? _mapTiles[(row, col)] : " ";
        }

        #endregion

        #region Private

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

            if (size is null)
            {
                return;
            }

            Rows = int.Parse(size[0]);
            Cols = int.Parse(size[1]);
        }

        private void ReadMapName(TextReader sr)
        {
            _name = sr.ReadLine();
        }

        private bool HasMapTile(int row, int col)
        {
            return _mapTiles.ContainsKey((row, col));
        }

        #endregion

        #endregion
    }
}
