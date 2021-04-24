using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Components;
using FormUI.FieldItems;
using FormUI.FieldItems.Tank;
using FormUI.Infrastructure;
using FormUI.Predictions;

namespace FormUI.Logic
{
    public static class BulletLogic
    {
        public static void CalculateBullets()
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

                    continue;
                }
            }

            PredictionLogic.CalculateMobilePredictions(State.ThisRound.Bullets, PredictionType.Bullet, x => x.CanShootThrough, true);

        }

        private static BaseTank CalculateNearestTank(Bullet bullet, int delta = 1)
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

        private static Bullet CalculateNearestBullet(Bullet bullet)
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

        private static Direction CalculateDirection(Point startPoint, Point endPoint)
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
    }
}
