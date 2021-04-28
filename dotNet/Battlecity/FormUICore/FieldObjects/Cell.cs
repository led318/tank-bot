using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using API.Components;
using FormUI.FieldItems;
using FormUI.FieldItems.Helpers;
using FormUI.FieldItems.Prize;
using FormUI.Infrastructure;
using FormUI.Predictions;
using FormUICore.FieldItems;
using FormUICore.Infrastructure;
using FormUICore.Predictions;
using Point = API.Components.Point;

namespace FormUICore.FieldObjects
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

        private readonly HashSet<Element> _wallElements = new HashSet<Element>
        {
            Element.BATTLE_WALL,
            Element.WALL,
            Element.WALL_DESTROYED_DOWN,
            Element.WALL_DESTROYED_DOWN_LEFT,
            Element.WALL_DESTROYED_DOWN_RIGHT,
            Element.WALL_DESTROYED_DOWN_TWICE,
            Element.WALL_DESTROYED_LEFT,
            Element.WALL_DESTROYED_LEFT_RIGHT,
            Element.WALL_DESTROYED_LEFT_TWICE,
            Element.WALL_DESTROYED_RIGHT,
            Element.WALL_DESTROYED_RIGHT_TWICE,
            Element.WALL_DESTROYED_RIGHT_UP,
            Element.WALL_DESTROYED_UP,
            Element.WALL_DESTROYED_UP_TWICE,
            Element.WALL_DESTROYED_UP_DOWN,
            Element.WALL_DESTROYED_UP_LEFT
        };

        public bool IsWall => IsBorderBattleWall || Items.Any(x => _wallElements.Contains(x.Element));
        public bool IsIce => Items.Any(x => x.Element == Element.ICE);
        public bool IsTree => Items.Any(x => x.Element == Element.TREE);
        public bool IsPrize => Items.Any(x => BasePrize.IsPrize(x.Element));
        public bool IsBlast => Items.Any(x => x.Element == Element.BANG);

        public List<Note> Notes => PredictionNotes.Concat(Items.SelectMany(x => x.Notes).ToList()).ToList();
        public List<Note> PredictionNotes => Predictions.GetPredictionNotes();

        public Color? BorderColor => PredictionColor ?? Item.BorderColor;
        public Color? PredictionColor => Predictions.GetPredictionBorderColor();


        public PredictionAggregate Predictions { get; set; } = new PredictionAggregate();

        public Cell(int i, int j)
        {
            Point = new Point(i, j);
        }

        public int CriticalDangerCount(int depth = 1)
        {
            return Predictions.DangerCellPredictions.Count(x => x.Depth == depth && x.IsCritical);
        }
        
        public int NonCriticalDangerCount(int depth = 1)
        {
            return Predictions.DangerCellPredictions.Count(x => x.Depth == depth && !x.IsCritical);
        }

        public BasePrediction AddPrediction(int depth, PredictionType type, List<Command> commands = null, BaseItem item = null)
        {
            var addedPrediction = Predictions.Add(type, depth, Point, commands, item);
            IsDirty = true;

            return addedPrediction;
        }

        public void Reset(bool clearMySelectedKills = false)
        {
            Predictions.Clear(clearMySelectedKills);
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
