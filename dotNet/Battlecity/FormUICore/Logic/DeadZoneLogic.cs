using System.Collections.Generic;
using System.Linq;
using API.Components;
using FormUI.Infrastructure;
using FormUICore.FieldObjects;
using FormUICore.Infrastructure;

namespace FormUICore.Logic
{
    public static class DeadZoneLogic
    {
        // ReSharper disable once InconsistentNaming
        private static readonly List<DeadZone> _deadZones = new List<DeadZone>();

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

        public static bool ProcessDeadZone()
        {
            if (State.ThisRound.MyTank == null)
                return false;

            var myTank = State.ThisRound.MyTank;
            var activeDeadZone = _deadZones.FirstOrDefault(x => x.IsInDeadZone(myTank));
            if (activeDeadZone == null)
                return false;

            State.ThisRound.IsInDeadZone = true;
            State.ThisRound.CurrentMoveCommands.Add(activeDeadZone.EscapeDirection);
            State.ThisRound.CurrentMoveCommands.Add(Direction.Act);

            return true;
        }
    }
}
