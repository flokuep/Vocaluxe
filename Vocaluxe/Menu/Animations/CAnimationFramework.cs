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
        Resize
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
        public EOffOn Reset;
        public float Speed;

        public Stopwatch Timer = new Stopwatch();
        public bool ResetMode = false;
        public bool AnimationDrawn = false;

        public abstract void Init();

        public virtual bool LoadAnimation(string item, XPathNavigator navigator)
        {
            _AnimationLoaded = true;

            _AnimationLoaded &= CHelper.TryGetFloatValueFromXML(item + "/Speed", navigator, ref Speed);
            _AnimationLoaded &= CHelper.TryGetEnumValueFromXML<EAnimationRepeat>(item + "/Repeat", navigator, ref Repeat);

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
        }

        public virtual SRectF getRect()
        {
            return new SRectF();
        }

        public virtual void setColor(SColorF color)
        {
        }

        public virtual SColorF getColor()
        {
            return new SColorF();
        }

        public virtual void setTexture(STexture texture)
        {
        }

        public virtual STexture getTexture()
        {
            return new STexture();
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
