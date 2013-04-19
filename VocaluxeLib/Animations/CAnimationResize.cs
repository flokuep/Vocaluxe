using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace VocaluxeLib.Menu.Animations
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

        private SRectF _FinalRect;
        private SRectF _CurrentRect;

        public CAnimationResize(int PartyModeID)
            : base(PartyModeID)
        {
        }

        public override void Init()
        {
            Type = EAnimationType.Resize;
        }

        public override bool LoadAnimation(string item, CXMLReader xmlReader)
        {
            _AnimationLoaded = true;

            //Load normal animation-options
            _AnimationLoaded &= base.LoadAnimation(item, xmlReader);

            //Load specific animation-options
            _AnimationLoaded &= xmlReader.TryGetFloatValue(item + "/Time", ref Time);
            _AnimationLoaded &= xmlReader.TryGetEnumValue<EAnimationRepeat>(item + "/Repeat", ref Repeat);
            _AnimationLoaded &= xmlReader.TryGetFloatValue(item + "/W", ref _FinalRect.W);
            _AnimationLoaded &= xmlReader.TryGetFloatValue(item + "/H", ref _FinalRect.H);
            _AnimationLoaded &= xmlReader.TryGetEnumValue<EAnimationResizePosition>(item + "/Position", ref Position);
            _AnimationLoaded &= xmlReader.TryGetEnumValue<EAnimationResizeOrder>(item + "/Order", ref Order);

            return _AnimationLoaded;
        }

        public override void setRect(SRectF rect)
        {
            OriginalRect = rect;

            switch (Position)
            {
                case EAnimationResizePosition.TopLeft:
                    _FinalRect.X = OriginalRect.X;
                    _FinalRect.Y = OriginalRect.Y;
                    break;

                case EAnimationResizePosition.TopRight:
                    _FinalRect.X = OriginalRect.X + OriginalRect.W - _FinalRect.W;
                    _FinalRect.Y = OriginalRect.Y;
                    break;

                case EAnimationResizePosition.BottomLeft:
                    _FinalRect.X = OriginalRect.X;
                    _FinalRect.Y = OriginalRect.Y + OriginalRect.H - _FinalRect.H;
                    break;

                case EAnimationResizePosition.BottomRight:
                    _FinalRect.X = OriginalRect.X + OriginalRect.W - _FinalRect.W;
                    _FinalRect.Y = OriginalRect.Y + OriginalRect.H - _FinalRect.H;
                    break;

                case EAnimationResizePosition.Center:
                    _FinalRect.X = OriginalRect.X + (OriginalRect.W - _FinalRect.W) / 2;
                    _FinalRect.Y = OriginalRect.Y + (OriginalRect.H - _FinalRect.H) / 2;
                    break;

            }
        }

        public override SRectF getRect()
        {
            if (AnimationDrawn && Repeat == EAnimationRepeat.None)
                return _FinalRect;
            else if (AnimationDrawn && Repeat == EAnimationRepeat.Reset)
                return OriginalRect;
            else
                return _CurrentRect;
        }

        public override void StartAnimation()
        {
            base.StartAnimation();

            _CurrentRect = OriginalRect;
        }

        public override void Update()
        {
            LastRect = _CurrentRect;

            bool finished = false;

            switch (Order)
            {
                case EAnimationResizeOrder.Both:
                    float factor = Timer.ElapsedMilliseconds / Time;
                    if (!ResetMode)
                    {  
                        _CurrentRect.X = OriginalRect.X + ((_FinalRect.X - OriginalRect.X) * factor);
                        _CurrentRect.Y = OriginalRect.Y + ((_FinalRect.Y - OriginalRect.Y) * factor);
                        _CurrentRect.H = OriginalRect.H + ((_FinalRect.H - OriginalRect.H) * factor);
                        _CurrentRect.W = OriginalRect.W + ((_FinalRect.W - OriginalRect.W) * factor);
                        if (factor >= 1f)
                            finished = true;
                    }
                    else
                    {
                        _CurrentRect.X = _FinalRect.X + ((OriginalRect.X - _FinalRect.X) * factor);
                        _CurrentRect.Y = _FinalRect.Y + ((OriginalRect.Y - _FinalRect.Y) * factor);
                        _CurrentRect.H = _FinalRect.H + ((OriginalRect.H - _FinalRect.H) * factor);
                        _CurrentRect.W = _FinalRect.W + ((OriginalRect.W - _FinalRect.W) * factor);
                        if (factor >= 1f)
                            finished = true;
                    }
                    break;

                case EAnimationResizeOrder.HeightFirst:
                    if (!ResetMode)
                    {
                        float factorH = Timer.ElapsedMilliseconds / (Time / 2);
                        float factorW = (Timer.ElapsedMilliseconds - (Time / 2)) / (Time / 2);
                        if (factorH < 1f)
                        {
                            _CurrentRect.Y = OriginalRect.Y + ((_FinalRect.Y - OriginalRect.Y) * factorH);
                            _CurrentRect.H = OriginalRect.H + ((_FinalRect.H - OriginalRect.H) * factorH);
                        }
                        else if (factorW < 1f)
                        {
                            _CurrentRect.W = OriginalRect.W + ((_FinalRect.W - OriginalRect.W) * factorW);
                            _CurrentRect.X = OriginalRect.X + ((_FinalRect.X - OriginalRect.X) * factorW);
                        }
                        else
                            finished = true;
                    }
                    else
                    {
                        float factorH = Timer.ElapsedMilliseconds / (Time / 2);
                        float factorW = (Timer.ElapsedMilliseconds - (Time / 2)) / (Time / 2);
                        if (factorH < 1f)
                        {
                            _CurrentRect.Y = _FinalRect.Y + ((OriginalRect.Y - _FinalRect.Y) * factorH);
                            _CurrentRect.H = _FinalRect.H + ((OriginalRect.H - _FinalRect.H) * factorH);
                        }
                        else if (factorW < 1f)
                        {
                            _CurrentRect.W = _FinalRect.W + ((OriginalRect.W - _FinalRect.W) * factorW);
                            _CurrentRect.X = _FinalRect.X + ((OriginalRect.X - _FinalRect.X) * factorW);
                        }
                        else
                            finished = true;
                    }
                    break;

                case EAnimationResizeOrder.WidthFirst:
                    if (!ResetMode)
                    {
                        float factorH = (Timer.ElapsedMilliseconds - (Time / 2)) / (Time / 2);
                        float factorW = Timer.ElapsedMilliseconds / (Time / 2);
                        if (factorW < 1f)
                        {
                            _CurrentRect.W = OriginalRect.W + ((_FinalRect.W - OriginalRect.W) * factorW);
                            _CurrentRect.X = OriginalRect.X + ((_FinalRect.X - OriginalRect.X) * factorW);
                        }
                        else if (factorH < 1f)
                        {
                            _CurrentRect.Y = OriginalRect.Y + ((_FinalRect.Y - OriginalRect.Y) * factorH);
                            _CurrentRect.H = OriginalRect.H + ((_FinalRect.H - OriginalRect.H) * factorH);
                        }
                        else
                            finished = true;
                    }
                    else
                    {
                        float factorH = (Timer.ElapsedMilliseconds - (Time / 2)) / (Time / 2);
                        float factorW = Timer.ElapsedMilliseconds / (Time / 2);
                        if (factorW < 1f)
                        {
                            _CurrentRect.W = _FinalRect.W + ((OriginalRect.W - _FinalRect.W) * factorW);
                            _CurrentRect.X = _FinalRect.X + ((OriginalRect.X - _FinalRect.X) * factorW);
                        }
                        else if (factorH < 1f)
                        {
                            _CurrentRect.Y = _FinalRect.Y + ((OriginalRect.Y - _FinalRect.Y) * factorH);
                            _CurrentRect.H = _FinalRect.H + ((OriginalRect.H - _FinalRect.H) * factorH);
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
                        _CurrentRect = OriginalRect;
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
