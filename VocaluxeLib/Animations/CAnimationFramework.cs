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
using System.Diagnostics;
using System.Xml;
using VocaluxeLib.Menu;
using VocaluxeLib.Draw;

namespace VocaluxeLib.Animations
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

    public abstract class CAnimationFramework : IAnimation
    {
        public bool AnimationLoaded;

        public EAnimationType Type;
        public EAnimationRepeat Repeat;
        public EAnimationEvent Event;

        public SRectF OriginalRect;
        public SRectF LastRect;
        public SColorF OriginalColor;
        public CTexture OriginalTexture;

        public float Time;

        public Stopwatch Timer = new Stopwatch();
        public bool ResetMode = false;
        public bool AnimationDrawn = false;
        public bool AnimationFromStart = true;

        protected int _PartyModeID;

        public CAnimationFramework(int partyModeID)
        {
            _PartyModeID = partyModeID;
            Init();
        }

        public abstract void Init();

        public virtual bool LoadAnimation(string item, CXMLReader xmlReader)
        {
            AnimationLoaded = true;

            AnimationLoaded &= xmlReader.TryGetEnumValue(item + "/Event", ref Event);

            if (AnimationLoaded)
                AnimationLoaded = Event != EAnimationEvent.None;

            return AnimationLoaded;
        }

        public virtual bool SaveAnimation(XmlWriter writer)
        {
            writer.WriteComment("<Type>: Type of animation: " + CHelper.ListStrings(Enum.GetNames(typeof(EAnimationType))));
            writer.WriteElementString("Type", Enum.GetName(typeof(EAnimationType), Type));
            writer.WriteComment("<Event>: Trigger for animation: " + CHelper.ListStrings(Enum.GetNames(typeof(EAnimationEvent))));
            writer.WriteElementString("Event", Enum.GetName(typeof(EAnimationEvent), Event));
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
            if (Repeat == EAnimationRepeat.None || Repeat == EAnimationRepeat.Reset || Repeat == EAnimationRepeat.OnlyReset)
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

        public virtual void SetRect(SRectF rect)
        {
            OriginalRect = rect;
            LastRect = rect;
        }

        public virtual SRectF GetRect()
        {
            return OriginalRect;
        }

        public virtual SRectF GetRectChanges()
        {
            SRectF changes = new SRectF();
            changes.X = GetRect().X - LastRect.X;
            changes.Y = GetRect().Y - LastRect.Y;
            changes.W = GetRect().W - LastRect.W;
            changes.H = GetRect().H - LastRect.H;
            changes.Rotation = GetRect().Rotation - LastRect.Rotation;
            return changes;
        }

        public virtual void SetColor(SColorF color)
        {
            OriginalColor = color;
        }

        public virtual SColorF GetColor()
        {
            return OriginalColor;
        }

        public virtual void SetTexture(ref CTexture texture)
        {
            OriginalTexture = texture;
        }

        public virtual CTexture GetTexture()
        {
            return OriginalTexture;
        }

        public virtual EAnimationEvent GetEvent()
        {
            return Event;
        }

        public virtual bool IsDrawn()
        {
            return AnimationDrawn;
        }

        public abstract void SetCurrentValues(SRectF rect, SColorF color);

        public abstract void Update();
    }
}