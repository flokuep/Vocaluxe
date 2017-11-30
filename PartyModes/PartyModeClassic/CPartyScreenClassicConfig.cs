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

using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using VocaluxeLib.Songs;

namespace VocaluxeLib.PartyModes.Classic
{
    public class CPartyScreenClassicConfig : CPartyScreenClassic
    {
        // Version number for theme files. Increment it, if you've changed something on the theme files!
        protected override int _ScreenVersion
        {
            get { return 1; }
        }

        private const string _SelectSlideNumRounds = "SelectSlideNumRounds";
        private const string _SelectSlideNumJokers = "SelectSlideNumJokers";
        private const string _SelectSlideRefillJokers = "SelectSlideRefillJokers";

        private const string _ButtonNext = "ButtonNext";
        private const string _ButtonBack = "ButtonBack";

        public override void Init()
        {
            base.Init();

            _ThemeSelectSlides = new string[]
                {
                    _SelectSlideNumRounds, _SelectSlideNumJokers, _SelectSlideNumRounds
                };
            _ThemeButtons = new string[] { _ButtonNext, _ButtonBack };
        }

        public override bool HandleInput(SKeyEvent keyEvent)
        {
            base.HandleInput(keyEvent);

            if (keyEvent.KeyPressed) { }
            else
            {
                switch (keyEvent.Key)
                {
                    case Keys.Back:
                    case Keys.Escape:
                        _PartyMode.Back();
                        break;

                    case Keys.Enter:
                        _UpdateSlides();

                        if (_Buttons[_ButtonBack].Selected)
                            _PartyMode.Back();

                        if (_Buttons[_ButtonNext].Selected)
                            _PartyMode.Next();
                        break;

                    case Keys.Left:
                        _UpdateSlides();
                        break;

                    case Keys.Right:
                        _UpdateSlides();
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
                _UpdateSlides();
                if (_Buttons[_ButtonBack].Selected)
                    _PartyMode.Back();

                if (_Buttons[_ButtonNext].Selected)
                    _PartyMode.Next();
            }

            if (mouseEvent.RB)
                _PartyMode.Back();

            return true;
        }

        public override void OnShow()
        {
            base.OnShow();

            Debug.Assert(CBase.Config.GetMaxNumMics() >= 2);

            _FillSlides();
            _UpdateSlides();
        }

        public override bool UpdateGame()
        {
            return true;
        }

        private void _FillSlides()
        {
            _SelectSlides[_SelectSlideNumRounds].Clear();
            for (int i = 4; i <= 15; i++)
                _SelectSlides[_SelectSlideNumRounds].AddValue(i.ToString());

            _SelectSlides[_SelectSlideNumRounds].SelectedValue = _PartyMode.GameData.NumRounds.ToString();

            // build num joker slide 1 to 10
            _SelectSlides[_SelectSlideNumJokers].Clear();
            for (int i = 1; i <= 10; i++)
            {
                _SelectSlides[_SelectSlideNumJokers].AddValue(i.ToString());
            }
            _SelectSlides[_SelectSlideNumJokers].SelectedValue = "5";

            //build joker config slide
            _SelectSlides[_SelectSlideRefillJokers].Clear();
            _SelectSlides[_SelectSlideRefillJokers].AddValue(CBase.Language.Translate("TR_BUTTON_NO", PartyModeID));
            _SelectSlides[_SelectSlideRefillJokers].AddValue(CBase.Language.Translate("TR_BUTTON_YES", PartyModeID));
            _SelectSlides[_SelectSlideRefillJokers].SelectLastValue();

        }

        private void _UpdateSlides()
        {
            _PartyMode.GameData.NumRounds = int.Parse(_SelectSlides[_SelectSlideNumRounds].SelectedValue);
            _PartyMode.GameData.NumJokers = _SelectSlides[_SelectSlideNumJokers].Selection + 1;
            _PartyMode.GameData.RefillJokers = (_SelectSlides[_SelectSlideRefillJokers].Selection == 1) ? true : false;


        }
    }
}
