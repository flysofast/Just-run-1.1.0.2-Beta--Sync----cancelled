using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Map
{
    

    public class ARunUploadData
    {

        public ARunUploadData()
        {
        }

        [JsonProperty]
        public double[] longitude;
        [JsonProperty]
        public double[] latitude;
        [JsonProperty]
        public DateTime datetime { set; get; }
        [JsonProperty]
        public double BurnedCalories { set; get; }
        [JsonProperty]
        public string Duration { set; get; }
        [JsonProperty]
        public double AvgSpeed { set; get; }
        [JsonProperty]
        public double AvgPace { set; get; }
        [JsonProperty]
        public double Distance { set; get; }
       
        
        public void ParseFrom (ARunData data)
        {
            longitude = new double[data.geoCollection.Count];
            latitude = new double[data.geoCollection.Count];
            for (int i = 0; i < data.geoCollection.Count; i++)
            {
                longitude[i] = data.geoCollection[i].Longitude;
                latitude[i] = data.geoCollection[i].Latitude;
            }
            datetime = data.datetime.ToUniversalTime(); // So Json Serializer/Deserializer could understands and do the Parsing
            //datetime = JsonConvert.SerializeObject(data.datetime, new JavaScriptDateTimeConverter());
            BurnedCalories = data.BurnedCalories;
            Duration = data.Duration;
            AvgSpeed = data.AvgSpeed;
            AvgPace = data.AvgPace;
            Distance = data.Distance;
        }

        public ARunData ToARunData()
        {
            ARunData data = new ARunData();
            for (int i = 0; i < longitude.Count(); i++)
            {
                GeoCoordinate geoCord = new GeoCoordinate();
                geoCord.Longitude = longitude[i];
                geoCord.Latitude = latitude[i];
                data.geoCollection.Add(geoCord);
            }
           
           
            //data.datetime = JsonConvert.DeserializeObject<DateTime>(datetime, new JavaScriptDateTimeConverter());
            try
            {
                data.datetime = TimeZoneInfo.ConvertTime(datetime, TimeZoneInfo.Local);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "The time will be set to default");
                data.datetime = DateTime.MinValue;
            }
            int dem = (int)IsolatedStorageSettings.ApplicationSettings["RunCount"];
            dem++;
            IsolatedStorageSettings.ApplicationSettings.Save();
            data.BurnedCalories = BurnedCalories;
            data.Duration = Duration;
            data.AvgSpeed = AvgSpeed;
            data.AvgPace = AvgPace;
            data.Distance = Distance;
            data.No = dem;
            data.IsSynced = true;
            return data;
        }
       
        
       
    }
}
