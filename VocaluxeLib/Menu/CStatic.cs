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
using System.Collections.Generic;
using System.Drawing;
using System.Xml;
using VocaluxeLib.Draw;
using VocaluxeLib.Animations;

namespace VocaluxeLib.Menu
{
    struct SThemeStatic
    {
        public string Name;
        public string TextureName;
        public string ColorName;
    }

    public class CStatic: CMenuProperties, IMenuElement
    {
        private readonly int _PartyModeID;

        private SThemeStatic _Theme;
        private bool _ThemeLoaded;

        public string GetThemeName()
        {
            return _Theme.Name;
        }

        private CTexture _Texture;
        public new CTexture Texture
        {
            get { return _Texture ?? CBase.Theme.GetSkinTexture(_Theme.TextureName, _PartyModeID); }

            set { _Texture = value; }
        }

        private SColorF _Color;
        private SRectF _Rect;

        public bool Reflection;
        public float ReflectionSpace;
        public float ReflectionHeight;

        public bool Animation;
        private readonly List<CAnimation> _Animations = new List<CAnimation>();

        private bool _Selected;
        public bool Selected
        {
            get { return _Selected; }
            set
            {
                if (value != _Selected)
                {
                    if (value)
                        CAnimations.SetOnSelectAnim(this);
                    else
                    {
                        CAnimations.SetAfterSelectAnim(this);
                        if (Event == EAnimationEvent.None)
                        {
                            Rect = _Rect;
                            Color = _Color;
                        }
                    }
                    _Selected = value;
                }
            }
        }
        private bool _Visible = true;
        public bool Visible
        {
            get { return _Visible; }
            set
            {
                if (value)
                    CAnimations.SetOnVisibleAnim(this);
                else
                    CAnimations.SetAfterVisibleAnim(this);
                _Visible = value;
            }
        }

        public float Alpha = 1;

        public EAspect Aspect = EAspect.Stretch;

        public CStatic(int partyModeID)
        {
            _PartyModeID = partyModeID;
        }

        public CStatic(CStatic s)
        {
            _PartyModeID = s._PartyModeID;

            _Texture = s.Texture;
            _Color = new SColorF(s.Color);
            _Rect = new SRectF(s.Rect);
            Reflection = s.Reflection;
            ReflectionSpace = s.ReflectionHeight;
            ReflectionHeight = s.ReflectionSpace;

            Selected = s.Selected;
            Alpha = s.Alpha;
            Visible = s.Visible;

            Animation = s.Animation;
            Event = s.Event;
            _Animations = s._Animations;

            SetProperties();
        }

        public CStatic(int partyModeID, CTexture texture, SColorF color, SRectF rect)
        {
            _PartyModeID = partyModeID;

            _Texture = texture;
            _Color = color;
            _Rect = rect;

            SetProperties();
        }

        public CStatic(int partyModeID, string textureSkinName, SColorF color, SRectF rect)
        {
            _PartyModeID = partyModeID;
            _Theme.TextureName = textureSkinName;
            _Color = color;
            _Rect = rect;

            SetProperties();
        }

        public bool LoadTheme(string xmlPath, string elementName, CXMLReader xmlReader, int skinIndex)
        {
            string item = xmlPath + "/" + elementName;
            _ThemeLoaded = true;

            _ThemeLoaded &= xmlReader.GetValue(item + "/Skin", out _Theme.TextureName, String.Empty);

            _ThemeLoaded &= xmlReader.TryGetFloatValue(item + "/X", ref _Rect.X);
            _ThemeLoaded &= xmlReader.TryGetFloatValue(item + "/Y", ref _Rect.Y);
            _ThemeLoaded &= xmlReader.TryGetFloatValue(item + "/Z", ref _Rect.Z);
            _ThemeLoaded &= xmlReader.TryGetFloatValue(item + "/W", ref _Rect.W);
            _ThemeLoaded &= xmlReader.TryGetFloatValue(item + "/H", ref _Rect.H);

            if (xmlReader.GetValue(item + "/Color", out _Theme.ColorName, String.Empty))
                _ThemeLoaded &= CBase.Theme.GetColor(_Theme.ColorName, skinIndex, out _Color);
            else
            {
                _ThemeLoaded &= xmlReader.TryGetFloatValue(item + "/R", ref _Color.R);
                _ThemeLoaded &= xmlReader.TryGetFloatValue(item + "/G", ref _Color.G);
                _ThemeLoaded &= xmlReader.TryGetFloatValue(item + "/B", ref _Color.B);
                _ThemeLoaded &= xmlReader.TryGetFloatValue(item + "/A", ref _Color.A);
            }

            if (xmlReader.ItemExists(item + "/Reflection"))
            {
                Reflection = true;
                _ThemeLoaded &= xmlReader.TryGetFloatValue(item + "/Reflection/Space", ref ReflectionSpace);
                _ThemeLoaded &= xmlReader.TryGetFloatValue(item + "/Reflection/Height", ref ReflectionHeight);
            }
            else
                Reflection = false;

            //Animations
            int i = 1;
            while (xmlReader.ItemExists(item + "/Animation" + i.ToString()))
            {
                Animation = true;
                CAnimation anim;
                string animName = String.Empty;
                if (xmlReader.GetValue(item + "/Animation" + i + "/Name", out animName, String.Empty))
                {
                    _ThemeLoaded &= CBase.Theme.GetAnimation(animName, skinIndex, out anim);
                }
                else
                {
                    EAnimationType type = new EAnimationType();
                    _ThemeLoaded &= xmlReader.TryGetEnumValue(item + "/Animation" + i.ToString() + "/Type", ref type);
                    anim = new CAnimation(type, _PartyModeID);
                    _ThemeLoaded &= anim.LoadAnimation(item + "/Animation" + i.ToString(), xmlReader);
                }
                if (_ThemeLoaded)
                    _Animations.Add(anim);
                i++;
            }

            if (_ThemeLoaded)
            {
                _Theme.Name = elementName;
                LoadTextures();
                //Give static-data to animation
                if (Animation)
                {
                    foreach (CAnimation anim in _Animations)
                    {
                        anim.SetColor(Color);
                        anim.SetRect(Rect);
                        CAnimations.Add(this, anim);
                    }
                }
            }
            return _ThemeLoaded;
        }

