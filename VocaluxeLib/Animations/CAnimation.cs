using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace VocaluxeLib.Menu.Animations
{
    public class CAnimation:IAnimation
    {
        private IAnimation _Animation;
        private int _PartyModeID;

        public CAnimation(EAnimationType Type, int PartyModeID)
        {
            _PartyModeID = PartyModeID;
            setAnimation(Type);
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

        public void ResetValues()
        {
            _Animation.ResetValues();
        }

        public bool AnimationActive()
        {
            return _Animation.AnimationActive();
        }

        public void Update()
        {
            _Animation.Update();
        }

        public void setRect(SRectF rect)
        {
            _Animation.setRect(rect);
        }

        public SRectF getRect()
        {
            return _Animation.getRect();
        }

        public SRectF getRectChanges()
        {
            return _Animation.getRectChanges();
        }

        public void setColor(SColorF color)
        {
            _Animation.setColor(color);
        }

        public SColorF getColor()
        {
            return _Animation.getColor();
        }

        public void setTexture(ref STexture texture)
        {
            _Animation.setTexture(ref texture);
        }

        public STexture getTexture()
        {
            return _Animation.getTexture();
        }

        public EAnimationEvent getEvent()
        {
            return _Animation.getEvent();
        }

        public bool isDrawn()
        {
            return _Animation.isDrawn();
        }

        public void setAnimation(EAnimationType Type)
        {
            switch (Type)
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
