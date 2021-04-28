using System.Linq;
using API.Components;
using FormUI.FieldObjects;
using FormUICore.Infrastructure;

namespace FormUICore.Logic
{
    public static class DeadZoneLogic
    {
        // ReSharper disable once InconsistentNaming
        //private static readonly List<DeadZone> _deadZones = new List<DeadZone>();

        private static readonly int _deadZoneRoundsThreshold = 5;
        private static int _deadZoneCurrentIndex;

        static DeadZoneLogic()
        {
            //if (AppSettings.IsOldMap)
            //{
            //    _deadZones.Add(new DeadZone(new Point(14, 1), new Point(19, 3)));
            //}
            //else
            //{
            //    _deadZones.Add(new DeadZone(new Point(11, 1), new Point(15, 4)));
            //    _deadZones.Add(new DeadZone(new Point(9, 23), new Point(10, 23)));
            //    _deadZones.Add(new DeadZone(new Point(14, 19), new Point(16, 23)));
            //}
        }

        public static void ResetDeadZoneIndex()
        {
            _deadZoneCurrentIndex = 0;

        }

        public static bool ProcessDeadZone()
        {
            if (State.ThisRound.MyTank == null)
            {
                ResetDeadZoneIndex();
                return false;
            }

            if (State.PrevRound == null)
            {
                ResetDeadZoneIndex();
                return false;
            }

            _deadZoneCurrentIndex++;
            if (_deadZoneCurrentIndex < _deadZoneRoundsThreshold)
            {
                return false;
            }

            var myTank = State.ThisRound.MyTank;

            var leftPoint = myTank.Point.ShiftLeft();
            var leftCell = Field.GetCell(leftPoint);

            if (leftCell.IsBattleWall || leftCell.Predictions.DangerCellPredictions.Any())
                return false;

            State.ThisRound.IsInDeadZone = true;
            State.ThisRound.CurrentMoveCommands.Add(Direction.Act);
            State.ThisRound.CurrentMoveCommands.Add(Direction.Left);

            return true;
        }
    }
}
