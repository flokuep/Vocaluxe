using System;
using System.Collections.Generic;
using System.Text;

using Vocaluxe.Base;
using Vocaluxe.Lib.Draw;

namespace Vocaluxe.Menu
{
    public interface IMenuProperties
    {
        bool Visible
        {
            get;
            set;
        }

        SRectF Rect
        {
            get;
            set;
        }

        SColorF Color
        {
            get;
            set;
        }

        STexture Texture
        {
            get;
            set;
        }
    }
}
