using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Vocaluxe.Menu.Animations
{
    interface IAnimation
    {
        void Init();
        bool LoadAnimation(string item, CXMLReader xmlReader);
        bool SaveAnimation(XmlWriter writer);

        void setRect(SRectF rect);
        SRectF getRect();
        SRectF getRectChanges();
        void setColor(SColorF color);
        SColorF getColor();
        void setTexture(ref STexture texture);
        STexture getTexture();
        EAnimationEvent getEvent();
        bool isDrawn();

        void StartAnimation();
        void StopAnimation();
        void ResetAnimation();
        void ResetValues();
        bool AnimationActive();
        void Update();
    }
}
