using System.Collections.Generic;
using System.Linq;
using API.Components;
using FormUI.FieldItems;
using FormUI.FieldItems.Tank;
using FormUI.FieldObjects;
using FormUI.Infrastructure;
using FormUICore.FieldItems;
using FormUICore.Infrastructure;

namespace FormUICore.Logic
{
    public static class TreeLogic
    {
        public static void PopulateItemsUnderTrees()
        {
            PopulateBulletsUnderTrees();
            PopulateAiTanksUnderTrees();
            PopulateEnemyTanksUnderTreesJustMovedIn();
            PopulateEnemyTanksUnderTreesStuckOnTheSamePosition();
            PopulateMyTankUnderTrees();
            PopulateMyBulletsUnderTrees();
        }

        private static void PopulateBulletsUnderTrees()
        {
            if (!State.HasPrevRound)
                return;

            var trees = State.ThisRound.Trees;
            var prevBullets = State.PrevRound.Bullets;

            foreach (var tree in trees)
            {
                var prevHiddenBullet =
                    prevBullets.FirstOrDefault(b => b.GetNextPositionNotCheckedForCanMove() == tree.Point);
                if (prevHiddenBullet == null)
                    continue;

                var thisHiddenBullet = prevHiddenBullet.DeepClone();
                thisHiddenBullet.Point = tree.Point;

                var cell = Field.GetCell(tree.Point);
                cell.Items.Insert(0, thisHiddenBullet);
                State.ThisRound.Bullets.Add(thisHiddenBullet);
            }
        }

        private static void PopulateMyBulletsUnderTrees()
        {
            if (!State.HasPrevRound || State.PrevRound.MyTank == null)
                return;

            if (State.PrevRound.MyTank.IsShotThisRound && State.PrevRound.CurrentMoveCommands.Contains(Direction.Act))
            {
                var trees = State.ThisRound.Trees;
                var prevMyTank = State.PrevRound.MyTank;

                var indexOfActCommand = State.PrevRound.CurrentMoveCommands.IndexOf(Direction.Act);

                var prevMyShotStartPoint = prevMyTank.Point;
                var prevMyShotDirection = prevMyTank.Direction;

                if (indexOfActCommand == 1)
                {
                    var moveCommand = State.PrevRound.CurrentMoveCommands[0];
                    prevMyShotStartPoint = prevMyShotStartPoint.Shift(moveCommand);
                    prevMyShotDirection = moveCommand;
                }

                if (prevMyShotDirection.HasValue)
                {
                    var shotTargetPoint = prevMyShotStartPoint.Shift(prevMyShotDirection.Value, Bullet.DefaultSpeed);
                    var shotTargetPointTree = trees.FirstOrDefault(x => x.Point == shotTargetPoint);
                    if (shotTargetPointTree != null)
                    {
                        var thisHiddenBullet = new Bullet(Element.BULLET, shotTargetPointTree.Point);
                        thisHiddenBullet.IsMyBullet = true;
                        thisHiddenBullet.Direction = prevMyShotDirection;

                        var cell = Field.GetCell(shotTargetPointTree.Point);
                        cell.Items.Insert(0, thisHiddenBullet);
                        State.ThisRound.Bullets.Add(thisHiddenBullet);
                    }
                }
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
                var prevHiddenAiTank =
                    prevAiTanks.FirstOrDefault(b => b.GetNextPositionNotCheckedForCanMove() == tree.Point);

                if (prevHiddenAiTank == null)
                    continue;

                var thisHiddenAiTank = prevHiddenAiTank.DeepClone();
                thisHiddenAiTank.Point = tree.Point;

                var cell = Field.GetCell(tree.Point);
                cell.Items.Insert(0, thisHiddenAiTank);
                State.ThisRound.AiTanks.Add(thisHiddenAiTank);

                if (prevHiddenAiTank.IsShotThisRound && prevHiddenAiTank.Direction.HasValue)
                {
                    var bulletPoint = BaseMobile.Shift(prevHiddenAiTank.Point, prevHiddenAiTank.Direction,
                        Bullet.DefaultSpeed);

                    var bullet = new Bullet(Element.BULLET, bulletPoint);
                    bullet.Direction = prevHiddenAiTank.Direction;

                    var bulletCell = Field.GetCell(bulletPoint);
                    bulletCell.Items.Insert(0, bullet);
                    State.ThisRound.Bullets.Add(bullet);
                }
            }
        }

