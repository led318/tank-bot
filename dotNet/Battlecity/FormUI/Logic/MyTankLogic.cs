using System.Collections.Generic;
using System.Linq;
using API.Components;
using FormUI.FieldItems;
using FormUI.FieldObjects;
using FormUI.Infrastructure;
using FormUI.Predictions;

namespace FormUI.Logic
{
    public static class MyTankLogic
    {
        private static readonly string _logFile = "log.txt";

        public static void AppendLog(string text)
        {
            //File.AppendAllLines(_logFile, new[] { text });
        }

        public static void CalculateMyMovePredictions()
        {
            if (State.ThisRound.MyTank == null)
                return;

            var myTank = State.ThisRound.MyTank;

            AppendLog("=========================== STARTED ================================");

            CalculateMyMoveDepth(myTank.Point, 1, new List<Direction>());

            var depth = 1;
            var hasNextDepth = true;
            while (hasNextDepth)
            {
                var predictions = Field.GetMyMoveDepthPredictions(depth);

                depth++;

                var anyPredictionHasNextDepth = false;

                foreach (var prediction in predictions)
                {
                    var currentPredictionHasNextDepth = CalculateMyMoveDepth(prediction.Point, depth, prediction.Command);

                    if (currentPredictionHasNextDepth)
                        anyPredictionHasNextDepth = true;
                }

                hasNextDepth = anyPredictionHasNextDepth;
            }
        }

        private static bool CalculateMyMoveDepth(Point startPoint, int depth, List<Direction> command)
        {
            if (depth > AppSettings.MyMovePredictionDepth)
                return false;

            var isNextDepthAdded = false;
            AppendLog($"DEPTH: {depth}, POINT: {startPoint}");

            foreach (var direction in BaseMobile.ValidDirections)
            {
                var point = startPoint.Shift(direction);
                var cell = Field.GetCell(point);

                if (IsSafeMoveNotVisitedCell(cell, depth))
                {
                    var currentCommand = command.DeepClone();
                    currentCommand.Add(direction);

                    var nextDepthPrediction = (MyMovePrediction)cell.AddPrediction(depth, PredictionType.MyMove, currentCommand);
                    Field.AddMyMoveDepthPredictions(depth, nextDepthPrediction);

                    AppendLog($"COMMAND: {nextDepthPrediction.CommandText}");
                    isNextDepthAdded = true;
                }
            }

            return isNextDepthAdded;
        }



        private static bool IsSafeMoveNotVisitedCell(Cell cell, int depth)
        {
            if (!cell.CanMove)
                return false;

            if (cell.HasMyMovePrediction(depth))
                return false;

            var hasDepthBullets = cell.HasDepthPrediction(depth, x => x.BulletPredictions);
            if (hasDepthBullets)
                return false;

            var hasAiShot = cell.HasDepthPrediction(depth, x => x.AiShotPredictions);
            if (hasAiShot)
                return false;

            var hasEnemyShot = cell.HasDepthPrediction(depth, x => x.EnemyShotPredictions);
            if (hasEnemyShot)
                return false;

            return true;
        }

        public static void CalculateMyShotPredictions()
        {
            if (State.ThisRound.MyTank == null)
                return;

            var myTank = State.ThisRound.MyTank;
            if (myTank.CurrentDirection.HasValue)
            {
                var command = new List<Direction> { Direction.Act };
                PredictionLogic.CalculateTankShotPredictions(myTank.Point, PredictionType.MyShot, myTank.CurrentDirection.Value, myTank, AppSettings.MyShotPredictionDepth, command);
            }

            foreach (var direction in BaseMobile.ValidDirections)
            {
                var point = myTank.Point.Shift(direction);

                var cell = Field.GetCell(point);
                if (cell.CanMove)
                {
                    var command = new List<Direction> { direction, Direction.Act };
                    PredictionLogic.CalculateTankShotPredictions(point, PredictionType.MyShot, direction, myTank, AppSettings.MyShotPredictionDepth, command);
                }
            }
        }
    }
}
