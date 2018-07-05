﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System;

namespace EventBuilder.Platforms
{
    public class Winforms : BasePlatform
    {
        public Winforms()
        {
            if (PlatformHelper.IsRunningOnMono()) {
                throw new NotSupportedException("Building events for Winforms on Mac is not implemented.");
            } else {
                // BackgroundWorker
                // EventLog
                // FileSystemWatcher
                // PerformanceCounter
                // Process
                // SerialPort
                Assemblies.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.1\System.dll");
                // DataSet
                Assemblies.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.1\System.Data.dll");
                // DirectoryEntry
                // DirectorySearcher
                Assemblies.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.1\System.DirectoryServices.dll");                
                // PrintDocument
                Assemblies.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.1\System.Drawing.dll");
                // MessageQueue
                Assemblies.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.1\System.Messaging.dll");                
                // BindingNavigator 
                // ToolStripButton 
                // ToolStripLabel 
                // ToolStripButton 
                // ToolStripButton 
                // ToolStripButton 
                // ToolStripSeparator 
                // ToolStripTextBox 
                // ToolStripSeparator 
                // ToolStripButton 
                // ToolStripButton 
                // ToolStripSeparator 
                // BindingSource 
                // Button 
                // CheckBox 
                // CheckedListBox 
                // ColorDialog 
                // ComboBox 
                // ContextMenuStrip 
                // DataGridView 
                // DateTimePicker 
                // DomainUpDown 
                // ErrorProvider 
                // WebBrowser 
                // VScrollBar 
                // TreeView 
                // ToolStripContainer 
                // TrackBar 
                // ToolStrip 
                // SplitContainer 
                // TabControl 
                // TabPage 
                // TableLayoutPanel 
                // TextBox 
                // TabPage 
                // StatusStrip 
                // Splitter 
                // RichTextBox 
                // RadioButton 
                // PropertyGrid 
                // ProgressBar 
                // PrintPreviewControl 
                // PictureBox 
                // Panel 
                // NumericUpDown 
                // MonthCalendar 
                // MaskedTextBox 
                // ListView 
                // ListBox 
                // LinkLabel 
                // Label 
                // HScrollBar 
                // GroupBox 
                // FlowLayoutPanel 
                // MenuStrip 
                // FolderBrowserDialog 
                // FontDialog 
                // HelpProvider 
                // ImageList 
                // NotifyIcon 
                // OpenFileDialog 
                // PageSetupDialog 
                // PrintDialog 
                // PrintPreviewDialog 
                // SaveFileDialog 
                // Timer 
                // ToolTip 
                Assemblies.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.1\System.Windows.Forms.dll");
                // Chart
                Assemblies.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.1\System.Windows.Forms.DataVisualization.dll");
                // ServiceController
                Assemblies.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.1\System.ServiceProcess.dll");

                CecilSearchDirectories.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.1");
            }
        }
    }
}
