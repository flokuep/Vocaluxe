using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;

using Vocaluxe.Base;
using VocaluxeLib;
using VocaluxeLib.Menu;

namespace Vocaluxe.Screens
{
    class CScreenJukebox : CMenu
    {
        // Version number for theme files. Increment it, if you've changed something on the theme files!
        protected override int _ScreenVersion
        {
            get { return 1; }
        }

        private Stopwatch _FadePlayerTimer;
        private int _FadePlayerDirection;

        private const string _StaticPlayerBG = "StaticPlayerBG";
        private const string _StaticCover = "StaticCover";

        private const string _TextArtist = "TextArtist";
        private const string _TextTitle = "TextTitle";
        private const string _TextAlbum = "TextAlbum";
        private const string _TextTimer = "TextTimer";

        private const string _ButtonPlay = "ButtonPlay";
        private const string _ButtonPause = "ButtonPause";
        private const string _ButtonNext = "ButtonNext";
        private const string _ButtonPrevious = "ButtonPrevious";
        private const string _ButtonRepeat = "ButtonRepeat";

        private EOffOn _OldBGSetting = EOffOn.TR_CONFIG_OFF;

        public override void Init()
        {
            base.Init();

            _ThemeStatics = new string[] { _StaticCover, _StaticPlayerBG };
            _ThemeTexts = new string[] { _TextAlbum, _TextArtist, _TextTimer, _TextTitle };
            _ThemeButtons = new string[] { _ButtonPlay, _ButtonPause, _ButtonNext, _ButtonPrevious, _ButtonRepeat };

            _FadePlayerTimer = new Stopwatch();
        }

        public override void OnShow()
        {
            base.OnShow();
            CGraphics.HidePopup(EPopupScreens.PopupPlayerControl);
            _OldBGSetting = CConfig.VideosToBackground;
            CConfig.VideosToBackground = EOffOn.TR_CONFIG_ON;
            CBackgroundMusic.VideoEnabled = true;
            _UpdatePlayerVisibility(1f);
        }

        public override void OnClose()
        {
            base.OnClose();

            CConfig.VideosToBackground = _OldBGSetting;
            CBackgroundMusic.VideoEnabled = CConfig.VideosToBackground == EOffOn.TR_CONFIG_ON;
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
                        CGraphics.Back();
                        break;
                }
            }
            return true;
        }

        public override bool HandleMouse(SMouseEvent mouseEvent)
        {
            base.HandleMouse(mouseEvent);

            if (!CHelper.IsInBounds(_Statics[_StaticPlayerBG].Rect, mouseEvent) && _Statics[_StaticPlayerBG].Visible && !_FadePlayerTimer.IsRunning)
            {
                _FadePlayerTimer.Start();
                _FadePlayerDirection = -1;
            }
            else if (CHelper.IsInBounds(_Statics[_StaticPlayerBG].Rect, mouseEvent) && _Statics[_StaticPlayerBG].Visible && _FadePlayerTimer.IsRunning)
            {
                _FadePlayerDirection = 1;
            }
            else if (CHelper.IsInBounds(_Statics[_StaticPlayerBG].Rect, mouseEvent) && !_Statics[_StaticPlayerBG].Visible && !_FadePlayerTimer.IsRunning)
            {
                _FadePlayerTimer.Start();
                _FadePlayerDirection = 1;
            }
            else if (!CHelper.IsInBounds(_Statics[_StaticPlayerBG].Rect, mouseEvent) && _Statics[_StaticPlayerBG].Visible && _FadePlayerTimer.IsRunning)
            {
                _FadePlayerDirection = -1;
            }
            if (mouseEvent.LB)
            {
            }
            else if (mouseEvent.RB)
                CGraphics.Back();

            return true;
        }

        public override bool Draw()
        {
            base.Draw();

            return true;
        }

        public override bool UpdateGame()
        {
            _UpdatePlayerContent();

            if (_FadePlayerTimer.IsRunning)
            {
                float alpha = 0f;
                float t = _FadePlayerTimer.ElapsedMilliseconds / 1000f;
                if(_FadePlayerDirection == -1)
                    alpha = (2f - t) / 2f;
                else if(_FadePlayerDirection == 1)
                    alpha = t / 2f;

                if ((_FadePlayerDirection == -1 && alpha <= 0f) || (_FadePlayerDirection == 1 && alpha >= 1f))
                {
                    _FadePlayerTimer.Stop();
                    _FadePlayerTimer.Reset();
                }

                _UpdatePlayerVisibility(alpha);
                
            }
            return true;
        }

        private void _UpdatePlayerContent()
        {
            float currentTime = CBackgroundMusic.CurrentTime;
            float songLength = CBackgroundMusic.SongLength;
            int minCurrent = (int)Math.Floor(currentTime / 60f);
            int secCurrent = (int)(currentTime - minCurrent * 60f);
            int minLength = (int)Math.Floor(songLength / 60f);
            int secLength = (int)(songLength - minLength * 60f);

            _Statics[_StaticCover].Texture = CBackgroundMusic.Cover;
            _Texts[_TextArtist].Text = CSongs.Songs[CBackgroundMusic.SongID].Artist;
            _Texts[_TextTitle].Text = CSongs.Songs[CBackgroundMusic.SongID].Title;
            //Texts[_TextAlbum].Text = CSongs.Songs[CBackgroundMusic.SongID].;
            _Texts[_TextTimer].Text = minCurrent.ToString("00") + ":" + secCurrent.ToString("00") + "/" + minLength.ToString("00") + ":" + secLength.ToString("00");
        }

        private void _UpdatePlayerVisibility(float alpha)
        {
            if (alpha > 1f)
                alpha = 1f;
            else if (alpha < 0f)
                alpha = 0f;
            _Statics[_StaticPlayerBG].Color.A = alpha;
            _Statics[_StaticCover].Color.A = alpha;
            _Texts[_TextArtist].Color.A = alpha;
            _Texts[_TextTitle].Color.A = alpha;
            //Texts[_TextAlbum].Color.A = alpha;
            _Texts[_TextTimer].Color.A = alpha;
            _Buttons[_ButtonNext].Color.A = alpha;
            _Buttons[_ButtonPause].Color.A = alpha;
            _Buttons[_ButtonPlay].Color.A = alpha;
            _Buttons[_ButtonPrevious].Color.A = alpha;
            _Buttons[_ButtonRepeat].Color.A = alpha;

            _Statics[_StaticPlayerBG].Visible = alpha > 0f;
            _Statics[_StaticCover].Visible = alpha > 0f;
            _Texts[_TextArtist].Visible = alpha > 0f;
            _Texts[_TextTitle].Visible = alpha > 0f;
            //Texts[_TextAlbum].Visible = alpha > 0f;
            _Texts[_TextTimer].Visible = alpha > 0f;
            _Buttons[_ButtonNext].Visible = alpha > 0f;
            _Buttons[_ButtonPause].Visible = alpha > 0f && CBackgroundMusic.IsPlaying;
            _Buttons[_ButtonPlay].Visible = alpha > 0f && !CBackgroundMusic.IsPlaying;
            _Buttons[_ButtonPrevious].Visible = alpha > 0f;
            _Buttons[_ButtonRepeat].Visible = alpha > 0f;
        }
    }
}
