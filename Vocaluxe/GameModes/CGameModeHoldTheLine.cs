using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaluxeLib;
using VocaluxeLib.Menu;
using VocaluxeLib.Songs;

namespace Vocaluxe.GameModes
{
    class CGameModeHoldTheLine : CGameMode
    {
        private int _Winner = -1;
        private float _SongLength;
        private List<SRectF> _AvatarPositions;
        private bool[] _Lost;

        public override bool IsNotesVisible(int p)
        {
            return !_Lost[p];
        }

        public override bool IsPointsVisible(int p)
        {
            return !_Lost[p];
        }

        public override bool IsRatingBarVisible(int p)
        {
            return !_Lost[p];
        }

        public override bool IsPlayerFinished(int p, double points, double pointsGolden, double pointsLineBonus)
        {
            if (_Winner > -1 && _Winner == p)
                return true;
            return false;
        }

        public override void OnInit(float songLenght, List<SRectF> avatarPositions)
        {
            base.OnInit(songLenght, avatarPositions);

            _SongLength = songLenght;
            _AvatarPositions = avatarPositions;
            _Lost = new bool[CBase.Game.GetNumPlayer()];
        }

        public override void OnUpdate(float time)
        {
            base.OnUpdate(time);
            if (_Winner > -1)
                return;

            var players = CBase.Game.GetPlayers();
            for (int i = 0; i < CBase.Game.GetNumPlayer(); i++)
            {
                if (players[i].Rating < _GetLossRating(players[i], time))
                {
                    _Lost[i] = true;
                    _Winner = _CheckWinner();
                }
            }
        }

        public override void OnDraw(float time)
        {
            base.OnDraw(time);

            for(int p = 0; p < CBase.Game.GetNumPlayer(); p++)
            {
                if(_Lost[p])
                {
                    var red = new SColorF(0.743f, 0, 0, 1f);
                    CBase.Drawing.DrawTexture(CBase.Themes.GetSkinTexture("Out", -1), _AvatarPositions[p], red);
                }
            }
        }

        private int _CheckWinner()
        {
            int plnum = 0; 
            int winner = 0;
            for (int p = 0; p < CBase.Game.GetNumPlayer(); p++)
            {
                if (!_Lost[p]) { 
                    plnum++;
                    winner = p;
                }
            }
            
            if (plnum == 1)
                return winner;
            return -1;
        }

        /// <summary>
        /// Calculate loss rating based on song progress and player difficulty
        /// </summary>
        /// <param name="player">player</param>
        /// <param name="currentTime">current time of song</param>
        /// <returns></returns>
        private float _GetLossRating(SPlayer player, float currentTime)
        {
            float rating = 0f;
            float progress = currentTime / _SongLength;
            switch(CBase.Profiles.GetDifficulty(player.ProfileID))
            {
                case EGameDifficulty.TR_CONFIG_EASY:
                    rating = 0.70f * progress;
                    break;

                case EGameDifficulty.TR_CONFIG_NORMAL:
                    rating = 0.75f * progress;
                    break;

                case EGameDifficulty.TR_CONFIG_HARD:
                    rating = 0.80f * progress;
                    break;
            }

            return rating;
        }
    }
}
