using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Components;
using FormUI.FieldItems.Helpers;
using FormUI.Infrastructure;
using Point = API.Components.Point;

namespace FormUI.Predictions
{
    public class PredictionAggregate
    {
        public List<AiMovePrediction> AiMovePredictions { get; set; } = new List<AiMovePrediction>();
        public List<AiShotPrediction> AiShotPredictions { get; set; } = new List<AiShotPrediction>();
        public List<BulletPrediction> BulletPredictions { get; set; } = new List<BulletPrediction>();
        public List<EnemyMovePrediction> EnemyMovePredictions { get; set; } = new List<EnemyMovePrediction>();
        public List<EnemyShotPrediction> EnemyShotPredictions { get; set; } = new List<EnemyShotPrediction>();
        public List<MyMovePrediction> MyMovePredictions { get; set; } = new List<MyMovePrediction>();
        public List<MyShotPrediction> MyShotPredictions { get; set; } = new List<MyShotPrediction>();

        public List<BasePrediction> AllPredictions => GetAllPredictions();

        private List<BasePrediction> GetAllPredictions()
        {
            var result = new List<BasePrediction>();

            result.AddRange(AiMovePredictions);
            result.AddRange(AiShotPredictions);
            result.AddRange(BulletPredictions);
            result.AddRange(EnemyMovePredictions);
            result.AddRange(EnemyShotPredictions);
            result.AddRange(MyMovePredictions);
            result.AddRange(MyShotPredictions);

            return result;
        }

        public BasePrediction Add(PredictionType type, int depth, Point point, List<Direction> command)
        {
            var prediction = PredictionFactory.Get(type, depth, point, command);

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

                default:
                    throw new NotImplementedException();
            }

            return prediction;
        }

        public void Clear()
        {
            AiMovePredictions.Clear();
            AiShotPredictions.Clear();
            BulletPredictions.Clear();
            EnemyMovePredictions.Clear();
            EnemyShotPredictions.Clear();
            MyMovePredictions.Clear();
            MyShotPredictions.Clear();
        }

        public List<Note> GetPredictionNotes()
        {
            var groups = AllPredictions.GroupBy(p => p.Type).OrderBy(g => (int)g.Key);
            var result = groups
                .Where(g => PredictionSettings.GetVisible(g.Key))
                .Select(g =>
                {
                    return new Note(g.Min(p => p.Depth), g.First().GetTextColor());
                }).ToList();

            return result;
        }

        public Color? GetPredictionBorderColor()
        {


            var groups = AllPredictions.GroupBy(p => p.Type).OrderBy(g => (int)g.Key);
            var colours = groups
                .Where(g => PredictionSettings.GetVisible(g.Key))
                .Select(g => g.First().GetBorderColor()).ToList();

            return colours.FirstOrDefault();
        }
    }
}
