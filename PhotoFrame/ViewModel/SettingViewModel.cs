using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace PhotoFrame.ViewModel
{
    public class SettingViewModel : INotifyPropertyChanged
    {
        string _ipAddress= "_ipAddress";

        public string IpAddress
        {
            get => Preferences.Get(_ipAddress, string.Empty);
            set
            {
                Preferences.Set(_ipAddress, value);
                OnPropertyChanged();
            }
        }

        public string _apiKey = "_apiKey";

       
        public string ApiKey
        {
            get => Preferences.Get(_apiKey, string.Empty);
            set
            {
                Preferences.Set(_apiKey, value);
                OnPropertyChanged();
            }
        }

        public ICommand ChangeIp { get; }

        public ICommand ChangeApiKey { get; }
        public SettingViewModel()
        {
            ChangeIp=new RelayCommand(ClickUpdateIP);
            ChangeApiKey=new RelayCommand(ClickUpdateAPIKey);
        }


        private async void ClickUpdateIP()
        {
             IpAddress = await Application.Current.MainPage.DisplayPromptAsync("Изменение ", "Введите ip адресс сервера ", placeholder: "192.168.0.1", maxLength: 11);
        }

        private async void ClickUpdateAPIKey()
        {
             ApiKey = await Application.Current.MainPage.DisplayPromptAsync("Изменение ", "Введите API ключ", placeholder: "Ключ из личного кабинета GigaChatApi");
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
