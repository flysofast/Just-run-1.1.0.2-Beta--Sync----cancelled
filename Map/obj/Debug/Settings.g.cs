﻿#pragma checksum "D:\Rin\Visual Studio projects\Windows Phone 8\Students\201306\Windows Phone 8\Nam\Map Beta\Map\Settings.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "31F38BE0B89BCAF2A5E3821E157C29F0"
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
using Microsoft.Phone.Controls.Primitives;
using Microsoft.Phone.Shell;
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
    
    
    public partial class Settings : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal Microsoft.Phone.Shell.ApplicationBarIconButton btSave_AB;
        
        internal Microsoft.Phone.Shell.ApplicationBarIconButton btCancel_AB;
        
        internal Microsoft.Phone.Shell.ApplicationBarIconButton btDefault_AB;
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal System.Windows.Controls.RadioButton rdbtLangUS;
        
        internal System.Windows.Controls.RadioButton rdbtLangVN;
        
        internal System.Windows.Controls.CheckBox cbx3DObjects;
        
        internal System.Windows.Controls.Slider sldPitchLevel;
        
        internal System.Windows.Controls.Slider sldZoomLevel;
        
        internal System.Windows.Controls.CheckBox cbxAutoHeading;
        
        internal Microsoft.Phone.Controls.Primitives.ToggleSwitchButton tbtnBackup;
        
        internal System.Windows.Controls.TextBlock tbAcc;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/Map;component/Settings.xaml", System.UriKind.Relative));
            this.btSave_AB = ((Microsoft.Phone.Shell.ApplicationBarIconButton)(this.FindName("btSave_AB")));
            this.btCancel_AB = ((Microsoft.Phone.Shell.ApplicationBarIconButton)(this.FindName("btCancel_AB")));
            this.btDefault_AB = ((Microsoft.Phone.Shell.ApplicationBarIconButton)(this.FindName("btDefault_AB")));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.rdbtLangUS = ((System.Windows.Controls.RadioButton)(this.FindName("rdbtLangUS")));
            this.rdbtLangVN = ((System.Windows.Controls.RadioButton)(this.FindName("rdbtLangVN")));
            this.cbx3DObjects = ((System.Windows.Controls.CheckBox)(this.FindName("cbx3DObjects")));
            this.sldPitchLevel = ((System.Windows.Controls.Slider)(this.FindName("sldPitchLevel")));
            this.sldZoomLevel = ((System.Windows.Controls.Slider)(this.FindName("sldZoomLevel")));
            this.cbxAutoHeading = ((System.Windows.Controls.CheckBox)(this.FindName("cbxAutoHeading")));
            this.tbtnBackup = ((Microsoft.Phone.Controls.Primitives.ToggleSwitchButton)(this.FindName("tbtnBackup")));
            this.tbAcc = ((System.Windows.Controls.TextBlock)(this.FindName("tbAcc")));
        }
    }
}

