using System;
using System.Collections.Generic;
using System.Linq;
using API.Components;
using FormUI.FieldItems;
using FormUI.FieldItems.Tank;
using FormUI.FieldObjects;

namespace FormUI.Infrastructure
{
    public class CalculationLogic
    {
        public void PerformCalculations()
        {
            PopulateHiddenWater();
            PopulateBulletsUnderTrees();
            PopulateAiTanksUnderTrees();

            PopulateShotCountdownsFromPrevRound();
            PerformTick();

            CalculateAiPrizeTanks();
            CalculateAiTanks();
            CalculateEnemyTanks();
            CalculateBullets();
            CalculateMyShotPredictions();
        }

        private void PerformTick()
        {
            foreach (var baseItem in Field.AllItems)
            {
                baseItem.Tick();
            }
        }

        private void PopulateShotCountdownsFromPrevRound()
        {
            if (!State.HasPrevRound)
            {

                return;
            }

            var allTanks = State.ThisRound.AllTanks;
            var allPrevTanks = State.PrevRound.AllTanks;

            foreach (var tank in allTanks)
            {
                var nearPoints = tank.Point.GetNearPoints(includeThis: true).ToList();

                var nearPrevTanks = allPrevTanks.Where(t => nearPoints.Contains(t.Point)).ToList();

                BaseTank prevTank = null;
                if (nearPrevTanks.Count == 1)
                {
                    prevTank = nearPrevTanks.First();
                }
                else
                {
                    var tankType = tank.GetType();

                    var nearPrevSameTypeTanks = nearPrevTanks.Where(t => t.GetType() == tankType).ToList();

                    if (nearPrevSameTypeTanks.Count == 1)
                    {
                        prevTank = nearPrevSameTypeTanks.First();
                    }
                    else
                    {
                        prevTank = nearPrevSameTypeTanks.FirstOrDefault(t =>
                            t.GetNextPoints(t.Point).Contains(tank.Point));
                    }
                }

                if (prevTank != null)
                {
                    tank.SetShotCountdown(prevTank.ShotCountdownLeft);
                }
            }
        }

        private void PopulateHiddenWater()
        {
            if (!State.HasPrevRound)
                return;

            var prevRivers = State.PrevRound.Rivers;
            var thisRivers = State.ThisRound.Rivers;

            foreach (var prevRiver in prevRivers)
            {
                var thisRiver = thisRivers.FirstOrDefault(x => x.Point == prevRiver.Point);
                if (thisRiver == null)
                {
                    Field.GetCell(prevRiver.Point).Items.Add(prevRiver);
                }
            }
        }

        private void PopulateAiTanksUnderTrees()
        {
            if (!State.HasPrevRound)
                return;

            var trees = State.ThisRound.Trees;
            var prevAiTanks = State.PrevRound.AiTanks;

            foreach (var tree in trees)
            {
                var prevHiddenAiTank = prevAiTanks.FirstOrDefault(b => b.GetNextPositionNotCheckedForCanMove() == tree.Point);

                if (prevHiddenAiTank != null)
                {
                    var thisHiddenAiTank = prevHiddenAiTank.DeepClone();
                    thisHiddenAiTank.Point = tree.Point;

                    var cell = Field.GetCell(tree.Point);
                    cell.Items.Insert(0, thisHiddenAiTank);
                    State.ThisRound.AiTanks.Add(thisHiddenAiTank);

                    if (prevHiddenAiTank.IsShotThisRound && prevHiddenAiTank.CurrentDirection.HasValue)
                    {
                        var bulletPoint = BaseMobile.Shift(prevHiddenAiTank.Point, prevHiddenAiTank.CurrentDirection, Bullet.DefaultSpeed);

                        var bullet = new Bullet(Element.BULLET, bulletPoint);
                        bullet.CurrentDirection = prevHiddenAiTank.CurrentDirection;

                        var bulletCell = Field.GetCell(bulletPoint);
                        bulletCell.Items.Insert(0, bullet);
                        State.ThisRound.Bullets.Add(bullet);
                    }
                }
            }
        }

        private void PopulateBulletsUnderTrees()
        {
            if (!State.HasPrevRound)
                return;

            var trees = State.ThisRound.Trees;
            var prevBullets = State.PrevRound.Bullets;

            foreach (var tree in trees)
            {
                var prevHiddenBullet = prevBullets.FirstOrDefault(b => b.GetNextPositionNotCheckedForCanMove() == tree.Point);

                if (prevHiddenBullet != null)
                {
                    var thisHiddenBullet = prevHiddenBullet.DeepClone();
                    thisHiddenBullet.Point = tree.Point;

                    var cell = Field.GetCell(tree.Point);
                    cell.Items.Insert(0, thisHiddenBullet);
                    State.ThisRound.Bullets.Add(thisHiddenBullet);
                }
            }
        }

