using Microsoft.Phone.Maps.Controls;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace Map
{
    // [Serializable]
    [DataContract]

    public class ARunData
    {

        public ARunData()
        {
        }
        [DataMember]
        public GeoCoordinateCollection geoCollection = new GeoCoordinateCollection();
        [DataMember]
        public DateTime datetime { set; get; }
        [DataMember]
        public double BurnedCalories { set; get; }
        [DataMember]
        public string Duration { set; get; }
        [DataMember]
        public double AvgSpeed { set; get; }
        [DataMember]
        public double AvgPace { set; get; }
        [DataMember]
        public double Distance { set; get; }
        [DataMember]
        public int No { set; get; }
        [DataMember]
        public bool IsSynced { set; get; }

        public void Save()
        {
            try
            {
                int TimeCount = 0;
                string _duration;
                _duration = Regex.Replace(Duration, @"[^\d]", " ");
                _duration = Regex.Replace(_duration, @"\s+", " ");
                string[] time = _duration.Trim().Split(' ');
                for (int i = 0; i < 3; i++)
                {
                    int baseNum = i == 0 ? 3600 : i == 1 ? 60 : 1;
                    TimeCount += int.Parse(time[i]) * baseNum;
                }

                IsolatedStorageSettings data = IsolatedStorageSettings.ApplicationSettings;
                int count = (int)data["RunCount"];
                if (AvgSpeed == 0 || double.IsNaN(this.AvgSpeed))
                    if (TimeCount != 0)
                        AvgSpeed = (Distance / TimeCount) * 3.6;
                    else AvgSpeed = 0;

                if (AvgPace == 0 || double.IsNaN(this.AvgPace))
                    if (Distance != 0)
                        AvgPace = (TimeCount / 60) / (Distance / 1000);
                    else
                        AvgPace = 0;
                if (double.IsNaN(this.BurnedCalories)) this.BurnedCalories = 0;
                if (double.IsNaN(this.Distance)) this.Distance = 0;
                if (!data.Contains("RunData"))
                {
                    No = count + 1;
                    List<ARunData> RunData = new List<ARunData>();

                    RunData.Add(this);
                    data.Add("RunData", RunData);
                }
                else
                {
                    List<ARunData> RunData = new List<ARunData>();
                    RunData = (List<ARunData>)data["RunData"];
                    No = count + 1;
                    RunData.Add(this);
                    data["RunData"] = RunData;
                }
                count++;
                data["RunCount"] = count;
                data.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
