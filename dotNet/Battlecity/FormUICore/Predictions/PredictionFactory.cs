using System;
using System.Collections.Generic;
using API.Components;
using FormUI.FieldItems;
using FormUI.Predictions;
using FormUICore.FieldObjects;

namespace FormUICore.Predictions
{
    public static class PredictionFactory
    {
        public static BasePrediction Get(PredictionType type, int depth, Point point, List<Command> commands = null, BaseItem item = null)
        {
            var prediction = Get(type);
            prediction.Depth = depth;
            prediction.Point = point;
            prediction.Item = item;

            if (commands != null)
                prediction.Commands.AddRange(commands);

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