        private void CalculateAiPrizeTanks()
        {
            var aiPrizeTanks = State.ThisRound.AiPrizeTanks;

            if (!aiPrizeTanks.Any() || !State.HasPrevRound)
                return;

            foreach (var aiPrizeTank in aiPrizeTanks)
            {
                var nearPoints = aiPrizeTank.Point.GetNearPoints();

                var prevRoundNearTanks = State.PrevRound.AiTanks
                    .Where(t => nearPoints.Contains(t.Point)).ToList();

                AiTank foundPrevRoundNearTank = null;

                if (prevRoundNearTanks.Count() == 1)
                {
                    foundPrevRoundNearTank = prevRoundNearTanks.First();
                }
                else
                {
                    foundPrevRoundNearTank = prevRoundNearTanks
                        .FirstOrDefault(p => p.GetNextPoints(p.Point).First() == aiPrizeTank.Point);
                }


                if (foundPrevRoundNearTank != null)
                {
                    aiPrizeTank.CurrentDirection = foundPrevRoundNearTank.CurrentDirection;
                }
            }
        }

        private void CalculateAiTanks()
        {
            var aiTanks = State.ThisRound.AiTanks;
            if (!State.ThisRound.AiPrizeTanks.Any() && State.HasPrevRound)
            {
                foreach (var aiTank in aiTanks)
                {
                    var nearPoints = aiTank.Point.GetNearPoints(includeThis: true);

                    var prevRoundNearPrizeTanks = State.PrevRound.AiPrizeTanks
                        .Where(t => nearPoints.Contains(t.Point)).ToList();

                    AiTank foundPrevRoundNearPrizeTank = null;

                    if (prevRoundNearPrizeTanks.Count() == 1)
                    {
                        foundPrevRoundNearPrizeTank = prevRoundNearPrizeTanks.First();
                    }
                    else
                    {
                        foundPrevRoundNearPrizeTank = prevRoundNearPrizeTanks
                            .FirstOrDefault(p => p.GetNextPoints(p.Point).First() == aiTank.Point);
                    }

                    if (foundPrevRoundNearPrizeTank != null)
                    {
                        State.ThisRound.AddAiPrizeTank(aiTank, foundPrevRoundNearPrizeTank);
                    }
                }
            }


            CalculateMobilePredictions(State.ThisRound.AiTanks, PredictionType.AiTankMove, x => x.CanMove);

            CalculateTanksShotPredictions(State.ThisRound.AiTanks, PredictionType.AiShot, AppSettings.MyShotPredictionDepth);
        }

        private void CalculateEnemyTanks()
        {
            foreach (var enemyTank in State.ThisRound.EnemyTanks)
            {
                CalculateEnemyTankShotPredictions(enemyTank);
            }

            CalculateMobilePredictions(State.ThisRound.EnemyTanks, PredictionType.EnemyTankMove, x => x.CanMove, depthResctriction: AppSettings.EnemyTankMovePredictionDepth);
        }

        private void CalculateBullets()
        {
            var bullets = State.ThisRound.Bullets;

            if (!bullets.Any())
                return;

            foreach (var bullet in bullets)
            {
                var prevRoundNearBullet = CalculateNearestBullet(bullet);
                if (prevRoundNearBullet != null)
                {
                    if (prevRoundNearBullet.CurrentDirection.HasValue)
                    {
                        bullet.CurrentDirection = prevRoundNearBullet.CurrentDirection;
                    }
                    else
                    {
                        bullet.CurrentDirection = CalculateDirection(prevRoundNearBullet.Point, bullet.Point);
                    }

                    //if (prevRoundNearBullet.IsMyBullet)
                    //{
                    //    State.ThisRound.AddMyBullet(bullet);
                    //}

                    continue;
                }

                var currentRoundNearTank = CalculateNearestTank(bullet);
                if (currentRoundNearTank == null)
                {
                    currentRoundNearTank = CalculateNearestTank(bullet, 2);
                }

                if (currentRoundNearTank != null)
                {
                    bullet.CurrentDirection = currentRoundNearTank.CurrentDirection;
                    currentRoundNearTank.Shot();

                    //if (currentRoundNearTank is MyTank)
                    //{
                    //    State.ThisRound.AddMyBullet(bullet);
                    //}

                    continue;
                }
            }



            CalculateMobilePredictions(State.ThisRound.Bullets, PredictionType.Bullet, x => x.CanShootThrough, true);

        }

        private Bullet CalculateNearestBullet(Bullet bullet)
        {
            if (!State.HasPrevRound)
                return null;

            var nearPoints = bullet.Point.GetNearPoints(2);
            var prevRoundNearBullets = State.PrevRound.Bullets
                .Where(t => nearPoints.Contains(t.Point)).ToList();

            Bullet foundPrevRoundNearBullet = null;

            if (prevRoundNearBullets.Count() == 1)
            {
                foundPrevRoundNearBullet = prevRoundNearBullets.First();
            }
            else
            {
                foundPrevRoundNearBullet = prevRoundNearBullets
                    .FirstOrDefault(p => p.GetNextPoints(p.Point).Last() == bullet.Point);
            }

            return foundPrevRoundNearBullet;
        }

