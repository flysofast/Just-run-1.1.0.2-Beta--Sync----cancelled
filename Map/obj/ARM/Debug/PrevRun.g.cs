﻿#pragma checksum "D:\Rin\Visual Studio projects\Windows Phone 8\Students\201306\Windows Phone 8\Nam\Map Beta\Map\PrevRun.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "5130393CA235D75C8366B334B84D8C92"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18051
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace Map {
    
    
    public partial class PrevRun : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.TextBlock AppName;
        
        internal System.Windows.Controls.TextBlock Date;
        
        internal System.Windows.Controls.TextBlock Time;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal Microsoft.Phone.Maps.Controls.Map Map;
        
        internal System.Windows.Controls.Slider sldZoomLevel;
        
        internal System.Windows.Controls.Button btHigher;
        
        internal System.Windows.Controls.Button btLower;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/Map;component/PrevRun.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.AppName = ((System.Windows.Controls.TextBlock)(this.FindName("AppName")));
            this.Date = ((System.Windows.Controls.TextBlock)(this.FindName("Date")));
            this.Time = ((System.Windows.Controls.TextBlock)(this.FindName("Time")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.Map = ((Microsoft.Phone.Maps.Controls.Map)(this.FindName("Map")));
            this.sldZoomLevel = ((System.Windows.Controls.Slider)(this.FindName("sldZoomLevel")));
            this.btHigher = ((System.Windows.Controls.Button)(this.FindName("btHigher")));
            this.btLower = ((System.Windows.Controls.Button)(this.FindName("btLower")));
        }
    }
}

