using System;
using System.Collections.Generic;
using System.Text;

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
                    am.element.Rect = am.anim.getRect();
                    am.element.Color = am.anim.getColor();
                    am.element.Texture = am.anim.getTexture();
                }
            }
        }
    }
}
