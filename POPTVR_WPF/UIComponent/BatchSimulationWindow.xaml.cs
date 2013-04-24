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
using System.Threading;
using POPTVR.PoptvrArchitecture;
using System.Windows.Threading;
using POPTVR.Entities;
using POPTVR.Utilities;

namespace POPTVR_WPF.UIComponent
{
    /// <summary>
    /// Interaction logic for BatchSimulationWindow.xaml
    /// </summary>
    public partial class BatchSimulationWindow : Window
    {
        private Thread thread;
        private bool done = false;
        private bool kill = false;
        public PoptvrSystem PopSystem { get; set; }

        public BatchSimulationWindow()
        {
            InitializeComponent();
            this.overallProgress_Bar.Maximum = 3;
            thread = new Thread(this.DoSlowProcess);
            thread.Start();
        }

        public void setVariables(PoptvrSystem popSystem)
        {
            this.PopSystem = popSystem;       
        }
        #region Long-Running Process
        private void DoSlowProcess()
        {

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate()
                {
                    this.overallProgress_Label.Content = "Read Simulation Data ...";
                    this.overallProgress_Bar.Value += 1;
                }
            );

            string filename = PopSystem.AppConfig.TestingFileAddress;
            DataSet dataset = DataFileReader.ReadData(filename);

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate()
                {
                    this.overallProgress_Label.Content = "Run Simulation ...";
                    this.overallProgress_Bar.Value += 1;
                }
            );
            PopSystem.PopTest(dataset);


            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (ThreadStart)delegate()
                    {
                        this.overallProgress_Label.Content = "All Finished!";
                        this.overallProgress_Bar.Value += 1;
                        this.cancel_Button.Content = "Done";

                        this.done = true;
                    }
                );
        }

        #endregion

        private void cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!done)
            {
                MessageBoxResult result = MessageBox.Show("Cancel Training Process?", "Cancel Action", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    if (thread.IsAlive)
                    {
                        thread.Abort();
                    }
                    this.kill = true;
                    this.Close();
                }
            }
            else
            {
                if (thread.IsAlive)
                {
                    thread.Abort();
                }
                this.Close();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!done && !kill)
            {
                MessageBoxResult result = MessageBox.Show("Cancel Training Process?", "Cancel Action", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    if (thread.IsAlive)
                    {
                        thread.Abort();
                    }
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else
            {
                if (thread.IsAlive)
                {
                    thread.Abort();
                }
            }
        }
    }
}
