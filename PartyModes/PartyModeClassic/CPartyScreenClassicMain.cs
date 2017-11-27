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
using System.Windows.Forms;
using VocaluxeLib.Menu;

namespace VocaluxeLib.PartyModes.Classic
{
    class CRoundsTableRow
    {
        public CText Number;
        public CText Game;
        public CText Winner;
    }

    public class CPartyScreenClassicMain : CPartyScreenClassic
    {
        // Version number for theme files. Increment it, if you've changed something on the theme files!
        protected override int _ScreenVersion
        {
            get { return 1; }
        }

        private string _TextNextRound = "TextNextRound";
        private string _TextNextRoundDesc = "TextNextRoundDesc";
        private string _TextRoundNumber = "TextRoundNumber";
        private string _TextRoundGame = "TextRoundGame";
        private string _TextRoundWinner = "TextRoundWinner";
        private string[] _TextNextPlayer;
        private string[] _TextTeamName;
        private string[] _TextTeamPlayer;
        private string[] _TextTeamPoints;

        private const string _ButtonNextRound = "ButtonNextRound";
        private const string _ButtonBack = "ButtonBack";
        private const string _ButtonExit = "ButtonExit";
        private const string _ButtonPopupYes = "ButtonPopupYes";
        private const string _ButtonPopupNo = "ButtonPopupNo";
        private const string _ButtonRoundsScrollUp = "ButtonRoundsScrollUp";
        private const string _ButtonRoundsScrollDown = "ButtonRoundsScrollDown";

        private const string _StaticRoundsTable = "StaticRoundsTable";
        private const string _StaticPopupBG = "StaticPopupBG";
        private string[] _StaticNextPlayer;
        private string[] _StaticTeams;

        private bool _ExitPopupVisible;

        private List<CRoundsTableRow> _RoundsTable;

        private SRectF _RoundsTableScrollArea;
        private int _RoundsTableOffset;
        private const int _NumPlayerVisible = 10;
        private int _NumRoundsVisible = 3;

        public override void Init()
        {
            base.Init();

            _StaticNextPlayer = new string[_PartyMode.MaxTeams];
            _StaticTeams = new string[_PartyMode.MaxTeams];
            _TextNextPlayer = new string[_PartyMode.MaxTeams];
            _TextTeamName = new string[_PartyMode.MaxTeams];
            _TextTeamPlayer = new string[_PartyMode.MaxTeams];
            _TextTeamPoints = new string[_PartyMode.MaxTeams];
            for (int i = 1; i <= _PartyMode.MaxTeams; i++)
            {
                _StaticNextPlayer[i - 1] = "StaticNextPlayer" + i;
                _StaticTeams[i - 1] = "StaticTeam" + i;
                _TextNextPlayer[i - 1] = "TextNextPlayer" + i;
                _TextTeamName[i - 1] = "TextTeamName" + i;
                _TextTeamPlayer[i - 1] = "TextTeamPlayer" + i;
                _TextTeamPoints[i - 1] = "TextTeamPoints" + i;
            }

            List<string> elements = new List<string>();

            elements.AddRange(_TextNextPlayer);
            elements.Add(_TextNextRound);
            elements.Add(_TextNextRoundDesc);
            elements.AddRange(_TextTeamName);
            elements.AddRange(_TextTeamPlayer);
            elements.AddRange(_TextTeamPoints);
            elements.Add(_TextRoundGame);
            elements.Add(_TextRoundNumber);
            elements.Add(_TextRoundWinner);
            _ThemeTexts = elements.ToArray();            

            _ThemeButtons = new string[]
                {
                    _ButtonNextRound, _ButtonBack, _ButtonExit, _ButtonPopupYes, _ButtonPopupNo, _ButtonRoundsScrollDown, _ButtonRoundsScrollUp
                };

            elements.Clear();
            elements.Add(_StaticPopupBG);
            elements.Add(_StaticRoundsTable);
            elements.AddRange(_StaticNextPlayer);
            elements.AddRange(_StaticTeams);
            _ThemeStatics = elements.ToArray();
        }

        public override void LoadTheme(string xmlPath)
        {
            base.LoadTheme(xmlPath);

            _CreateRoundsTable();
        }

