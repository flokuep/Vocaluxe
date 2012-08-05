using System;
using System.Collections.Generic;
using System.Text;

using Vocaluxe.Base;
using Vocaluxe.Lib.Draw;

namespace Vocaluxe.Menu.Animations
{
    public static class CAnimations
    {
        struct SAnimationMenu {
            public IMenuProperties element;
            public CAnimation anim;

            public SAnimationMenu(IMenuProperties e, CAnimation a)
            {
                element = e;
                anim = a;
            }
        }

        private static List<SAnimationMenu> Elements = new List<SAnimationMenu>();

        public static void Add(IMenuProperties e, CAnimation anim)
        {
            Elements.Add(new SAnimationMenu(e, anim));
        }

        public static void Update()
        {
            foreach (SAnimationMenu am in Elements)
            {
                if (am.element.Visible)
                {
                    if (!am.anim.AnimationActive() && !am.anim.isDrawn())
                    {
                        am.anim.StartAnimation();
                    }
                    am.anim.Update();
                    float x = am.element.Rect.X + (am.element.Rect.X - am.anim.getRect().X);
                    float y = am.element.Rect.Y + (am.element.Rect.Y - am.anim.getRect().Y);
                    float w = am.element.Rect.W + (am.element.Rect.W - am.anim.getRect().W);
                    float h = am.element.Rect.H + (am.element.Rect.H - am.anim.getRect().H);
                    am.element.Rect = new SRectF(x, y, w, h, am.element.Rect.Z);
                    am.element.Color = am.anim.getColor();
                    am.element.Texture = am.anim.getTexture();
                }
            }
        }
    }
}
