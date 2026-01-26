using PhotoFrame.ViewModel;

namespace PhotoFrame
{
    public partial class MainPage : ContentPage
    {
        static readonly MainViewModel VM = new MainViewModel();
        public MainPage()
        {
            InitializeComponent();
            this.BindingContext = VM;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            VM.OnPropertyChanged(nameof(VM.Settings));
        }

    }
}
