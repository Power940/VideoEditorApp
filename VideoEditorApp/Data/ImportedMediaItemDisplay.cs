using System;
using System.Collections.Generic;
using System.Text;

namespace VideoEditorApp.Data
{
    public class ImportedMediaItemDisplay
    {
        public required ImageSource Thumbnail { get; set; }
        public required TimeSpan Duration { get; set; }
        public required string FileDisplayText { get; set; }
        public required string FolderPath { get; set; }
    }
}
