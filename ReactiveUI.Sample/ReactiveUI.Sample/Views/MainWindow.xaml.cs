using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ReactiveUI.Sample.Models;
using ReactiveUI.Sample.ViewModels;

namespace ReactiveUI.Sample.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindowViewModel ViewModel { get; protected set; }

        public MainWindow()
        {
            ViewModel = new MainWindowViewModel(new AppModel());
            InitializeComponent();
        }
    }
}
