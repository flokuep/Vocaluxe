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
    public class CAnimationMoveLinear : CAnimationFramework
    {
        public EAnimationResizePosition Position;
        public EAnimationResizeOrder Order;

        private SRectF _FinalRect;
        private SRectF _CurrentRect;

        public CAnimationMoveLinear(int partyModeID)
            : base(partyModeID) {}

        public override void Init()
        {
            Type = EAnimationType.MoveLinear;
        }

        public override bool LoadAnimation(string item, CXMLReader xmlReader)
        {
            AnimationLoaded = true;

            //Load normal animation-options
            AnimationLoaded &= base.LoadAnimation(item, xmlReader);

            //Load specific animation-options
            AnimationLoaded &= xmlReader.TryGetFloatValue(item + "/Time", ref Time);
            AnimationLoaded &= xmlReader.TryGetEnumValue(item + "/Repeat", ref Repeat);
            AnimationLoaded &= xmlReader.TryGetFloatValue(item + "/X", ref _FinalRect.X);
            AnimationLoaded &= xmlReader.TryGetFloatValue(item + "/Y", ref _FinalRect.Y);


            return AnimationLoaded;
        }

        public override bool SaveAnimation(System.Xml.XmlWriter writer)
        {
            if (AnimationLoaded)
            {
                base.SaveAnimation(writer);
                writer.WriteComment("<Time>: Duration of animation in ms");
                writer.WriteElementString("Time", Time.ToString("#0.00"));
                writer.WriteComment("<Repeat>: Repeat-Mode of animation: " + CHelper.ListStrings(Enum.GetNames(typeof(EAnimationRepeat))));
                writer.WriteElementString("Repeat", Enum.GetName(typeof(EAnimationRepeat), Repeat));
                writer.WriteComment("<X> and <Y>: Element destination");
                writer.WriteElementString("X", _FinalRect.X.ToString("#0.00"));
                writer.WriteElementString("Y", _FinalRect.Y.ToString("#0.00"));
                return true;
            }
            else
                return false;
        }

        public override void SetRect(SRectF rect)
        {
            OriginalRect = rect;

            _FinalRect.H = OriginalRect.H;
            _FinalRect.W = OriginalRect.W;
        }

        public override void SetCurrentValues(SRectF rect, SColorF color)
        {
            _CurrentRect = rect;

            _FinalRect.H = rect.H;
            _FinalRect.W = rect.W;
        }

        public override SRectF GetRect()
        {
            if (AnimationDrawn && Repeat == EAnimationRepeat.None)
                return _FinalRect;
            if (AnimationDrawn && Repeat == EAnimationRepeat.Reset)
                return OriginalRect;
            return _CurrentRect;
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
            if ((ResetMode && Timer.ElapsedMilliseconds > TimeoutReset) || (!ResetMode && Timer.ElapsedMilliseconds > Timeout))
            {
                float factor;
                if (!ResetMode)
                {
                    factor = (Timer.ElapsedMilliseconds - Timeout) / Time;
                    _CurrentRect.X = OriginalRect.X + ((_FinalRect.X - OriginalRect.X) * factor);
                    _CurrentRect.Y = OriginalRect.Y + ((_FinalRect.Y - OriginalRect.Y) * factor);
                    if (factor >= 1f)
                        finished = true;
                }
                else
                {
                    factor = (Timer.ElapsedMilliseconds - TimeoutReset) / Time;
                    _CurrentRect.X = _FinalRect.X + ((OriginalRect.X - _FinalRect.X) * factor);
                    _CurrentRect.Y = _FinalRect.Y + ((OriginalRect.Y - _FinalRect.Y) * factor);
                    if (factor >= 1f)
                        finished = true;
                }
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