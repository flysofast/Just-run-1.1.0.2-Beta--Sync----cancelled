using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Device.Location;
using Windows.Devices.Geolocation;
using Microsoft.Phone.Controls;
using System.Windows.Media;
using System.Windows.Shapes;


namespace Map
{
    public static class CoordinateConverter
    {
        public static GeoCoordinate ConvertGeoCoodinate(Geocoordinate geocoodinate)
        {
            return new GeoCoordinate
            (
                geocoodinate.Latitude,
                geocoodinate.Longitude,
                geocoodinate.Altitude ?? Double.NaN,
                geocoodinate.Accuracy,
                geocoodinate.AltitudeAccuracy ?? Double.NaN,
                geocoodinate.Speed ?? Double.NaN,
                geocoodinate.Heading ?? Double.NaN

            );
        }
    }
}
