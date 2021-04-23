using System;
using System.Collections.Generic;
using System.Linq;
using API.Components;
using FormUI.FieldItems;
using FormUI.FieldItems.Tank;
using FormUI.FieldObjects;
using FormUI.Infrastructure;
using FormUI.Predictions;

namespace FormUI.Logic
{
    public static class MyTankLogic
    {
        //private static readonly string _logFile = "log.txt";

        public static void AppendLog(string text)
        {
            //File.AppendAllLines(_logFile, new[] { text });
        }

        public static void CalculateMyTankData()
        {
            CalculateMyMovePredictions();
            CalculateMyKillPredictions();
        }

        private static void CalculateMyMovePredictions()
        {
            if (State.ThisRound.MyTank == null)
                return;

            var myTank = State.ThisRound.MyTank;

            AppendLog("=========================== STARTED ================================");

            CalculateMyShotPredictions(myTank.Point, myTank.CurrentDirection.Value, new List<Direction>());

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
                    var currentPredictionHasNextDepth = CalculateMyMoveDepth(prediction.Point, depth, prediction.Commands);

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
                    var currentCommand = command.ToList();
                    currentCommand.Add(direction);

                    var nextDepthPrediction = (MyMovePrediction)cell.AddPrediction(depth, PredictionType.MyMove, currentCommand);
                    Field.AddMyMoveDepthPredictions(depth, nextDepthPrediction);

                    AppendLog($"COMMAND: {nextDepthPrediction.CommandsText}");
                    isNextDepthAdded = true;


                    CalculateMyShotPredictions(cell.Point, direction, currentCommand);
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

        private static void CalculateMyShotPredictions(Point currentPoint, Direction lastDirection, List<Direction> command)
        {
            var currentCommand = command.ToList();
            CalculateMyTankShotPredictions(currentPoint, lastDirection, currentCommand);

            foreach (var direction in BaseMobile.ValidDirections)
            {
                var movePoint = currentPoint.Shift(direction);

                var cell = Field.GetCell(movePoint);
                if (cell.CanMove)
                {
                    var directionCommand = command.ToList();
                    directionCommand.Add(direction);
                    CalculateMyTankShotPredictions(movePoint, direction, directionCommand);
                }
            }
        }

        private static void CalculateMyTankShotPredictions(Point point, Direction direction, List<Direction> command)
        {
            var startShotPoint = point;

            var startIndex = Math.Min(-1 - State.ThisRound.MyTank.ShotCountdownLeft, 1);

            for (var i = startIndex; i <= Bullet.DefaultSpeed * AppSettings.MyShotPredictionDepth; i++)
            {
                var shotPoint = BaseMobile.Shift(startShotPoint, direction);
                var shotCell = Field.GetCell(shotPoint);

                var depth = (int)Math.Ceiling((decimal)i / 2);

                if (i >= -1)
                {
                    var actualDepth = (command.Count - 1) + Math.Max(0, State.ThisRound.MyTank.ShotCountdownLeft) + depth;

                    var directionActCommand = command.ToList();
                    directionActCommand.Add(Direction.Act);
                    shotCell.AddPrediction(actualDepth, PredictionType.MyShot, directionActCommand);
                }

                if (shotCell.CanShootThrough)
                {
                    startShotPoint = shotPoint;
                }
                else
                {
                    break;
                }
            }
        }

        private static void CalculateMyKillPredictions()
        {
            var allCells = Field.AllCells;

            var aiMoveCells = allCells.Where(x => x.Predictions.AiMovePredictions.Any()).ToList();

            foreach (var aiMoveCell in aiMoveCells)
            {
                var oneHealthAiMovePredictions = aiMoveCell.Predictions.AiMovePredictions.Where(x => ((BaseTank) x.Item).Health == 1).ToList();

                foreach (var aiMovePrediction in oneHealthAiMovePredictions)
                {
                    var mySameDepthShots = aiMoveCell.Predictions.MyShotPredictions
                        .Where(x => x.Depth == aiMovePrediction.Depth).ToList();

                    foreach (var mySameDepthShot in mySameDepthShots)
                    {
                        aiMoveCell.AddPrediction(aiMovePrediction.Depth, PredictionType.MyKill, mySameDepthShot.Commands);
                    }
                }
            }
        }
    }
}