        private BaseTank CalculateNearestTank(Bullet bullet, int delta = 1)
        {
            var nearPoints = bullet.Point.GetNearPoints(delta).ToList();


            var currentRoundNearTanks = State.ThisRound.AllTanks
                .Where(t => nearPoints.Contains(t.Point)).ToList();

            BaseTank foundCurrentRoundNearTank = null;

            if (currentRoundNearTanks.Count() == 1)
            {
                foundCurrentRoundNearTank = currentRoundNearTanks.First();
            }
            else
            {
                foundCurrentRoundNearTank = currentRoundNearTanks
                    .FirstOrDefault(p => p.GetNextPoints(p.Point).Last() == bullet.Point);
            }

            return foundCurrentRoundNearTank;
        }

        private Direction CalculateDirection(Point startPoint, Point endPoint)
        {
            var xDiff = startPoint.X - endPoint.X;
            var yDiff = startPoint.Y - endPoint.Y;

            if (xDiff == 0)
            {
                return yDiff > 0 ? Direction.Up : Direction.Down;
            }

            if (yDiff == 0)
            {
                return xDiff > 0 ? Direction.Left : Direction.Right;
            }

            return Direction.Down;
        }

        private void CalculateMobilePredictions(IEnumerable<BaseMobile> mobileItems, PredictionType type, Func<Cell, bool> breakCondition, bool includeFirstObstacle = false, int? depthResctriction = null)
        {
            var maxDepth = depthResctriction ?? AppSettings.PredictionDepth;

            foreach (var mobileItem in mobileItems)
            {
                var predictionStartPoint = mobileItem.Point;
                var canNotMoveFurther = false;

                for (var depth = 1; depth <= maxDepth; depth++)
                {
                    var nextPoints = mobileItem.GetNextPoints(predictionStartPoint);

                    foreach (var nextPoint in nextPoints)
                    {
                        var nextCell = Field.GetCell(nextPoint);

                        if (breakCondition(nextCell))
                        {
                            nextCell.AddPrediction(depth, type);
                            predictionStartPoint = nextPoint;
                        }
                        else
                        {
                            if (includeFirstObstacle)
                            {
                                nextCell.AddPrediction(depth, type);
                            }

                            canNotMoveFurther = true;
                            break;
                        }
                    }

                    if (canNotMoveFurther)
                        break;
                }
            }
        }

        private void CalculateEnemyTankShotPredictions(BaseTank tank)
        {
            if (tank.CurrentDirection.HasValue)
            {
                CalculateTankShotPredictions(tank.Point, PredictionType.EnemyShot, tank.CurrentDirection.Value, tank, AppSettings.EnemyTankShotPredictionDepth);
            }

            var directions = BaseMobile.ValidDirections;
            foreach (var direction in directions)
            {
                var point = tank.Point.Shift(direction);
                var cell = Field.GetCell(point);
                if (cell.CanMove)
                {
                    CalculateTankShotPredictions(point, PredictionType.EnemyShot, direction, tank, AppSettings.EnemyTankShotPredictionDepth);
                }
            }
        }

        private void CalculateMyShotPredictions()
        {
            if (State.ThisRound.MyTank == null)
                return;

            var myTank = State.ThisRound.MyTank;
            if (myTank.CurrentDirection.HasValue)
            {
                var command = new List<Direction> { Direction.Act };
                CalculateTankShotPredictions(myTank.Point, PredictionType.MyShot, myTank.CurrentDirection.Value, myTank, AppSettings.MyShotPredictionDepth, command);
            }

            var directions = BaseMobile.ValidDirections;
            foreach (var direction in directions)
            {
                var point = myTank.Point.Shift(direction);

                var cell = Field.GetCell(point);
                if (cell.CanMove)
                {
                    var command = new List<Direction> { direction, Direction.Act };
                    CalculateTankShotPredictions(point, PredictionType.MyShot, direction, myTank, AppSettings.MyShotPredictionDepth, command);
                }
            }
        }

        private void CalculateTanksShotPredictions(IEnumerable<BaseTank> tanks, PredictionType type, int maxDepth = 1)
        {
            foreach (var tank in tanks)
            {
                if (tank.CurrentDirection.HasValue)
                {
                    CalculateTankShotPredictions(tank.Point, type, tank.CurrentDirection.Value, tank, maxDepth);
                }
            }
        }

        private void CalculateTankShotPredictions(Point point, PredictionType type, Direction direction, BaseTank tank, int maxDepth = 1, List<Direction> command = null)
        {

            var startShotPoint = point;

            var startIndex = Math.Min(-1 - tank.ShotCountdownLeft, 1);

            //if (!tank.IsShotThisRound)
            //    return;


            for (var i = startIndex; i <= Bullet.DefaultSpeed * maxDepth; i++)
            {
                var shotPoint = BaseMobile.Shift(startShotPoint, direction);
                var shotCell = Field.GetCell(shotPoint);

                var depth = (int)Math.Ceiling((decimal)i / 2);

                if (i >= -1)
                {
                    var actualDepth = Math.Max(0, tank.ShotCountdownLeft) + depth;
                    shotCell.AddPrediction(actualDepth, type, command);
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
    }
}
