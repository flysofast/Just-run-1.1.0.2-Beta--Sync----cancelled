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

namespace Map
{
    public partial class Chart1 : PhoneApplicationPage
    {
        public Chart1()
        {
            InitializeComponent();
            
            

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                JustRunDataContext db = new JustRunDataContext(JustRunDataContext.ConnectionString);
                db.CreateIfNotExists();
                db.LogDebug = true;
                IsolatedStorageSettings data = IsolatedStorageSettings.ApplicationSettings;
                List<ARunData> aa=new List<ARunData>();

                if (data.Contains("RunData"))
                {
                    aa = (List<ARunData>)data["RunData"];

                }
                
                foreach(var item in aa)
                {
                    RunData a = new RunData();
                    a.Duration = item.Duration;
                    a.Distance = item.Distance;
                    a.AvgPace = item.AvgPace;
                    a.AvgSpeed = item.AvgSpeed;
                    a.BurnedCalories = item.BurnedCalories;
                    a.Datetime = item.datetime;
                    db.RunDatas.InsertOnSubmit(a);
                    db.SubmitChanges();
                }
                
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                JustRunDataContext db = new JustRunDataContext(JustRunDataContext.ConnectionString);
                var q = db.RunDatas;
                foreach (var item in q)
                    MessageBox.Show(item.Datetime.ToString());
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }
    }
}