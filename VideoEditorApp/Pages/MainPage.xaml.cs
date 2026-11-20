using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using FFMpegCore;
using System.Collections.ObjectModel;
using VideoEditorApp.Data;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;

namespace VideoEditorApp
{
    public partial class MainPage : ContentPage
    {

        public ObservableCollection<ImportedMediaItemDisplay> ImportedMedia { get; set; } = new();

        public MainPage()
        {
            InitializeComponent();

            ImportedMediaCollection_CV.ItemsSource = ImportedMedia;

            UpdateImportedMediaView();
        }

        public void UpdateImportedMediaView()
        {
            foreach (string fileFolderPath in Directory.EnumerateDirectories(FileHelperMethods.GetProxyDirectory()))
            {
                string? clipFile = Directory.EnumerateFiles(fileFolderPath, "clip.*").FirstOrDefault();
                if (String.IsNullOrWhiteSpace(clipFile)) { continue; }

                if (FileHelperMethods.videoExtensions.Contains(Path.GetExtension(clipFile)))
                {
                    ImportedMedia.Add(new()
                    {
                        Thumbnail = "video_placeholder.jpg",
                        Duration = FileHelperMethods.GetDuration(clipFile),
                        FileDisplayText = new DirectoryInfo(fileFolderPath).Name,
                        FolderPath = fileFolderPath
                    });
                }
                else if (FileHelperMethods.audioExtensions.Contains(Path.GetExtension(clipFile)))
                {
                    ImportedMedia.Add(new()
                    {
                        Thumbnail = "audio_placeholder.jpg",
                        Duration = FileHelperMethods.GetDuration(clipFile),
                        FileDisplayText = new DirectoryInfo(fileFolderPath).Name,
                        FolderPath = fileFolderPath
                    });
                }
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

            UpdateImportedMediaView();
        }

        private void ImportedMediaCollection_CV_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            ImportedMediaItemDisplay item = (ImportedMediaItemDisplay)ImportedMediaCollection_CV.SelectedItem;
            MediaPreview_ME.Source = Path.Combine(item.FolderPath, $"clip.mp4");
        }

        private void ToStart_BTN_Pressed(object? sender, EventArgs e)
        {
            MediaPreview_ME.Stop();
            PlayPause_BTN.Text = "Pause";
        }

        private async void Back10_BTN_Pressed(object? sender, EventArgs e)
        {
            await MediaPreview_ME.SeekTo(MediaPreview_ME.Position - TimeSpan.FromSeconds(10));
        }

        private void PlayPause_BTN_Pressed(object? sender, EventArgs e)
        {
            if (PlayPause_BTN.Text == "Play")
            {
                MediaPreview_ME.Play();
                PlayPause_BTN.Text = "Pause";
            }
            else
            {
                MediaPreview_ME.Pause();
                PlayPause_BTN.Text = "Play";
            }
        }

        private async void Forward10_BTN_Pressed(object? sender, EventArgs e)
        {
            await MediaPreview_ME.SeekTo(MediaPreview_ME.Position + TimeSpan.FromSeconds(10));
        }

        private async void ToEnd_BTN_Pressed(object? sender, EventArgs e)
        {
            MediaPreview_ME.Pause();
            await MediaPreview_ME.SeekTo(MediaPreview_ME.Duration);
            PlayPause_BTN.Text = "Pause";
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
    }
}
