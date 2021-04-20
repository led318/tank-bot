using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Components;

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
    }
}
