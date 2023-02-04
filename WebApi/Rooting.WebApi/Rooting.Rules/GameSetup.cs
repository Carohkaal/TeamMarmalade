using Rooting.Models;
using System.Linq;

namespace Rooting.Rules
{
    public class GameSetup
    {
        public void Clear()
        {
            Cards.Clear();
        }

        internal void NotFound(string label, string message)
        {
            logLines.Add(new LogLine(LogLineType.NotFound, label, message));
        }

        internal void Invalid(string label, string message)
        {
            logLines.Add(new LogLine(LogLineType.Invalid, label, message));
        }

        public Dictionary<string, Requirement> Requirements { get; } = new();
        public Dictionary<string, ActionBase> Actions { get; } = new();
        public Dictionary<string, CardBase> Cards { get; } = new();
        public Dictionary<int, CardBase> Deck { get; } = new();
        public int RowCount { get; } = 0;
        public int ColCount { get; } = 0;

        public TileBase Tile(int row, int col)
        {
            var key = row << 8 + col;
            return map.ContainsKey(key)
                ? map[key]
                : new TileBase(row, col);
        }

        public FamilyTypes TileBaseType(int row, int col)
        {
            var key = row << 8 + col;
            return map.ContainsKey(key)
                ? map[key].FamilyType
                : FamilyTypes.None;
        }

        public bool AddTile(int row, int col, FamilyTypes familyType)
        {
            var key = row << 8 + col;
            if (map.ContainsKey(key)) return false;
            map.Add(key,
                new TileBase
                {
                    Col = col,
                    Row = row,
                    FamilyType = familyType
                });
            return true;
        }

        public void DefineMap(string mapString)
        {
            var row = 0;
            var lines = mapString.Split('\n', '\r', StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var col = 0;
                foreach (var c in line)
                {
                    var fam = c.ToFamilyType();
                    if (fam != FamilyTypes.None)
                    {
                        AddTile(row, col, fam);
                    }
                    col++;
                }
                row++;
            }
        }

        public IEnumerable<TileBase> Map => map.Values;
        private readonly Dictionary<int, TileBase> map = new();

        public IEnumerable<LogLine> LogLines => logLines;
        private readonly List<LogLine> logLines = new();
    }
}