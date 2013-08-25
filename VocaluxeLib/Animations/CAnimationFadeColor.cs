#region license
// /*
//     This file is part of Vocaluxe.
// 
//     Vocaluxe is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     Vocaluxe is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with Vocaluxe. If not, see <http://www.gnu.org/licenses/>.
//  */
#endregion

using System;
using VocaluxeLib.Menu;

namespace VocaluxeLib.Animations
{
    public class CAnimationFadeColor : CAnimationFramework
    {
        private string _StartColorName;
        private string _EndColorName;

        private SColorF _CurrentColor;
        private SColorF _StartColor;
        private SColorF _EndColor;

        public CAnimationFadeColor(int partyModeID)
            : base(partyModeID) {}

        public override void Init()
        {
            Type = EAnimationType.FadeColor;
        }

        public override bool LoadAnimation(string item, CXMLReader xmlReader)
        {
            AnimationLoaded = true;
            AnimationLoaded &= base.LoadAnimation(item, xmlReader);
            AnimationLoaded &= xmlReader.TryGetFloatValue(item + "/Time", ref Time);
            AnimationLoaded &= xmlReader.TryGetEnumValue(item + "/Repeat", ref Repeat);
            if (xmlReader.GetValue(item + "/StartColor", out _StartColorName, String.Empty))
                _StartColor = CBase.Theme.GetColor(_StartColorName, _PartyModeID);
            else
            {
                AnimationLoaded &= xmlReader.TryGetFloatValue(item + "/StartR", ref _StartColor.R);
                AnimationLoaded &= xmlReader.TryGetFloatValue(item + "/StartG", ref _StartColor.G);
                AnimationLoaded &= xmlReader.TryGetFloatValue(item + "/StartB", ref _StartColor.B);
                AnimationLoaded &= xmlReader.TryGetFloatValue(item + "/StartA", ref _StartColor.A);
            }
            if (xmlReader.GetValue(item + "/EndColor", out _EndColorName, String.Empty))
                _EndColor = CBase.Theme.GetColor(_EndColorName, _PartyModeID);
            else
            {
                AnimationLoaded &= xmlReader.TryGetFloatValue(item + "/EndR", ref _EndColor.R);
                AnimationLoaded &= xmlReader.TryGetFloatValue(item + "/EndG", ref _EndColor.G);
                AnimationLoaded &= xmlReader.TryGetFloatValue(item + "/EndB", ref _EndColor.B);
                AnimationLoaded &= xmlReader.TryGetFloatValue(item + "/EndA", ref _EndColor.A);
            }
            return AnimationLoaded;
        }

        public override void StartAnimation()
        {
            base.StartAnimation();

            if (AnimationFromStart && Repeat != EAnimationRepeat.OnlyReset)
                _CurrentColor = _StartColor;
            else if (AnimationFromStart && Repeat == EAnimationRepeat.OnlyReset)
            {
                _CurrentColor = _EndColor;
                ResetMode = true;
            }
            else if (!AnimationFromStart && Repeat == EAnimationRepeat.OnlyReset)
                ResetMode = true;
        }

        public override SColorF GetColor()
        {
            if (AnimationDrawn && Repeat == EAnimationRepeat.None)
                return _EndColor;
            if (AnimationDrawn && (Repeat == EAnimationRepeat.Reset || Repeat == EAnimationRepeat.OnlyReset))
                return _StartColor;
            return _CurrentColor;
        }

        public override void SetCurrentValues(SRectF rect, SColorF color)
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

                    case EAnimationRepeat.OnlyReset:
                    case EAnimationRepeat.None:
                        StopAnimation();
                        break;
                }
            }
        }
    }
}