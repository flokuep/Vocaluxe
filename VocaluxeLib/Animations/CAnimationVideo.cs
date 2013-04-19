using System;
using System.Collections.Generic;
using System.Text;

using System.Drawing;

namespace VocaluxeLib.Menu.Animations
{
    public class CAnimationVideo : CAnimationFramework
    {
        string _VideoName;
        STexture _VideoTexture;

        public CAnimationVideo(int PartyModeID)
            : base(PartyModeID)
        {
        }

        public override void Init()
        {
            Type = EAnimationType.Video;
            Repeat = EAnimationRepeat.Repeat;
        }

        public override bool LoadAnimation(string item, CXMLReader xmlReader)
        {
            _AnimationLoaded = true;
            _AnimationLoaded &= base.LoadAnimation(item, xmlReader);
            _AnimationLoaded &= xmlReader.GetValue(item + "/Video", ref _VideoName, String.Empty);

            return _AnimationLoaded;
        }

        public override STexture getTexture()
        {
            return _VideoTexture;
        }

        public override void SetCurrentValues(SRectF rect, SColorF color, STexture texture)
        {
        }

        public override void Update()
        {
            _VideoTexture = CBase.Theme.GetSkinVideoTexture(_VideoName, _PartyModeID);
        }
    }
}