
using SkiaSharp;
using System.ComponentModel;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Windows.Input;

namespace PhotoFrame.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        static public readonly HttpClient client=new HttpClient();

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

        public ICommand ReloadImageCommand { get; }

        public ICommand SendPromtCommand { get; }

        public MainViewModel()
        {
            client.Timeout= TimeSpan.FromSeconds(10);
            string ip = Preferences.Get("_ipAddress", String.Empty);
            if (!String.IsNullOrWhiteSpace(ip))
                MainViewModel.client.BaseAddress=new Uri($"http://{ip}:5131/");
            GoSetting = new RelayCommand(() => Application.Current.MainPage.Navigation.PushModalAsync(new SettingsPage()));
            SelectImageCommand = new RelayCommand(SelectImage);
            ReloadImageCommand = new RelayCommand(ReloadImage);
            SendPromtCommand = new RelayCommand(SendPrompt);
        }

        private async void SendPrompt()
        {
            try
            {
                if(String.IsNullOrWhiteSpace(PromptString))
                {
                    Application.Current?.MainPage?.DisplayAlertAsync("Ошибка", "Введите промпт!", "Закрыть");
                    return;
                }
                IsRefreching = true;
                SendPromptAsync();
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

        private async Task SendPromptAsync()
        {
            var json = JsonSerializer.Serialize(PromptString);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await MainViewModel.client.PostAsync("api/frame/print-image", content);
        }

        private async void ReloadImage()
        {
            try
            {
                IsRefreching = true;
                await LoadToImageAsync();
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

        private async void SelectImage()
        {
            FileResult? photo = await CapturePhotoAsync();
            if (photo is null)
                return;

            await using (Stream input = await photo.OpenReadAsync()){
                await UploadJpgAsync(ConvertStreamToJpegBytes(input));
            }
            ReloadImage();
        }


        private async Task<FileResult?> CapturePhotoAsync()
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

        private async Task UploadJpgAsync(byte[] jpegBytes)
        {
            try
            {
                IsRefreching = true;
                using var form = new MultipartFormDataContent();
                var fileContent = new ByteArrayContent(jpegBytes);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Image.Jpeg);
                form.Add(fileContent, "file", "photo.jpg");
                using var resp = await MainViewModel.client.PostAsync("api/frame/upload-image", form);
                resp.EnsureSuccessStatusCode();
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

        private async Task LoadToImageAsync()
        {
            try
            {
                IsRefreching = true;
                using var resp = await MainViewModel.client.GetAsync("api/frame/download-image", HttpCompletionOption.ResponseHeadersRead);
                resp.EnsureSuccessStatusCode();
                var bytes = await resp.Content.ReadAsByteArrayAsync();
                Image = ImageSource.FromStream(() => new MemoryStream(bytes));
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

        public event PropertyChangedEventHandler? PropertyChanged;

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
