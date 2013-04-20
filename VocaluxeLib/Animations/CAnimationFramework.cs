using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace VocaluxeLib.Menu.Animations
{
    public enum EAnimationType
    {
        Resize,
        MoveLinear,
        Video,
        FadeColor,
        Rotate
    }

    public enum EAnimationRepeat
    {
        None,
        OnlyReset,
        Reset,
        Repeat,
        RepeatWithReset
    }

    public enum EAnimationEvent
    {
        None, //Couldn't choosen by theme-designer
        OnVisible,
        Visible,
        AfterVisible,
        OnSelected,
        Selected,
        AfterSelected
    }

    public abstract class CAnimationFramework:IAnimation
    {
        public bool _AnimationLoaded;

        public EAnimationType Type;
        public EAnimationRepeat Repeat;
        public EAnimationEvent Event;

        public SRectF OriginalRect;
        public SRectF LastRect;
        public SColorF OriginalColor;
        public  STexture OriginalTexture;

        public float Time;

        public Stopwatch Timer = new Stopwatch();
        public bool ResetMode = false;
        public bool AnimationDrawn = false;
        public bool AnimationFromStart = true;

        protected int _PartyModeID;

        public CAnimationFramework(int PartyModeID)
        {
            _PartyModeID = PartyModeID;
            Init();
        }

        public abstract void Init();

        public virtual bool LoadAnimation(string item, CXMLReader xmlReader)
        {
            _AnimationLoaded = true;

            _AnimationLoaded &= xmlReader.TryGetEnumValue<EAnimationEvent>(item + "/Event", ref Event);

            if (_AnimationLoaded)
                _AnimationLoaded = Event != EAnimationEvent.None;

            return _AnimationLoaded;
        }

        public virtual bool SaveAnimation(XmlWriter writer)
        {
            return false;
        }

        public virtual void StartAnimation()
        {
            AnimationDrawn = false;
            Timer.Start();
        }

        public virtual void StopAnimation()
        {
            Timer.Stop();
            if(Repeat == EAnimationRepeat.None || Repeat == EAnimationRepeat.Reset || Repeat == EAnimationRepeat.OnlyReset)
                AnimationDrawn = true;
        }

        public virtual void ResetAnimation()
        {
            ResetMode = !ResetMode;
            Timer.Stop();
            Timer.Reset();
            Timer.Start();
            AnimationDrawn = false;
        }

        public virtual void ResetValues(bool fromStart)
        {
            ResetMode = false;
            Timer.Stop();
            Timer.Reset();
            AnimationDrawn = false;
            AnimationFromStart = fromStart;
        }

        public virtual bool AnimationActive()
        {
            return Timer.IsRunning || (AnimationDrawn && Repeat == EAnimationRepeat.None);
        }

        public virtual void setRect(SRectF rect)
        {
            OriginalRect = rect;
            LastRect = rect;
        }

        public virtual SRectF getRect()
        {
            return OriginalRect;
        }

        public virtual SRectF getRectChanges()
        {
            SRectF changes = new SRectF();
            changes.X = getRect().X - LastRect.X;
            changes.Y = getRect().Y - LastRect.Y;
            changes.W = getRect().W - LastRect.W;
            changes.H = getRect().H - LastRect.H;
            changes.Rotation = getRect().Rotation - LastRect.Rotation;
            return changes;
        }

        public virtual void setColor(SColorF color)
        {
            OriginalColor = color;
        }

        public virtual SColorF getColor()
        {
            return OriginalColor;
        }

        public virtual void setTexture(ref STexture texture)
        {
            OriginalTexture = texture;
        }

        public virtual STexture getTexture()
        {
            return OriginalTexture;
        }

        public virtual EAnimationEvent getEvent()
        {
            return Event;
        }

        public virtual bool isDrawn()
        {
            return AnimationDrawn;
        }

        public abstract void SetCurrentValues(SRectF rect, SColorF color, STexture texture);

        public abstract void Update();
    }
}
