﻿#pragma checksum "..\..\DelBackup.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "C65454BBE1FB51ED468F81233FAECD13"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
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
using WhiteHorseCore;


namespace WhiteHorseCore {
    
    
    /// <summary>
    /// DelBackupUI
    /// </summary>
    public partial class DelBackupUI : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 19 "..\..\DelBackup.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton currentRadio;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\DelBackup.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton otherRadio;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\DelBackup.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView fileList;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\DelBackup.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button deleteBtn;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\DelBackup.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button cancelBtn;
        
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
            System.Uri resourceLocater = new System.Uri("/WhiteHorseCore;component/delbackup.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\DelBackup.xaml"
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
            this.currentRadio = ((System.Windows.Controls.RadioButton)(target));
            
            #line 19 "..\..\DelBackup.xaml"
            this.currentRadio.Checked += new System.Windows.RoutedEventHandler(this.CurrentRadio_Checked);
            
            #line default
            #line hidden
            return;
            case 2:
            this.otherRadio = ((System.Windows.Controls.RadioButton)(target));
            
            #line 20 "..\..\DelBackup.xaml"
            this.otherRadio.Checked += new System.Windows.RoutedEventHandler(this.OtherRadio_Checked);
            
            #line default
            #line hidden
            return;
            case 3:
            this.fileList = ((System.Windows.Controls.ListView)(target));
            return;
            case 4:
            this.deleteBtn = ((System.Windows.Controls.Button)(target));
            
            #line 33 "..\..\DelBackup.xaml"
            this.deleteBtn.Click += new System.Windows.RoutedEventHandler(this.DeleteBtn_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.cancelBtn = ((System.Windows.Controls.Button)(target));
            
            #line 34 "..\..\DelBackup.xaml"
            this.cancelBtn.Click += new System.Windows.RoutedEventHandler(this.CancelBtn_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

