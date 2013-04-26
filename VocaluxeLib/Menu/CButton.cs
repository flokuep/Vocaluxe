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
using System.Xml;
using VocaluxeLib.Animations;

namespace VocaluxeLib.Menu
{
    struct SThemeButton
    {
        public string Name;

        public string ColorName;
        public string SelColorName;

        public string TextureName;
        public string SelTextureName;
    }

    public class CButton : IMenuElement, IMenuProperties
    {
        private SThemeButton _Theme;
        private bool _ThemeLoaded;
        private readonly int _PartyModeID;

        private SRectF _Rect;
        public SRectF Rect
        {
            get { return _Rect; }
            set { _Rect = value; }
        }
        private SColorF _Color;
        public SColorF Color
        {
            get { return _Color; }
            set { _Color = value; }
        }

        public STexture SelTexture;
        public SColorF SelColor;

        private STexture _Texture;
        public STexture Texture
        {
            get { return _Texture; }
            set { _Texture = value; }
        }

        public readonly CText Text;
        private readonly CText _SelText;
        private bool _IsSelText;

        private bool _Reflection;
        private float _ReflectionSpace;
        private float _ReflectionHeight;

        private bool _SelReflection;
        private float _SelReflectionSpace;
        private float _SelReflectionHeight;

        public bool Animation;
        private readonly List<CAnimation> _Animations;
        public EAnimationEvent Event { get; set; }

        public bool Pressed;

        public bool EditMode
        {
            get { return Text.EditMode; }
            set
            {
                Text.EditMode = value;
                _SelText.EditMode = value;
            }
        }

        private bool _Selected;
        public bool Selected
        {
            get { return _Selected; }
            set
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
                Text.Selected = value;
            }
        }
        private bool _Visible;
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

        public bool Enabled;

        public string GetThemeName()
        {
            return _Theme.Name;
        }

        public CButton(int partyModeID)
        {
            _PartyModeID = partyModeID;
            _Theme = new SThemeButton();

            _Rect = new SRectF();
            _Color = new SColorF();
            Rect = new SRectF();
            Color = new SColorF();
            SelColor = new SColorF();

            Texture = new STexture();
            SelTexture = new STexture();
            _IsSelText = false;
            Text = new CText(_PartyModeID);
            _SelText = new CText(_PartyModeID);
            Selected = false;
            Visible = true;
            EditMode = false;
            Enabled = true;

            _Reflection = false;
            _ReflectionSpace = 0f;
            _ReflectionHeight = 0f;

            Animation = false;
            _Animations = new List<CAnimation>();
            _SelReflection = false;
            _SelReflectionSpace = 0f;
            _SelReflectionHeight = 0f;
        }

        public CButton(CButton button)
        {
            _PartyModeID = button._PartyModeID;
            _Theme = new SThemeButton
                {
                    ColorName = button._Theme.ColorName,
                    SelColorName = button._Theme.SelColorName,
                    TextureName = button._Theme.TextureName,
                    SelTextureName = button._Theme.SelTextureName
                };

            Rect = new SRectF(button.Rect);
            Color = new SColorF(button.Color);
            SelColor = new SColorF(button.Color);
            Texture = button.Texture;
            SelTexture = button.SelTexture;

            _IsSelText = false;
            Text = new CText(button.Text);
            _SelText = new CText(button._SelText);
            Selected = false;
            Visible = true;
            EditMode = false;
            _ThemeLoaded = false;
            Enabled = button.Enabled;

            _Reflection = button._Reflection;
            _ReflectionHeight = button._ReflectionHeight;
            _ReflectionSpace = button._ReflectionSpace;

            _SelReflection = button._SelReflection;
            _SelReflectionHeight = button._SelReflectionHeight;
            _SelReflectionSpace = button._SelReflectionSpace;
        }

