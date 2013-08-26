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
using VocaluxeLib.Menu;
using VocaluxeLib.Draw;

namespace VocaluxeLib.Animations
{
    public class CAnimationVideo : CAnimationFramework
    {
        private string _VideoName;
        private CTexture _VideoTexture;

        public CAnimationVideo(int partyModeID)
            : base(partyModeID) {}

        public override void Init()
        {
            Type = EAnimationType.Video;
            Repeat = EAnimationRepeat.Repeat;
        }

        public override bool LoadAnimation(string item, CXMLReader xmlReader)
        {
            AnimationLoaded = true;
            AnimationLoaded &= base.LoadAnimation(item, xmlReader);
            AnimationLoaded &= xmlReader.GetValue(item + "/Video", out _VideoName, String.Empty);

            return AnimationLoaded;
        }

        public override bool SaveAnimation(System.Xml.XmlWriter writer)
        {
            if (AnimationLoaded)
            {
                base.SaveAnimation(writer);
                writer.WriteComment("<Video>: Video-texture-name from skin");
                writer.WriteElementString("Video", _VideoName);
                return true;
            }
            else
                return false;
        }

        public override CTexture GetTexture()
        {
            return _VideoTexture;
        }

        public override void SetCurrentValues(SRectF rect, SColorF color) {}

        public override void Update()
        {
            _VideoTexture = CBase.Theme.GetSkinVideoTexture(_VideoName, _PartyModeID);
        }
    }
}