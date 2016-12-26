using System;
using System.Collections.Generic;
using VocaluxeLib.Menu;

namespace VocaluxeLib.PartyModes.Classic
{
    public class CPartyScreenClassicGameModes : CMenuPartyGameModeSelection
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

        public override void Back()
        {
            _PartyMode.Back();
        }

        public override void Next()
        {
            _PartyMode.GameData.GameModes.Clear();
            _PartyMode.GameData.GameModes.AddRange(_GetSelectedGameModes());
            _PartyMode.Next();
        }

        public override bool UpdateGame()
        {
            return base.UpdateGame();
        }

        protected override List<EGameMode> _GetBlacklist()
        {
            return new List<EGameMode>();
        }

        protected override List<EGameMode> _GetWhitelist()
        {
            return new List<EGameMode>();
        }

        protected override List<string> _GetPartyModeGames()
        {
            return new List<string>();
        }
    }
}
