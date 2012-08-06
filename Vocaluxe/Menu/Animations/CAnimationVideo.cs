using System;
using System.Collections.Generic;
using System.Text;

using Vocaluxe.Base;
using Vocaluxe.Lib.Draw;
using System.Drawing;

namespace Vocaluxe.Menu.Animations
{
    public class CAnimationVideo : CAnimationFramework
    {
        string _VideoName;
        STexture _VideoTexture;

        public CAnimationVideo()
        {
            Init();
        }

        public override void Init()
        {
            Type = EAnimationType.Video;
            Repeat = EAnimationRepeat.Repeat;
        }

        public override bool LoadAnimation(string item, System.Xml.XPath.XPathNavigator navigator)
        {
            _AnimationLoaded = true;
            _AnimationLoaded &= base.LoadAnimation(item, navigator);
            _AnimationLoaded &= CHelper.GetValueFromXML(item + "/Video", navigator, ref _VideoName, String.Empty);

            return _AnimationLoaded;
        }

        public override STexture getTexture()
        {
            return _VideoTexture;
        }

        public override void Update()
        {
            _VideoTexture = CTheme.GetSkinVideoTexture(_VideoName);
        }
    }
}