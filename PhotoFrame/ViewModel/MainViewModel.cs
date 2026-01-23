using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PhotoFrame.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private HttpClient client=new HttpClient();

        bool _isRefreching=false;

        public bool IsRefreching
        {
            get=>_isRefreching;
            set
            {
                _isRefreching = value;
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

        bool _isChechedGenMode;

        public bool IsChechedGenMode
        {
            get => _isChechedGenMode;
            set
            {
                ChangeMode();
                _isChechedGenMode = value;
                OnPropertyChanged();
            }
        }


        public MainViewModel()
        {
            
        }

        private async void LoadData()
        {
            try
            {
                IsRefreching = true;
                IsChechedGenMode= true;

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

        

        public event PropertyChangedEventHandler? PropertyChanged;

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
