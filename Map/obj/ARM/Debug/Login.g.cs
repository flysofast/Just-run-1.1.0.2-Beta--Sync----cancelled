﻿#pragma checksum "D:\Rin\Visual Studio projects\Windows Phone 8\Students\201306\Windows Phone 8\Nam\Map Beta\Map\Login.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "DCD647D03C3249DAA0336378F7F88745"
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
    
    
    public partial class Login : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal System.Windows.Controls.TextBox tbEmail;
        
        internal System.Windows.Controls.TextBox tbUsername;
        
        internal System.Windows.Controls.TextBox tbPassword;
        
        internal System.Windows.Controls.Button btSignUp;
        
        internal System.Windows.Controls.Button btSignIn;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/Map;component/Login.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.tbEmail = ((System.Windows.Controls.TextBox)(this.FindName("tbEmail")));
            this.tbUsername = ((System.Windows.Controls.TextBox)(this.FindName("tbUsername")));
            this.tbPassword = ((System.Windows.Controls.TextBox)(this.FindName("tbPassword")));
            this.btSignUp = ((System.Windows.Controls.Button)(this.FindName("btSignUp")));
            this.btSignIn = ((System.Windows.Controls.Button)(this.FindName("btSignIn")));
        }
    }
}

