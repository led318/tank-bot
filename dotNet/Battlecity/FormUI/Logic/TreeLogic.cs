using System.Linq;
using API.Components;
using FormUI.FieldItems;
using FormUI.FieldObjects;
using FormUI.Infrastructure;

namespace FormUI.Logic
{
    public static class TreeLogic
    {
        public static void PopulateBulletsUnderTrees()
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

        public static void PopulateAiTanksUnderTrees()
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
    }
}
