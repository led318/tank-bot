using System.Linq;
using API.Components;
using FormUI.FieldItems;
using FormUI.FieldObjects;
using FormUI.Infrastructure;

namespace FormUICore.Logic
{
    public static class TreeLogic
    {
        public static void PopulateItemsUnderTrees()
        {
            PopulateBulletsUnderTrees();
            PopulateAiTanksUnderTrees();
            PopulateMyTankUnderTrees();
        }

        private static void PopulateBulletsUnderTrees()
        {
            if (!State.HasPrevRound)
                return;

            var trees = State.ThisRound.Trees;
            var prevBullets = State.PrevRound.Bullets;

            foreach (var tree in trees)
            {
                var prevHiddenBullet = prevBullets.FirstOrDefault(b => b.GetNextPositionNotCheckedForCanMove() == tree.Point);
                if (prevHiddenBullet == null)
                    continue;

                var thisHiddenBullet = prevHiddenBullet.DeepClone();
                thisHiddenBullet.Point = tree.Point;

                var cell = Field.GetCell(tree.Point);
                cell.Items.Insert(0, thisHiddenBullet);
                State.ThisRound.Bullets.Add(thisHiddenBullet);
            }
        }

        private static void PopulateAiTanksUnderTrees()
        {
            if (!State.HasPrevRound)
                return;

            var trees = State.ThisRound.Trees;
            var prevAiTanks = State.PrevRound.AiTanks;

            foreach (var tree in trees)
            {
                var prevHiddenAiTank = prevAiTanks.FirstOrDefault(b => b.GetNextPositionNotCheckedForCanMove() == tree.Point);

                if (prevHiddenAiTank == null)
                    continue;

                var thisHiddenAiTank = prevHiddenAiTank.DeepClone();
                thisHiddenAiTank.Point = tree.Point;

                var cell = Field.GetCell(tree.Point);
                cell.Items.Insert(0, thisHiddenAiTank);
                State.ThisRound.AiTanks.Add(thisHiddenAiTank);

                if (prevHiddenAiTank.IsShotThisRound && prevHiddenAiTank.Direction.HasValue)
                {
                    var bulletPoint = BaseMobile.Shift(prevHiddenAiTank.Point, prevHiddenAiTank.Direction, Bullet.DefaultSpeed);

                    var bullet = new Bullet(Element.BULLET, bulletPoint);
                    bullet.Direction = prevHiddenAiTank.Direction;

                    var bulletCell = Field.GetCell(bulletPoint);
                    bulletCell.Items.Insert(0, bullet);
                    State.ThisRound.Bullets.Add(bullet);
                }
            }
        }

        private static void PopulateMyTankUnderTrees()
        {
            if (!State.HasPrevRound || Settings.Get.ShowMyTankUnderTree || State.ThisRound.MyTank != null)
                return;

            var trees = State.ThisRound.Trees;
            var prevMyTank = State.PrevRound.MyTank;

            if (prevMyTank == null)
                return;

            var isPrevCommandAct = State.PrevRound.CurrentMoveCommands.Count == 1 &&
                                   State.PrevRound.CurrentMoveCommands[0] == Direction.Act;

            var prevDirection = isPrevCommandAct
                ? prevMyTank.Direction
                : State.PrevRound.CurrentMoveCommands.FirstOrDefault(x => BaseMobile.ValidDirections.Contains(x));

            var prevMyTankStepPosition = prevMyTank.GetNextPositionNotCheckedForCanMove(prevDirection);

            foreach (var tree in trees)
            {
                if (prevMyTankStepPosition != tree.Point)
                    continue;

                var thisHiddenMyTank = prevMyTank.DeepClone();
                thisHiddenMyTank.Point = tree.Point;
                thisHiddenMyTank.Direction = prevDirection;
                thisHiddenMyTank.UpdateElementByDirection();

                var cell = Field.GetCell(tree.Point);
                cell.Items.Insert(0, thisHiddenMyTank);
                State.ThisRound.MyTank = thisHiddenMyTank;

                if (State.PrevRound.CurrentMoveCommands.Contains(Direction.Act))
                {
                    var bulletPoint = BaseMobile.Shift(prevMyTank.Point, prevDirection, Bullet.DefaultSpeed);

                    var bullet = new Bullet(Element.BULLET, bulletPoint);
                    bullet.Direction = prevDirection;

                    var bulletCell = Field.GetCell(bulletPoint);
                    bulletCell.Items.Insert(0, bullet);
                    State.ThisRound.Bullets.Add(bullet);
                }
            }
        }
    }
}
