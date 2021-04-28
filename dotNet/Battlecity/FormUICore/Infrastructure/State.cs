using API;
using FormUI.FieldObjects;

namespace FormUICore.Infrastructure
{
    public static class State
    {
        public static Round ThisRound { get; set; }
        public static Round PrevRound { get; set; }
        public static bool HasPrevRound => PrevRound != null;

        public static bool GameIsRunning => ThisRound.Board.BoardString != PrevRound?.Board?.BoardString;

        public static bool IsMyShotThisRound => ThisRound?.MyTank?.IsShotThisRound ?? false;

        public static void SetThisRound(Board board)
        {
            PrevRound = ThisRound;
            ThisRound = new Round(board);
        }
    }
}
