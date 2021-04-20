using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms.VisualStyles;
using API.Components;
using FormUI.FieldItems;
using FormUI.FieldItems.Helpers;
using FormUI.Infrastructure;
using Point = API.Components.Point;

namespace FormUI.FieldObjects
{
    public class Cell
    {
        public bool IsDirty { get; set; }

        public List<BaseItem> Items = new List<BaseItem>();

        public BaseItem Item => Items.First();
        public Element Element => Item.Element;

        public Point Point { get; set; }

        public bool IsBorderBattleWall => Point.X == 0 || Point.X == Constants.FieldWidth - 1 ||
                                          Point.Y == 0 || Point.Y == Constants.FieldHeight - 1;

        public List<Note> Notes => PredictionNotes.Concat(Items.SelectMany(x => x.Notes).ToList()).ToList();
        //public List<Note> NotesOverride { get; set; } = new List<Note>();
        public List<Note> PredictionNotes => GetPredictionNotes();

        public Color? BorderColor => PredictionColor ?? Item.BorderColor;
        //public Color? BorderColorOverride { get; set; }
        public Color? PredictionColor => GetPredictionBorderColor();

        public List<Prediction> Predictions { get; set; } = new List<Prediction>();
        public Cell(int i, int j)
        {
            Point = new Point(i, j);
        }

        public void AddNote<T>(T text, Brush color)
        {
            //NotesOverride.Add(new Note(text.ToString(), color));
        }

        public void AddPrediction(int depth, PredictionType type)
        {
            Predictions.Add(new Prediction { Depth = depth, Type = type, Item = Item });
            IsDirty = true;
        }

        public void Reset()
        {
            //NotesOverride.Clear();
            Predictions.Clear();
            IsDirty = false;
        }

        private List<Note> GetPredictionNotes()
        {
            var groups = Predictions.GroupBy(p => p.Type).OrderBy(g => (int)g.Key);
            var result = groups.Select(g =>
            {
                return new Note(g.Min(p => p.Depth), g.First().GetTextColor());
            }).ToList();

            return result;
        }

        private Color? GetPredictionBorderColor()
        {
            var groups = Predictions.GroupBy(p => p.Type).OrderBy(g => (int)g.Key);
            var colours = groups.Select(g => g.First().GetBorderColor()).ToList();

            return colours.FirstOrDefault();
        }
    }
}
