using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using API.Components;

namespace FormUICore.FieldObjects
{
    public class CommandsAggregate : List<Command>
    {
        private const string _commandsSeparator = "|";

        public bool IsSingleAct()
        {
            return Count == 1 && this[0].IsSingleActCommand();
        }

        public bool StartWith(List<Direction> directions)
        {
            return directions.Any(x => StartWith(x));
        }

        public bool StartWith(Direction direction)
        {
            if (Count == 0)
                return false;

            var firstCommand = this[0];

            if (firstCommand.Count == 0)
                return false;

            return firstCommand[0] == direction;
        }

        public override string ToString()
        {
            return string.Join(_commandsSeparator, this);
        }
    }
}
