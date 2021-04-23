using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Components;

namespace FormUI.Predictions
{
    public static class PredictionFactory
    {
        public static BasePrediction Get(PredictionType type, int depth, Point point, List<Direction> command)
        {
            var prediction = Get(type);
            prediction.Depth = depth;
            prediction.Point = point;

            if (command != null)
                prediction.Command.AddRange(command);

            return prediction;
        }

        public static BasePrediction Get(PredictionType type)
        {
            switch (type)
            {
                case PredictionType.Bullet:
                    return new BulletPrediction();
                case PredictionType.AiMove:
                    return new AiMovePrediction();
                case PredictionType.AiShot:
                    return new AiShotPrediction();
                case PredictionType.EnemyMove:
                    return new EnemyMovePrediction();
                case PredictionType.EnemyShot:
                    return new EnemyShotPrediction();
                case PredictionType.MyShot:
                    return new MyShotPrediction();
                case PredictionType.MyMove:
                    return new MyMovePrediction();

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
