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
using System.Xml;
using VocaluxeLib.Menu;

namespace VocaluxeLib.Animations
{
    public class CAnimationRotate : CAnimationFramework
    {
        private float _Degree;
        private SRectF _FinalRect;
        private SRectF _CurrentRect;

        public CAnimationRotate(int partyModeID)
            : base(partyModeID) {}

        public override void Init()
        {
            Type = EAnimationType.Rotate;
        }

        public override bool LoadAnimation(string item, CXMLReader xmlReader)
        {
            AnimationLoaded = true;

            //Load normal animation-options
            AnimationLoaded &= base.LoadAnimation(item, xmlReader);

            //Load specific animation-options
            AnimationLoaded &= xmlReader.TryGetFloatValue(item + "/Time", ref Time);
            AnimationLoaded &= xmlReader.TryGetEnumValue(item + "/Repeat", ref Repeat);
            AnimationLoaded &= xmlReader.TryGetFloatValue(item + "/Degree", ref _Degree);

            return AnimationLoaded;
        }

        public override bool SaveAnimation(XmlWriter writer)
        {
            if (AnimationLoaded)
            {
                base.SaveAnimation(writer);
                writer.WriteComment("<Time>: Duration of animation in ms");
                writer.WriteElementString("Time", Time.ToString("#0.00"));
                writer.WriteComment("<Repeat>: Repeat-Mode of animation: " + CHelper.ListStrings(Enum.GetNames(typeof(EAnimationRepeat))));
                writer.WriteElementString("Repeat", Enum.GetName(typeof(EAnimationRepeat), Repeat));
                writer.WriteComment("<Degree>: Rotation");
                writer.WriteElementString("Degree", _FinalRect.X.ToString("#0.00"));
                return true;
            }
            else
                return false;
        }

        public override void SetRect(SRectF rect)
        {
            OriginalRect = rect;

            _FinalRect = OriginalRect;
            _FinalRect.Rotation = OriginalRect.Rotation + _Degree;
        }

        public override SRectF GetRect()
        {
            if (AnimationDrawn && Repeat == EAnimationRepeat.None)
                return _FinalRect;
            if (AnimationDrawn && Repeat == EAnimationRepeat.Reset)
                return OriginalRect;
            return _CurrentRect;
        }

        public override void SetCurrentValues(SRectF rect, SColorF color)
        {
            _CurrentRect = rect;
        }

        public override void StartAnimation()
        {
            base.StartAnimation();

            if (AnimationFromStart && Repeat != EAnimationRepeat.OnlyReset)
                _CurrentRect = OriginalRect;
            else if (AnimationFromStart && Repeat == EAnimationRepeat.OnlyReset)
            {
                _CurrentRect = _FinalRect;
                ResetMode = true;
            }
            else if (!AnimationFromStart && Repeat == EAnimationRepeat.OnlyReset)
                ResetMode = true;
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

                    case EAnimationRepeat.OnlyReset:
                    case EAnimationRepeat.None:
                        StopAnimation();
                        break;
                }
            }
        }
    }
}