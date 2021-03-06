﻿#pragma checksum "..\..\CreateJoinWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "DF647CA951AED98485C975752FB20D6E"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace MemoryGameServiceClient {
    
    
    /// <summary>
    /// CreateJoinWindow
    /// </summary>
    public partial class CreateJoinWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 13 "..\..\CreateJoinWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button CreateHostOnline_btn;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\CreateJoinWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Join_btn;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\CreateJoinWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox numOfPlays_txt;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\CreateJoinWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label numOfPlayer_la;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\CreateJoinWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label numOfPlays_la;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\CreateJoinWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox numOfPlayer_txt;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\CreateJoinWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox LevelBox;
        
        #line default
        #line hidden
        
        
        #line 57 "..\..\CreateJoinWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label level;
        
        #line default
        #line hidden
        
        
        #line 62 "..\..\CreateJoinWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button CreateHostNetwork_btn;
        
        #line default
        #line hidden
        
        
        #line 67 "..\..\CreateJoinWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox RoomName_txt;
        
        #line default
        #line hidden
        
        
        #line 72 "..\..\CreateJoinWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label NameRoom_la;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/MemoryGameServiceClient;component/createjoinwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\CreateJoinWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.CreateHostOnline_btn = ((System.Windows.Controls.Button)(target));
            
            #line 13 "..\..\CreateJoinWindow.xaml"
            this.CreateHostOnline_btn.Click += new System.Windows.RoutedEventHandler(this.CreateHostOnline_btn_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.Join_btn = ((System.Windows.Controls.Button)(target));
            
            #line 18 "..\..\CreateJoinWindow.xaml"
            this.Join_btn.Click += new System.Windows.RoutedEventHandler(this.Join_btn_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.numOfPlays_txt = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.numOfPlayer_la = ((System.Windows.Controls.Label)(target));
            return;
            case 5:
            this.numOfPlays_la = ((System.Windows.Controls.Label)(target));
            return;
            case 6:
            this.numOfPlayer_txt = ((System.Windows.Controls.TextBox)(target));
            return;
            case 7:
            this.LevelBox = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 8:
            this.level = ((System.Windows.Controls.Label)(target));
            return;
            case 9:
            this.CreateHostNetwork_btn = ((System.Windows.Controls.Button)(target));
            
            #line 62 "..\..\CreateJoinWindow.xaml"
            this.CreateHostNetwork_btn.Click += new System.Windows.RoutedEventHandler(this.CreateHostNetwork_btn_Click);
            
            #line default
            #line hidden
            return;
            case 10:
            this.RoomName_txt = ((System.Windows.Controls.TextBox)(target));
            return;
            case 11:
            this.NameRoom_la = ((System.Windows.Controls.Label)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

