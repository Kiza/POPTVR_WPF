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

namespace POPTVR_WPF.UIComponent
{
    /// <summary>
    /// Interaction logic for SystemOutput.xaml
    /// </summary>
    public partial class SystemOutputWindow : Window
    {
        private bool kill = false;
        private int count = 0;

        public bool Kill
        {
            set
            {
                this.kill = value;
            }
        }

        public SystemOutputWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!kill)
            {
                e.Cancel = true;
                this.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        public void addOutput(string text)
        {
            this.textBlock.Text += count + ": " + text;
            this.textBlock.ScrollToEnd();
            count++;
        }
    }
}
