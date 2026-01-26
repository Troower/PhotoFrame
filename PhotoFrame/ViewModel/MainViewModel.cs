using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhotoFrame.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        static public readonly HttpClient client = new HttpClient();

        bool _isRefreching=false;


        public bool Settings
        {
            get => String.IsNullOrWhiteSpace(Preferences.Get("_ipAddress", String.Empty)) || String.IsNullOrWhiteSpace(Preferences.Get("_apiKey", String.Empty));
        }

        public bool IsRefreching
        {
            get=>_isRefreching;
            set
            {
                _isRefreching = value;
                OnPropertyChanged();
            }
        }

        ImageSource _image;
        public ImageSource Image
        {
            get => _image;
            set
            {
                _image = value;
                OnPropertyChanged();
            }
        }

        string _promptString;

        public string PromptString
        {
            get => _promptString;
            set
            {
                _promptString = value;
                OnPropertyChanged();
            }
        }


        public ICommand GoSetting { get; }

        public ICommand SelectImageCommand { get; }

        public MainViewModel()
        {
            string ip = Preferences.Get("_ipAddress", String.Empty);
            if (!String.IsNullOrWhiteSpace(ip))
                MainViewModel.client.BaseAddress=new Uri($"http://{ip}/");
            GoSetting = new RelayCommand(() => Application.Current.MainPage.Navigation.PushModalAsync(new SettingsPage()));
            SelectImageCommand = new RelayCommand(SelectImage);
        }

        private async void SelectImage()
        {
            FileResult? photo = await CapturePhotoAsync();
            if (photo is null)
                return ;
            await using Stream input = await photo.OpenReadAsync();
            MemoryStream memoryStream = new MemoryStream();
            await input.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            Image = ImageSource.FromStream(()=> memoryStream);
            
        }

        private async void LoadData()
        {
            try
            {
                IsRefreching = true;

            }
            catch(Exception ex)
            {
                Application.Current?.MainPage?.DisplayAlertAsync("Ошибка",ex.Message,"Закрыть");
            }
            finally
            {
                IsRefreching=false;
            }
        }

        private void ChangeMode()
        {
            try
            {
                IsRefreching = true;

            }
            catch (Exception ex)
            {
                Application.Current?.MainPage?.DisplayAlertAsync("Ошибка", ex.Message, "Закрыть");
            }
            finally
            {
                IsRefreching = false;
            }
        }
        public async Task<FileResult?> CapturePhotoAsync()
        {
            if (!MediaPicker.Default.IsCaptureSupported)
                return null;
            return await MediaPicker.Default.PickPhotoAsync();
        }
        private static byte[] ConvertStreamToJpegBytes(Stream input, int quality = 85)
        {
            using var bitmap = SKBitmap.Decode(input);
            if (bitmap is null)
                throw new InvalidOperationException("Не удалось декодировать изображение (SKBitmap.Decode вернул null).");

            using SKData data = bitmap.Encode(SKEncodedImageFormat.Jpeg, quality);
            return data.ToArray();
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
