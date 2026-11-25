using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using FFMpegCore;
using System.Collections.ObjectModel;
using System.IO.Compression;
using VideoEditorApp.Data;
using VideoEditorApp.Helper;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;

namespace VideoEditorApp
{
    public partial class MainPage : ContentPage
    {

        private ObservableCollection<ImportedMediaItemDisplay> ImportedMedia { get; set; } = [];
        private bool deleteImportsMode = false;
        private bool ignoreSelectionChangedImportedMedia = false;


        public MainPage()
        {
            InitializeComponent();

            ImportedMediaCollection_CV.ItemsSource = ImportedMedia;

            UpdateImportedMediaView();
        }

        private void AddImportedMediaItemToView(string fullClipFilePath, string folderPath)
        {
            Logger.Log($"AddImportedMediaItemToView | fullClipFilePath:{fullClipFilePath} | folderPath:{folderPath}");
            if (FileHelperMethods.videoExtensions.Contains(Path.GetExtension(fullClipFilePath)))
            {
                ImportedMedia.Add(new()
                {
                    Thumbnail = "video_placeholder.jpg",
                    Duration = FileHelperMethods.GetDuration(fullClipFilePath),
                    FileDisplayText = new DirectoryInfo(folderPath).Name,
                    FolderPath = folderPath
                });
            }
            else if (FileHelperMethods.audioExtensions.Contains(Path.GetExtension(fullClipFilePath)))
            {
                ImportedMedia.Add(new()
                {
                    Thumbnail = "audio_placeholder.jpg",
                    Duration = FileHelperMethods.GetDuration(fullClipFilePath),
                    FileDisplayText = new DirectoryInfo(folderPath).Name,
                    FolderPath = folderPath
                });
            }
        }

        private void UpdateImportedMediaView()
        {
            foreach (string fileFolderPath in Directory.EnumerateDirectories(FileHelperMethods.GetProxyDirectory()))
            {
                string? clipFile = Directory.EnumerateFiles(fileFolderPath, "clip.*").FirstOrDefault();
                if (String.IsNullOrWhiteSpace(clipFile)) { continue; }

                AddImportedMediaItemToView(clipFile, fileFolderPath);
            }
        }

        private async void Import_BTN_Clicked(object? sender, EventArgs e)
        {
            FileResult? file = await FileHelperMethods.PickSingleFileAsync();
            if (file == null) { return; }

            string folderPath = Path.Combine(FileHelperMethods.GetProxyDirectory(), Path.GetFileNameWithoutExtension(file.FileName));

            if (Directory.Exists(folderPath))
            {
                folderPath += "(1)";
                int i = 2;
                while (Directory.Exists(folderPath))
                {
                    folderPath = $"{folderPath[..^2]}{i++})";
                }
            }

            Directory.CreateDirectory(folderPath);

            File.Copy(file.FullPath, Path.Combine(folderPath, $"clip{Path.GetExtension(file.FileName)}"));

            AddImportedMediaItemToView(Path.Combine(folderPath, $"clip{Path.GetExtension(file.FileName)}"), folderPath);
        }

        private async void ImportMulti_BTN_Clicked(object? sender, EventArgs e)
        {
            List<FileResult>? files = await FileHelperMethods.PickMultipleFilesAsync();
            if (files == null || files.Count == 0) { return; }

            foreach(FileResult file in files)
            {
                string folderPath = Path.Combine(FileHelperMethods.GetProxyDirectory(), Path.GetFileNameWithoutExtension(file.FileName));

                if (Directory.Exists(folderPath))
                {
                    folderPath += "(1)";
                    int i = 2;
                    while (Directory.Exists(folderPath))
                    {
                        folderPath = $"{folderPath[..^2]}{i++})";
                    }
                }

                Directory.CreateDirectory(folderPath);

                File.Copy(file.FullPath, Path.Combine(folderPath, $"clip{Path.GetExtension(file.FileName)}"));

                AddImportedMediaItemToView(Path.Combine(folderPath, $"clip{Path.GetExtension(file.FileName)}"), folderPath);
            }
        }

