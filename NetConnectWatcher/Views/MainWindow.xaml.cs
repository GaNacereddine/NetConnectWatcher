using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using NetConnectWatcher.Abstractions;

namespace NetConnectWatcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        IConnectionsFilterer connectionsFilterer;

        public MainWindow()
        {
            DataContextChanged += MainWindow_DataContextChanged;
            InitializeComponent();
        }

        private void MainWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            connectionsFilterer = DataContext as IConnectionsFilterer;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textValue = (sender as TextBox)?.Text;

            if (string.IsNullOrEmpty(textValue))
            {
                connectionsFilterer?.FilterConnectionsByPid(-1);
            }
            else if (int.TryParse(textValue ?? "-1", out int value))
            {
                connectionsFilterer?.FilterConnectionsByPid(value);
            }
        }
    }
}
