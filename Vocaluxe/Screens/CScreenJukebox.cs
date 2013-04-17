using System.Windows.Forms;
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

        public override void Init()
        {
            base.Init();
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

            STexture VideoTexture = CBase.BackgroundMusic.GetVideoTexture();
            if (VideoTexture.Height > 0)
            {
                RectangleF bounds = new RectangleF(0f, 0f, CBase.Settings.GetRenderW(), CBase.Settings.GetRenderH());
                RectangleF rect = new RectangleF(0f, 0f, VideoTexture.Width, VideoTexture.Height);
                CHelper.SetRect(bounds, ref rect, rect.Width / rect.Height, EAspect.Crop);

                CBase.Drawing.DrawTexture(VideoTexture, new SRectF(rect.X, rect.Y, rect.Width, rect.Height, CBase.Settings.GetZFar() / 4));
            }
            return true;
        }

        public override bool UpdateGame()
        {
            return true;
        }
    }
}
