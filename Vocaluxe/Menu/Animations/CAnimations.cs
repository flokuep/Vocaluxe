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
                if (am.element.Event == am.anim.getEvent())
                {
                    if (!am.anim.AnimationActive() && !am.anim.isDrawn())
                    {
                        am.anim.StartAnimation();
                    }
                    else if (am.element.Event == EAnimationEvent.AfterSelected)
                        am.element.Event = EAnimationEvent.Visible;
                    am.anim.Update();
                    float x = am.element.Rect.X + am.anim.getRectChanges().X;
                    float y = am.element.Rect.Y + am.anim.getRectChanges().Y;
                    float w = am.element.Rect.W + am.anim.getRectChanges().W;
                    float h = am.element.Rect.H + am.anim.getRectChanges().H;
                    float r = am.element.Rect.Rotation + am.anim.getRectChanges().Rotation;
                    am.element.Rect = new SRectF(x, y, w, h, am.element.Rect.Z, r);
                    am.element.Color = am.anim.getColor();
                    am.element.Texture = am.anim.getTexture();
                }
            }
        }

        public static bool AnimAvailable(IMenuProperties e, EAnimationEvent ev)
        {
            foreach (SAnimationMenu am in Elements)
            {
                if (am.element == e)
                    if (am.anim.getEvent() == ev)
                        return true;
            }
            return false;
        }
    }
}
