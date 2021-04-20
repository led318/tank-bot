using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Components;
using Point = API.Components.Point;

namespace FormUI.FieldItems
{
    public abstract class BaseMobile : BaseItem
    {
        public abstract int Speed { get; }

        public Direction? _direction;
        private static readonly HashSet<Direction> _validDirections = new[] { Direction.Left, Direction.Right, Direction.Up, Direction.Down }.ToHashSet();

        public bool IsValidDirection(Direction? direction)
        {
            return direction == null || _validDirections.Contains(direction.Value);
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
                result.Add(Shift(point));
            }
            else
            {
                var tempPoint = point;

                for (int i = 1; i <= Speed; i++)
                {
                    var tempResult = Shift(tempPoint);
                    result.Add(tempResult);
                    tempPoint = tempResult;
                }
            }

            return result;
        }

        private Point Shift(Point point)
        {
            switch (CurrentDirection)
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
