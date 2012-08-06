using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Xml.XPath;

using Vocaluxe.Base;
using Vocaluxe.Lib.Draw;

namespace Vocaluxe.Menu.Animations
{
    public enum EAnimationType
    {
        Resize,
        MoveLinear,
        Video
    }

    public enum EAnimationRepeat
    {
        None,
        Reset,
        Repeat,
        RepeatWithReset
    }

    public abstract class CAnimationFramework:IAnimation
    {
        public bool _AnimationLoaded;

        public EAnimationType Type;
        public EAnimationRepeat Repeat;

        public SRectF OriginalRect;
        public SRectF LastRect;
        public SColorF OriginalColor;
        public  STexture OriginalTexture;

        public EOffOn Reset;
        public float Time;

        public Stopwatch Timer = new Stopwatch();
        public bool ResetMode = false;
        public bool AnimationDrawn = false;

        public abstract void Init();

        public virtual bool LoadAnimation(string item, XPathNavigator navigator)
        {
            _AnimationLoaded = true;

            return _AnimationLoaded;
        }

        public virtual bool SaveAnimation(XmlWriter writer)
        {
            return false;
        }

        public virtual void StartAnimation()
        {
            Timer.Start();
        }

        public virtual void StopAnimation()
        {
            Timer.Stop();
            Timer.Reset();
            if(Repeat == EAnimationRepeat.None)
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

        public virtual bool AnimationActive()
        {
            return Timer.IsRunning || (AnimationDrawn && Repeat == EAnimationRepeat.None);
        }

        public virtual void setRect(SRectF rect)
        {
            OriginalRect = rect;
        }

        public virtual SRectF getRect()
        {
            return OriginalRect;
        }

        public virtual float[] getRectChanges()
        {
            float[] changes = new float[4];
            changes[0] = getRect().X - LastRect.X;
            changes[1] = getRect().Y - LastRect.Y;
            changes[2] = getRect().W - LastRect.W;
            changes[3] = getRect().H - LastRect.H;
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

        public virtual bool isDrawn()
        {
            return AnimationDrawn;
        }

        public virtual void setAnimationReset(EOffOn reset)
        {
            Reset = reset;
        }

        public virtual EOffOn getAnimationReset()
        {
            return Reset;
        }

        public abstract void Update();
    }
}
