using System;
using System.Collections.Generic;
using System.Text;

using Vocaluxe.Base;
using Vocaluxe.Lib.Draw;

namespace Vocaluxe.Menu.Animations
{
    public class CAnimationFadeColor : CAnimationFramework
    {
        private string _StartColorName;
        private string _EndColorName;

        private SColorF _CurrentColor;
        private SColorF _StartColor;
        private SColorF _EndColor;

        public CAnimationFadeColor()
        {
            Init();
        }

        public override void Init()
        {
            Type = EAnimationType.FadeColor;
        }

        public override bool LoadAnimation(string item, System.Xml.XPath.XPathNavigator navigator)
        {
            _AnimationLoaded = true;
            _AnimationLoaded &= base.LoadAnimation(item, navigator);
            _AnimationLoaded &= CHelper.TryGetFloatValueFromXML(item + "/Time", navigator, ref Time);
            _AnimationLoaded &= CHelper.TryGetEnumValueFromXML<EAnimationRepeat>(item + "/Repeat", navigator, ref Repeat);
            if (CHelper.GetValueFromXML(item + "/StartColor", navigator, ref _StartColorName, String.Empty))
            {
                _StartColor = CTheme.GetColor(_StartColorName);
            }
            else
            {
                _AnimationLoaded &= CHelper.TryGetFloatValueFromXML(item + "/StartR", navigator, ref _StartColor.R);
                _AnimationLoaded &= CHelper.TryGetFloatValueFromXML(item + "/StartG", navigator, ref _StartColor.G);
                _AnimationLoaded &= CHelper.TryGetFloatValueFromXML(item + "/StartB", navigator, ref _StartColor.B);
                _AnimationLoaded &= CHelper.TryGetFloatValueFromXML(item + "/StartA", navigator, ref _StartColor.A);
            }
            if (CHelper.GetValueFromXML(item + "/EndColor", navigator, ref _EndColorName, String.Empty))
            {
                _EndColor = CTheme.GetColor(_EndColorName);
            }
            else
            {
                _AnimationLoaded &= CHelper.TryGetFloatValueFromXML(item + "/EndR", navigator, ref _EndColor.R);
                _AnimationLoaded &= CHelper.TryGetFloatValueFromXML(item + "/EndG", navigator, ref _EndColor.G);
                _AnimationLoaded &= CHelper.TryGetFloatValueFromXML(item + "/EndB", navigator, ref _EndColor.B);
                _AnimationLoaded &= CHelper.TryGetFloatValueFromXML(item + "/EndA", navigator, ref _EndColor.A);
            }
            return _AnimationLoaded;
        }

        public override void StartAnimation()
        {
            base.StartAnimation();

            _CurrentColor = _StartColor;
        }

        public override SColorF getColor()
        {
            if (AnimationDrawn && Repeat == EAnimationRepeat.None)
                return _EndColor;
            else if (AnimationDrawn && Repeat == EAnimationRepeat.Reset)
                return _StartColor;
            else
                return _CurrentColor;
        }

        public override void Update()
        {
            bool finished = false;
            float factor = Timer.ElapsedMilliseconds / Time;

            if (!ResetMode)
            {
                _CurrentColor.R = _StartColor.R + factor * (_EndColor.R - _StartColor.R);
                _CurrentColor.G = _StartColor.G + factor * (_EndColor.G - _StartColor.G);
                _CurrentColor.B = _StartColor.B + factor * (_EndColor.B - _StartColor.B);
                _CurrentColor.A = _StartColor.A + factor * (_EndColor.A - _StartColor.A);
            }
            else
            {
                _CurrentColor.R = _EndColor.R + factor * (_StartColor.R - _EndColor.R);
                _CurrentColor.G = _EndColor.G + factor * (_StartColor.G - _EndColor.G);
                _CurrentColor.B = _EndColor.B + factor * (_StartColor.B - _EndColor.B);
                _CurrentColor.A = _EndColor.A + factor * (_StartColor.A - _EndColor.A);
            }

            if (factor >= 1f)
                finished = true;

            //If Animation finished
            if (finished)
            {
                switch (Repeat)
                {
                    case EAnimationRepeat.Repeat:
                        StopAnimation();
                        _CurrentColor = _StartColor;
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
