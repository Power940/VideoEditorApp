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
        //Description: Create a class that represents a media clip placed on the timeline, including its source media and timing information.
        //Acceptance Criteria:
        //Clip stores a reference to its source media.
        //Clip stores its timeline start position.
        //Clip stores its start and end trim positions.
        //Clip can calculate its resulting duration.

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
