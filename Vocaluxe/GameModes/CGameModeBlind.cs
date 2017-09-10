using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vocaluxe.GameModes
{
    public class CGameModeBlind : CGameMode
    {
        public override bool IsNotesVisible(int p)
        {
            return false;
        }

        public override bool IsPlayerInformationVisible(int p)
        {
            return false;
        }

        public override bool IsPointsVisible(int p)
        {
            return false;
        }

        public override bool IsRatingBarVisible(int p)
        {
            return false;
        }
    }
}
