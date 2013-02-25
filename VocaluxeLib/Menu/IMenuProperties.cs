using System;
using System.Collections.Generic;
using System.Text;

using Vocaluxe.Menu.Animations;

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

        EAnimationEvent Event
        {
            get;
            set;
        }
    }
}
