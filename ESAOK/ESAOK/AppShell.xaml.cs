using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ESAOK
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }
    }
}
