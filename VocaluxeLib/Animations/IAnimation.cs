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

using System.Xml;
using VocaluxeLib.Menu;

namespace VocaluxeLib.Animations
{
    interface IAnimation
    {
        void Init();
        bool LoadAnimation(string item, CXMLReader xmlReader);
        bool SaveAnimation(XmlWriter writer);

        void SetRect(SRectF rect);
        SRectF GetRect();
        SRectF GetRectChanges();
        void SetColor(SColorF color);
        SColorF GetColor();
        void SetTexture(ref STexture texture);
        STexture GetTexture();
        void SetCurrentValues(SRectF rect, SColorF color);
        EAnimationEvent GetEvent();
        bool IsDrawn();

        void StartAnimation();
        void StopAnimation();
        void ResetAnimation();
        void ResetValues(bool fromStart);
        bool AnimationActive();
        void Update();
    }
}