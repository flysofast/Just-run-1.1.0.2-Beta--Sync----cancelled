using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Map.Resources;

namespace Map
{
    public partial class AddRun : PhoneApplicationPage
    {
        public AddRun()
        {
            InitializeComponent();
        }

        private void btSave_Click(object sender, RoutedEventArgs e)
        {
            ARunData rundata = new ARunData();
            DateTime date = (DateTime)dpDate.Value;
            DateTime datetime = date + ((DateTime)tpTime.Value).TimeOfDay;

            rundata.datetime = datetime;
            try
            {
                rundata.BurnedCalories = double.Parse(tbCalo.Text);
            }
            catch 
            {
            }
            try
            {
                rundata.Distance = double.Parse(tbDistance.Text)*1000;
            }
            catch 
            {
            }
            try
            {
                int _timeCount = ((int)double.Parse(tbDuration.Text)*60);
                rundata.Duration =  string.Format("{0}h {1}m {2}s", _timeCount / 3600, (_timeCount / 60) % 60, _timeCount % 60);
            }
            catch 
            {
            }
            rundata.Save();
            MessageBox.Show(AppResources.Saved);
           // NavigationService.GoBack();
            //
            
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}