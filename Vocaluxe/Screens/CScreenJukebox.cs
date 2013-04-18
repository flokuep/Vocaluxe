using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using Vocaluxe.Base;
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

        private Stopwatch FadePlayerTimer;

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
        }

        public override void OnShow()
        {
            base.OnShow();
            CGraphics.HidePopup(EPopupScreens.PopupPlayerControl);
            _OldBGSetting = CConfig.VideosToBackground;
            CConfig.VideosToBackground = EOffOn.TR_CONFIG_ON;
        }

        public override void OnClose()
        {
            base.OnClose();

            CConfig.VideosToBackground = _OldBGSetting;
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

            if (mouseEvent.LB && IsMouseOver(mouseEvent))
            {
            }
            else if (mouseEvent.RB)
                CGraphics.Back();

            return true;
        }

        public override bool Draw()
        {
            base.Draw();

            /**STexture VideoTexture = CBase.BackgroundMusic.GetVideoTexture();
            if (VideoTexture.Height > 0)
            {
                RectangleF bounds = new RectangleF(0f, 0f, CBase.Settings.GetRenderW(), CBase.Settings.GetRenderH());
                RectangleF rect = new RectangleF(0f, 0f, VideoTexture.Width, VideoTexture.Height);
                CHelper.SetRect(bounds, ref rect, rect.Width / rect.Height, EAspect.Crop);

                CBase.Drawing.DrawTexture(VideoTexture, new SRectF(rect.X, rect.Y, rect.Width, rect.Height, CBase.Settings.GetZFar() / 4));
            }
            foreach (CStatic stat in Statics)
                stat.Draw();
            foreach (CText text in Texts)
                text.Draw();
            foreach (CButton btn in Buttons)
                btn.Draw();
            **/
            return true;
        }

        public override bool UpdateGame()
        {
            return true;
        }
    }
}
