﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace VocaluxeLib.Menu.Animations
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
        void SetCurrentValues(SRectF rect, SColorF color, STexture texture);
        EAnimationEvent getEvent();
        bool isDrawn();

        void StartAnimation();
        void StopAnimation();
        void ResetAnimation();
        void ResetValues(bool fromStart);
        bool AnimationActive();
        void Update();
    }
}
