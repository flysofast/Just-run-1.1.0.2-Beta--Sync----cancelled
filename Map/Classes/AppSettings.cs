using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Map
{
    [DataContract]
    public class AppSettings
    {
        [DataMember]
        public string _language;

        [DataMember]
        public bool _3DObjects;

        [DataMember]
        public bool _AutoHeading;

        [DataMember]
        public double _DefaultZoomLevel;

        [DataMember]
        public double _DefaultPitchLevel;

        [DataMember]
        public bool _Sync=false;

        public AppSettings()
        {
            _language = Thread.CurrentThread.CurrentCulture.ToString();
            _3DObjects = _AutoHeading = true;
            _DefaultZoomLevel = 17;
            _DefaultPitchLevel = 55;
        }
    }
}