        public bool LoadTheme(string xmlPath, string elementName, CXMLReader xmlReader, int skinIndex)
        {
            string item = xmlPath + "/" + elementName;
            _ThemeLoaded = true;

            _ThemeLoaded &= xmlReader.GetValue(item + "/Skin", out _Theme.TextureName, String.Empty);
            _ThemeLoaded &= xmlReader.GetValue(item + "/SkinSelected", out _Theme.SelTextureName, String.Empty);

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

            if (xmlReader.GetValue(item + "/SColor", out _Theme.SelColorName, String.Empty))
                _ThemeLoaded &= CBase.Theme.GetColor(_Theme.SelColorName, skinIndex, out SelColor);
            else
            {
                _ThemeLoaded &= xmlReader.TryGetFloatValue(item + "/SR", ref SelColor.R);
                _ThemeLoaded &= xmlReader.TryGetFloatValue(item + "/SG", ref SelColor.G);
                _ThemeLoaded &= xmlReader.TryGetFloatValue(item + "/SB", ref SelColor.B);
                _ThemeLoaded &= xmlReader.TryGetFloatValue(item + "/SA", ref SelColor.A);
            }

            _ThemeLoaded &= Text.LoadTheme(item, "Text", xmlReader, skinIndex, true);
            Text.Z = Rect.Z;
            if (xmlReader.ItemExists(item + "/SText"))
            {
                _IsSelText = true;
                _ThemeLoaded &= _SelText.LoadTheme(item, "SText", xmlReader, skinIndex, true);
                _SelText.Z = Rect.Z;
            }


            //Reflections
            if (xmlReader.ItemExists(item + "/Reflection"))
            {
                _Reflection = true;
                _ThemeLoaded &= xmlReader.TryGetFloatValue(item + "/Reflection/Space", ref _ReflectionSpace);
                _ThemeLoaded &= xmlReader.TryGetFloatValue(item + "/Reflection/Height", ref _ReflectionHeight);
            }
            else
                _Reflection = false;

            if (xmlReader.ItemExists(item + "/SReflection"))
            {
                _SelReflection = true;
                _ThemeLoaded &= xmlReader.TryGetFloatValue(item + "/SReflection/Space", ref _SelReflectionSpace);
                _ThemeLoaded &= xmlReader.TryGetFloatValue(item + "/SReflection/Height", ref _SelReflectionHeight);
            }
            else
                _SelReflection = false;

            //Animations
            int i = 1;
            while (xmlReader.ItemExists(item + "/Animation" + i.ToString()))
            {
                Animation = true;
                EAnimationType type = new EAnimationType();
                _ThemeLoaded &= xmlReader.TryGetEnumValue(item + "/Animation" + i.ToString() + "/Type", ref type);
                CAnimation anim = new CAnimation(type, _PartyModeID);
                _Animations.Add(anim);
                i++;
            }

            //Load Animations
            if (Animation)
            {
                i = 1;
                foreach (CAnimation anim in _Animations)
                {
                    _ThemeLoaded &= anim.LoadAnimation(item + "/Animation" + i.ToString(), xmlReader);
                    i++;
                }
            }

            if (_ThemeLoaded)
            {
                _Theme.Name = elementName;
                LoadTextures();
                //Give button-data to animation
                if (Animation)
                {
                    foreach (CAnimation anim in _Animations)
                    {
                        anim.SetColor(Color);
                        anim.SetRect(Rect);
                        anim.SetTexture(ref _Texture);
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

                writer.WriteComment("<SkinSelected>: Texture name for selected button");
                writer.WriteElementString("SkinSelected", _Theme.SelTextureName);

                writer.WriteComment("<X>, <Y>, <Z>, <W>, <H>: Button position, width and height");
                writer.WriteElementString("X", Rect.X.ToString("#0"));
                writer.WriteElementString("Y", Rect.Y.ToString("#0"));
                writer.WriteElementString("Z", Rect.Z.ToString("#0.00"));
                writer.WriteElementString("W", Rect.W.ToString("#0"));
                writer.WriteElementString("H", Rect.H.ToString("#0"));

                writer.WriteComment("<Color>: Button color from ColorScheme (high priority)");
                writer.WriteComment("or <R>, <G>, <B>, <A> (lower priority)");
                if (_Theme.ColorName != "")
                    writer.WriteElementString("Color", _Theme.ColorName);
                else
                {
                    writer.WriteElementString("R", Color.R.ToString("#0.00"));
                    writer.WriteElementString("G", Color.G.ToString("#0.00"));
                    writer.WriteElementString("B", Color.B.ToString("#0.00"));
                    writer.WriteElementString("A", Color.A.ToString("#0.00"));
                }

                writer.WriteComment("<SColor>: Selected button color from ColorScheme (high priority)");
                writer.WriteComment("or <SR>, <SG>, <SB>, <SA> (lower priority)");
                if (_Theme.SelColorName != "")
                    writer.WriteElementString("SColor", _Theme.SelColorName);
                else
                {
                    writer.WriteElementString("SR", SelColor.R.ToString("#0.00"));
                    writer.WriteElementString("SG", SelColor.G.ToString("#0.00"));
                    writer.WriteElementString("SB", SelColor.B.ToString("#0.00"));
                    writer.WriteElementString("SA", SelColor.A.ToString("#0.00"));
                }

                Text.SaveTheme(writer);
                if (_IsSelText)
                    _SelText.SaveTheme(writer);

                writer.WriteComment("<Reflection> If exists:");
                writer.WriteComment("   <Space>: Reflection Space");
                writer.WriteComment("   <Height>: Reflection Height");
                if (_Reflection)
                {
                    writer.WriteStartElement("Reflection");
                    writer.WriteElementString("Space", _ReflectionSpace.ToString("#0"));
                    writer.WriteElementString("Height", _ReflectionHeight.ToString("#0"));
                    writer.WriteEndElement();
                }

                writer.WriteComment("<SReflection> If exists:");
                writer.WriteComment("   <Space>: Reflection Space of selected button");
                writer.WriteComment("   <Height>: Reflection Height of selected button");
                if (_SelReflection)
                {
                    writer.WriteStartElement("SReflection");
                    writer.WriteElementString("Space", _ReflectionSpace.ToString("#0"));
                    writer.WriteElementString("Height", _ReflectionHeight.ToString("#0"));
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();

                return true;
            }
            return false;
        }

        public void Draw(bool forceDraw = false)
        {
            if (!Visible && CBase.Settings.GetGameState() != EGameState.EditTheme && !forceDraw)
                return;

            STexture texture;

            if (!Selected && !Pressed || !Enabled)
            {
                texture = Texture.Index != -1 ? Texture : CBase.Theme.GetSkinTexture(_Theme.TextureName, _PartyModeID);

                CBase.Drawing.DrawTexture(texture, Rect, Color);
                Text.DrawRelative(Rect.X, Rect.Y);

                if (_Reflection)
                {
                    CBase.Drawing.DrawTextureReflection(texture, Rect, Color, Rect, _ReflectionSpace, _ReflectionHeight);
                    Text.DrawRelative(Rect.X, Rect.Y, _ReflectionSpace, _ReflectionHeight, Rect.H);
                }
                else
                    Text.DrawRelative(Rect.X, Rect.Y);
            }
            else if (!_IsSelText)
            {
                texture = Texture.Index != -1 ? Texture : CBase.Theme.GetSkinTexture(_Theme.SelTextureName, _PartyModeID);

                bool anim = (Event == EAnimationEvent.OnSelected && CAnimations.AnimAvailable(this, EAnimationEvent.OnSelected)) ||
                            (Event == EAnimationEvent.Selected && CAnimations.AnimAvailable(this, EAnimationEvent.Selected));
                if (anim)
                    CBase.Drawing.DrawTexture(texture, Rect, Color);
                else
                    CBase.Drawing.DrawTexture(texture, Rect, SelColor);
                Text.DrawRelative(Rect.X, Rect.Y);

                if (_Reflection)
                {
                    if (anim)
                        CBase.Drawing.DrawTextureReflection(texture, Rect, Color, Rect, _ReflectionSpace, _ReflectionHeight);
                    else
                        CBase.Drawing.DrawTextureReflection(texture, Rect, SelColor, Rect, _ReflectionSpace, _ReflectionHeight);
                    Text.DrawRelative(Rect.X, Rect.Y, _ReflectionSpace, _ReflectionHeight, Rect.H);
                }
                else
                    Text.DrawRelative(Rect.X, Rect.Y);
            }
            else if (_IsSelText)
            {
                texture = SelTexture.Index != -1 ? SelTexture : CBase.Theme.GetSkinTexture(_Theme.SelTextureName, _PartyModeID);

                CBase.Drawing.DrawTexture(texture, Rect, SelColor);
                _SelText.DrawRelative(Rect.X, Rect.Y);

                if (_Reflection)
                {
                    CBase.Drawing.DrawTextureReflection(texture, Rect, SelColor, Rect, _ReflectionSpace, _ReflectionHeight);
                    _SelText.DrawRelative(Rect.X, Rect.Y, _ReflectionSpace, _ReflectionHeight, Rect.H);
                }
                else
                    _SelText.DrawRelative(Rect.X, Rect.Y);
            }
        }

        public void ProcessMouseMove(int x, int y)
        {
            Selected = CHelper.IsInBounds(Rect, x, y);
        }

        public void UnloadTextures()
        {
            Text.UnloadTextures();
        }

        public void LoadTextures()
        {
            Text.LoadTextures();
            Texture = CBase.Theme.GetSkinTexture(_Theme.TextureName, _PartyModeID);

            if (_Theme.ColorName != "")
                Color = CBase.Theme.GetColor(_Theme.ColorName, _PartyModeID);

            if (_Theme.SelColorName != "")
                SelColor = CBase.Theme.GetColor(_Theme.SelColorName, _PartyModeID);
        }

        public void ReloadTextures()
        {
            UnloadTextures();
            LoadTextures();
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
            if (Rect.W <= 0)
                _Rect.W = 1;

            _Rect.H += stepH;
            if (Rect.H <= 0)
                _Rect.H = 1;
        }
        #endregion ThemeEdit
    }
}