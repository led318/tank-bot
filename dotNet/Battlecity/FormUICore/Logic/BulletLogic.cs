using System.Linq;
using API.Components;
using FormUI.FieldItems;
using FormUI.FieldItems.Tank;
using FormUI.Infrastructure;
using FormUI.Predictions;

namespace FormUICore.Logic
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
                    bullet.Direction = prevRoundNearBullet.Direction ?? 
                                              CalculateDirection(prevRoundNearBullet.Point, bullet.Point);

                    continue;
                }

                var currentRoundNearTank = CalculateNearestTank(bullet) ?? CalculateNearestTank(bullet, Bullet.DefaultSpeed);
                if (currentRoundNearTank != null)
                {
                    bullet.Direction = currentRoundNearTank.Direction;
                    currentRoundNearTank.Shot();

                    continue;
                }
            }

            BulletPredictionLogic.CalculateBulletPredictions(State.ThisRound.Bullets);
        }

        private static BaseTank CalculateNearestTank(Bullet bullet, int delta = 1)
        {
            var nearPoints = bullet.Point.GetNearPoints(delta).ToList();
            var currentRoundNearTanks = State.ThisRound.AllTanks.Where(t => nearPoints.Contains(t.Point)).ToList();
            var foundCurrentRoundNearTank = currentRoundNearTanks.Count == 1
                ? currentRoundNearTanks.First()
                : currentRoundNearTanks.FirstOrDefault(p => p.GetNextPoints(p.Point).Last() == bullet.Point);

            return foundCurrentRoundNearTank;
        }

        private static Bullet CalculateNearestBullet(Bullet bullet)
        {
            if (!State.HasPrevRound)
                return null;

            var nearPoints = bullet.Point.GetNearPoints(2);
            var prevRoundNearBullets = State.PrevRound.Bullets.Where(t => nearPoints.Contains(t.Point)).ToList();
            var foundPrevRoundNearBullet = prevRoundNearBullets.Count == 1
                ? prevRoundNearBullets.First()
                : prevRoundNearBullets.FirstOrDefault(p => p.GetNextPoints(p.Point).Last() == bullet.Point);

            return foundPrevRoundNearBullet;
        }

        private static Direction CalculateDirection(Point startPoint, Point endPoint)
        {
            var xDiff = startPoint.X - endPoint.X;
            var yDiff = startPoint.Y - endPoint.Y;

            if (xDiff == 0)
                return yDiff < 0 ? Direction.Up : Direction.Down;

            if (yDiff == 0)
                return xDiff > 0 ? Direction.Left : Direction.Right;

            return Direction.Down;
        }
    }
}
