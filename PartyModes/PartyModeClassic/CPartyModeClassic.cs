#region license
// This file is part of Vocaluxe.
// 
// Vocaluxe is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Vocaluxe is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Vocaluxe. If not, see <http://www.gnu.org/licenses/>.
#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using VocaluxeLib.Menu;
using VocaluxeLib.Songs;

[assembly: ComVisible(false)]

namespace VocaluxeLib.PartyModes.Classic
{
    public enum ESongSource
    {
        TR_ALLSONGS,
        TR_CATEGORY,
        TR_PLAYLIST
    }

    public class CResultTableRow : IComparable
    {
        public int Position;
        public int PlayerID;
        public int NumPlayed;
        public int NumWon;
        public int NumSingPoints;
        public int NumGamePoints;

        public int CompareTo(object obj)
        {
            var row = obj as CResultTableRow;
            if (row != null)
            {
                int res = row.NumGamePoints.CompareTo(NumGamePoints);
                if (res == 0)
                {
                    res = row.NumSingPoints.CompareTo(NumSingPoints);
                    if (res == 0)
                        res = row.NumWon.CompareTo(NumWon);
                }
                return res;
            }

            throw new ArgumentException("object is not a ResultTableRow");
        }
    }

    public abstract class CPartyScreenClassic : CMenuParty
    {
        protected new CPartyModeClassic _PartyMode;

        public override void Init()
        {
            base.Init();
            _PartyMode = (CPartyModeClassic)base._PartyMode;
        }
    }

