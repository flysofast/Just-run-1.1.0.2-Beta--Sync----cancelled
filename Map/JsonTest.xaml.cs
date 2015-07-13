using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.IO;
using System.Runtime.Serialization.Json;

namespace Map
{
    public partial class JsonTest : PhoneApplicationPage
    {
        public JsonTest()
        {
            InitializeComponent();

            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            var data = ((List<ARunData>)settings["RunData"])[0];


            MemoryStream stream = new MemoryStream();
            DataContractJsonSerializer sr = new DataContractJsonSerializer(data.GetType());

            sr.WriteObject(stream, data);
            stream.Position = 0;

            StreamReader reader = new StreamReader(stream);
            string jsonResult = reader.ReadToEnd();
            NavigationService.Navigate(new Uri("http://sđ.a?json="+jsonResult,UriKind.Relative);
            a.Text = jsonResult;
            
        }

    }
}