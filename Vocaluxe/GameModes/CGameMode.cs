using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vocaluxe.GameModes
{
    public abstract class CGameMode
    {
        public CGameMode()
        {
        }
        public virtual bool IsNotesVisible(int p)
        {
            return true;
        }
        public virtual bool IsPointsVisible(int p)
        {
            return true;
        }
        public virtual bool IsPlayerInformationVisible(int p)
        {
            return true;
        }

        public virtual bool IsRatingBarVisible(int p)
        {
            return true;
        }

        public virtual bool IsPlayerFinished(int p, double points, double pointsGolden, double pointsLineBonus)
        {
            return false;
        }

        #region events / graphics
        public virtual void OnUpdate(float time)
        {
            return;
        }

        public virtual void OnDraw(float time)
        {
            return;
        }
        #endregion
    }
}
