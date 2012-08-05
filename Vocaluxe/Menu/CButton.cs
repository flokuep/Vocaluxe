using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;

using Vocaluxe.Base;
using Vocaluxe.Lib.Draw;
using Vocaluxe.Menu.Animations;

namespace Vocaluxe.Menu
{
    struct SThemeButton
    {
        public string Name;

        public string ColorName;
        public string SColorName;

        public string TextureName;
        public string STextureName;
    }

    class CButton : IMenuElement, IMenuProperties
    {
        private SThemeButton _Theme;
        private bool _ThemeLoaded;

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
        public SColorF SColor;

        private STexture _Texture;
        public STexture Texture
        {
            get { return _Texture; }
            set { _Texture = value; }
        }

        public bool SelText;
        public CText Text;
        public CText SText;
        
        public bool Reflection;
        public float ReflectionSpace;
        public float ReflectionHeight;

        public bool SReflection;
        public float SReflectionSpace;
        public float SReflectionHeight;

        public bool Animation;
        private List<CAnimation> _Animations;

        private bool _Selected;
        public bool Pressed;
        public bool Selected
        {
            get { return _Selected; }
            set
            {
                _Selected = value;
                Text.Selected = value;
            }
        }
        private bool _Visible;
        public bool Visible
        {
            get { return _Visible; }
            set { _Visible = value; }
        }

        public CButton()
        {
            _Theme = new SThemeButton();
            Rect = new SRectF();
            Color = new SColorF();
            SColor = new SColorF();

            SelText = false;
            Text = new CText();
            SText = new CText();
            Selected = false;
            Visible = true;

            Reflection = false;
            ReflectionSpace = 0f;
            ReflectionHeight = 0f;

            SReflection = false;
            SReflectionSpace = 0f;
            SReflectionHeight = 0f;

            Animation = false;
            _Animations = new List<CAnimation>();
        }

