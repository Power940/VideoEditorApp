using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Text;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace VideoEditorApp
{
    internal class TimelineClip
    {

        private MediaFile MediaSource { get; set; }
        private string StartPosition { get; set; }
        private string EndPosition { get; set; }
        private int TrimStart { get; set; }
        private int TrimDuration { get; set; }

        TimelineClip(MediaFile mediaSource, string startPosition, string endPosition)
        {
            MediaSource = mediaSource;
            StartPosition = startPosition;
            EndPosition = endPosition;
        }

        public override string ToString()
        {
            return $"Media Source: {MediaSource}, Start: {StartPosition}, End: {EndPosition}";
        }
    }
}
