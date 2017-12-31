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
using VocaluxeLib.Game;
using VocaluxeLib.Menu;
using VocaluxeLib.Songs;

[assembly: ComVisible(false)]

namespace VocaluxeLib.PartyModes.Classic
{
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
            SongSelection,
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
            public Guid ProfileID;
            public int SingPoints;
            public int GamePoints;
            public int Won;
        }

        public SData GameData;
        private EStage _Stage;

        public CPartyModeClassic(int id) : base(id)
        {
            _ScreenSongOptions.Selection.RandomOnly = false;
            _ScreenSongOptions.Selection.PartyMode = true;
            _ScreenSongOptions.Selection.CategoryChangeAllowed = false;
            _ScreenSongOptions.Selection.NumJokers = new int[] { 5, 5 };
            _ScreenSongOptions.Selection.TeamNames = new string[] { "foo", "bar" };

            _ScreenSongOptions.Sorting.SearchString = String.Empty;
            _ScreenSongOptions.Sorting.SearchActive = false;
            _ScreenSongOptions.Sorting.DuetOptions = EDuetOptions.NoDuets;
            _ScreenSongOptions.Sorting.FilterPlaylistID = -1;

            GameData = new SData
            {
                Teams = new List<List<Guid>>(),
                CurrentRoundNr = 0,
                SongSource = ESongSource.TR_SONGSOURCE_ALLSONGS,
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
            _ScreenSongOptions.Selection.CategoryIndex = -1;

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

            SetDefaults();
            return true;
        }

        public override void UpdateGame()
        {
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
                case EStage.SongSelection:
                    return CBase.Graphics.GetScreen(EScreen.Song);
                case EStage.Singing:
                    return CBase.Graphics.GetScreen(EScreen.Sing);
                default:
                    throw new ArgumentException("Invalid stage: " + _Stage);
            }
        }

        private void _FadeToScreen()
        {
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
                    _Stage = EStage.SongSelection;
                    _PrepareSongSelection();
                    break;
                case EStage.SongSelection:
                    _Stage = EStage.Singing;
                    break;
                case EStage.Singing:
                    _Stage = EStage.Main;
                    _UpdateScores();
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
            return _ScreenSongOptions;
        }

        public override void OnSongChange(int songIndex, ref SScreenSongOptions screenSongOptions)
        {
            if (_ScreenSongOptions.Selection.SelectNextRandomSong && songIndex != -1)
                _ScreenSongOptions.Selection.SelectNextRandomSong = false;

            _ScreenSongOptions.Selection.SongIndex = songIndex;

            screenSongOptions = _ScreenSongOptions;
        }

        public override void OnCategoryChange(int categoryIndex, ref SScreenSongOptions screenSongOptions)
        {
            if (categoryIndex != -1 || CBase.Config.GetTabs() == EOffOn.TR_CONFIG_OFF)
            {
                //If category is selected or tabs off: only random song selection
                _ScreenSongOptions.Selection.SelectNextRandomSong = true;
                _ScreenSongOptions.Selection.RandomOnly = true;
            }
            else
            {
                //If no category is selected: let user choose category
                _ScreenSongOptions.Selection.SongIndex = -1;
                _ScreenSongOptions.Selection.RandomOnly = false;
            }

            _ScreenSongOptions.Selection.CategoryIndex = categoryIndex;

            screenSongOptions = _ScreenSongOptions;
        }

        public override void SetSearchString(string searchString, bool visible)
        {
            _ScreenSongOptions.Sorting.SearchString = searchString;
            _ScreenSongOptions.Sorting.SearchActive = visible;
        }

        public override void JokerUsed(int teamNr)
        {
            if (_ScreenSongOptions.Selection.NumJokers == null)
                return;

            if (teamNr >= _ScreenSongOptions.Selection.NumJokers.Length)
                return;

            if (!GameData.RefillJokers)
            {
                CRound round = GameData.Rounds[GameData.CurrentRoundNr - 1];
                GameData.Jokers[teamNr]--;
            }
            _ScreenSongOptions.Selection.NumJokers[teamNr]--;
            _ScreenSongOptions.Selection.RandomOnly = true;
            _ScreenSongOptions.Selection.CategoryChangeAllowed = false;
        }

        public override void SongSelected(int songID)
        {
            _PrepareRound(new int[] { songID });

            Next();
        }

        public override void LeavingHighscore()
        {
            //Remember sung songs, so they don't will be selected a second time
            for (int i = 0; i < CBase.Game.GetNumSongs(); i++)
                CBase.Songs.AddPartySongSung(CBase.Game.GetSong(i).ID);

            GameData.CurrentRoundNr++;

            Next();
        }

        /// <summary>
        /// Setup options for song selection
        /// </summary>
        private void _PrepareSongSelection()
        {
            _ScreenSongOptions.Selection.RandomOnly = true;
            _ScreenSongOptions.Selection.SelectNextRandomSong = true;

            _ScreenSongOptions.Sorting.IgnoreArticles = CBase.Config.GetIgnoreArticles();

            switch (GameData.SongSource)
            {
                case ESongSource.TR_SONGSOURCE_ALLSONGS:
                    _ScreenSongOptions.Sorting.SongSorting = CBase.Config.GetSongSorting();
                    _ScreenSongOptions.Sorting.Tabs = CBase.Config.GetTabs();
                    _ScreenSongOptions.Sorting.FilterPlaylistID = -1;

                    _ScreenSongOptions.Selection.CategoryIndex = -1;
                    _ScreenSongOptions.Selection.CategoryChangeAllowed = true;
                    break;

                case ESongSource.TR_SONGSOURCE_CATEGORY:
                    _ScreenSongOptions.Sorting.SongSorting = CBase.Config.GetSongSorting();
                    _ScreenSongOptions.Sorting.Tabs = EOffOn.TR_CONFIG_ON;
                    _ScreenSongOptions.Sorting.FilterPlaylistID = -1;

                    _ScreenSongOptions.Selection.CategoryIndex = GameData.CategoryIndex;
                    _ScreenSongOptions.Selection.CategoryChangeAllowed = false;
                    break;

                case ESongSource.TR_SONGSOURCE_PLAYLIST:
                    _ScreenSongOptions.Sorting.SongSorting = CBase.Config.GetSongSorting();
                    _ScreenSongOptions.Sorting.Tabs = EOffOn.TR_CONFIG_OFF;
                    _ScreenSongOptions.Sorting.FilterPlaylistID = GameData.PlaylistID;

                    _ScreenSongOptions.Selection.CategoryChangeAllowed = false;
                    break;
            }

            _SetNumJokers();
            _SetTeamNames();
        }

        /// <summary>
        /// Prepare next game and fill song queue based on configuration and given songs.
        /// </summary>
        /// <param name="songIDs">Array of SongIDs that are selected</param>
        /// <returns>false, if something can't setup correctly</returns>
        private bool _PrepareRound(int[] songIDs)
        {
            //Reset game
            CBase.Game.Reset();
            CBase.Game.ClearSongs();

            #region PlayerNames
            CBase.Game.SetNumPlayer(GameData.NumTeams);
            SPlayer[] players = CBase.Game.GetPlayers();
            if (players == null || players.Length < GameData.NumTeams)
                return false;

            //Get current round
            CRound c = GameData.Rounds[GameData.CurrentRoundNr - 1];

            for (int i = 0; i < GameData.NumTeams; i++)
            {
                //try to fill with correct player data
                if (c != null)
                    players[i].ProfileID = c.Singer[i];
                else
                    players[i].ProfileID = Guid.Empty;
            }
            #endregion PlayerNames

            #region SongQueue
            //Add all songs with configure game mode to song queue
            for (int i = 0; i < songIDs.Length; i++)
                CBase.Game.AddSong(songIDs[i], GameData.SongMode);
            #endregion SongQueue

            return true;

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

        private void _UpdateScores()
        {
            //Prepare results table
            if (GameData.ResultTable.Count == 0)
            {
                for (int i = 0; i < GameData.NumPlayer; i++)
                {
                    var row = new CResultTableRow { PlayerID = GameData.ProfileIDs[i], NumPlayed = 0, NumWon = 0, NumSingPoints = 0, NumGamePoints = 0 };
                    GameData.ResultTable.Add(row);
                }

                GameData.Results = new int[GameData.NumRounds, GameData.NumPlayerAtOnce];
                for (int i = 0; i < GameData.NumRounds; i++)
                {
                    for (int j = 0; j < GameData.NumPlayerAtOnce; j++)
                        GameData.Results[i, j] = 0;
                }
            }

            //Get points from game
            CPoints points = CBase.Game.GetPoints();
            SPlayer[] players = CBase.Game.GetPlayers();

            //Go over all rounds and sum up points
            for (int round = 0; round < points.NumRounds; round++)
            {
                SPlayer[] res = points.GetPlayer(round, GameData.NumPlayerAtOnce);

                if (res == null || res.Length < GameData.NumPlayerAtOnce)
                    return;

                for (int p = 0; p < GameData.NumPlayerAtOnce; p++)
                {
                    players[p].Points += res[p].Points;
                    players[p].PointsGoldenNotes += res[p].PointsGoldenNotes;
                    players[p].PointsLineBonus += res[p].PointsLineBonus;
                }
            }
            //Calculate average points
            for (int p = 0; p < GameData.NumPlayerAtOnce; p++)
            {
                players[p].Points /= points.NumRounds;
                players[p].PointsGoldenNotes /= points.NumRounds;
                players[p].PointsLineBonus /= points.NumRounds;

                //Save points in GameData
                GameData.Results[GameData.CurrentRoundNr - 2, p] = (int)Math.Round(players[p].Points);
            }

            List<SStats> stats = _GetPointsForPlayer(players);

            for (int i = 0; i < GameData.NumPlayerAtOnce; i++)
            {
                //Find matching row in results table
                int index = -1;
                for (int j = 0; j < GameData.ResultTable.Count; j++)
                {
                    if (stats[i].ProfileID == GameData.ResultTable[j].PlayerID)
                    {
                        index = j;
                        break;
                    }
                }

                if (index == -1)
                    continue;
                CResultTableRow row = GameData.ResultTable[index];

                //Update results entry
                row.NumPlayed++;
                row.NumWon += stats[i].Won;
                row.NumSingPoints += stats[i].SingPoints;
                row.NumGamePoints += stats[i].GamePoints;

                GameData.ResultTable[index] = row;
            }

            GameData.ResultTable.Sort();

            //Update position-number
            int pos = 1;
            int lastPoints = 0;
            int lastSingPoints = 0;
            foreach (CResultTableRow resultRow in GameData.ResultTable)
            {
                if (lastPoints > resultRow.NumGamePoints || lastSingPoints > resultRow.NumSingPoints)
                    pos++;
                resultRow.Position = pos;
                lastPoints = resultRow.NumGamePoints;
                lastSingPoints = resultRow.NumSingPoints;
            }
        }

        private List<SStats> _GetPointsForPlayer(SPlayer[] results)
        {
            var result = new List<SStats>();
            for (int i = 0; i < GameData.NumTeams; i++)
            {
                var stat = new SStats { ProfileID = results[i].ProfileID, SingPoints = (int)Math.Round(results[i].Points), Won = 0, GamePoints = 0 };
                result.Add(stat);
            }

            result.Sort((s1, s2) => s1.SingPoints.CompareTo(s2.SingPoints));

            int current = result[result.Count - 1].SingPoints;
            int points = result.Count;
            bool wonset = false;

            for (int i = result.Count - 1; i >= 0; i--)
            {
                SStats res = result[i];

                if (i < result.Count - 1)
                {
                    if (current > res.SingPoints)
                    {
                        res.GamePoints = i * 2;
                        wonset = true;
                        points = res.GamePoints;
                    }
                    else
                    {
                        if (!wonset)
                            res.Won = 1;
                        res.GamePoints = points;
                    }
                }
                else
                {
                    res.GamePoints = i * 2;
                    res.Won = 1;
                }

                current = res.SingPoints;

                result[i] = res;
            }


            return result;
        }
    }
}