        public bool LoadTheme(string XmlPath, string ElementName, XPathNavigator navigator, int SkinIndex)
        {
            string item = XmlPath + "/" + ElementName;
            _ThemeLoaded = true;

            _ThemeLoaded &= CHelper.GetValueFromXML(item + "/Skin", navigator, ref _Theme.TextureName, String.Empty);
            _ThemeLoaded &= CHelper.GetValueFromXML(item + "/SkinSelected", navigator, ref _Theme.STextureName, String.Empty);
            
            _ThemeLoaded &= CHelper.TryGetFloatValueFromXML(item + "/X", navigator, ref _Rect.X);
            _ThemeLoaded &= CHelper.TryGetFloatValueFromXML(item + "/Y", navigator, ref _Rect.Y);
            _ThemeLoaded &= CHelper.TryGetFloatValueFromXML(item + "/Z", navigator, ref _Rect.Z);
            _ThemeLoaded &= CHelper.TryGetFloatValueFromXML(item + "/W", navigator, ref _Rect.W);
            _ThemeLoaded &= CHelper.TryGetFloatValueFromXML(item + "/H", navigator, ref _Rect.H);

            if (CHelper.GetValueFromXML(item + "/Color", navigator, ref _Theme.ColorName, String.Empty))
            {
                _ThemeLoaded &= CTheme.GetColor(_Theme.ColorName, SkinIndex, ref _Color);
            }
            else
            {
                _ThemeLoaded &= CHelper.TryGetFloatValueFromXML(item + "/R", navigator, ref _Color.R);
                _ThemeLoaded &= CHelper.TryGetFloatValueFromXML(item + "/G", navigator, ref _Color.G);
                _ThemeLoaded &= CHelper.TryGetFloatValueFromXML(item + "/B", navigator, ref _Color.B);
                _ThemeLoaded &= CHelper.TryGetFloatValueFromXML(item + "/A", navigator, ref _Color.A);
            }

            if (CHelper.GetValueFromXML(item + "/SColor", navigator, ref _Theme.SColorName, String.Empty))
            {
                _ThemeLoaded &= CTheme.GetColor(_Theme.SColorName, SkinIndex, ref SColor);
            }
            else
            {
                _ThemeLoaded &= CHelper.TryGetFloatValueFromXML(item + "/SR", navigator, ref SColor.R);
                _ThemeLoaded &= CHelper.TryGetFloatValueFromXML(item + "/SG", navigator, ref SColor.G);
                _ThemeLoaded &= CHelper.TryGetFloatValueFromXML(item + "/SB", navigator, ref SColor.B);
                _ThemeLoaded &= CHelper.TryGetFloatValueFromXML(item + "/SA", navigator, ref SColor.A);
            }

            _ThemeLoaded &= Text.LoadTheme(item, "Text", navigator, SkinIndex, true);
            Text.Z = Rect.Z;
            if (CHelper.ItemExistsInXML(item + "/SText", navigator))
            {
                SelText = true;
                _ThemeLoaded &= SText.LoadTheme(item, "SText", navigator, SkinIndex, true);
                SText.Z = Rect.Z;
            }
            
            
            //Reflections
            if (CHelper.ItemExistsInXML(item + "/Reflection", navigator))
            {
                Reflection = true;
                _ThemeLoaded &= CHelper.TryGetFloatValueFromXML(item + "/Reflection/Space", navigator, ref ReflectionSpace);
                _ThemeLoaded &= CHelper.TryGetFloatValueFromXML(item + "/Reflection/Height", navigator, ref ReflectionHeight);
            }
            else
                Reflection = false;

            if (CHelper.ItemExistsInXML(item + "/SReflection", navigator))
            {
                SReflection = true;
                _ThemeLoaded &= CHelper.TryGetFloatValueFromXML(item + "/SReflection/Space", navigator, ref SReflectionSpace);
                _ThemeLoaded &= CHelper.TryGetFloatValueFromXML(item + "/SReflection/Height", navigator, ref SReflectionHeight);
            }
            else
                SReflection = false;

            //Animations
            int i = 1;
            while (CHelper.ItemExistsInXML(item + "/Animation"+i.ToString(), navigator))
            {
                Animation = true;
                EAnimationType type = new EAnimationType();
                _ThemeLoaded &= CHelper.TryGetEnumValueFromXML<EAnimationType>(item + "/Animation" + i.ToString() + "/Type", navigator, ref type);
                CAnimation anim = new CAnimation(type);
                _Animations.Add(anim);
                i++;
            }

            //Load Animations
            if (Animation)
            {
                i = 1;
                foreach (CAnimation anim in _Animations)
                {
                    _ThemeLoaded &= anim.LoadAnimation(item + "/Animation" + i.ToString(), navigator);
                    i++;
                }
            }

            if (_ThemeLoaded)
            {
                _Theme.Name = ElementName;
                LoadTextures();
                //Give button-data to animation
                if (Animation)
                {
                    foreach (CAnimation anim in _Animations) 
                    {
                        anim.setColor(Color);
                        anim.setRect(Rect);
                        anim.setTexture(Texture);
                        //Add this to CAnimations
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
                writer.WriteElementString("SkinSelected", _Theme.STextureName);

                writer.WriteComment("<X>, <Y>, <Z>, <W>, <H>: Button position, width and height");
                writer.WriteElementString("X", Rect.X.ToString("#0"));
                writer.WriteElementString("Y", Rect.Y.ToString("#0"));
                writer.WriteElementString("Z", Rect.Z.ToString("#0.00"));
                writer.WriteElementString("W", Rect.W.ToString("#0"));
                writer.WriteElementString("H", Rect.H.ToString("#0"));

                writer.WriteComment("<Color>: Button color from ColorScheme (high priority)");
                writer.WriteComment("or <R>, <G>, <B>, <A> (lower priority)");
                if (_Theme.ColorName != String.Empty)
                {
                    writer.WriteElementString("Color", _Theme.ColorName);
                }
                else
                {
                    writer.WriteElementString("R", Color.R.ToString("#0.00"));
                    writer.WriteElementString("G", Color.G.ToString("#0.00"));
                    writer.WriteElementString("B", Color.B.ToString("#0.00"));
                    writer.WriteElementString("A", Color.A.ToString("#0.00"));
                }

                writer.WriteComment("<SColor>: Selected button color from ColorScheme (high priority)");
                writer.WriteComment("or <SR>, <SG>, <SB>, <SA> (lower priority)");
                if (_Theme.SColorName != String.Empty)
                {
                    writer.WriteElementString("SColor", _Theme.SColorName);
                }
                else
                {
                    writer.WriteElementString("SR", SColor.R.ToString("#0.00"));
                    writer.WriteElementString("SG", SColor.G.ToString("#0.00"));
                    writer.WriteElementString("SB", SColor.B.ToString("#0.00"));
                    writer.WriteElementString("SA", SColor.A.ToString("#0.00"));
                }

                Text.SaveTheme(writer);
                if (SelText)
                    SText.SaveTheme(writer);

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

                writer.WriteComment("<SReflection> If exists:");
                writer.WriteComment("   <Space>: Reflection Space of selected button");
                writer.WriteComment("   <Height>: Reflection Height of selected button");
                if (SReflection)
                {
                    writer.WriteStartElement("SReflection");
                    writer.WriteElementString("Space", ReflectionSpace.ToString("#0"));
                    writer.WriteElementString("Height", ReflectionHeight.ToString("#0"));
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();

                return true;
            }
            return false;
        }

        public void Draw()
        {
            Draw(false);
        }

        public void ForceDraw()
        {
            Draw(true);
        }

        public void Draw(bool ForceDraw)
        {
            if (!Visible && CSettings.GameState != EGameState.EditTheme && !ForceDraw)
                return;

            if (!Selected && !Pressed)
            {
                CDraw.DrawTexture(Texture, Rect, Color);
                Text.DrawRelative(Rect.X, Rect.Y);
                if (Reflection)
                {
                    CDraw.DrawTextureReflection(Texture, Rect, Color, Rect, ReflectionSpace, ReflectionHeight);
                    Text.DrawRelative(Rect.X, Rect.Y, ReflectionSpace, ReflectionHeight, Rect.H);
                }
            }
            else if(!SelText)
            {
                Texture = CTheme.GetSkinTexture(_Theme.STextureName);
                CDraw.DrawTexture(Texture, Rect, SColor);
                Text.DrawRelative(Rect.X, Rect.Y);
                if (Reflection)
                {
                    CDraw.DrawTextureReflection(Texture, Rect, SColor, Rect, ReflectionSpace, ReflectionHeight);
                    Text.DrawRelative(Rect.X, Rect.Y, ReflectionSpace, ReflectionHeight, Rect.H);
                }
            }
            else if(SelText)
            {
                Texture = CTheme.GetSkinTexture(_Theme.STextureName);
                CDraw.DrawTexture(Texture, Rect, SColor);
                SText.DrawRelative(Rect.X, Rect.Y);
                if (Reflection)
                {
                    CDraw.DrawTextureReflection(Texture, Rect, SColor, Rect, ReflectionSpace, ReflectionHeight);
                    SText.DrawRelative(Rect.X, Rect.Y, ReflectionSpace, ReflectionHeight, Rect.H);
                }
            }
        }

        public void UnloadTextures()
        {
            Text.UnloadTextures();
        }

        public void LoadTextures()
        {
            Text.LoadTextures();
            Texture = CTheme.GetSkinTexture(_Theme.TextureName);

            if (_Theme.ColorName != String.Empty)
                Color = CTheme.GetColor(_Theme.ColorName);

            if (_Theme.SColorName != String.Empty)
                SColor = CTheme.GetColor(_Theme.SColorName);
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
