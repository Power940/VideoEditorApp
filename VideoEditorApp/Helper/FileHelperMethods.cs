using FFMpegCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace VideoEditorApp
{
    public static class FileHelperMethods
    {
        public static readonly string[] videoExtensions = {
            ".mp4", ".mov", ".mkv", ".webm"
        };
        public static readonly string[] audioExtensions = {
            ".mp3", ".wav", ".flac", ".m4a", ".wma"
        };
        public static readonly string[] stringExtensions = [.. videoExtensions, .. audioExtensions];

        private static readonly PickOptions singleOptions = new()
        {
            PickerTitle = "Please select an audio or video file",
            FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>> { { DevicePlatform.WinUI, stringExtensions } })
        };
        private static readonly PickOptions multiOptions = new()
        {
            PickerTitle = "Please select any number of audio or video files",
            FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>> { { DevicePlatform.WinUI, stringExtensions } })
        };

        public static async Task<FileResult?> PickSingleFileAsync()
        {
            try
            {
                FileResult? result = await FilePicker.Default.PickAsync(singleOptions);

                if (result == null || !File.Exists(result.FullPath)) { return null; }

                string extension = Path.GetExtension(result.FileName);
                if (extension != String.Empty && stringExtensions.Contains(extension)) { return result; }
            }
            catch (Exception) { }

            return null;
        }

        public static async Task<List<FileResult>?> PickMultipleFilesAsync()
        {
            try
            {
                IEnumerable<FileResult>? results = await FilePicker.Default.PickMultipleAsync(multiOptions);
                if (results == null || !results.Any()) { return null; }

                List<FileResult> safeReturn = [];

                foreach (FileResult? file in results)
                {
                    if (file == null || !File.Exists(file.FullPath)) { continue; }

                    string extension = Path.GetExtension(file.FileName);
                    if (extension != String.Empty && stringExtensions.Contains(extension))
                    {
                        safeReturn.Add(file);
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

        public static string GetProxyDirectory()
        {
            string proxyPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "VideoEditorApp");
            Directory.CreateDirectory(proxyPath);
            return proxyPath;
        }

    }
}
