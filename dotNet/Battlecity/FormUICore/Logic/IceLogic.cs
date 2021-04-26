using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FormUI.FieldObjects;
using FormUI.Infrastructure;

namespace FormUICore.Logic
{
    public static class IceLogic
    {
        public static void PopulateHiddenIce()
        {
            if (!State.HasPrevRound)
                return;

            var prevIceItems = State.PrevRound.Ice;
            var thisIceItems = State.ThisRound.Ice;

            foreach (var prevIceItem in prevIceItems)
            {
                var thisIceItem = thisIceItems.FirstOrDefault(x => x.Point == prevIceItem.Point);
                if (thisIceItem == null)
                {
                    Field.GetCell(prevIceItem.Point).Items.Add(prevIceItem);
                }
            }
        }
    }
}
