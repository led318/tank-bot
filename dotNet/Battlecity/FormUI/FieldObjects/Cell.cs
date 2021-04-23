using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using API.Components;
using FormUI.FieldItems;
using FormUI.FieldItems.Helpers;
using FormUI.Infrastructure;
using FormUI.Predictions;
using Point = API.Components.Point;

namespace FormUI.FieldObjects
{
    public class Cell
    {
        public bool IsDirty { get; set; }

        public List<BaseItem> Items = new List<BaseItem>();

        public BaseItem Item => Items.First();
        public bool CanMove => Items.All(x => x.CanMove);
        public bool CanShootThrough => Items.All(x => x.CanShootThrough);

        public Point Point { get; set; }

        public bool IsBorderBattleWall => Point.X == 0 || Point.X == Constants.FieldWidth - 1 ||
                                          Point.Y == 0 || Point.Y == Constants.FieldHeight - 1;

        public List<Note> Notes => PredictionNotes.Concat(Items.SelectMany(x => x.Notes).ToList()).ToList();
        //public List<Note> NotesOverride { get; set; } = new List<Note>();
        public List<Note> PredictionNotes => Predictions.GetPredictionNotes();

        public Color? BorderColor => PredictionColor ?? Item.BorderColor;
        //public Color? BorderColorOverride { get; set; }
        public Color? PredictionColor => Predictions.GetPredictionBorderColor();

        //public List<BasePrediction> Predictions { get; set; } = new List<BasePrediction>();

        public PredictionAggregate Predictions { get; set; } = new PredictionAggregate();

        public Cell(int i, int j)
        {
            Point = new Point(i, j);
        }

        public void AddNote<T>(T text, Brush color)
        {
            //NotesOverride.Add(new Note(text.ToString(), color));
        }

        public BasePrediction AddPrediction(int depth, PredictionType type, List<Direction> command = null, BaseItem item = null)
        {
            var addedPrediction = Predictions.Add(type, depth, Point, command, item);
            IsDirty = true;

            return addedPrediction;
        }

        public void Reset()
        {
            //NotesOverride.Clear();
            Predictions.Clear();
            IsDirty = false;
        }

        public bool HasMyMovePrediction(int depth)
        {
            return Predictions.MyMovePredictions.Any(p => p.Depth <= depth + 1);
        }

        public bool HasDepthPrediction<T>(int depth, Func<PredictionAggregate, IEnumerable<T>> func) where T: BasePrediction
        {
            var hasDepthPredictions = func(Predictions).Any(x => x.Depth == depth);

            return hasDepthPredictions;
        }
    }
}
