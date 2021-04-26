using System;
using System.Collections.Generic;
using API.Components;
using FormUI.FieldItems;
using FormUICore.Predictions;

namespace FormUI.Predictions
{
    public static class PredictionFactory
    {
        public static BasePrediction Get(PredictionType type, int depth, Point point, List<Direction> command = null, BaseItem item = null)
        {
            var prediction = Get(type);
            prediction.Depth = depth;
            prediction.Point = point;
            prediction.Item = item;

            if (command != null)
                prediction.Commands.AddRange(command);

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
                case PredictionType.MyKill:
                    return new MyKillPrediction();
                case PredictionType.DangerCell:
                    return new DangerCellPrediction();

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
