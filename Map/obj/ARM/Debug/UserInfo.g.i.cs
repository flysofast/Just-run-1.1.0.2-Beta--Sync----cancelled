﻿#pragma checksum "D:\Rin\Visual Studio projects\Windows Phone 8\Students\201306\Windows Phone 8\Nam\Map Beta\Map\UserInfo.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "501A30E7FA8F8417534D6C9C7A3B5CE6"
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
    
    
    public partial class UserInfo : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.TextBox tbAge;
        
        internal System.Windows.Controls.TextBox tbWeight;
        
        internal System.Windows.Controls.Button btSaveUserInfo;
        
        internal System.Windows.Controls.ProgressBar prgBar;
        
        internal System.Windows.Controls.TextBox tbGrade;
        
        internal Microsoft.Phone.Controls.ListPicker lpGender;
        
        internal System.Windows.Controls.TextBlock tbTotalCalories;
        
        internal System.Windows.Controls.TextBlock tbTotalDistance;
        
        internal System.Windows.Controls.TextBlock tbLastRun;
        
        internal System.Windows.Controls.TextBlock tbTotalDuration;
        
        internal System.Windows.Controls.TextBlock tbAvgSpeed;
        
        internal System.Windows.Controls.TextBlock tbAvgPace;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/Map;component/UserInfo.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.tbAge = ((System.Windows.Controls.TextBox)(this.FindName("tbAge")));
            this.tbWeight = ((System.Windows.Controls.TextBox)(this.FindName("tbWeight")));
            this.btSaveUserInfo = ((System.Windows.Controls.Button)(this.FindName("btSaveUserInfo")));
            this.prgBar = ((System.Windows.Controls.ProgressBar)(this.FindName("prgBar")));
            this.tbGrade = ((System.Windows.Controls.TextBox)(this.FindName("tbGrade")));
            this.lpGender = ((Microsoft.Phone.Controls.ListPicker)(this.FindName("lpGender")));
            this.tbTotalCalories = ((System.Windows.Controls.TextBlock)(this.FindName("tbTotalCalories")));
            this.tbTotalDistance = ((System.Windows.Controls.TextBlock)(this.FindName("tbTotalDistance")));
            this.tbLastRun = ((System.Windows.Controls.TextBlock)(this.FindName("tbLastRun")));
            this.tbTotalDuration = ((System.Windows.Controls.TextBlock)(this.FindName("tbTotalDuration")));
            this.tbAvgSpeed = ((System.Windows.Controls.TextBlock)(this.FindName("tbAvgSpeed")));
            this.tbAvgPace = ((System.Windows.Controls.TextBlock)(this.FindName("tbAvgPace")));
        }
    }
}

