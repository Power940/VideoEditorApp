using FFMpegCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace VideoEditorApp
{
    public static class FileHelperMethods
    {
        private static readonly string[] stringExtensions = {
            ".mp4", ".mov", ".mkv", ".webm", 
            ".mp3", ".wav", ".flac", ".m4a", ".wma",
        };
        private static readonly PickOptions options = new()
        {
            PickerTitle = "Please select a file",
            FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>> {{ DevicePlatform.WinUI, stringExtensions}})
        };

        public static async Task<string?> PickSingleFileAsync()
        {
            try
            {
                FileResult? result = await FilePicker.Default.PickAsync(options);

                if (result == null) { return null; }

                string extension = Path.GetExtension(result.FileName);
                if (extension != String.Empty && stringExtensions.Contains(extension)) { return result.FullPath; }
            }
            catch (Exception) { }

            return null;
        }

        public static async Task<IEnumerable<FileResult>?> PickMultipleFilesAsync()
        {
            try
            {
                IEnumerable<FileResult>? results = await FilePicker.Default.PickMultipleAsync(options);
                if (results == null) { return null; }

                IEnumerable<FileResult> safeReturn = [];

                foreach (FileResult? file in results)
                {
                    if (file == null) { continue; }

                    string extension = Path.GetExtension(file.FileName);
                    if (extension != String.Empty && stringExtensions.Contains(extension))
                    {
                        _ = safeReturn.Append(file);
                    }
                }

                return safeReturn;
            }
            catch (Exception) { }

            return null;
        }

        public static (int? width, int? height) GetResolution(string filePath)
        {
            VideoStream? videoStream = FFProbe.Analyse(filePath).PrimaryVideoStream;

            if (videoStream != null)
            {
                return (videoStream.Width, videoStream.Height);
            }

            return (null, null);
        }

        public static TimeSpan GetDuration(string filePath)
        {
            return FFProbe.Analyse(filePath).Duration;
        }

        public static IMediaAnalysis GetRawVideoAudioData(string filePath)
        {
            return FFProbe.Analyse(filePath);
        }
    }
}