        public override bool HandleInput(SKeyEvent keyEvent)
        {
            base.HandleInput(keyEvent);

            if (keyEvent.KeyPressed) {}
            else
            {
                switch (keyEvent.Key)
                {
                    case Keys.Back:
                    case Keys.Escape:
                        if (!_ExitPopupVisible)
                        {
                            if (_PartyMode.GameData.CurrentRoundNr == 1)
                                _PartyMode.Back();
                            else
                                _ShowPopup(true);
                        }
                        else
                            _ShowPopup(false);
                        break;

                    case Keys.Enter:
                        if (!_ExitPopupVisible)
                        {
                            if (_Buttons[_ButtonNextRound].Selected)
                                _PartyMode.Next();
                            if (_Buttons[_ButtonBack].Selected && _PartyMode.GameData.CurrentRoundNr == 1)
                                _PartyMode.Back();
                            if (_Buttons[_ButtonExit].Selected && _PartyMode.GameData.CurrentRoundNr > 1)
                                _ShowPopup(true);
                            if (_Buttons[_ButtonRoundsScrollUp].Selected)
                                _ScrollRoundsTable(-1);
                            if (_Buttons[_ButtonRoundsScrollDown].Selected)
                                _ScrollRoundsTable(1);
                        }
                        else
                        {
                            if (_Buttons[_ButtonPopupYes].Selected)
                                _EndParty();
                            if (_Buttons[_ButtonPopupNo].Selected)
                                _ShowPopup(false);
                        }
                        break;
                }
            }
            return true;
        }

        public override bool HandleMouse(SMouseEvent mouseEvent)
        {
            base.HandleMouse(mouseEvent);

            if (mouseEvent.LB && _IsMouseOverCurSelection(mouseEvent))
            {
                if (!_ExitPopupVisible)
                {
                    if (_Buttons[_ButtonNextRound].Selected)
                        _PartyMode.Next();
                    if (_Buttons[_ButtonBack].Selected && _PartyMode.GameData.CurrentRoundNr == 1)
                        _PartyMode.Back();
                    if (_Buttons[_ButtonExit].Selected && _PartyMode.GameData.CurrentRoundNr > 1)
                        _ShowPopup(true);
                    if (_Buttons[_ButtonRoundsScrollUp].Selected)
                        _ScrollRoundsTable(-1);
                    if (_Buttons[_ButtonRoundsScrollDown].Selected)
                        _ScrollRoundsTable(1);
                }
                else
                {
                    if (_Buttons[_ButtonPopupYes].Selected)
                        _EndParty();
                    if (_Buttons[_ButtonPopupNo].Selected)
                        _ShowPopup(false);
                }
            }

            if (mouseEvent.RB)
            {
                if (!_ExitPopupVisible)
                {
                    if (_PartyMode.GameData.CurrentRoundNr == 1)
                        _PartyMode.Back();
                    else
                        _ShowPopup(true);
                }
                else
                    _ShowPopup(false);
            }

            if (mouseEvent.Wheel != 0)
            {
                if (CHelper.IsInBounds(_RoundsTableScrollArea, mouseEvent))
                    _ScrollRoundsTable(mouseEvent.Wheel);
            }

            return true;
        }

        public override void OnShow()
        {
            base.OnShow();

            _UpdateTexts();
            _UpdateVisibilites();

            _RoundsTableOffset = 0;

            if (_PartyMode.GameData.CurrentRoundNr == 1)
                _BuildRoundsTable();
            else
                _ScrollRoundsTable(_PartyMode.GameData.CurrentRoundNr - 2);
            _UpdateRoundsTable();

            if (_PartyMode.GameData.CurrentRoundNr == 1)
            {
                _Buttons[_ButtonBack].Visible = true;
                _Buttons[_ButtonExit].Visible = false;
            }
            else
            {
                _Buttons[_ButtonBack].Visible = false;
                _Buttons[_ButtonExit].Visible = true;
            }

            if (_PartyMode.GameData.CurrentRoundNr <= _PartyMode.GameData.Rounds.Count)
            {
                _Buttons[_ButtonNextRound].Visible = true;
                _SelectElement(_Buttons[_ButtonNextRound]);
            }
            else
            {
                _Buttons[_ButtonNextRound].Visible = false;
                _SelectElement(_Buttons[_ButtonExit]);
            }

            _ShowPopup(false);
        }

        public override bool UpdateGame()
        {
            return true;
        }

