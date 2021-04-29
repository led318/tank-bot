using System;
using System.Collections.Generic;
using System.Linq;
using API.Components;
using FormUI.FieldItems;
using FormUI.FieldItems.Tank;
using FormUI.FieldObjects;
using FormUI.Predictions;
using FormUICore.FieldItems;
using FormUICore.FieldObjects;
using FormUICore.Infrastructure;
using FormUICore.Predictions;

namespace FormUICore.Logic
{
    public static class MyTankPredictionLogic
    {

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
                    //State.ThisRound.MyTank.Shot();
                }
            }
        }

        private static void CalculateMyMovePredictions()
        {
            if (State.ThisRound.MyTank == null)
                return;

            var myTank = State.ThisRound.MyTank;


            CalculateMyShotPredictions(myTank.Point, myTank.Direction.Value, new List<Command>(), true);

            CalculateMyMoveDepth(myTank.Point, 1, new List<Command>());

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

        private static bool CalculateMyMoveDepth(Point startPoint, int depth, List<Command> commands)
        {
            if (depth > AppSettings.MyMovePredictionDepth)
                return false;

            var isNextDepthAdded = false;

            foreach (var direction in BaseMobile.ValidDirections)
            {
                var point = startPoint.Shift(direction);
                var cell = Field.GetCell(point);

                if (IsSafeMoveNotVisitedCell(cell, depth))
                {
                    var currentCommands = commands.ToList();
                    currentCommands.Add(new Command(direction));

                    var nextDepthPrediction = (MyMovePrediction)cell.AddPrediction(depth, PredictionType.MyMove, currentCommands);
                    Field.AddMyMoveDepthPredictions(depth, nextDepthPrediction);

                    isNextDepthAdded = true;


                    CalculateMyShotPredictions(cell.Point, direction, currentCommands);
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

            var hasDepthAiTanks = cell.HasDepthPrediction(depth, x => x.AiMovePredictions);
            if (hasDepthAiTanks)
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

            //if (cell.IsIce) //todo: maybe remove
            //     return false;

            return true;
        }

        private static void CalculateMyShotPredictions(Point currentPoint, Direction lastDirection, List<Command> commands, bool correctFirstShotDepth = false)
        {
            var currentCommands = commands.ToList();
            var command = new List<Direction>();

            CalculateMyShotsForDirection(currentPoint, lastDirection, currentCommands, command, correctFirstShotDepth);

            foreach (var direction in BaseMobile.ValidDirections)
            {
                var movePoint = currentPoint.Shift(direction);

                var directionCommand = new List<Direction> { direction };

                var cell = Field.GetCell(movePoint);
                if (cell.CanMove)
                {
                    var directionCommands = commands.ToList();
                    CalculateMyShotsForDirection(movePoint, direction, directionCommands, directionCommand);
                }
                else
                {
                    var directionCommands = commands.ToList();
                    CalculateMyShotsForDirection(currentPoint, direction, directionCommands, directionCommand);
                }
            }
        }

        private static void CalculateMyShotsForDirection(Point point, Direction direction, List<Command> commands, List<Direction> command, bool correctFirstShotDepth = false)
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

                var directionActCommands = commands.ToList();
                directionActCommands.Add(new Command(directionActCommand.ToArray()));

                var commandRoundsCount = directionActCommands.Count;
                var shotDepth = depth;

                //var depthShotDelay = commandRoundsCount + shotDepth;

                //var shotCountDownDifference = State.ThisRound.MyTank.ShotCountdownLeft - depthShotDelay;

                var notShotRoundsCount = directionActCommands.Count(x => !x.IsActCommand());
                var shotCountDownLeft = Math.Max(State.ThisRound.MyTank.ShotCountdownLeft - notShotRoundsCount, 0);
                //var shotCountDownLeft = 0;

                var actualDepth = commandRoundsCount + shotDepth + shotCountDownLeft;

                //var currentDirectionStr = directionActCommand.CommandsToString();

                //var shotCellPredictions = shotCell.Predictions.MyShotPredictions
                //   .Where(x => x.Depth == actualDepth && x.Commands.AreSameCommands(directionActCommands))
                //    .ToList();

                //if (!shotCellPredictions.Any())
                shotCell.AddPrediction(actualDepth, PredictionType.MyShot, directionActCommands);

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
                        var killPrediction = (MyKillPrediction)aiMoveCell.AddPrediction(aiMovePrediction.Depth, PredictionType.MyKill, mySameDepthShot.Commands, aiMovePrediction.Item);
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
                    .Where(x => CheckIfEnemyShouldBeTargeted(x)).ToList();

                foreach (var enemyMovePrediction in enemyMovePredictions)
                {
                    var mySameDepthShots = enemyMoveCell.Predictions.MyShotPredictions
                        .Where(x => x.Depth == enemyMovePrediction.Depth).ToList();

                    foreach (var mySameDepthShot in mySameDepthShots)
                    {
                        var killPrediction = (MyKillPrediction)enemyMoveCell.AddPrediction(enemyMovePrediction.Depth, PredictionType.MyKill, mySameDepthShot.Commands, enemyMovePrediction.Item);
                        killPrediction.MyShot = mySameDepthShot;
                        killPrediction.TargetMove = enemyMovePrediction;
                    }
                }
            }
        }

        private static bool CheckIfEnemyShouldBeTargeted(EnemyMovePrediction prediction)
        {
            var enemyTank = (EnemyTank)prediction.Item;

            if (enemyTank.IsPhantom)
                return false;

            if (enemyTank.IsStuck)
                return true;

            if (prediction.Depth <= AppSettings.IgnoreEnemyMoveDepthMoreThan)
                return true;

            return false;
        }
    }
}
