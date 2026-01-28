using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
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
            try
            {
                string ip = await Application.Current.MainPage.DisplayPromptAsync("Изменение ", "Введите ip адресс сервера ", placeholder: "192.168.0.1", maxLength: 15);
                Regex reg = new("\\b\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\b");
                if (!String.IsNullOrEmpty(ip) && reg.IsMatch(ip))
                {
                    IpAddress = ip;
                    Application.Current.MainPage?.DisplayAlertAsync("Успех", "Ip адрес успешно изменен, перезапустите приложение для продолжения работы по новому адрессу!", "Закрыть");
                }
                else
                    await Application.Current.MainPage.DisplayAlertAsync("Ошибка", "Некоректно ввведен ip!", "Закрыть");
            }
            catch (Exception ex)
            {
                Application.Current?.MainPage?.DisplayAlertAsync("Ошибка", ex.Message, "Закрыть");
            }
        }

        private async void ClickUpdateAPIKey()
        {
            try { 
            string apiKey = await Application.Current.MainPage.DisplayPromptAsync("Изменение ", "Введите API ключ", placeholder: "Ключ из личного кабинета GigaChatApi");
            if (!String.IsNullOrWhiteSpace(apiKey))
                ApiKey = apiKey;
            else
                await Application.Current.MainPage.DisplayAlertAsync("Ошибка", "Некоректно ввведен api key!", "Закрыть");
            }
            catch (Exception ex)
            {
                Application.Current?.MainPage?.DisplayAlertAsync("Ошибка", ex.Message, "Закрыть");
            }

        }


        public event PropertyChangedEventHandler? PropertyChanged;

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
