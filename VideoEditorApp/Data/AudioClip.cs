using System;
using System.Collections.Generic;
using System.Text;

namespace VideoEditorApp.Data
{
    internal class AudioClip : TimelineClip 
    { 
        AudioClip(MediaFile mediaSource, string startPosition, string endPosition) : base(mediaSource, startPosition, endPosition) {}
    }
}
