using Xamarin.Forms;

namespace shopping_list_client
{
    public partial class App : Application
    {
        public delegate void AppClosing();
        public static event AppClosing onAppClosing;

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
            onAppClosing();
        }

        protected override void OnResume()
        {
        }
    }
}
