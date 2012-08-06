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
                    float x = am.element.Rect.X + am.anim.getRectChanges()[0];
                    float y = am.element.Rect.Y + am.anim.getRectChanges()[1];
                    float w = am.element.Rect.W + am.anim.getRectChanges()[2];
                    float h = am.element.Rect.H + am.anim.getRectChanges()[3];
                    am.element.Rect = new SRectF(x, y, w, h, am.element.Rect.Z);                   
                    am.element.Color = am.anim.getColor();
                    am.element.Texture = am.anim.getTexture();
                }
            }
        }
    }
}
