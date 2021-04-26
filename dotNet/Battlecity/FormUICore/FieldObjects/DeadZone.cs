using API.Components;
using FormUI.FieldItems.Tank;

namespace FormUICore.FieldObjects
{
    public class DeadZone
    {
        private readonly Point _start;// = new Point(13, 1);
        private readonly Point _end;// = new Point(20, 4);
        public Direction EscapeDirection { get; }

        public DeadZone(Point start, Point end, Direction escapeDirection = Direction.Left)
        {
            _start = start;
            _end = end;
            EscapeDirection = escapeDirection;
        }

        public bool IsInDeadZone(MyTank myTank)
        {
            var isXInDeadZone = myTank.Point.X >= _start.X && myTank.Point.X <= _end.X;
            var isYInDeadZone = myTank.Point.Y >= _start.Y && myTank.Point.Y <= _end.Y;

            return isXInDeadZone && isYInDeadZone;
        }
    }
}