        public bool SaveTheme(XmlWriter writer)
        {
            if (_ThemeLoaded)
            {
                writer.WriteStartElement(_Theme.Name);

                writer.WriteComment("<Skin>: Texture name");
                writer.WriteElementString("Skin", _Theme.TextureName);

                writer.WriteComment("<X>, <Y>, <Z>, <W>, <H>: Static position, width and height");
                writer.WriteElementString("X", Rect.X.ToString("#0"));
                writer.WriteElementString("Y", Rect.Y.ToString("#0"));
                writer.WriteElementString("Z", Rect.Z.ToString("#0.00"));
                writer.WriteElementString("W", Rect.W.ToString("#0"));
                writer.WriteElementString("H", Rect.H.ToString("#0"));

                writer.WriteComment("<Color>: Static color from ColorScheme (high priority)");
                writer.WriteComment("or <R>, <G>, <B>, <A> (lower priority)");
                if (!String.IsNullOrEmpty(_Theme.ColorName))
                    writer.WriteElementString("Color", _Theme.ColorName);
                else
                {
                    writer.WriteElementString("R", Color.R.ToString("#0.00"));
                    writer.WriteElementString("G", Color.G.ToString("#0.00"));
                    writer.WriteElementString("B", Color.B.ToString("#0.00"));
                    writer.WriteElementString("A", Color.A.ToString("#0.00"));
                }

                writer.WriteComment("<Reflection> If exists:");
                writer.WriteComment("   <Space>: Reflection Space");
                writer.WriteComment("   <Height>: Reflection Height");
                if (Reflection)
                {
                    writer.WriteStartElement("Reflection");
                    writer.WriteElementString("Space", ReflectionSpace.ToString("#0"));
                    writer.WriteElementString("Height", ReflectionHeight.ToString("#0"));
                    writer.WriteEndElement();
                }

                for (int i = 0; i < _Animations.Count; i++)
                {
                    writer.WriteStartElement("Animation" + (i + 1));
                    _Animations[i].SaveAnimation(writer);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                return true;
            }
            return false;
        }

        public void Draw(bool forceDraw = false)
        {
            Draw(1f, Rect.Z, Aspect, forceDraw);
        }

        public void Draw(EAspect aspect)
        {
            Draw(1f, Rect.Z, aspect);
        }

        public void Draw(float scale, EAspect aspect)
        {
            Draw(scale, Rect.Z, aspect);
        }

        public void Draw(float scale, float z, EAspect aspect, bool forceDraw = false)
        {
            CTexture texture = Texture;
            if (texture == null)
                texture = new CTexture((int)Rect.W, (int)Rect.H);

            SRectF bounds = new SRectF(
                Rect.X - Rect.W * (scale - 1f),
                Rect.Y - Rect.H * (scale - 1f),
                Rect.W + 2 * Rect.W * (scale - 1f),
                Rect.H + 2 * Rect.H * (scale - 1f),
                z);

            SRectF rect = bounds;

            if (aspect != EAspect.Stretch)
            {
                RectangleF bounds2 = new RectangleF(bounds.X, bounds.Y, bounds.W, bounds.H);
                RectangleF rect2;
                CHelper.SetRect(bounds2, out rect2, texture.OrigAspect, aspect);

                rect.X = rect2.X;
                rect.Y = rect2.Y;
                rect.W = rect2.Width;
                rect.H = rect2.Height;
            }

            SColorF color = new SColorF(Color.R, Color.G, Color.B, Color.A * Alpha);
            if (Visible || forceDraw || (CBase.Settings.GetGameState() == EGameState.EditTheme))
            {
                CBase.Drawing.DrawTexture(texture, rect, color, bounds);
                if (Reflection)
                    CBase.Drawing.DrawTextureReflection(texture, rect, color, bounds, ReflectionSpace, ReflectionHeight);
            }

            if (Selected && (CBase.Settings.GetGameState() == EGameState.EditTheme))
                CBase.Drawing.DrawColor(new SColorF(1f, 1f, 1f, 0.5f), rect);
        }

        public void UnloadTextures() {}

        public void LoadTextures()
        {
            if (!String.IsNullOrEmpty(_Theme.ColorName))
                _Color = CBase.Theme.GetColor(_Theme.ColorName, _PartyModeID);

            SetProperties();
        }

        public void ReloadTextures()
        {
            UnloadTextures();
            LoadTextures();
        }

        public override void SetProperties()
        {
            Color = _Color;
            Texture = _Texture;
            Rect = _Rect;
        }

        #region ThemeEdit
        public void MoveElement(int stepX, int stepY)
        {
            _Rect.X += stepX;
            _Rect.Y += stepY;
        }

        public void ResizeElement(int stepW, int stepH)
        {
            _Rect.W += stepW;
            if (_Rect.W <= 0)
                _Rect.W = 1;

            _Rect.H += stepH;
            if (_Rect.H <= 0)
                _Rect.H = 1;
        }
        #endregion ThemeEdit
    }
}