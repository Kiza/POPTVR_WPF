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
using System.Windows.Threading;
using POPTVR.PoptvrArchitecture;
using POPTVR.Entities;
using POPTVR.Utilities;
using POPTVR.PoptvrArchitecture.ClusterModel;

namespace POPTVR_WPF.UIComponent
{
    /// <summary>
    /// Interaction logic for TrainingProgressWindow.xaml
    /// </summary>
    public partial class TrainingProgressWindow : Window
    {
        private Thread thread;
        private bool done = false;
        private bool kill = false;
        public PoptvrSystem PopSystem{get;set;}
        public Settings Settings {get; set;}

        public void setVariables(PoptvrSystem popSystem, Settings settings)
        {
            this.PopSystem = popSystem;
            this.Settings = settings;
        }

        public TrainingProgressWindow()
        {
            InitializeComponent();
            this.overallProgress_Bar.Maximum = 6;
            thread = new Thread(this.DoSlowProcess);
            thread.Start();
        }

        #region Long-Running Process
        private void DoSlowProcess()
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate()
                {
                    this.overallProgress_Label.Content = "Get Cluster Settings ...";
                }
            );
            ClusterSetting clusterSetting = ClusterSetting.getClusterSettings(Settings);

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, 
                (ThreadStart) delegate()
                {
                    this.overallProgress_Label.Content = "Read Trainging Data ...";
                    this.overallProgress_Bar.Value += 1;
                }
            );
            string filename = Settings.TrainingFileAddress;
            DataSet dataset = DataFileReader.ReadData(filename);

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate()
                {
                    this.overallProgress_Label.Content = "Setup Network ...";
                    this.overallProgress_Bar.Value += 1;
                }
            );

            ClusterFacadeInterface clusterFacade = null;
            if (this.Settings.ClusterMethod.Equals("clusterMethod_MLVQ"))
            {
                clusterFacade = new MlvqFacade();
            }
            else if (this.Settings.ClusterMethod.Equals("clusterMethod_RCT"))
            {
                clusterFacade = new RctFacade();
            }
            else if (this.Settings.ClusterMethod.Equals("clusterMethod_SO"))
            {
                clusterFacade = new SOclusterFacade();
            }
            else if (this.Settings.ClusterMethod.Equals("Method_SRCT"))
            {
                clusterFacade = new SrctFacade();
            }
            PopSystem = new PoptvrSystem(dataset, clusterSetting, clusterFacade, this.Settings);
            PopSystem.InitClusters();

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate()
                {
                    this.overallProgress_Label.Content = "Training Network ...";
                    this.overallProgress_Bar.Value += 1;
                }
            );
            PopSystem.PopLearn();

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate()
                {
                    this.overallProgress_Label.Content = "Read Testing Data ...";
                    this.overallProgress_Bar.Value += 1;
                }
            );
            filename = Settings.TrainingFileAddress;
            dataset = DataFileReader.ReadData(filename);

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate()
                {
                    this.overallProgress_Label.Content = "Run Testing Data ...";
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

                        this.Settings.TraingDone = true;
                        this.Settings.Save();

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
