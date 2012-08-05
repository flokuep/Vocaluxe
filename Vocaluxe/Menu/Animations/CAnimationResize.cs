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
    public enum EAnimationResizePosition {
        Center,
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    public enum EAnimationResizeOrder
    {
        WidthFirst,
        HeightFirst,
        Both
    }

    public class CAnimationResize : CAnimationFramework
    {
        public EAnimationResizePosition Position;
        public EAnimationResizeOrder Order;
        public SRectF FinalRect;
        public SRectF CurrentRect;
        public SRectF OriginalRect;

        public CAnimationResize()
        {
        }

        public override void Init()
        {
            Type = EAnimationType.Resize;
        }

        public override bool LoadAnimation(string item, XPathNavigator navigator)
        {
            _AnimationLoaded = true;

            //Load normal animation-options
            _AnimationLoaded &= base.LoadAnimation(item, navigator);

            //Load specific animation-options
            _AnimationLoaded &= CHelper.TryGetFloatValueFromXML(item + "/W", navigator, ref FinalRect.W);
            _AnimationLoaded &= CHelper.TryGetFloatValueFromXML(item + "/H", navigator, ref FinalRect.H);
            _AnimationLoaded &= CHelper.TryGetEnumValueFromXML<EAnimationResizePosition>(item + "/Position", navigator, ref Position);
            _AnimationLoaded &= CHelper.TryGetEnumValueFromXML<EAnimationResizeOrder>(item + "/Order", navigator, ref Order);

            return _AnimationLoaded;
        }

        public override void setRect(SRectF rect)
        {
            OriginalRect = rect;

            switch (Position)
            {
                case EAnimationResizePosition.TopLeft:
                    FinalRect.X = OriginalRect.X;
                    FinalRect.Y = OriginalRect.Y;
                    break;

                case EAnimationResizePosition.TopRight:
                    FinalRect.X = OriginalRect.X + OriginalRect.W - FinalRect.W;
                    FinalRect.Y = OriginalRect.Y;
                    break;

                case EAnimationResizePosition.BottomLeft:
                    FinalRect.X = OriginalRect.X;
                    FinalRect.Y = OriginalRect.Y + OriginalRect.H - FinalRect.H;
                    break;

                case EAnimationResizePosition.BottomRight:
                    FinalRect.X = OriginalRect.X + OriginalRect.W - FinalRect.W;
                    FinalRect.Y = OriginalRect.Y + OriginalRect.H - FinalRect.H;
                    break;

                case EAnimationResizePosition.Center:
                    FinalRect.X = OriginalRect.X + (OriginalRect.W - FinalRect.W) / 2;
                    FinalRect.Y = OriginalRect.Y + (OriginalRect.H - FinalRect.H) / 2;
                    break;

            }
        }

        public override SRectF getRect()
        {
            if (AnimationDrawn && Repeat == EAnimationRepeat.None)
                return FinalRect;
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
            bool finished = false;

            switch (Order)
            {
                case EAnimationResizeOrder.Both:
                    float factor = Timer.ElapsedMilliseconds / Speed;
                    if (!ResetMode)
                    {  
                        CurrentRect.X = OriginalRect.X + ((FinalRect.X - OriginalRect.X) * factor);
                        CurrentRect.Y = OriginalRect.Y + ((FinalRect.Y - OriginalRect.Y) * factor);
                        CurrentRect.H = OriginalRect.H + ((FinalRect.H - OriginalRect.H) * factor);
                        CurrentRect.W = OriginalRect.W + ((FinalRect.W - OriginalRect.W) * factor);
                        if (factor >= 1f)
                            finished = true;
                    }
                    else
                    {
                        CurrentRect.X = FinalRect.X + ((OriginalRect.X - FinalRect.X) * factor);
                        CurrentRect.Y = FinalRect.Y + ((OriginalRect.Y - FinalRect.Y) * factor);
                        CurrentRect.H = FinalRect.H + ((OriginalRect.H - FinalRect.H) * factor);
                        CurrentRect.W = FinalRect.W + ((OriginalRect.W - FinalRect.W) * factor);
                        if (factor >= 1f)
                            finished = true;
                    }
                    break;

                case EAnimationResizeOrder.HeightFirst:
                    if (!ResetMode)
                    {
                        float factorH = Timer.ElapsedMilliseconds / (Speed / 2);
                        float factorW = (Timer.ElapsedMilliseconds - (Speed / 2)) / (Speed / 2);
                        if (factorH < 1f)
                        {
                            CurrentRect.Y = OriginalRect.Y + ((FinalRect.Y - OriginalRect.Y) * factorH);
                            CurrentRect.H = OriginalRect.H + ((FinalRect.H - OriginalRect.H) * factorH);
                        }
                        else if (factorW < 1f)
                        {
                            CurrentRect.W = OriginalRect.W + ((FinalRect.W - OriginalRect.W) * factorW);
                            CurrentRect.X = OriginalRect.X + ((FinalRect.X - OriginalRect.X) * factorW);
                        }
                        else
                            finished = true;
                    }
                    else
                    {
                        float factorH = Timer.ElapsedMilliseconds / (Speed / 2);
                        float factorW = (Timer.ElapsedMilliseconds - (Speed / 2)) / (Speed / 2);
                        if (factorH < 1f)
                        {
                            CurrentRect.Y = FinalRect.Y + ((OriginalRect.Y - FinalRect.Y) * factorH);
                            CurrentRect.H = FinalRect.H + ((OriginalRect.H - FinalRect.H) * factorH);
                        }
                        else if (factorW < 1f)
                        {
                            CurrentRect.W = FinalRect.W + ((OriginalRect.W - FinalRect.W) * factorW);
                            CurrentRect.X = FinalRect.X + ((OriginalRect.X - FinalRect.X) * factorW);
                        }
                        else
                            finished = true;
                    }
                    break;

                case EAnimationResizeOrder.WidthFirst:
                    if (!ResetMode)
                    {
                        float factorH = (Timer.ElapsedMilliseconds - (Speed / 2)) / (Speed / 2);
                        float factorW = Timer.ElapsedMilliseconds / (Speed / 2);
                        if (factorW < 1f)
                        {
                            CurrentRect.W = OriginalRect.W + ((FinalRect.W - OriginalRect.W) * factorW);
                            CurrentRect.X = OriginalRect.X + ((FinalRect.X - OriginalRect.X) * factorW);
                        }
                        else if (factorH < 1f)
                        {
                            CurrentRect.Y = OriginalRect.Y + ((FinalRect.Y - OriginalRect.Y) * factorH);
                            CurrentRect.H = OriginalRect.H + ((FinalRect.H - OriginalRect.H) * factorH);
                        }
                        else
                            finished = true;
                    }
                    else
                    {
                        float factorH = (Timer.ElapsedMilliseconds - (Speed / 2)) / (Speed / 2);
                        float factorW = Timer.ElapsedMilliseconds / (Speed / 2);
                        if (factorW < 1f)
                        {
                            CurrentRect.W = FinalRect.W + ((OriginalRect.W - FinalRect.W) * factorW);
                            CurrentRect.X = FinalRect.X + ((OriginalRect.X - FinalRect.X) * factorW);
                        }
                        else if (factorH < 1f)
                        {
                            CurrentRect.Y = FinalRect.Y + ((OriginalRect.Y - FinalRect.Y) * factorH);
                            CurrentRect.H = FinalRect.H + ((OriginalRect.H - FinalRect.H) * factorH);
                        }
                        else
                            finished = true;
                    }
                    break;
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
