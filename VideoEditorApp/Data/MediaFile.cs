using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Windows.Storage.Pickers.Provider;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace VideoEditorApp
{
    internal class MediaFile
    {
        private string FileName { get; set; }
        private string FilePath { get; set; }
        private string FileType { get; set; }
        private string FileDuration { get; set; }

        MediaFile(string fileName, string filePath, string fileType, string fileDuration)
        {
            FileName = fileName;
            FilePath = filePath;
            FileType = fileType;
            FileDuration = fileDuration;
        }

        public override string ToString()
        {
            return $"File Name: {FileName}, File Path: {FilePath}, File Type: {FileType}, File Duration: {FileDuration}";
        }

    }
}
