using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Vocaluxe.Menu.Animations
{
    public class CAnimationMoveLinear : CAnimationFramework
    {
        public EAnimationResizePosition Position;
        public EAnimationResizeOrder Order;

        private SRectF _FinalRect;
        private SRectF _CurrentRect;

        public CAnimationMoveLinear(int PartyModeID)
            : base(PartyModeID)
        {
        }

        public override void Init()
        {
            Type = EAnimationType.MoveLinear;
        }

        public override bool LoadAnimation(string item, CXMLReader xmlReader)
        {
            _AnimationLoaded = true;

            //Load normal animation-options
            _AnimationLoaded &= base.LoadAnimation(item, xmlReader);

            //Load specific animation-options
            _AnimationLoaded &= xmlReader.TryGetFloatValue(item + "/Time", ref Time);
            _AnimationLoaded &= xmlReader.TryGetEnumValue<EAnimationRepeat>(item + "/Repeat", ref Repeat);
            _AnimationLoaded &= xmlReader.TryGetFloatValue(item + "/X", ref _FinalRect.X);
            _AnimationLoaded &= xmlReader.TryGetFloatValue(item + "/Y", ref _FinalRect.Y);


            return _AnimationLoaded;
        }

        public override void setRect(SRectF rect)
        {
            OriginalRect = rect;

            _FinalRect.H = OriginalRect.H;
            _FinalRect.W = OriginalRect.W;
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

            float factor = Timer.ElapsedMilliseconds / Time;
            if (!ResetMode)
            {
                _CurrentRect.X = OriginalRect.X + ((_FinalRect.X - OriginalRect.X) * factor);
                _CurrentRect.Y = OriginalRect.Y + ((_FinalRect.Y - OriginalRect.Y) * factor);
                if (factor >= 1f)
                    finished = true;
            }
            else
            {
                _CurrentRect.X = _FinalRect.X + ((OriginalRect.X - _FinalRect.X) * factor);
                _CurrentRect.Y = _FinalRect.Y + ((OriginalRect.Y - _FinalRect.Y) * factor);
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
