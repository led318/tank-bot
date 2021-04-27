using System;
using System.Collections.Generic;
using System.Linq;
using API.Components;
using FormUI.FieldItems;
using FormUI.FieldItems.Tank;
using FormUI.FieldObjects;
using FormUI.Infrastructure;
using FormUI.Predictions;
using FormUICore.FieldItems;
using FormUICore.FieldObjects;
using FormUICore.Predictions;

namespace FormUICore.Logic
{
    public static class MyTankPredictionLogic
    {
        //private static readonly string _logFile = "log.txt";

        public static void AppendLog(string text)
        {
            //File.AppendAllLines(_logFile, new[] { text });
        }

        public static void CalculateMyTankData()
        {
            CalculateMyShotBasedOnPrevRound();
            CalculateMyMovePredictions();
            CalculateMyKillAiPredictions();
            CalculateMyKillEnemyPredictions();
        }

        private static void CalculateMyShotBasedOnPrevRound()
        {
            if (State.HasPrevRound && State.ThisRound.MyTank != null)
            {
                if (State.PrevRound.CurrentMoveCommands.Contains(Direction.Act))
                {
                    State.ThisRound.MyTank.Shot();
                }
            }
        }

        private static void CalculateMyMovePredictions()
        {
            if (State.ThisRound.MyTank == null)
                return;

            var myTank = State.ThisRound.MyTank;

            AppendLog("=========================== STARTED ================================");

            CalculateMyShotPredictions(myTank.Point, myTank.Direction.Value, new List<Direction>());

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

            var hasDepthBullets = cell.HasDepthPrediction(depth, x => x.NotMyBulletPredictions);
            if (hasDepthBullets)
                return false;

            var hasPrevDepthBullets = cell.HasDepthPrediction(depth - 1, x => x.NotMyBulletPredictions);
            if (hasPrevDepthBullets)
                return false;

            var hasAiShot = cell.HasDepthPrediction(depth, x => x.AiShotPredictions);
            if (hasAiShot)
                return false;

            var hasEnemyShot = cell.HasDepthPrediction(depth, x => x.EnemyShotPredictions);
            if (hasEnemyShot)
                return false;

            var hasDangerCells = cell.Predictions.DangerCellPredictions.Any(x => x.Depth == depth);
            if (hasDangerCells)
                return false;

            return true;
        }

        private static void CalculateMyShotPredictions(Point currentPoint, Direction lastDirection, List<Direction> command)
        {
            var currentCommand = command.ToList();
            CalculateMyShotsForDirection(currentPoint, lastDirection, currentCommand, true);

            foreach (var direction in BaseMobile.ValidDirections)
            {
                var movePoint = currentPoint.Shift(direction);

                var cell = Field.GetCell(movePoint);
                if (cell.CanMove)
                {
                    var directionCommand = command.ToList();
                    directionCommand.Add(direction);
                    CalculateMyShotsForDirection(movePoint, direction, directionCommand);
                }
                else
                {
                    var directionCommand = command.ToList();
                    directionCommand.Add(direction);
                    CalculateMyShotsForDirection(currentPoint, direction, directionCommand);
                }
            }
        }

        private static void CalculateMyShotsForDirection(Point point, Direction direction, List<Direction> command, bool correctFirstShotDepth = false)
        {
            var startShotPoint = point;

            //var startIndex = Math.Min(1 - State.ThisRound.MyTank.ShotCountdownLeft, 1);
            var startIndex = 1;

            for (var i = startIndex; i <= Bullet.DefaultSpeed * AppSettings.MyShotPredictionDepth; i++)
            {
                var shotPoint = BaseMobile.Shift(startShotPoint, direction);
                var shotCell = Field.GetCell(shotPoint);

                if (shotCell.IsWall)
                    break;

                var depth = (int)Math.Ceiling((decimal)i / 2);
                if (correctFirstShotDepth)
                    depth--;

                var directionActCommand = command.ToList();
                directionActCommand.Add(Direction.Act);

                var commandRoundsCount = directionActCommand.RoundsCount();
                var shotCountDownLeft = State.ThisRound.MyTank.ShotCountdownLeft;
                var shotDepth = depth;

                var actualDepth = commandRoundsCount + shotCountDownLeft + shotDepth;

                shotCell.AddPrediction(actualDepth, PredictionType.MyShot, directionActCommand);

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

        private static void CalculateMyKillAiPredictions()
        {
            var allCells = Field.AllCells;

            var aiMoveCells = allCells.Where(x => x.Predictions.AiMovePredictions.Any()).ToList();

            foreach (var aiMoveCell in aiMoveCells)
            {
                var potentialTargetAiMovePredictions = AppSettings.IgnorePrizeAiTanks
                    ? aiMoveCell.Predictions.AiMovePredictions.Where(x => ((BaseTank)x.Item).Health == 1).ToList()
                    : aiMoveCell.Predictions.AiMovePredictions;

                foreach (var aiMovePrediction in potentialTargetAiMovePredictions)
                {
                    var mySameDepthShots = aiMoveCell.Predictions.MyShotPredictions
                        .Where(x => x.Depth == aiMovePrediction.Depth).ToList();

                    foreach (var mySameDepthShot in mySameDepthShots)
                    {
                        var killPrediction = (MyKillPrediction)aiMoveCell.AddPrediction(aiMovePrediction.Depth, PredictionType.MyKill, mySameDepthShot.Commands);
                        killPrediction.MyShot = mySameDepthShot;
                        killPrediction.TargetMove = aiMovePrediction;
                    }
                }
            }
        }

        private static void CalculateMyKillEnemyPredictions()
        {
            var allCells = Field.AllCells;

            var enemyMoveCells = allCells.Where(x => x.Predictions.EnemyMovePredictions.Any()).ToList();

            foreach (var enemyMoveCell in enemyMoveCells)
            {
                var enemyMovePredictions = enemyMoveCell.Predictions.EnemyMovePredictions
                    .Where(x => ((BaseTank)x.Item).IsStuck || x.Depth <= AppSettings.IgnoreEnemyMoveDepthMoreThan).ToList();

                foreach (var enemyMovePrediction in enemyMovePredictions)
                {
                    var mySameDepthShots = enemyMoveCell.Predictions.MyShotPredictions
                        .Where(x => x.Depth == enemyMovePrediction.Depth).ToList();

                    foreach (var mySameDepthShot in mySameDepthShots)
                    {
                        var killPrediction = (MyKillPrediction)enemyMoveCell.AddPrediction(enemyMovePrediction.Depth, PredictionType.MyKill, mySameDepthShot.Commands);
                        killPrediction.MyShot = mySameDepthShot;
                        killPrediction.TargetMove = enemyMovePrediction;
                    }
                }
            }
        }
    }
}
