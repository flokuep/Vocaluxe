using System;
using System.Collections.Generic;
using System.Text;

namespace VocaluxeLib.Menu.Animations
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
                        am.anim.SetCurrentValues(am.element.Rect, am.element.Color, am.element.Texture);
                        am.anim.StartAnimation();
                    }
                    else if (am.element.Event == EAnimationEvent.AfterSelected && am.anim.isDrawn())
                    {
                        if (AnimAvailable(am.element, EAnimationEvent.Visible))
                            am.element.Event = EAnimationEvent.Visible;
                        else
                            am.element.Event = EAnimationEvent.None;
                    }
                    else if (am.element.Event == EAnimationEvent.OnSelected && am.anim.isDrawn())
                    {
                        if (AnimAvailable(am.element, EAnimationEvent.Selected))
                            am.element.Event = EAnimationEvent.Selected;
                        else
                            am.element.Event = EAnimationEvent.None;
                    }
                    else if (am.element.Event == EAnimationEvent.OnVisible && am.anim.isDrawn())
                    {
                        if (AnimAvailable(am.element, EAnimationEvent.Visible))
                            am.element.Event = EAnimationEvent.Visible;
                        else
                            am.element.Event = EAnimationEvent.None;
                    }
                    else if (am.element.Event == EAnimationEvent.AfterVisible && am.anim.isDrawn())
                    {
                        if (AnimAvailable(am.element, EAnimationEvent.AfterVisible))
                            am.element.Event = EAnimationEvent.AfterVisible;
                        else
                            am.element.Event = EAnimationEvent.None;
                    }

                    if (!am.anim.isDrawn())
                    {
                        am.anim.Update();
                        float x = am.element.Rect.X + am.anim.getRectChanges().X;
                        float y = am.element.Rect.Y + am.anim.getRectChanges().Y;
                        float w = am.element.Rect.W + am.anim.getRectChanges().W;
                        float h = am.element.Rect.H + am.anim.getRectChanges().H;
                        float r = am.element.Rect.Rotation + am.anim.getRectChanges().Rotation;
                        am.element.Rect = new SRectF(x, y, w, h, am.element.Rect.Z);
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
            if (e.Event != EAnimationEvent.OnSelected)
            {
                if (AnimAvailable(e, EAnimationEvent.OnSelected))
                    e.Event = EAnimationEvent.OnSelected;
                else if (AnimAvailable(e, EAnimationEvent.Selected))
                    e.Event = EAnimationEvent.Selected;
                ResetAnimation(e, EAnimationEvent.OnSelected);
            }
        }

        public static void SetAfterSelectAnim(IMenuProperties e)
        {
            if (e.Event != EAnimationEvent.AfterSelected)
            {
                if (AnimAvailable(e, EAnimationEvent.AfterSelected))
                    e.Event = EAnimationEvent.AfterSelected;
                else if (AnimAvailable(e, EAnimationEvent.Visible))
                    e.Event = EAnimationEvent.Visible;
                else
                    e.Event = EAnimationEvent.None;
                ResetAnimation(e, EAnimationEvent.AfterSelected);
            }
        }

        public static void SetOnVisibleAnim(IMenuProperties e)
        {
            if (e.Event != EAnimationEvent.OnVisible)
            {
                if (AnimAvailable(e, EAnimationEvent.OnVisible))
                    e.Event = EAnimationEvent.OnVisible;
                else if (AnimAvailable(e, EAnimationEvent.Visible))
                    e.Event = EAnimationEvent.Visible;
                ResetAnimation(e, EAnimationEvent.OnVisible);
            }
        }

        public static void SetAfterVisibleAnim(IMenuProperties e)
        {
            if (e.Event != EAnimationEvent.AfterVisible)
            {
                if (AnimAvailable(e, EAnimationEvent.AfterVisible))
                    e.Event = EAnimationEvent.AfterVisible;
                else
                    e.Event = EAnimationEvent.None;
                ResetAnimation(e, EAnimationEvent.AfterVisible);
            }
        }

        public static void UpdateEvent(EAnimationEvent evt)
        {
            foreach (SAnimationMenu am in Elements)
            {
                if (AnimAvailable(am.element, evt))
                {
                    am.anim.StopAnimation();
                    am.element.Event = evt;
                    ResetAnimation(am.element, evt);
                }
            }
        }

        public static void ResetAnimation(IMenuProperties e, EAnimationEvent ev)
        {
            bool fromStart = true;
            if (GetCurrentAnimation(e) != null)
            {
                fromStart = !GetCurrentAnimation(e).AnimationActive();
            }
            foreach (SAnimationMenu am in Elements)
            {
                if (am.element == e)
                    if (am.anim.getEvent() == ev)
                        am.anim.ResetValues(fromStart);
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

        public static CAnimation GetCurrentAnimation(IMenuProperties e)
        {
            foreach (SAnimationMenu am in Elements)
            {
                if (am.element == e)
                    return am.anim;
            }
            return null;
        }
    }
}
