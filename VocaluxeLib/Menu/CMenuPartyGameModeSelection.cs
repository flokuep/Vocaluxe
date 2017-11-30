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
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using VocaluxeLib.Draw;
using VocaluxeLib.Profile;

namespace VocaluxeLib.Menu
{
    public class CPartyGameMode
    {
        public EGameMode GameMode;
        public string GameModeName;
        public string GameModeDescription;
        public bool IsPartyGameMode
        {
            get
            {
                return !string.IsNullOrWhiteSpace(GameModeName);
            }
        }

        public EOffOn Active;
        public string TranslationName
        {
            get
            {
                return IsPartyGameMode ? GameModeName : GameMode.ToString();
            }
        }

        public string TranslationDescription
        {
            get
            {
                return IsPartyGameMode ? GameModeDescription : GameMode.ToString() + "_DESC";
            }
        }
    }

    public abstract class CMenuPartyGameModeSelection : CMenuParty
    {
        //This list will contain all available game-modes for this party-mode
        private List<CPartyGameMode> _GameModes;

        private const int _CustomGameModesOption = 0;
        private const int _AllGameModesOption = 1;

        private const int _NumGMSelectSlides = 5;

        private const string _ButtonNext = "ButtonNext";
        private const string _ButtonBack = "ButtonBack";
        private const string _SelectSlideGameMode = "SelectSlideGameMode";
        private List<string> _SelectSlideGameModes;
        private List<string> _TextGameMode;
        private List<string> _TextGameModeDesc;

        private int _CurrentGlobalSelection = -1;
        private int _Offset = 0;

        public override void Init()
        {
            base.Init();
            _GameModes = new List<CPartyGameMode>();

            _SelectSlideGameModes = new List<string>();
            _TextGameMode = new List<string>();
            _TextGameModeDesc = new List<string>();

            for (int i = 0; i < _NumGMSelectSlides; i++)
            {
                _SelectSlideGameModes.Add("SelectSlideGameMode" + (i + 1));
                _TextGameMode.Add("TextGameMode" + (i + 1));
                _TextGameModeDesc.Add("TextGameModeDesc" + (i + 1));
            }

            List<string> themeHelper = new List<string>();
            themeHelper.Add(_SelectSlideGameMode);
            themeHelper.AddRange(_SelectSlideGameModes);

            _ThemeButtons = new string[] { _ButtonBack, _ButtonNext };
            _ThemeSelectSlides = themeHelper.ToArray();

            themeHelper.Clear();
            themeHelper.AddRange(_TextGameMode);
            themeHelper.AddRange(_TextGameModeDesc);

            _ThemeTexts = themeHelper.ToArray();

            _BuildGameModeList();
        }

        public override void LoadTheme(string xmlPath)
        {
            base.LoadTheme(xmlPath);

            _SelectSlides[_SelectSlideGameMode].AddValue("TR_GAMEMODE_CUSTOM");
            _SelectSlides[_SelectSlideGameMode].AddValue("TR_GAMEMODE_ALL");
            foreach (CPartyGameMode pgm in _GameModes)
                _SelectSlides[_SelectSlideGameMode].AddValue(pgm.TranslationName);

            _SelectSlides[_SelectSlideGameMode].Selection = _AllGameModesOption;

            _UpdateGameModeSelectSlides();
        }

        public override bool HandleInput(SKeyEvent keyEvent)
        {
            switch(keyEvent.Key)
            {
                case Keys.Down:
                    if(_SelectSlides[_SelectSlideGameModes[_NumGMSelectSlides - 1]].Selected
                        && _Offset + _NumGMSelectSlides < _GameModes.Count)
                    {
                        _Offset++;
                        _UpdateGameModeSelectSlides();
                        return true;
                    }
                    break;

                case Keys.Up:
                    if (_SelectSlides[_SelectSlideGameModes[0]].Selected
                        && _Offset > 0)
                    {
                        _Offset--;
                        _UpdateGameModeSelectSlides();
                        return true;
                    }
                    break;
            }
            base.HandleInput(keyEvent);

            switch (keyEvent.Key)
            {
                case Keys.Back:
                case Keys.Escape:
                    Back();
                    break;

                case Keys.Enter:
                    if (_Buttons[_ButtonBack].Selected)
                        Back();

                    if (_Buttons[_ButtonNext].Selected)
                        Next();
                    break;

                case Keys.Left:
                case Keys.Right:
                    _ApplyGameModeSelection();
                    break;
            }
            return true;
        }

        public override bool HandleMouse(SMouseEvent mouseEvent)
        {
            base.HandleMouse(mouseEvent);
            if (mouseEvent.LB && _IsMouseOverCurSelection(mouseEvent))
            {
                if (_Buttons[_ButtonBack].Selected)
                    Back();

                if (_Buttons[_ButtonNext].Selected)
                    Next();

                _ApplyGameModeSelection();
            }
            if (mouseEvent.RB)
                Back();

            if (mouseEvent.Wheel != 0)
            {
                for (int i = 0; i < Math.Min(_GameModes.Count, _NumGMSelectSlides); i++)
                {
                    if (CHelper.IsInBounds(_SelectSlides[_SelectSlideGameModes[i]].Rect, mouseEvent) && _GameModes.Count > _NumGMSelectSlides)
                    {
                        _Offset += mouseEvent.Wheel;
                        _Offset = _Offset.Clamp(0, _GameModes.Count - _NumGMSelectSlides);

                        _UpdateGameModeSelectSlides();
                        break;
                    }
                }
            }

            return true;
        }

