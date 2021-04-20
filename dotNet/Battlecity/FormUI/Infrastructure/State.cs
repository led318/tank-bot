using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormUI.FieldItems;
using FormUI.FieldItems.Tank;
using FormUI.FieldObjects;

namespace FormUI.Infrastructure
{
    public static class State
    {
        public static Round CurrentRound { get; set; }
        public static Round PrevRound { get; set; }
        public static bool HasPrevRound => PrevRound != null;

        

        public static void SetCurrentRound(Round currentRound)
        {
            PrevRound = CurrentRound;
            CurrentRound = currentRound;
        }
    }
}
