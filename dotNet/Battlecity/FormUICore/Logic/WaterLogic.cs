﻿using System.Linq;
using FormUI.FieldObjects;
using FormUI.Infrastructure;
using FormUICore.Infrastructure;

namespace FormUICore.Logic
{
    public static class WaterLogic
    {
        public static void PopulateHiddenWater()
        {
            if (!State.HasPrevRound)
                return;

            var prevRivers = State.PrevRound.Rivers;
            var thisRivers = State.ThisRound.Rivers;

            foreach (var prevRiver in prevRivers)
            {
                var thisRiver = thisRivers.FirstOrDefault(x => x.Point == prevRiver.Point);
                if (thisRiver == null)
                {
                    Field.GetCell(prevRiver.Point).Items.Add(prevRiver);
                    State.ThisRound.Rivers.Add(prevRiver);
                }
            }
        }
    }
}
