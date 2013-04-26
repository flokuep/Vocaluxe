using System.Xml;
using VocaluxeLib.Menu;
using VocaluxeLib.Menu.Animations;

namespace VocaluxeLib.Animations
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