using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
using MahApps.Metro.Controls.Dialogs;
using Pablo.Classes;
using Pablo.Classes.Interfaces;

namespace Pablo.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        #region Fields

        private readonly IDialogCoordinator _dialogCoordinator;
        private readonly DispatcherTimer _dispatcherTimer;

        private IList<ImageViewModel> slideShowFiles;

        private ImageViewModel _selectedFile;
        private bool _isSlideShowPlaying;
        private string _selectedFolder;
        private bool _showSettings;
        private PersistenceService _persistenceService;

        #endregion

        #region Constructor

        public MainWindowViewModel(IDialogCoordinator dialogCoordinator)
        {
            _dialogCoordinator = dialogCoordinator;
            _dispatcherTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 3) };
            _dispatcherTimer.Tick += OnDispatcherTimerTick;

            FolderContents = new ObservableCollection<ImageViewModel>();

            _persistenceService = new PersistenceService();

            PlayAllCommand = new DelegateCommand(PlayAll);
            PlayFavoritesCommand = new DelegateCommand(PlayFavourites);
            ShowInputDialogCommand = new DelegateCommand(ShowDialogAndLoadImages);
            ChangeFolderPathCommand = new DelegateCommand(LoadDirectory);
            SaveFavoritesCommand = new DelegateCommand(SaveFavorites);
        }

        private void SaveFavorites()
        {
            _persistenceService.Save(SelectedFolder, FolderContents.ToList());
        }

        #endregion

        #region Memebers

        public ICommand PlayAllCommand { get; }

        public ICommand PlayFavoritesCommand { get; }

        public ICommand SaveFavoritesCommand { get; }

        public ICommand ShowInputDialogCommand { get; }

        public ICommand ChangeFolderPathCommand { get; }

        public ObservableCollection<ImageViewModel> FolderContents { get; }

        public ImageViewModel SelectedFile
        {
            get { return _selectedFile; }

            set
            {
                _selectedFile = value;
                OnPropertyChanged();
            }
        }

        public bool ShowSettings
        {
            get { return _showSettings; }

            set
            {
                _showSettings = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Private Methods
        private void OnDispatcherTimerTick(object sender, EventArgs e)
        {
            if (SelectedFile == null)
            {
                return;
            }

            var index = slideShowFiles.IndexOf(SelectedFile);
            var nextIndex = ++index == slideShowFiles.Count ? 0 : index;

            SelectedFile = slideShowFiles[nextIndex];
        }

        private void SlideShow(IList<ImageViewModel> images)
        {
            slideShowFiles = images;
            if (!_isSlideShowPlaying)
            {
                _dispatcherTimer.Start();
                _isSlideShowPlaying = true;
            }
            else
            {
                _dispatcherTimer.Stop();
                _isSlideShowPlaying = false;
            }
        }

        private void PlayAll()
        {
            var slideShowFiles = FolderContents.ToList();
            SlideShow(slideShowFiles);
        }

        private void PlayFavourites()
        {
            var slideShowFiles = FolderContents.Where(x => x.IsFavourite).ToList();
            SlideShow(slideShowFiles);
        }

        private async void ShowDialogAndLoadImages()
        {
            var folderPath = await _dialogCoordinator.ShowInputAsync(this, "Enter Folder Path",
                "Folder path can be changed by clicking \"Settings\" in the window top right.");

            if (!string.IsNullOrEmpty(folderPath))
            {
                SelectedFolder = folderPath;
                LoadDirectory();
            }
        }

        public string SelectedFolder
        {
            get { return _selectedFolder; }
            set
            {
                _selectedFolder = value;
                OnPropertyChanged();
            }
        }

        private void LoadDirectory()
        {
            FolderContents.Clear();

            var persistedFiles = _persistenceService.Load(SelectedFolder);

            //TODO use unity / inject service
            IFileService fileService = new FileService();
            var extensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".png", ".jpg", ".jpeg" };
            foreach (var filePath in fileService.LoadFiles(SelectedFolder, extensions))
            {
                var persistedFile = persistedFiles
                                        .Where(x => x.FilePath == filePath)
                                        .Select(a => a)
                                        .FirstOrDefault();

                var image = persistedFile == null ? new ImageViewModel(filePath) : persistedFile;
                FolderContents.Add(image);
            }

            SelectedFile = FolderContents.FirstOrDefault();
        }

        #endregion
    }
}
