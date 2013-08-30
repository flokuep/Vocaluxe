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
    public enum EAnimationResizePosition
    {
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

        public CAnimationResize(int partyModeID)
            : base(partyModeID) {}

        public override void Init()
        {
            Type = EAnimationType.Resize;
        }

        public override bool LoadAnimation(string item, CXMLReader xmlReader)
        {
            AnimationLoaded = true;

            //Load normal animation-options
            AnimationLoaded &= base.LoadAnimation(item, xmlReader);

            //Load specific animation-options
            AnimationLoaded &= xmlReader.TryGetFloatValue(item + "/Time", ref Time);
            AnimationLoaded &= xmlReader.TryGetEnumValue(item + "/Repeat", ref Repeat);
            AnimationLoaded &= xmlReader.TryGetFloatValue(item + "/W", ref _FinalRect.W);
            AnimationLoaded &= xmlReader.TryGetFloatValue(item + "/H", ref _FinalRect.H);
            AnimationLoaded &= xmlReader.TryGetEnumValue(item + "/Position", ref Position);
            AnimationLoaded &= xmlReader.TryGetEnumValue(item + "/Order", ref Order);

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
                writer.WriteComment("<W> and <H>: Final size");
                writer.WriteElementString("W", _FinalRect.W.ToString("#0.00"));
                writer.WriteElementString("H", _FinalRect.H.ToString("#0.00"));
                writer.WriteComment("<Position>: Position of final rect: " + CHelper.ListStrings(Enum.GetNames(typeof(EAnimationResizePosition))));
                writer.WriteElementString("Position", Enum.GetName(typeof(EAnimationResizePosition), Position));
                writer.WriteComment("<Order>: Order of resizing: " + CHelper.ListStrings(Enum.GetNames(typeof(EAnimationResizeOrder))));
                writer.WriteElementString("Order", Enum.GetName(typeof(EAnimationResizeOrder), Order));
                return true;
            }
            else
                return false;
        }

        public override void SetRect(SRectF rect)
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

        public override SRectF GetRect()
        {
            if (AnimationDrawn && Repeat == EAnimationRepeat.None)
                return _FinalRect;
            if (AnimationDrawn && (Repeat == EAnimationRepeat.Reset || Repeat == EAnimationRepeat.OnlyReset))
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
            if ((ResetMode && Timer.ElapsedMilliseconds > TimeoutReset) || (!ResetMode && Timer.ElapsedMilliseconds > Timeout)){

                switch (Order)
                {
                    case EAnimationResizeOrder.Both:
                        float factor;
                        if (!ResetMode)
                        {
                            factor = (Timer.ElapsedMilliseconds - Timeout) / Time;
                            _CurrentRect.X = OriginalRect.X + ((_FinalRect.X - OriginalRect.X) * factor);
                            _CurrentRect.Y = OriginalRect.Y + ((_FinalRect.Y - OriginalRect.Y) * factor);
                            _CurrentRect.H = OriginalRect.H + ((_FinalRect.H - OriginalRect.H) * factor);
                            _CurrentRect.W = OriginalRect.W + ((_FinalRect.W - OriginalRect.W) * factor);
                            if (factor >= 1f)
                                finished = true;
                        }
                        else
                        {
                            factor = (Timer.ElapsedMilliseconds - TimeoutReset) / Time;
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
                            float factorH = (Timer.ElapsedMilliseconds - Timeout) / (Time / 2);
                            float factorW = ((Timer.ElapsedMilliseconds - TimeoutReset) - (Time / 2)) / (Time / 2);
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
                            float factorH = (Timer.ElapsedMilliseconds - TimeoutReset) / (Time / 2);
                            float factorW = ((Timer.ElapsedMilliseconds - TimeoutReset) - (Time / 2)) / (Time / 2);
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
                            float factorH = ((Timer.ElapsedMilliseconds - Timeout) - (Time / 2)) / (Time / 2);
                            float factorW = (Timer.ElapsedMilliseconds - Timeout) / (Time / 2);
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
                            float factorH = ((Timer.ElapsedMilliseconds - TimeoutReset) - (Time / 2)) / (Time / 2);
                            float factorW = (Timer.ElapsedMilliseconds - TimeoutReset) / (Time / 2);
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