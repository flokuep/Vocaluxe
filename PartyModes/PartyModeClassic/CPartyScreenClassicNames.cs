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
using VocaluxeLib.Menu;

namespace VocaluxeLib.PartyModes.Classic
{
    public class CPartyScreenClassicNames : CMenuPartyNameSelection
    {
        // Version number for theme files. Increment it, if you've changed something on the theme files!
        protected override int _ScreenVersion
        {
            get { return 1; }
        }

        private new CPartyModeClassic _PartyMode;

        public override void Init()
        {
            base.Init();
            _PartyMode = (CPartyModeClassic)base._PartyMode;
        }

        public override void OnShow()
        {
            base.OnShow();

            int[] numPlayerPerTeam = new int[_PartyMode.GameData.Teams.Count];
            int totalNumPlayer = 0;
            List<Guid>[] selectedPlayer = new List<Guid>[_PartyMode.GameData.Teams.Count];
            for (int t = 0; t < _PartyMode.GameData.Teams.Count; t++)
            {
                //Set default player num or saved one.
                if (_PartyMode.GameData.Teams[t].Count == 0)
                    numPlayerPerTeam[t] = 2;
                else
                    numPlayerPerTeam[t] = _PartyMode.GameData.Teams[t].Count;

                totalNumPlayer += numPlayerPerTeam[t];

                selectedPlayer[t] = new List<Guid>();
                selectedPlayer[t].AddRange(_PartyMode.GameData.Teams[t]);
            }


            SetPartyModeData(numPlayerPerTeam.Length, totalNumPlayer, numPlayerPerTeam);
            SetPartyModeProfiles(selectedPlayer);
        }

        public override void Back()
        {
            _PartyMode.GameData.Teams.Clear();
            foreach (List<Guid> l in _TeamList)
                _PartyMode.GameData.Teams.Add(l);

            _PartyMode.Back();
        }

        public override void Next()
        {
            _PartyMode.GameData.Teams.Clear();
            foreach (List<Guid> l in _TeamList)
                _PartyMode.GameData.Teams.Add(l);

            _PartyMode.Next();
        }
    }
}
