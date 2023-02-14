using Rooting.Models;
using System.Linq;

namespace Rooting.Rules
{
    public class GameSetup : ICloneable
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

        public int MapColums()
        {
            var col = 0;
            foreach (var m in map.Values)
            {
                if (m.Col > col) col = m.Col;
            }
            return col;
        }

        public int MapRows()
        {
            var row = 0;
            foreach (var m in map.Values)
            {
                if (m.Row > row) row = m.Row;
            }
            return row;
        }

        public IEnumerable<TileBase> Tiles() => map.Values;

        public TileBase Tile(int row, int col)
        {
            var key = (row << 8) + col;
            return map.ContainsKey(key)
                ? map[key]
                : new TileBase(row, col);
        }

        public FamilyTypes TileBaseType(int row, int col)
        {
            var key = (row << 8) + col;
            return map.ContainsKey(key)
                ? map[key].FamilyType
                : FamilyTypes.Any;
        }

        public bool AddTile(int row, int col, FamilyTypes familyType)
        {
            var key = (row << 8) + col;
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
                    if (fam != FamilyTypes.Any)
                    {
                        AddTile(row, col, fam);
                    }
                    col++;
                }
                row++;
            }
        }

        public object Clone()
        {
            var r = new GameSetup();
            foreach (var item in Cards)
            {
                r.Cards.Add(item.Key, (CardBase)item.Value.Clone());
            }

            foreach (var item in Actions)
            {
                r.Actions.Add(item.Key, (ActionBase)item.Value.Clone());
            }

            foreach (var item in Requirements)
            {
                r.Requirements.Add(item.Key, (Requirement)item.Value.Clone());
            }

            foreach (var item in Deck)
            {
                r.Deck.Add(item.Key, (PlayingCard)item.Value.Clone());
            }

            foreach (var item in map)
            {
                r.map.Add(item.Key, (TileBase)item.Value.Clone());
            }

            return r;
        }

        public IEnumerable<TileBase> Map => map.Values;
        private readonly Dictionary<int, TileBase> map = new();

        public IEnumerable<LogLine> LogLines => logLines;
        private readonly List<LogLine> logLines = new();
    }
}