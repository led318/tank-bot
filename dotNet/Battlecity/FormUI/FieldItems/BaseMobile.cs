using System.Collections.Generic;
using System.Drawing;
using API.Components;
using Point = API.Components.Point;

namespace FormUI.FieldItems
{
    public abstract class BaseMobile : BaseItem
    {
        public abstract int Speed { get; }

        public Direction? _direction;
        public static List<Direction> ValidDirections { get; } = new() { Direction.Left, Direction.Right, Direction.Up, Direction.Down };

        public bool IsValidDirection(Direction? direction)
        {
            return direction == null || ValidDirections.Contains(direction.Value);
        }

        public Direction? CurrentDirection
        {
            get => _direction;
            set
            {
                if (!IsValidDirection(value))
                {
                    _direction = null; //todo: throw error?
                }

                _direction = value;
            }
        }

        protected BaseMobile(Element element, Point point) : base(element, point)
        {
            SetDirection();
            //SetDirectionNote();
        }

        protected abstract void SetDirection();

        protected void SetDirectionNote()
        {
            if (CurrentDirection.HasValue)
            {
                AddNote(CurrentDirection.ToString()[0], Brushes.Red);
            }
        }

        public List<Point> GetNextPoints(Point point)
        {
            var result = new List<Point>();

            if (Speed == 1)
            {
                result.Add(Shift(point, CurrentDirection));
            }
            else
            {
                var tempPoint = point;

                for (int i = 1; i <= Speed; i++)
                {
                    var tempResult = Shift(tempPoint, CurrentDirection);
                    result.Add(tempResult);
                    tempPoint = tempResult;
                }
            }

            return result;
        }

        public static Point Shift(Point point, Direction? direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return point.ShiftTop();
                case Direction.Down:
                    return point.ShiftBottom();
                case Direction.Left:
                    return point.ShiftLeft();
                case Direction.Right:
                    return point.ShiftRight();
            }

            return point;
        }
    }
}
