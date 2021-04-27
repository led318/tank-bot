using System.Collections.Generic;
using System.Linq;
using API.Components;
using FormUI.FieldItems;
using FormUI.FieldItems.Tank;
using FormUI.FieldObjects;
using FormUI.Infrastructure;

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
                    //prevRoundNearBullet.Direction ?? 
                    bullet.Direction = prevRoundNearBullet.Point.CalculateDirectionToPoint(bullet.Point);

                    continue;
                }

                var currentRoundNearTank = CalculateNearestTank(bullet) ?? CalculateNearestTank(bullet, Bullet.DefaultSpeed);
                if (currentRoundNearTank != null)
                {
                    bullet.Direction = currentRoundNearTank.Direction;
                    currentRoundNearTank.Shot();

                    continue;
                }

                var currentRoundNearTrees = CalculateNearestTrees(bullet, Bullet.DefaultSpeed);
                if (currentRoundNearTrees.Any())
                {
                    foreach (var currentRoundNearTree in currentRoundNearTrees)
                    {
                        var direction = currentRoundNearTree.Point.CalculateDirectionToPoint(bullet.Point);

                        bullet.Direction = direction;

                        var enemyTank = new EnemyTank(Element.OTHER_TANK_DOWN, currentRoundNearTree.Point);
                        enemyTank.Direction = direction;
                        enemyTank.UpdateElementByDirection();
                        State.ThisRound.EnemyTanks.Add(enemyTank);
                        var cell = Field.GetCell(currentRoundNearTree.Point);
                        cell.Items.Insert(0, enemyTank);
                    }
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

        private static List<Tree> CalculateNearestTrees(Bullet bullet, int delta = 1)
        {
            var nearPoints = bullet.Point.GetNearPoints(delta).ToList();
            var currentRoundNearTrees = State.ThisRound.Trees.Where(t => nearPoints.Contains(t.Point)).ToList();

            return currentRoundNearTrees;
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
    }
}
