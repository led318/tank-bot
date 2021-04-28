using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using API.Components;
using FormUI.FieldObjects;
using FormUICore.FieldItems;
using Point = API.Components.Point;

namespace FormUI.FieldItems
{
    public abstract class BaseMobile : BaseItem
    {
        public abstract int Speed { get; }

        public Direction? _direction;
        public static List<Direction> ValidDirections { get; } = new List<Direction>
        {
            API.Components.Direction.Left, 
            API.Components.Direction.Right, 
            API.Components.Direction.Up, 
            API.Components.Direction.Down
        };

        public bool IsValidDirection(Direction? direction)
        {
            return direction == null || ValidDirections.Contains(direction.Value);
        }

        public Direction? Direction
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
            if (Direction.HasValue)
            {
                AddNote(Direction.ToString()[0], Brushes.Red);
            }
        }

        public List<Point> GetNextPoints(Point point, Direction? direction = null)
        {
            direction ??= Direction;

            var result = new List<Point>();

            if (Speed == 1)
            {
                result.Add(Shift(point, direction));
            }
            else
            {
                var tempPoint = point;

                for (int i = 1; i <= Speed; i++)
                {
                    var tempResult = Shift(tempPoint, direction);
                    result.Add(tempResult);
                    tempPoint = tempResult;
                }
            }

            return result.Where(x => !Field.IsOutOfField(x)).ToList();
        }

        public virtual Point GetNextPositionNotCheckedForCanMove(Direction? direction = null)
        {
            direction ??= Direction;

            var nextPoints = GetNextPoints(Point, direction);

            if (!nextPoints.Any())
                return Point;

            var nextPoint = nextPoints.Last();

            //if (Field.IsOutOfField(nextPoint))
            //    return Point;

            //if (Field.GetCell(nextPoint).CanMove)
            return nextPoint;

            //return Point;
        }

        public static Point Shift(Point point, Direction? direction, int delta = 1)
        {
            switch (direction)
            {
                case API.Components.Direction.Up:
                    return point.ShiftTop(delta);
                case API.Components.Direction.Down:
                    return point.ShiftBottom(delta);
                case API.Components.Direction.Left:
                    return point.ShiftLeft(delta);
                case API.Components.Direction.Right:
                    return point.ShiftRight(delta);
            }

            return point;
        }
    }
}