        private static void PopulateEnemyTanksUnderTreesJustMovedIn()
        {
            if (!State.HasPrevRound)
                return;

            var trees = State.ThisRound.Trees;
            var prevEnemyTanks = State.PrevRound.EnemyTanks;
            var thisEnemyTanks = State.ThisRound.EnemyTanks;

            foreach (var prevEnemyTank in prevEnemyTanks)
            {
                var prevCell = Field.GetCell(prevEnemyTank.Point);
                if (prevCell.IsTree)
                    continue;

                if (prevCell.IsBlast)
                    continue;

                var prevEnemyTankNearPointsIncludingCurrent = prevEnemyTank.Point.GetNearPoints(1, true);
                var thisEnemyTank = thisEnemyTanks.FirstOrDefault(x => prevEnemyTankNearPointsIncludingCurrent.Contains(x.Point));
                if (thisEnemyTank != null)
                    continue;

                var prevEnemyTankNearPointsWithoutCurrent = prevEnemyTank.Point.GetNearPoints();

                foreach (var tree in trees)
                {
                    var isTreeWhereLostTankMoved = prevEnemyTankNearPointsWithoutCurrent.Contains(tree.Point);

                    if (!isTreeWhereLostTankMoved)
                        continue;

                    var thisHiddenEnemyTank = prevEnemyTank.DeepClone();
                    thisHiddenEnemyTank.Point = tree.Point;
                    thisHiddenEnemyTank.Direction = prevEnemyTank.Point.CalculateDirectionToPoint(tree.Point);
                    thisHiddenEnemyTank.UpdateElementByDirection();
                    thisHiddenEnemyTank.IsPhantom = true;

                    var cell = Field.GetCell(tree.Point);
                    cell.Items.Insert(0, thisHiddenEnemyTank);
                    State.ThisRound.EnemyTanks.Add(thisHiddenEnemyTank);
                }
            }
        }

        private static void PopulateEnemyTanksUnderTreesStuckOnTheSamePosition()
        {
            if (!State.HasPrevRound)
                return;

            var trees = State.ThisRound.Trees;
            var prevEnemyTanks = State.PrevRound.EnemyTanks;

            foreach (var tree in trees)
            {
                var prevEnemyTank = prevEnemyTanks.FirstOrDefault(b => b.Point == tree.Point);
                if (prevEnemyTank == null)
                    continue;

                var nearPoints = prevEnemyTank.Point.GetNearPoints(1);
                var movedEnemyTankFound = false;

                foreach (var nearPoint in nearPoints)
                {
                    var nearCell = Field.GetCell(nearPoint);
                    if (EnemyTank.IsEnemyTank(nearCell.Item.Element))
                        movedEnemyTankFound = true;
                }

                if (movedEnemyTankFound)
                    continue;

                if (prevEnemyTank.HiddenRoundsInRow > 3)
                    continue;

                var thisHiddenEnemyTank = prevEnemyTank.DeepClone();
                thisHiddenEnemyTank.Point = tree.Point;
                thisHiddenEnemyTank.HiddenRoundsInRow++;
                thisHiddenEnemyTank.IsStuck = true;
                thisHiddenEnemyTank.IsPhantom = true;

                var cell = Field.GetCell(tree.Point);
                cell.Items.Insert(0, thisHiddenEnemyTank);
                State.ThisRound.EnemyTanks.Add(thisHiddenEnemyTank);
            }
        }

        private static void PopulateMyTankUnderTrees()
        {
            if (!State.HasPrevRound || ServerSettings.Settings.ShowMyTankUnderTree || State.ThisRound.MyTank != null)
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

            PopulateMyHiddenTank(prevMyTank, trees, prevDirection);

            if (State.ThisRound.MyTank == null)
            {
                var directionsToCheck = BaseMobile.ValidDirections.Where(x => x != prevDirection).ToList();
                foreach (var direction in directionsToCheck)
                {
                    PopulateMyHiddenTank(prevMyTank, trees, direction);

                    if (State.ThisRound.MyTank != null)
                        break;
                }
            }
        }

        private static void PopulateMyHiddenTank(MyTank prevMyTank, List<Tree> trees, Direction? direction)
        {
            var prevMyTankStepPosition = prevMyTank.GetNextPositionNotCheckedForCanMove(direction);

            foreach (var tree in trees)
            {
                if (prevMyTankStepPosition != tree.Point)
                    continue;

                var thisHiddenMyTank = prevMyTank.DeepClone();
                thisHiddenMyTank.Point = tree.Point;
                thisHiddenMyTank.Direction = direction;
                thisHiddenMyTank.UpdateElementByDirection();

                var cell = Field.GetCell(tree.Point);
                cell.Items.Insert(0, thisHiddenMyTank);
                State.ThisRound.MyTank = thisHiddenMyTank;

                if (State.PrevRound.CurrentMoveCommands.Contains(Direction.Act))
                {
                    var bulletPoint = BaseMobile.Shift(prevMyTank.Point, direction, Bullet.DefaultSpeed);

                    var bullet = new Bullet(Element.BULLET, bulletPoint);
                    bullet.Direction = direction;
                    bullet.IsMyBullet = true;

                    var bulletCell = Field.GetCell(bulletPoint);
                    bulletCell.Items.Insert(0, bullet);
                    State.ThisRound.Bullets.Add(bullet);
                }
            }

        }
    }
}
