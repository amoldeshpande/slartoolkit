﻿#pragma checksum "I:\github\slartoolkit\trunk\SLARToolKit\Source\SLARToolKitWinPhoneSample\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "894DE0CC903AE0C8B7E2180C37CE5201"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
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


namespace SLARToolKitWinPhoneSample {
    
    
    public partial class MainPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal System.Windows.Controls.Grid CompositionGrid;
        
        internal System.Windows.Shapes.Rectangle Viewport;
        
        internal System.Windows.Controls.Image ViewportOverlay;
        
        internal System.Windows.Controls.TextBox Txt;
        
        internal System.Windows.Controls.Image Img;
        
        internal System.Windows.Controls.TextBlock TxtDiag;
        
        internal System.Windows.Controls.TextBlock ApplicationTitle;
        
        internal System.Windows.Controls.RadioButton RBImage;
        
        internal System.Windows.Controls.RadioButton RBText;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/SLARToolKitWinPhoneSample;component/MainPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.CompositionGrid = ((System.Windows.Controls.Grid)(this.FindName("CompositionGrid")));
            this.Viewport = ((System.Windows.Shapes.Rectangle)(this.FindName("Viewport")));
            this.ViewportOverlay = ((System.Windows.Controls.Image)(this.FindName("ViewportOverlay")));
            this.Txt = ((System.Windows.Controls.TextBox)(this.FindName("Txt")));
            this.Img = ((System.Windows.Controls.Image)(this.FindName("Img")));
            this.TxtDiag = ((System.Windows.Controls.TextBlock)(this.FindName("TxtDiag")));
            this.ApplicationTitle = ((System.Windows.Controls.TextBlock)(this.FindName("ApplicationTitle")));
            this.RBImage = ((System.Windows.Controls.RadioButton)(this.FindName("RBImage")));
            this.RBText = ((System.Windows.Controls.RadioButton)(this.FindName("RBText")));
        }
    }
}

