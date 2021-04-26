using System.Collections.Generic;
using System.Linq;
using API.Components;
using FormUI.Infrastructure;
using FormUICore.FieldObjects;

namespace FormUICore.Logic
{
    public static class DeadZoneLogic
    {
        // ReSharper disable once InconsistentNaming
        private static readonly List<DeadZone> _deadZones = new List<DeadZone>();

        static DeadZoneLogic()
        {
            _deadZones.Add(new DeadZone(new Point(13, 1), new Point(20, 4)));
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
