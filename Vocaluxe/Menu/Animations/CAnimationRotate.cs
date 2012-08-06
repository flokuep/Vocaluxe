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
    public class CAnimationRotate : CAnimationFramework
    {
        private float _Degree;
        private SRectF _FinalRect;
        private SRectF _CurrentRect;

        public CAnimationRotate()
        {
            Init();
        }

        public override void Init()
        {
            Type = EAnimationType.Rotate;
        }

        public override bool LoadAnimation(string item, XPathNavigator navigator)
        {
            _AnimationLoaded = true;

            //Load normal animation-options
            _AnimationLoaded &= base.LoadAnimation(item, navigator);

            //Load specific animation-options
            _AnimationLoaded &= CHelper.TryGetFloatValueFromXML(item + "/Time", navigator, ref Time);
            _AnimationLoaded &= CHelper.TryGetEnumValueFromXML<EAnimationRepeat>(item + "/Repeat", navigator, ref Repeat);
            _AnimationLoaded &= CHelper.TryGetFloatValueFromXML(item + "/Degree", navigator, ref _Degree);

            return _AnimationLoaded;
        }

        public override void setRect(SRectF rect)
        {
            OriginalRect = rect;

            _FinalRect = OriginalRect;
            _FinalRect.Rotation = OriginalRect.Rotation + _Degree;
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
                _CurrentRect.Rotation = OriginalRect.Rotation + ((_FinalRect.Rotation - OriginalRect.Rotation) * factor);
                if (factor >= 1f)
                    finished = true;
            }
            else
            {
                _CurrentRect.Rotation = _FinalRect.Rotation + ((OriginalRect.Rotation - _FinalRect.Rotation) * factor);
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