        public abstract void Back();

        public abstract void Next();

        public override bool UpdateGame()
        {
            //Only update _SelectedModes if something has changed
            if(_SelectSlides[_SelectSlideGameMode].Selection != _CurrentGlobalSelection)
            {
                _CurrentGlobalSelection = _SelectSlides[_SelectSlideGameMode].Selection;

                if (_CurrentGlobalSelection == _CustomGameModesOption || _CurrentGlobalSelection == _AllGameModesOption)
                {
                    foreach(CPartyGameMode pgm in _GameModes)
                        pgm.Active = EOffOn.TR_CONFIG_ON;

                    _UpdateGameModeSelectSlides();
                }
                //Only one game-mode will be used
                else
                {
                    for(int i=0; i<_GameModes.Count; i++)
                    {
                        if (i == _CurrentGlobalSelection - 2)
                            _GameModes[i].Active = EOffOn.TR_CONFIG_ON;
                        else
                            _GameModes[i].Active = EOffOn.TR_CONFIG_OFF;
                    }
                    _UpdateGameModeSelectSlides();
                }
            }
            return true;
        }

        protected List<CPartyGameMode> _GetSelectedGameModes()
        {
            return _GameModes.Where(s => s.Active == EOffOn.TR_CONFIG_ON).ToList();
        }

        protected abstract List<EGameMode> _GetBlacklist();
        protected abstract List<EGameMode> _GetWhitelist();
        protected abstract List<string> _GetPartyModeGames();

        private void _BuildGameModeList()
        {
            _GameModes.Clear();
            if(_GetWhitelist().Count > 0)
            {
                foreach(EGameMode gm in _GetWhitelist())
                {
                    CPartyGameMode pgm = new CPartyGameMode();
                    pgm.Active = EOffOn.TR_CONFIG_ON;
                    pgm.GameMode = gm;
                    _GameModes.Add(pgm);
                }
            }
            else if(_GetBlacklist().Count > 0)
            {
                foreach(EGameMode gm in Enum.GetValues(typeof(EGameMode)))
                {
                    if(!_GetBlacklist().Contains(gm))
                    {
                        CPartyGameMode pgm = new CPartyGameMode();
                        pgm.Active = EOffOn.TR_CONFIG_ON;
                        pgm.GameMode = gm;
                        _GameModes.Add(pgm);
                    }
                }
            }
            else
            {
                foreach (EGameMode gm in Enum.GetValues(typeof(EGameMode)))
                {
                    CPartyGameMode pgm = new CPartyGameMode();
                    pgm.Active = EOffOn.TR_CONFIG_ON;
                    pgm.GameMode = gm;
                    _GameModes.Add(pgm);
                }
            }

            foreach(string gm in _GetPartyModeGames())
            {
                CPartyGameMode pgm = new CPartyGameMode();
                pgm.Active = EOffOn.TR_CONFIG_ON;
                pgm.GameModeName = gm;
                _GameModes.Add(pgm);
            }
        }

        private void _UpdateGameModeSelectSlides()
        {
            for (int i = 0; i < _SelectSlideGameModes.Count; i++)
            {
                if (i + _Offset < _GameModes.Count)
                {
                    _SelectSlides[_SelectSlideGameModes[i]].Visible = true;
                    _Texts[_TextGameMode[i]].Visible = true;
                    _Texts[_TextGameModeDesc[i]].Visible = true;

                    _SelectSlides[_SelectSlideGameModes[i]].Clear();
                    _SelectSlides[_SelectSlideGameModes[i]].SetValues<EOffOn>((int)_GameModes[i + _Offset].Active);
                    _Texts[_TextGameMode[i]].Text = _GameModes[i + _Offset].TranslationName;
                    _Texts[_TextGameModeDesc[i]].Text = _GameModes[i + _Offset].TranslationDescription;
                }
                else
                {
                    _SelectSlides[_SelectSlideGameModes[i]].Visible = false;
                    _Texts[_TextGameMode[i]].Visible = false;
                    _Texts[_TextGameModeDesc[i]].Visible = false;
                }
            }
        }

        /// <summary>
        /// Check (and save) gamemode selection and select custom modes if needed
        /// </summary>
        private void _ApplyGameModeSelection()
        {
            int selection = -1;
            for (int i = 0; i < Math.Min(_GameModes.Count, _NumGMSelectSlides); i++)
            {
                if (_SelectSlides[_SelectSlideGameModes[i]].Selected
                    && _GameModes[i + _Offset].Active != (EOffOn)_SelectSlides[_SelectSlideGameModes[i]].Selection)
                {
                    selection = i;
                    _GameModes[selection + _Offset].Active = (EOffOn)_SelectSlides[_SelectSlideGameModes[selection]].Selection;
                    break;
                }
            }
            if (selection > -1 && _CurrentGlobalSelection != _CustomGameModesOption)
            {
                _CurrentGlobalSelection = _CustomGameModesOption;
                _SelectSlides[_SelectSlideGameMode].Selection = _CurrentGlobalSelection;
            }
        }
    }
}