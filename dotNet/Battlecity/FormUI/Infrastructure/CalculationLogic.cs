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
            CalculateAiPrizeTanks();
            CalculateAiTanks();
            CalculateEnemyTanks();
            CalculateBullets();
        }

        private void CalculateAiPrizeTanks()
        {
            var aiPrizeTanks = State.CurrentRound.AiPrizeTanks;

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
            var aiTanks = State.CurrentRound.AiTanks;
            if (!State.CurrentRound.AiPrizeTanks.Any() && State.HasPrevRound)
            {
                foreach (var aiTank in aiTanks)
                {
                    var nearPoints = aiTank.Point.GetNearPoints();

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
                        State.CurrentRound.AddAiPrizeTank(aiTank, foundPrevRoundNearPrizeTank);
                    }
                }
            }


            CalculateMobilePredictions(State.CurrentRound.AiTanks, PredictionType.AiTank, x => x.CanMove);
        }

        private void CalculateEnemyTanks()
        {
            CalculateMobilePredictions(State.CurrentRound.EnemyTanks, PredictionType.EnemyTank, x => x.CanMove, depthResctriction: AppSettings.EnemyTanksPredictionDepth);
        }



        private void CalculateBullets()
        {
            var bullets = State.CurrentRound.Bullets;

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


                    if (prevRoundNearBullet.IsMyBullet)
                    {
                        State.CurrentRound.AddMyBullet(bullet);
                    }

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

                    if (currentRoundNearTank is MyTank)
                    {
                        State.CurrentRound.AddMyBullet(bullet);
                    }

                    continue;
                }
            }



            CalculateMobilePredictions(State.CurrentRound.Bullets, PredictionType.Bullet, x => x.CanShootThrough, true);

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


            var currentRoundNearTanks = State.CurrentRound.AllTanks
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

        private void CalculateMobilePredictions(IEnumerable<BaseMobile> mobileItems, PredictionType type, Func<BaseItem, bool> breakCondition, bool includeFirstObstacle = false, int? depthResctriction = null)
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
                        var nextCell = Field.Cells[nextPoint.X, nextPoint.Y];

                        if (breakCondition(nextCell.Item))
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
    }
}