        private void _EndParty()
        {
            CBase.Graphics.FadeTo(EScreen.Party);
        }

        private void _ShowPopup(bool visible)
        {
            _ExitPopupVisible = visible;

            _Statics[_StaticPopupBG].Visible = _ExitPopupVisible;
            _Buttons[_ButtonPopupYes].Visible = _ExitPopupVisible;
            _Buttons[_ButtonPopupNo].Visible = _ExitPopupVisible;

            if (_ExitPopupVisible)
                _SelectElement(_Buttons[_ButtonPopupNo]);
        }

        private void _UpdateTexts()
        {
            int currentRoundNr = _PartyMode.GameData.CurrentRoundNr;
            CRound currentRound = _PartyMode.GameData.Rounds[currentRoundNr - 1];

            for (int i = 0; i < _PartyMode.GameData.Teams.Count; i++)
            {
                //Build team player strings
                _Texts[_TextTeamPlayer[i]].Text = "";
                for (int player = 0; player < _PartyMode.GameData.Teams[i].Count; player++)
                {
                    _Texts[_TextTeamPlayer[i]].Text += CBase.Profiles.GetPlayerName(_PartyMode.GameData.Teams[i][player]);
                    if (player + 1 < _PartyMode.GameData.Teams[i].Count)
                        _Texts[_TextTeamPlayer[i]].Text += ", ";
                }

                //Update next player
                Guid profileId = currentRound.Singer[i];
                _Texts[_TextNextPlayer[i]].Text = CBase.Profiles.GetPlayerName(profileId);

                //Update team points
                _Texts[_TextTeamPoints[i]].Text = 0+"";
            }

            //Update game mode texts
            CPartyGameMode currentMode = currentRound.GameMode;
            _Texts[_TextNextRound].Text = CBase.Language.Translate("TR_SCREENMAIN_ROUND", _PartyMode.ID) + " " + currentRoundNr + ": " + CBase.Language.Translate(currentMode.TranslationName, _PartyMode.ID);
            _Texts[_TextNextRoundDesc].Text = CBase.Language.Translate("TR_SCREENMAIN_NEXT_PLAYER", _PartyMode.ID) + " " + CBase.Language.Translate(currentMode.TranslationDescription, _PartyMode.ID);
        }

        private void _UpdateVisibilites()
        {
            bool finished = _PartyMode.GameData.CurrentRoundNr > _PartyMode.GameData.NumRounds;
            for (int i = 0; i < _PartyMode.MaxTeams; i++)
            {
                _Statics[_StaticNextPlayer[i]].Visible = i < _PartyMode.GameData.NumTeams && !finished;
                _Statics[_StaticTeams[i]].Visible = i < _PartyMode.GameData.NumTeams && !finished;
                _Texts[_TextNextPlayer[i]].Visible = i < _PartyMode.GameData.NumTeams && !finished;
                _Texts[_TextTeamName[i]].Visible = i < _PartyMode.GameData.NumTeams && !finished;
                _Texts[_TextTeamPlayer[i]].Visible = i < _PartyMode.GameData.NumTeams && !finished;
                _Texts[_TextTeamPoints[i]].Visible = i < _PartyMode.GameData.NumTeams && !finished;
            }
        }

        private void _CreateRoundsTable()
        {
            //Create lists
            _RoundsTable = new List<CRoundsTableRow>();
            _RoundsTableScrollArea = _Statics[_StaticRoundsTable].Rect;

            float entryHeight = _Texts[_TextRoundNumber].H + 3f;

            for (int i = 0; i < Math.Floor(_RoundsTableScrollArea.H / entryHeight); i++)
            {
                var rtr = new CRoundsTableRow();
                _RoundsTable.Add(rtr);
            }
            //Create statics and texts for rounds
            foreach (CRoundsTableRow roundRow in _RoundsTable)
            {
                //Round-number
                CText text = GetNewText(_Texts[_TextRoundNumber]);
                _AddText(text);
                roundRow.Number = text;

                //Round-game
                text = GetNewText(_Texts[_TextRoundGame]);
                _AddText(text);
                roundRow.Game = text;

                //Round-winner
                text = GetNewText(_Texts[_TextRoundWinner]);
                _AddText(text);
                roundRow.Winner = text;
            }

            _Texts[_TextRoundNumber].Visible = false;
            _Texts[_TextRoundGame].Visible = false;
            _Texts[_TextRoundWinner].Visible = false;
        }

