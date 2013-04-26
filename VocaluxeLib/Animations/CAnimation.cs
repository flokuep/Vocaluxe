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

using System.Xml;
using VocaluxeLib.Menu;

namespace VocaluxeLib.Animations
{
    public class CAnimation : IAnimation
    {
        private IAnimation _Animation;
        private readonly int _PartyModeID;

        public CAnimation(EAnimationType type, int partyModeID)
        {
            _PartyModeID = partyModeID;
            SetAnimation(type);
        }

        public void Init()
        {
            _Animation.Init();
        }

        public bool LoadAnimation(string item, CXMLReader xmlReader)
        {
            return _Animation.LoadAnimation(item, xmlReader);
        }

        public bool SaveAnimation(XmlWriter writer)
        {
            return _Animation.SaveAnimation(writer);
        }

        public void StartAnimation()
        {
            _Animation.StartAnimation();
        }

        public void StopAnimation()
        {
            _Animation.StopAnimation();
        }

        public void ResetAnimation()
        {
            _Animation.ResetAnimation();
        }

        public void ResetValues(bool fromStart)
        {
            _Animation.ResetValues(fromStart);
        }

        public bool AnimationActive()
        {
            return _Animation.AnimationActive();
        }

        public void Update()
        {
            _Animation.Update();
        }

        public void SetRect(SRectF rect)
        {
            _Animation.SetRect(rect);
        }

        public SRectF GetRect()
        {
            return _Animation.GetRect();
        }

        public SRectF GetRectChanges()
        {
            return _Animation.GetRectChanges();
        }

        public void SetColor(SColorF color)
        {
            _Animation.SetColor(color);
        }

        public SColorF GetColor()
        {
            return _Animation.GetColor();
        }

        public void SetTexture(ref STexture texture)
        {
            _Animation.SetTexture(ref texture);
        }

        public STexture GetTexture()
        {
            return _Animation.GetTexture();
        }

        public void SetCurrentValues(SRectF rect, SColorF color)
        {
            _Animation.SetCurrentValues(rect, color);
        }

        public EAnimationEvent GetEvent()
        {
            return _Animation.GetEvent();
        }

        public bool IsDrawn()
        {
            return _Animation.IsDrawn();
        }

        public void SetAnimation(EAnimationType type)
        {
            switch (type)
            {
                case EAnimationType.Resize:
                    _Animation = new CAnimationResize(_PartyModeID);
                    break;

                case EAnimationType.MoveLinear:
                    _Animation = new CAnimationMoveLinear(_PartyModeID);
                    break;

                case EAnimationType.Video:
                    _Animation = new CAnimationVideo(_PartyModeID);
                    break;

                case EAnimationType.FadeColor:
                    _Animation = new CAnimationFadeColor(_PartyModeID);
                    break;

                case EAnimationType.Rotate:
                    _Animation = new CAnimationRotate(_PartyModeID);
                    break;
            }
        }
    }
}