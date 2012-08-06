using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Xml.XPath;

using Vocaluxe.Base;
using Vocaluxe.Lib.Draw;

namespace Vocaluxe.Menu.Animations
{
    public class CAnimationMoveLinear : CAnimationFramework
    {
        public EAnimationResizePosition Position;
        public EAnimationResizeOrder Order;
        public EAnimationType Type = new EAnimationType();
        public EAnimationRepeat Repeat;

        public SRectF FinalRect;
        public SRectF CurrentRect;
        public SRectF OriginalRect;

        public CAnimationMoveLinear()
        {
            Init();
        }

        public override void Init()
        {
            Type = EAnimationType.MoveLinear;
        }

        public override bool LoadAnimation(string item, XPathNavigator navigator)
        {
            _AnimationLoaded = true;

            //Load normal animation-options
            _AnimationLoaded &= base.LoadAnimation(item, navigator);

            //Load specific animation-options
            _AnimationLoaded &= CHelper.TryGetFloatValueFromXML(item + "/Time", navigator, ref Time);
            _AnimationLoaded &= CHelper.TryGetEnumValueFromXML<EAnimationRepeat>(item + "/Repeat", navigator, ref Repeat);
            _AnimationLoaded &= CHelper.TryGetFloatValueFromXML(item + "/X", navigator, ref FinalRect.X);
            _AnimationLoaded &= CHelper.TryGetFloatValueFromXML(item + "/Y", navigator, ref FinalRect.Y);


            return _AnimationLoaded;
        }

        public override void setRect(SRectF rect)
        {
            OriginalRect = rect;

            FinalRect.H = OriginalRect.H;
            FinalRect.W = OriginalRect.W;
        }

        public override SRectF getRect()
        {
            if (AnimationDrawn && Repeat == EAnimationRepeat.None)
                return FinalRect;
            else if (AnimationDrawn && Repeat == EAnimationRepeat.Reset)
                return OriginalRect;
            else
                return CurrentRect;
        }

        public override void StartAnimation()
        {
            base.StartAnimation();

            CurrentRect = OriginalRect;
        }

        public override void Update()
        {
            LastRect = CurrentRect;

            bool finished = false;

            float factor = Timer.ElapsedMilliseconds / Time;
            if (!ResetMode)
            {
                CurrentRect.X = OriginalRect.X + ((FinalRect.X - OriginalRect.X) * factor);
                CurrentRect.Y = OriginalRect.Y + ((FinalRect.Y - OriginalRect.Y) * factor);
                if (factor >= 1f)
                    finished = true;
            }
            else
            {
                CurrentRect.X = FinalRect.X + ((OriginalRect.X - FinalRect.X) * factor);
                CurrentRect.Y = FinalRect.Y + ((OriginalRect.Y - FinalRect.Y) * factor);
                if (factor >= 1f)
                    finished = true;
            }

            //If Animation finished
            if (finished)
            {
                switch (Repeat)
                {
                    case EAnimationRepeat.Repeat:
                        StopAnimation();
                        CurrentRect = OriginalRect;
                        StartAnimation();
                        break;

                    case EAnimationRepeat.RepeatWithReset:
                        ResetAnimation();
                        break;

                    case EAnimationRepeat.Reset:
                        if (!ResetMode)
                            ResetAnimation();
                        else
                            StopAnimation();
                        break;

                    case EAnimationRepeat.None:
                        StopAnimation();
                        break;
                }
            }
        }
    }
}
