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
                    else if (am.element.Event == EAnimationEvent.AfterSelected && am.anim.isDrawn())
                    {
                        if (AnimAvailable(am.element, EAnimationEvent.Visible))
                        {
                            am.element.Event = EAnimationEvent.Visible;
                            am.anim.ResetValues();
                        }
                        else
                        {
                            am.element.Event = EAnimationEvent.None;
                        }
                    }
                    else if (am.element.Event == EAnimationEvent.OnSelected && am.anim.isDrawn())
                    {
                        if (AnimAvailable(am.element, EAnimationEvent.Selected))
                        {
                            am.element.Event = EAnimationEvent.Selected;
                            am.anim.ResetValues();
                        }
                    }
                    else if (am.element.Event == EAnimationEvent.OnVisible && am.anim.isDrawn())
                    {
                        if (AnimAvailable(am.element, EAnimationEvent.Visible))
                        {
                            am.element.Event = EAnimationEvent.Visible;
                            am.anim.ResetValues();
                        }
                    }

                    if (!am.anim.isDrawn())
                    {
                        am.anim.Update();
                        float x = am.element.Rect.X + am.anim.getRectChanges().X;
                        float y = am.element.Rect.Y + am.anim.getRectChanges().Y;
                        float w = am.element.Rect.W + am.anim.getRectChanges().W;
                        float h = am.element.Rect.H + am.anim.getRectChanges().H;
                        float r = am.element.Rect.Rotation + am.anim.getRectChanges().Rotation;
                        am.element.Rect = new SRectF(x, y, w, h, am.element.Rect.Z, r);
                    }
                    am.element.Color = am.anim.getColor();
                    am.element.Texture = am.anim.getTexture();
                }
                else
                    if (am.anim.AnimationActive())
                        am.anim.StopAnimation();
            }
        }

        public static void SetOnSelectAnim(IMenuProperties e)
        {
            if (AnimAvailable(e, EAnimationEvent.OnSelected))
                e.Event = EAnimationEvent.OnSelected;
            else if (AnimAvailable(e, EAnimationEvent.Selected))
                e.Event = EAnimationEvent.Selected;
        }

        public static void SetAfterSelectAnim(IMenuProperties e)
        {
            if (AnimAvailable(e, EAnimationEvent.AfterSelected))
                e.Event = EAnimationEvent.AfterSelected;
            else if (AnimAvailable(e, EAnimationEvent.Visible))
                e.Event = EAnimationEvent.Visible;
            else
                e.Event = EAnimationEvent.None;
        }

        public static void UpdateEvent(EAnimationEvent evt)
        {
            foreach (SAnimationMenu am in Elements)
            {
                if (AnimAvailable(am.element, evt))
                {
                    am.anim.StopAnimation();
                    am.element.Event = evt;
                    am.anim.StartAnimation();
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