        private void ImportedMediaCollection_CV_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (ignoreSelectionChangedImportedMedia) { return; }
            ImportedMediaItemDisplay item = (ImportedMediaItemDisplay)ImportedMediaCollection_CV.SelectedItem;
            try
            {
                MediaPreview_ME.Source = Path.Combine(item.FolderPath, $"clip.mp4");
            }
            catch (Exception) { return; }
        }

        private void ToStart_BTN_Pressed(object? sender, EventArgs e)
        {
            MediaPreview_ME.Stop();
        }

        private async void Back10_BTN_Pressed(object? sender, EventArgs e)
        {
            TimeSpan newPos = MediaPreview_ME.Position.Subtract(TimeSpan.FromSeconds(10));
            await MediaPreview_ME.SeekTo((newPos < TimeSpan.Zero) ? TimeSpan.Zero : newPos);
        }

        private void PlayPause_BTN_Pressed(object? sender, EventArgs e)
        {
            if (PlayPause_BTN.Text == "Play")
            {
                MediaPreview_ME.Play();
            }
            else
            {
                MediaPreview_ME.Pause();
            }
        }

        private async void Forward10_BTN_Pressed(object? sender, EventArgs e)
        {
            TimeSpan newPos = MediaPreview_ME.Position.Add(TimeSpan.FromSeconds(10));
            await MediaPreview_ME.SeekTo((newPos > MediaPreview_ME.Duration) ? MediaPreview_ME.Duration : newPos);
        }

        private async void ToEnd_BTN_Pressed(object? sender, EventArgs e)
        {
            MediaPreview_ME.Pause();
            await MediaPreview_ME.SeekTo(MediaPreview_ME.Duration);
        }

        private void SpeedChange_BTN_Pressed(object? sender, EventArgs e)
        {
            if (SpeedChange_BTN.Text == "2x")
            {
                MediaPreview_ME.Speed = 2;
                SpeedChange_BTN.Text = "1x";
            }
            else if (SpeedChange_BTN.Text == "1x")
            {
                MediaPreview_ME.Speed = 1;
                SpeedChange_BTN.Text = "2x";
            }
        }

        private void MediaPreview_ME_StateChanged(object? sender, EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                PlayPause_BTN.Text = MediaPreview_ME.CurrentState switch
                {
                    MediaElementState.Playing => "Pause",
                    MediaElementState.Paused or
                    MediaElementState.Stopped or
                    MediaElementState.Failed => "Play",
                    _ => PlayPause_BTN.Text
                };
            });
        }

        private void DeleteMode_BTN_Clicked(object? sender, EventArgs e)
        {
            deleteImportsMode = !deleteImportsMode;

            if (deleteImportsMode)
            {
                ignoreSelectionChangedImportedMedia = true;
                ImportedMediaCollection_CV.SelectedItem = null;
                ImportedMediaCollection_CV.SelectedItems = null;
                ImportedMediaCollection_CV.SelectionMode = SelectionMode.Multiple;
            }
            else
            {
                List<ImportedMediaItemDisplay> tempList = [..ImportedMediaCollection_CV.SelectedItems.Cast<ImportedMediaItemDisplay>()];
                foreach (ImportedMediaItemDisplay item in tempList)
                {
                    try
                    {
                        Directory.Delete(item.FolderPath, true);
                        ImportedMedia.Remove(item);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Imported media deletion failed: {ex.Message}");
                    }
                }

                ImportedMediaCollection_CV.SelectedItem = null;
                ImportedMediaCollection_CV.SelectedItems = null;
                ignoreSelectionChangedImportedMedia = false;
                ImportedMediaCollection_CV.SelectionMode = SelectionMode.Single;
            }
        }
    }
}
