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

using VocaluxeLib.Animations;
using VocaluxeLib.Draw;

namespace VocaluxeLib.Menu
{
    public abstract class CMenuProperties
    {
        protected SRectF _Rect;
        protected SColorF _Color;
        protected CTexture _Texture;

        public SRectF OriginalRect
        {
            set 
            { 
                _Rect = value;
                Rect = value; 
            }
            get { return _Rect; }
        }

        public float OriginalRectX
        {
            set 
            { 
                _Rect.X = value;
                Rect.X = value; 
            }
            get { return _Rect.X; }
        }

        public float OriginalRectY
        {
            set 
            { 
                _Rect.Y = value;
                Rect.Y = value; 
            }
            get { return _Rect.Y; }
        }

        public float OriginalRectH
        {
            set 
            { 
                _Rect.H = value;
                Rect.H = value;
            }
            get { return _Rect.H; }
        }

        public float OriginalRectW
        {
            set 
            { 
                _Rect.W = value;
                Rect.W = value;
            }
            get { return _Rect.W; }
        }

        public float OriginalRectZ
        {
            set 
            { 
                _Rect.Z = value;
                Rect.Z = value;
            }
            get { return _Rect.Z; }
        }

        public SColorF OriginalColor
        {
            set 
            { 
                _Color = value;
                Color = value; 
            }
            get { return _Color; }
        }

        public float OriginalColorR
        {
            set 
            { 
                _Color.R = value;
                Color.R = value; 
            }
            get { return _Color.R; }
        }

        public float OriginalColorG
        {
            set 
            { 
                _Color.G = value;
                Color.G = value; 
            }
            get { return _Color.G; }
        }

        public float OriginalColorB
        {
            set
            { 
                _Color.B = value;
                Color.B = value;
            }
            get { return _Color.B; }
        }

        public float OriginalColorA
        {
            set 
            { 
                _Color.A = value;
                Color.A = value; 
            }
            get { return _Color.A; }
        }

        public CTexture OriginalTexture
        {
            set 
            {
                _Texture = value; 
                Texture = value; 
            }
            get { return _Texture; }
        }

        public bool Visible;
        public SRectF Rect;
        public SColorF Color;
        public CTexture Texture;
        public EAnimationEvent Event;

        public abstract void SetProperties();
    }
}