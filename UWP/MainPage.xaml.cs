using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            ViewModel = new PlcViewModel();
            ViewModel.OnError += ViewModel_OnError;
            ViewModel.OnMessage += ViewModel_OnMessage; ;
        }

        private async void ViewModel_OnMessage(object sender, string e)
        {
            await ShowDialog("Message", e);
        }

        private async void ViewModel_OnError(object sender, Exception e)
        {
            await ShowDialog(e.GetType().ToString(), e.Message);
        }

        private async Task ShowDialog(string title, string message)
        {
            ContentDialog dialog = new ContentDialog()
            {
                Title = title,
                Content = message,
                CloseButtonText = "OK"
            };
            await dialog.ShowAsync();
        }

        public PlcViewModel ViewModel { get; set; }
    }
}
