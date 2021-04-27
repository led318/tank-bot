using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using API.Components;
using FormUI.FieldItems;
using FormUI.FieldItems.Helpers;
using FormUI.Infrastructure;
using FormUICore.FieldItems;
using FormUICore.Predictions;
using Point = API.Components.Point;

namespace FormUI.Predictions
{
    public class PredictionAggregate
    {
        public List<AiMovePrediction> AiMovePredictions { get; set; } = new List<AiMovePrediction>();
        public List<AiShotPrediction> AiShotPredictions { get; set; } = new List<AiShotPrediction>();
        public List<BulletPrediction> BulletPredictions { get; set; } = new List<BulletPrediction>();
        public List<BulletPrediction> NotMyBulletPredictions => BulletPredictions.Where(x => !((Bullet)x.Item).IsMyBullet).ToList();

        public List<EnemyMovePrediction> EnemyMovePredictions { get; set; } = new List<EnemyMovePrediction>();
        public List<EnemyShotPrediction> EnemyShotPredictions { get; set; } = new List<EnemyShotPrediction>();
        public List<MyMovePrediction> MyMovePredictions { get; set; } = new List<MyMovePrediction>();
        public List<MyShotPrediction> MyShotPredictions { get; set; } = new List<MyShotPrediction>();
        public List<MyKillPrediction> MyKillPredictions { get; set; } = new List<MyKillPrediction>();
        public List<DangerCellPrediction> DangerCellPredictions { get; set; } = new List<DangerCellPrediction>();

        public List<MyKillPrediction> MySelectedKillPredictions { get; set; } = new List<MyKillPrediction>();

        public List<BasePrediction> AllVisiblePredictions => _allVisiblePredictionsLazy.Value;

        private Lazy<List<BasePrediction>> _allVisiblePredictionsLazy => new Lazy<List<BasePrediction>>(() => GetAllVisiblePredictions());

        private List<BasePrediction> GetAllVisiblePredictions()
        {
            //var result = new List<BasePrediction>();

            //result.AddRange(AiMovePredictions);
            //result.AddRange(AiShotPredictions);
            //result.AddRange(BulletPredictions);
            //result.AddRange(EnemyMovePredictions);
            //result.AddRange(EnemyShotPredictions);
            //result.AddRange(MyMovePredictions);
            //result.AddRange(MyShotPredictions);
            //result.AddRange(MyKillPredictions);
            //result.AddRange(DangerCellPredictions);

            return GetVisiblePredictions(PredictionType.AiMove, AiMovePredictions.Select(x => (BasePrediction)x))
                .Concat(GetVisiblePredictions(PredictionType.AiShot, AiShotPredictions.Select(x => (BasePrediction)x)))
                .Concat(GetVisiblePredictions(PredictionType.Bullet, BulletPredictions.Select(x => (BasePrediction)x)))
                .Concat(GetVisiblePredictions(PredictionType.EnemyMove, EnemyMovePredictions.Select(x => (BasePrediction)x)))
                .Concat(GetVisiblePredictions(PredictionType.EnemyShot, EnemyShotPredictions.Select(x => (BasePrediction)x)))
                .Concat(GetVisiblePredictions(PredictionType.MyMove, MyMovePredictions.Select(x => (BasePrediction)x)))
                .Concat(GetVisiblePredictions(PredictionType.MyShot, MyShotPredictions.Select(x => (BasePrediction)x)))
                .Concat(GetVisiblePredictions(PredictionType.MyKill, MyKillPredictions.Select(x => (BasePrediction)x)))
                .Concat(GetVisiblePredictions(PredictionType.DangerCell, DangerCellPredictions.Select(x => (BasePrediction)x)))
                .ToList();
        }

        private List<BasePrediction> GetVisiblePredictions(PredictionType type, IEnumerable<BasePrediction> list)
        {
            return PredictionSettings.GetVisible(type)
                ? list.ToList()
                : new List<BasePrediction>();
        }

        public BasePrediction Add(PredictionType type, int depth, Point point, List<Direction> command = null, BaseItem item = null)
        {
            var prediction = PredictionFactory.Get(type, depth, point, command, item);

            switch (type)
            {
                case PredictionType.AiMove:
                    AiMovePredictions.Add((AiMovePrediction)prediction);
                    break;
                case PredictionType.AiShot:
                    AiShotPredictions.Add((AiShotPrediction)prediction);
                    break;
                case PredictionType.Bullet:
                    BulletPredictions.Add((BulletPrediction)prediction);
                    break;
                case PredictionType.EnemyMove:
                    EnemyMovePredictions.Add((EnemyMovePrediction)prediction);
                    break;
                case PredictionType.EnemyShot:
                    EnemyShotPredictions.Add((EnemyShotPrediction)prediction);
                    break;
                case PredictionType.MyMove:
                    MyMovePredictions.Add((MyMovePrediction)prediction);
                    break;
                case PredictionType.MyShot:
                    MyShotPredictions.Add((MyShotPrediction)prediction);
                    break;
                case PredictionType.MyKill:
                    MyKillPredictions.Add((MyKillPrediction)prediction);
                    break;
                case PredictionType.DangerCell:
                    DangerCellPredictions.Add((DangerCellPrediction)prediction);
                    break;

                default:
                    throw new NotImplementedException();
            }

            return prediction;
        }

        public void Clear(bool clearMySelectedKills = false)
        {
            AiMovePredictions.Clear();
            AiShotPredictions.Clear();
            BulletPredictions.Clear();
            EnemyMovePredictions.Clear();
            EnemyShotPredictions.Clear();
            MyMovePredictions.Clear();
            MyShotPredictions.Clear();
            MyKillPredictions.Clear();
            DangerCellPredictions.Clear();

            if (clearMySelectedKills)
                MySelectedKillPredictions.Clear();
        }

        public List<Note> GetPredictionNotes()
        {
            var groups = AllVisiblePredictions.GroupBy(p => p.Type).OrderBy(g => (int)g.Key);
            var result = groups
                .Where(g => PredictionSettings.GetVisible(g.Key))
                .Select(g =>
                {
                    return new Note(g.Min(p => p.Depth), g.First().GetTextColor());
                }).ToList();


            var mySelectedKillPredictionNote = MySelectedKillPredictions
                .OrderBy(x => x.Depth)
                .Select(x => new Note(x.Depth, x.GetTextColor()))
                .FirstOrDefault();

            if (mySelectedKillPredictionNote != null)
                result.Insert(0, mySelectedKillPredictionNote);

            return result;
        }

        public Color? GetPredictionBorderColor()
        {
            var groups = AllVisiblePredictions.GroupBy(p => p.Type).OrderBy(g => (int)g.Key);
            var colors = groups
                .Where(g => PredictionSettings.GetVisible(g.Key))
                .Select(g => g.First().GetBorderColor()).ToList();

            return colors.FirstOrDefault();
        }
    }
}
