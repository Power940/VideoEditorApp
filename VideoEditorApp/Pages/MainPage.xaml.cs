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

            MediaCollectionView.ItemsSource = ImportedMedia;

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

        private void MediaCollectionView_CV_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            //MyMediaElement.Source = Path.Combine(((ImportedMediaItemDisplay)MediaCollectionView.SelectedItem).FolderPath, "clip.mp4");
        }
    }
}
