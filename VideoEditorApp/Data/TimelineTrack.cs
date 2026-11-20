using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Principal;
using System.Text;

namespace VideoEditorApp.Data
{
    internal class TimelineTrack
    {

        private TimelineClip[]? TimelineClips { get; set; }
        private bool IsVideo { get; set; }

        TimelineTrack (bool isVideo)
        {
            IsVideo = isVideo;
        }

        public override string ToString()
        {
            return $"Clips: {TimelineClips.Length}, isVideoTrack: {IsVideo}";
        }
    }
}
