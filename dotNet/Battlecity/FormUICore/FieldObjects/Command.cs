﻿using System.Collections.Generic;
using API.Components;

namespace FormUICore.FieldObjects
{
    public class Command : List<Direction>
    {
        private const string _separator = ",";

        public Command(params Direction[] directions)
        {
            AddRange(directions);
        }

        public void AddDirection(Direction direction)
        {
            Add(direction);
        }

        public bool IsSingleActCommand()
        {
            return Count == 1 && this[0] == Direction.Act;
        }

        public bool IsActCommand()
        {
            return this.Contains(Direction.Act);
        }

        public override string ToString()
        {
            return string.Join(_separator, this);
        }
    }
}