    // ReSharper disable ClassNeverInstantiated.Global
    public sealed class CPartyModeClassic : CPartyMode
    // ReSharper restore ClassNeverInstantiated.Global
    {
        public override int MinMics
        {
            get { return 2; }
        }
        public override int MaxMics
        {
            get { return CBase.Config.GetMaxNumMics(); }
        }
        public override int MinPlayers
        {
            get { return MinTeams * MinPlayersPerTeam; }
        }
        public override int MaxPlayers
        {
            get { return MaxTeams * MaxPlayersPerTeam; }
        }
        public override int MinTeams
        {
            get { return 2; }
        }
        public override int MaxTeams
        {
            get { return 6; }
        }
        public override int MinPlayersPerTeam
        {
            get { return 1; }
        }
        public override int MaxPlayersPerTeam
        {
            get { return 5; }
        }

        private enum EStage
        {
            Config,
            GameModes,
            Names,
            Main,
            Singing
        }

        public struct SData
        {
            public List<List<Guid>> Teams;
            public int NumTeams
            {
                get
                {
                    return Teams != null ? Teams.Count : 0;
                }
            }

            public ESongSource SongSource;
            public int CategoryIndex;
            public int PlaylistID;

            public ESongMode SongMode;

            public List<CRound> Rounds;
            public List<int> Songs;
            public List<CPartyGameMode> GameModes;

            public int CurrentRoundNr;
            public int NumRounds;

            public bool RefillJokers;
            public int[] Jokers;
            public int NumJokers;
        }

        private struct SStats
        {
            public int ProfileID;
            public int SingPoints;
            public int GamePoints;
            public int Won;
        }

        public SData GameData;
        private EStage _Stage;

        public CPartyModeClassic(int id) : base(id)
        {
            _ScreenSongOptions.Selection.RandomOnly = true;
            _ScreenSongOptions.Selection.PartyMode = true;
            _ScreenSongOptions.Selection.CategoryChangeAllowed = false;
            _ScreenSongOptions.Selection.NumJokers = new int[] { 5, 5 };
            _ScreenSongOptions.Selection.TeamNames = new string[] { "foo", "bar" };

            _ScreenSongOptions.Sorting.SearchString = String.Empty;
            _ScreenSongOptions.Sorting.SearchActive = false;
            _ScreenSongOptions.Sorting.DuetOptions = EDuetOptions.NoDuets;

            GameData = new SData
            {
                Teams = new List<List<Guid>>(),
                CurrentRoundNr = 0,
                SongSource = ESongSource.TR_ALLSONGS,
                PlaylistID = 0,
                CategoryIndex = 0,
                SongMode = ESongMode.TR_SONGMODE_NORMAL,
                Rounds = new List<CRound>(),
                Songs = new List<int>(),
                GameModes = new List<CPartyGameMode>(),
                RefillJokers = false,
                Jokers = new int[MaxTeams],
                NumJokers = 3,
                NumRounds = 5
            };
        }

        public override void SetDefaults()
        {
            _Stage = EStage.Config;

            _ScreenSongOptions.Sorting.IgnoreArticles = CBase.Config.GetIgnoreArticles();
            _ScreenSongOptions.Sorting.SongSorting = CBase.Config.GetSongSorting();
            _ScreenSongOptions.Sorting.Tabs = EOffOn.TR_CONFIG_OFF;
            _ScreenSongOptions.Selection.SongIndex = -1;

            if (CBase.Config.GetTabs() == EOffOn.TR_CONFIG_ON && _ScreenSongOptions.Sorting.SongSorting != ESongSorting.TR_CONFIG_NONE)
                _ScreenSongOptions.Sorting.Tabs = EOffOn.TR_CONFIG_ON;

            GameData.Songs.Clear();
            GameData.Rounds.Clear();
            GameData.Teams.Clear();
            GameData.GameModes.Clear();

            //Set default team number to 2
            GameData.Teams.Add(new List<Guid>());
            GameData.Teams.Add(new List<Guid>());
        }

        public override bool Init()
        {
            if (!base.Init())
                return false;
            _Stage = EStage.Config;

            SetDefaults();
            return true;
        }

        public override void UpdateGame()
        {
            /*
            if (CBase.Songs.IsInCategory() || _ScreenSongOptions.Sorting.Tabs == EOffOn.TR_CONFIG_OFF)
                _ScreenSongOptions.Selection.RandomOnly = true;
            else
                _ScreenSongOptions.Selection.RandomOnly = false;*/
        }

        private IMenu _GetNextScreen()
        {
            switch (_Stage)
            {
                case EStage.Config:
                    return _Screens["CPartyScreenClassicConfig"];
                case EStage.GameModes:
                    return _Screens["CPartyScreenClassicGameModes"];
                case EStage.Names:
                    return _Screens["CPartyScreenClassicNames"];
                case EStage.Main:
                    return _Screens["CPartyScreenClassicMain"];
                case EStage.Singing:
                    return CBase.Graphics.GetScreen(EScreen.Song);
                default:
                    throw new ArgumentException("Invalid stage: " + _Stage);
            }
        }

        private void _FadeToScreen()
        {
            if (CBase.Graphics.GetNextScreen() != _GetNextScreen())
                CBase.Graphics.FadeTo(_GetNextScreen());
        }

        public void Next()
        {
            switch (_Stage)
            {
                case EStage.Config:
                    _Stage = EStage.GameModes;
                    break;
                case EStage.GameModes:
                    _Stage = EStage.Names;
                    break;
                case EStage.Names:
                    _Stage = EStage.Main;
                    CBase.Songs.ResetSongSung();
                    GameData.CurrentRoundNr = 1;
                    _CreateRounds();
                    break;
                case EStage.Main:
                    _Stage = EStage.Singing;
                    _StartNextRound();
                    break;
                case EStage.Singing:
                    _Stage = EStage.Main;
                    _SetRoundWinner(GameData.CurrentRoundNr - 1);
                    break;
                default:
                    throw new ArgumentException("Invalid stage: " + _Stage);
            }
            _FadeToScreen();
        }

        public void Back()
        {
            switch (_Stage)
            {
                case EStage.Config:
                    CBase.Graphics.FadeTo(EScreen.Party);
                    return;
                case EStage.GameModes:
                    _Stage = EStage.Config;
                    break;
                case EStage.Names:
                    _Stage = EStage.GameModes;
                    break;
                case EStage.Main:
                    _Stage = EStage.Names;
                    break;
                default: // Rest is not allowed
                    throw new ArgumentException("Invalid stage: " + _Stage);
            }
            _FadeToScreen();
        }

        public override IMenu GetStartScreen()
        {
            return _Screens["CPartyScreenClassicConfig"];
        }

        public override SScreenSongOptions GetScreenSongOptions()
        {
            _ScreenSongOptions.Sorting.SongSorting = CBase.Config.GetSongSorting();
            _ScreenSongOptions.Sorting.Tabs = CBase.Config.GetTabs();
            _ScreenSongOptions.Sorting.IgnoreArticles = CBase.Config.GetIgnoreArticles();

            return _ScreenSongOptions;
        }

        public override void OnSongChange(int songIndex, ref SScreenSongOptions screenSongOptions)
        {
            /* _ScreenSongOptions.Selection.SongIndex = -1;

             if (_ScreenSongOptions.Selection.SelectNextRandomSong && songIndex != -1)
             {
                 _ScreenSongOptions.Selection.SelectNextRandomSong = false;
                 _CreateCatSongIndices();

                 if (GameData.CatSongIndices != null)
                 {
                     if (GameData.CatSongIndices[CBase.Songs.GetCurrentCategoryIndex()] == -1)
                         GameData.CatSongIndices[CBase.Songs.GetCurrentCategoryIndex()] = songIndex;
                 }
             }

             screenSongOptions = _ScreenSongOptions;*/
        }

        // ReSharper disable RedundantAssignment
        public override void OnCategoryChange(int categoryIndex, ref SScreenSongOptions screenSongOptions)
        // ReSharper restore RedundantAssignment
        {
            /*
            if (GameData.CatSongIndices != null && categoryIndex != -1)
            {
                if (GameData.CatSongIndices[categoryIndex] == -1)
                    _ScreenSongOptions.Selection.SelectNextRandomSong = true;
                else
                    _ScreenSongOptions.Selection.SongIndex = GameData.CatSongIndices[categoryIndex];
            }

            if (GameData.CatSongIndices == null && categoryIndex != -1)
                _ScreenSongOptions.Selection.SelectNextRandomSong = true;

            if (categoryIndex == -1)
                _ScreenSongOptions.Selection.RandomOnly = false;

            if (_ScreenSongOptions.Sorting.Tabs == EOffOn.TR_CONFIG_OFF || categoryIndex != -1)
                _ScreenSongOptions.Selection.RandomOnly = true;

            screenSongOptions = _ScreenSongOptions;
            */
        }

        public override void SetSearchString(string searchString, bool visible)
        {
            /*
            _ScreenSongOptions.Sorting.SearchString = searchString;
            _ScreenSongOptions.Sorting.SearchActive = visible;
            */
            throw new ArgumentException("Not required!");
        }

        public override void JokerUsed(int teamNr)
        {
            /*
            if (_ScreenSongOptions.Selection.NumJokers == null)
                return;

            if (teamNr >= _ScreenSongOptions.Selection.NumJokers.Length)
                return;

            if (!GameData.RefillJokers)
            {
                CRound round = GameData.Rounds[GameData.CurrentRoundNr - 1];
                GameData.Jokers[round.Players[teamNr]]--;
            }
            */

            _ScreenSongOptions.Selection.NumJokers[teamNr]--;
            _ScreenSongOptions.Selection.RandomOnly = true;
            _ScreenSongOptions.Selection.CategoryChangeAllowed = false;
        }

        public override void SongSelected(int songID)
        {
            CRound c = GameData.Rounds[GameData.CurrentRoundNr - 1];

            CBase.Game.Reset();
            CBase.Game.ClearSongs();
            CBase.Game.AddSong(songID, GameData.SongMode, c.GameMode.GameMode);
            CBase.Game.SetNumPlayer(GameData.NumTeams);

            SPlayer[] players = CBase.Game.GetPlayers();
            if (players == null)
                return;

            if (players.Length < GameData.NumTeams)
                return;

            c.SongID = songID;
            bool isDuet = CBase.Songs.GetSongByID(songID).IsDuet;

            for (int i = 0; i < GameData.NumTeams; i++)
            {
                //try to fill with the right data
                if (c != null)
                    players[i].ProfileID = c.Singer[i];
                else
                    players[i].ProfileID = Guid.Empty;

                if (isDuet)
                    players[i].VoiceNr = i % 2;
            }

            CBase.Graphics.FadeTo(EScreen.Sing);
        }

        public override void LeavingHighscore()
        {
            CBase.Songs.AddPartySongSung(CBase.Game.GetSong(0).ID);
            GameData.CurrentRoundNr++;
            Next();
        }

        private void _SetRoundWinner(int round)
        {
            SPlayer[] results = CBase.Game.GetPlayers();
            List<int> winner = new List<int>();

            double maxPoints = double.MinValue;
            for (int team = 0; team < GameData.NumTeams; team++)
            {
                if (results[team].Points > maxPoints)
                {
                    winner.Clear();
                    winner.Add(team);
                    maxPoints = results[team].Points;
                }
                else if (results[team].Points == maxPoints)
                {
                    winner.Add(team);
                }
            }

            GameData.Rounds[round - 1].WinnerTeams = winner;
        }

        private void _CreateRounds()
        {
            GameData.Rounds = new List<CRound>();
            List<List<Guid>> teams = new List<List<Guid>>();
            for (int t = 0; t < GameData.Teams.Count; t++)
                teams.Add(new List<Guid>());

            List<CPartyGameMode> gameModes = new List<CPartyGameMode>();
            gameModes.AddRange(GameData.GameModes);

            for (int i = 0; i < GameData.NumRounds; i++)
            {
                CRound r = new CRound();

                //Add next player
                r.Singer = new List<Guid>();
                for (int t = 0; t < GameData.Teams.Count; t++)
                {
                    if (teams[t].Count == 0)
                    {
                        teams[t].AddRange(GameData.Teams[t]);
                        teams[t].Shuffle();
                    }
                    r.Singer.Add(teams[t][0]);
                    teams[t].RemoveAt(0);
                }

                //Select game mode
                if (gameModes.Count == 0)
                {
                    gameModes.AddRange(GameData.GameModes);
                    gameModes.Shuffle();
                }
                r.GameMode = gameModes[0];
                gameModes.RemoveAt(0);

                GameData.Rounds.Add(r);
            }
        }

        private void _StartNextRound()
        {
            _ScreenSongOptions.Selection.RandomOnly = true;
            _ScreenSongOptions.Selection.CategoryChangeAllowed = false;
            _SetNumJokers();
            _SetTeamNames();
        }

        private void _SetNumJokers()
        {
            if (GameData.RefillJokers)
            {
                for (int i = 0; i < GameData.Jokers.Length; i++)
                    GameData.Jokers[i] = GameData.NumJokers;
            }
            else
            {
                if (GameData.Jokers == null)
                    GameData.Jokers = new int[GameData.NumTeams];

                for (int i = 0; i < GameData.NumTeams; i++)
                    GameData.Jokers[i] = GameData.NumJokers;
            }
            _ScreenSongOptions.Selection.NumJokers = GameData.Jokers;
        }

        private void _SetTeamNames()
        {
            CRound c = GameData.Rounds[GameData.CurrentRoundNr - 1];

            for (int i = 0; i < GameData.NumTeams; i++)
                _ScreenSongOptions.Selection.TeamNames[i] = CBase.Profiles.GetPlayerName(c.Singer[i]);
        }

        /*
        public void UpdateSongList()
        {
            if (GameData.Songs.Count > 0)
                return;

            switch (GameData.SongSource)
            {
                case ESongSource.TR_PLAYLIST:
                    for (int i = 0; i < CBase.Playlist.GetSongCount(GameData.PlaylistID); i++)
                    {
                        int id = CBase.Playlist.GetSong(GameData.PlaylistID, i).SongID;
                        if (CBase.Songs.GetSongByID(id).AvailableSongModes.Contains(GameData.SongMode))
                            GameData.Songs.Add(id);
                    }
                    break;

                case ESongSource.TR_ALLSONGS:
                    ReadOnlyCollection<CSong> avSongs = CBase.Songs.GetSongs();
                    GameData.Songs.AddRange(avSongs.Where(song => song.AvailableSongModes.Contains(GameData.SongMode)).Select(song => song.ID));
                    break;

                case ESongSource.TR_CATEGORY:
                    CBase.Songs.SetCategory(GameData.CategoryIndex);
                    avSongs = CBase.Songs.GetVisibleSongs();
                    GameData.Songs.AddRange(avSongs.Where(song => song.AvailableSongModes.Contains(GameData.SongMode)).Select(song => song.ID));

                    CBase.Songs.SetCategory(-1);
                    break;
            }
            GameData.Songs.Shuffle();
        }

        private void _StartRound(int roundNr)
        {
            int numTeams = GameData.Teams.Count;
            CBase.Game.Reset();
            CBase.Game.ClearSongs();

            CBase.Game.SetNumPlayer(numTeams);

            SPlayer[] players = CBase.Game.GetPlayers();
            if (players == null)
                return;

            if (players.Length < numTeams)
                return;

            CRound round = GameData.Rounds[roundNr];
            bool isDuet = CBase.Songs.GetSongByID(round.SongID).IsDuet;

            for (int i = 0; i < numTeams; i++)
            {
                players[i].ProfileID = -1;
                if(isDuet)
                    players[i].VoiceNr = i % 2;
            }

            //try to fill with the right data
            //players[0].ProfileID = GameData.ProfileIDsTeam1[round.SingerTeam1];
            if (isDuet)
                players[0].VoiceNr = 0;

            //players[1].ProfileID = GameData.ProfileIDsTeam2[round.SingerTeam2];
            if (isDuet)
                players[1].VoiceNr = 1;

            EGameMode gm = EGameMode.TR_GAMEMODE_NORMAL;
            CPartyGameMode pgm = GameData.GameModes[0];
            if (!pgm.IsPartyGameMode)
                gm = pgm.GameMode;
            GameData.GameModes.RemoveAt(0);

            CBase.Game.AddSong(round.SongID, GameData.SongMode, gm);
        }*/

        private void _UpdateScores()
        {
            if (!GameData.Rounds[GameData.CurrentRoundNr].Finished)
                GameData.CurrentRoundNr++;

            SPlayer[] results = CBase.Game.GetPlayers();
            if (results == null)
                return;

            if (results.Length < 2)
                return;
            /*
            GameData.Rounds[GameData.CurrentRoundNr].PointsTeam1 = (int)Math.Round(results[0].Points);
            GameData.Rounds[GameData.CurrentRoundNr].PointsTeam2 = (int)Math.Round(results[1].Points);
            GameData.Rounds[GameData.CurrentRoundNr].Finished = true;
            if (GameData.Rounds[GameData.CurrentRoundNr].PointsTeam1 < GameData.Rounds[GameData.FieldNr].PointsTeam2)
                GameData.Rounds[GameData.CurrentRoundNr].Winner = 2;
            else if (GameData.Rounds[GameData.CurrentRoundNr].PointsTeam1 > GameData.Rounds[GameData.FieldNr].PointsTeam2)
                GameData.Rounds[GameData.CurrentRoundNr].Winner = 1;
            else
            {
                GameData.Rounds[GameData.CurrentRoundNr].Finished = false;
                GameData.CurrentRoundNr--;
            }*/
        }
    }
}
