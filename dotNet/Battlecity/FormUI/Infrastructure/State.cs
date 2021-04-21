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
        public static Round ThisRound { get; set; }
        public static Round PrevRound { get; set; }
        public static bool HasPrevRound => PrevRound != null;

        public static bool GameIsRunning => ThisRound.Board.BoardString != PrevRound?.Board?.BoardString;

        public static bool IsMyShotThisRound => ThisRound?.MyTank?.IsShotThisRound ?? false;


        public static void SetCurrentRound(Round currentRound)
        {
            PrevRound = ThisRound;
            ThisRound = currentRound;
        }
    }
}