        private void _BuildRoundsTable()
        {
            float posY = _RoundsTableScrollArea.Y;
            float posX = _RoundsTableScrollArea.X;

            //Update statics and texts for rounds
            foreach (CRoundsTableRow roundRow in _RoundsTable)
            {
                roundRow.Number.Y = roundRow.Game.Y = roundRow.Winner.Y = posY;
                roundRow.Number.X += posX;
                roundRow.Game.X += posX;
                roundRow.Winner.X += posX;
                posY += roundRow.Number.H + 3f;
            }
        }

        private void _UpdateRoundsTable()
        {
            for (int i = 0; i < _RoundsTable.Count; i++)
            {
                if (_PartyMode.GameData.Rounds.Count > i + _RoundsTableOffset)
                {
                    _RoundsTable[i].Number.Visible = true;
                    _RoundsTable[i].Game.Visible = true;
                    _RoundsTable[i].Winner.Visible = true;

                    _RoundsTable[i].Number.Text = (i + 1 + _RoundsTableOffset) + ")";
                    _RoundsTable[i].Game.Text = _PartyMode.GameData.Rounds[i + _RoundsTableOffset].GameMode.TranslationName;

                    if ((_PartyMode.GameData.CurrentRoundNr - 1) > i + _RoundsTableOffset)
                    {
                        List<int> winner = _PartyMode.GameData.Rounds[i + _RoundsTableOffset].WinnerTeams;
                        if (winner.Count == 0)
                            _RoundsTable[i].Winner.Text = "TR_SCREENMAIN_NO_WINNER";
                        else if (winner.Count == 1)
                            _RoundsTable[i].Winner.Text = CBase.Language.Translate("TR_TEAM_" + (winner[0] + 1).ToString());
                        else
                        {
                            _RoundsTable[i].Winner.Text = CBase.Language.Translate("TR_TEAMS") + " ";
                            for (int t = 1; t <= winner.Count; t++)
                            {
                                _RoundsTable[i].Winner.Text += t;

                                if (t + 1 <= winner.Count)
                                    _RoundsTable[i].Winner.Text += ", ";
                            }
                        }
                    }
                    else
                        _RoundsTable[i].Winner.Text = "";
                }
                else if (_PartyMode.GameData.Rounds.Count < i + _RoundsTableOffset || i + 1 > _NumRoundsVisible)
                {
                    _RoundsTable[i].Number.Visible = false;
                    _RoundsTable[i].Game.Visible = false;
                    _RoundsTable[i].Winner.Visible = false;
                }
            }

            _Buttons[_ButtonRoundsScrollUp].Visible = _RoundsTableOffset > 0;
            _Buttons[_ButtonRoundsScrollDown].Visible = _PartyMode.GameData.Rounds.Count - _NumRoundsVisible - _RoundsTableOffset > 0;
        }

        private void _ScrollRoundsTable(int offset)
        {
            if (_PartyMode.GameData.Rounds.Count <= _NumRoundsVisible)
                _RoundsTableOffset = 0;
            else if (offset < 0 && _RoundsTableOffset + offset >= 0)
                _RoundsTableOffset += offset;
            else if (offset < 0 && _RoundsTableOffset + offset < 0)
                _RoundsTableOffset = 0;
            else if (offset > 0 && _RoundsTableOffset + offset <= _PartyMode.GameData.Rounds.Count - _NumRoundsVisible)
                _RoundsTableOffset += offset;
            else if (offset > 0 && _RoundsTableOffset + offset > _PartyMode.GameData.Rounds.Count - _NumRoundsVisible)
                _RoundsTableOffset = _PartyMode.GameData.Rounds.Count - _NumRoundsVisible;

            _UpdateRoundsTable();
        }

        private string _GetPlayerWinString()
        {
            
            string s = "";
            /*
            for (int i = 0; i < _PartyMode.GameData.ResultTable.Count; i++)
            {
                if (_PartyMode.GameData.ResultTable[i].Position == 1)
                {
                    if (i > 0)
                        s += ", ";
                    s += CBase.Profiles.GetPlayerName(_PartyMode.GameData.ResultTable[i].PlayerID);
                }
                else
                    break;
            }
            */
            return s;
        }
    }
}
