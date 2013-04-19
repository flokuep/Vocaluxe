using System;
using System.Collections.Generic;
using System.Text;

namespace VocaluxeLib.Menu.Animations
{
    public class CAnimationFadeColor : CAnimationFramework
    {
        private string _StartColorName;
        private string _EndColorName;

        private SColorF _CurrentColor;
        private SColorF _StartColor;
        private SColorF _EndColor;

        public CAnimationFadeColor(int PartyModeID)
            :base(PartyModeID)
        {
        }

        public override void Init()
        {
            Type = EAnimationType.FadeColor;
        }

        public override bool LoadAnimation(string item, CXMLReader xmlReader)
        {
            _AnimationLoaded = true;
            _AnimationLoaded &= base.LoadAnimation(item, xmlReader);
            _AnimationLoaded &= xmlReader.TryGetFloatValue(item + "/Time", ref Time);
            _AnimationLoaded &= xmlReader.TryGetEnumValue<EAnimationRepeat>(item + "/Repeat", ref Repeat);
            if (xmlReader.GetValue(item + "/StartColor", ref _StartColorName, String.Empty))
            {
                _StartColor = CBase.Theme.GetColor(_StartColorName, _PartyModeID);
            }
            else
            {
                _AnimationLoaded &= xmlReader.TryGetFloatValue(item + "/StartR", ref _StartColor.R);
                _AnimationLoaded &= xmlReader.TryGetFloatValue(item + "/StartG", ref _StartColor.G);
                _AnimationLoaded &= xmlReader.TryGetFloatValue(item + "/StartB", ref _StartColor.B);
                _AnimationLoaded &= xmlReader.TryGetFloatValue(item + "/StartA", ref _StartColor.A);
            }
            if (xmlReader.GetValue(item + "/EndColor", ref _EndColorName, String.Empty))
            {
                _EndColor = CBase.Theme.GetColor(_EndColorName, _PartyModeID);
            }
            else
            {
                _AnimationLoaded &= xmlReader.TryGetFloatValue(item + "/EndR", ref _EndColor.R);
                _AnimationLoaded &= xmlReader.TryGetFloatValue(item + "/EndG", ref _EndColor.G);
                _AnimationLoaded &= xmlReader.TryGetFloatValue(item + "/EndB", ref _EndColor.B);
                _AnimationLoaded &= xmlReader.TryGetFloatValue(item + "/EndA", ref _EndColor.A);
            }
            return _AnimationLoaded;
        }

        public override void StartAnimation()
        {
            base.StartAnimation();

            if(AnimationFromStart)
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

        public override void SetCurrentValues(SRectF rect, SColorF color, STexture texture)
        {
            _CurrentColor = color;
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
            {
                if (!ResetMode)
                    _CurrentColor = _EndColor;
                else
                    _CurrentColor = _StartColor;
                finished = true;
            }

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
