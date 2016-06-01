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

        private ImageViewModel _selectedFile;
        private bool _isSlideShowPlaying;

        #endregion

        #region Constructor

        public MainWindowViewModel(IDialogCoordinator dialogCoordinator)
        {
            _dialogCoordinator = dialogCoordinator;
            _dispatcherTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 3) };
            _dispatcherTimer.Tick += OnDispatcherTimerTick;
            FolderContents = new ObservableCollection<ImageViewModel>();
            StartSlideshowCommand = new DelegateCommand(StartSlideshow);
            ShowInputDialogCommand = new DelegateCommand(ShowDialogAndLoadImages);
        }

        #endregion
        
        #region Memebers

        public ICommand StartSlideshowCommand { get; }

        public ICommand ShowInputDialogCommand { get; }

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

        #endregion

        #region Private Methods
        private void OnDispatcherTimerTick(object sender, EventArgs e)
        {
            if (SelectedFile == null)
            {
                return;
            }

            var index = FolderContents.IndexOf(SelectedFile);
            var nextIndex = ++index == FolderContents.Count ? 0 : index;

            SelectedFile = FolderContents[nextIndex];
        }

        private void StartSlideshow()
        {
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

        private async void ShowDialogAndLoadImages()
        {
            var folderPath = await _dialogCoordinator.ShowInputAsync(this, "Enter Folder Path",
                "All images found below this directory will be loaded into Pablo.");

            if (!string.IsNullOrEmpty(folderPath))
            {
                LoadDirectory(folderPath);
            }
        }

        private void LoadDirectory(string folderPath)
        {
            FolderContents.Clear();

            // TODO use unity/inject service
            IFileService fileService = new FileService();
            var extensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {".png", ".jpg", ".jpeg"};
            foreach (var filePath in fileService.LoadFiles(folderPath, extensions))
            {
                var imageViewModel = new ImageViewModel(filePath);
                FolderContents.Add(imageViewModel);
            }

            SelectedFile = FolderContents.FirstOrDefault();
        }

        #endregion
    }
}
