using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Map.Resources;

namespace Map
{
    public partial class about : PhoneApplicationPage
    {
        public about()
        {
            InitializeComponent();
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();

            emailComposeTask.Subject = "Just run suggestion";
            emailComposeTask.Body = "Hello Nam! I have an advice for you: ";
            emailComposeTask.To = "lehainam.it@outlook.com";

            emailComposeTask.Show();
        }

        private void HyperlinkButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MessageBox.Show(AppResources.PrivacyPolicyContent);
        }
    }
}