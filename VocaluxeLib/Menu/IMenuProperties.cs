using System;
using System.Collections.Generic;
using System.Text;

using VocaluxeLib.Menu.Animations;

namespace VocaluxeLib.Menu
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

        EAnimationEvent Event
        {
            get;
            set;
        }
    }
}
