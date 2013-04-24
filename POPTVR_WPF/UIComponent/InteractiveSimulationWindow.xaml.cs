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

namespace POPTVR_WPF.UIComponent
{
    /// <summary>
    /// Interaction logic for InteractiveSimulationWindow.xaml
    /// </summary>
    public partial class InteractiveSimulationWindow : Window
    {
        private bool done = false;
        private PoptvrSystem popSys;
        private Thread thread;
        private bool fio2Set = false;
        private double fio2;
        private bool peepSet = false;
        private double peep;
        private bool setRRSet = false;
        private double setRR;

        SolidColorBrush redBrush = new SolidColorBrush();
        SolidColorBrush greenBrush = new SolidColorBrush();

        public InteractiveSimulationWindow()
        {
            InitializeComponent();
            redBrush.Color = Colors.Red;
            greenBrush.Color = Colors.Green;

            thread = new Thread(this.DoSlowProcess);
            thread.Start();
        }

        public void setVariables(PoptvrSystem popSys)
        {
            this.popSys = popSys;
        }

        #region Long-Running Process
        private void DoSlowProcess()
        {
            string filename = popSys.AppConfig.TestingFileAddress;
            DataSet dataset = DataFileReader.ReadData(filename);
            double _fio2;
            double _pip;
            double _peep;
            double _map;
            double _setRR;
            double _etv;
            double _sao2;

            for (int i = 0; i < dataset.TotalNumberOfRecords; i++)
            {
                if (!this.fio2Set)
                {
                    _fio2 = dataset.Inputdata[i, 0]; //FiO2
                }
                else
                {
                    _fio2 = this.fio2;
                }

                _pip = dataset.Inputdata[i, 1];

                if (!this.peepSet)
                {
                    _peep = dataset.Inputdata[i, 2]; //PEEP
                }
                else
                {
                    _peep = this.peep;
                }

                _map = dataset.Inputdata[i, 3];

                if (!this.setRRSet)
                {
                    _setRR = dataset.Inputdata[i, 4];
                }
                else
                {
                    _setRR = setRR;
                }

                _etv = dataset.Inputdata[i, 5];
                _sao2 = dataset.Inputdata[i, 6];


                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (ThreadStart)delegate()
                    {

                        this.fio2_textBox.Text = _fio2.ToString(); //FiO2
                        this.pip_textBox.Text = _pip.ToString();
                        this.peep_textBox.Text = _peep.ToString(); //PEEP
                        this.map_textBox.Text = _map.ToString();
                        this.setRR_textBox.Text = _setRR.ToString();

                        this.etv_textBox.Text = _etv.ToString();
                        this.saO2_textBox.Text = _sao2.ToString(); //SaO2

                        if (_fio2 <= 40 && _peep <= 30 && _sao2 <= 95 && _sao2 >= 92)
                        {
                            this.currentState_Label.Content = "Good";
                            this.currentState_Rectangle.Fill = greenBrush;
                        }
                        else
                        {
                            this.currentState_Label.Content = "Bad";
                            this.currentState_Rectangle.Fill = redBrush;
                        }

                        this.time_Label.Content = i + 1;
                    }
                );

                double[] data = new double[4];
                data[0] = _fio2;
                data[1] = _peep;
                data[2] = _setRR;
                data[3] = _sao2;

                int result = PoptvrModel.POPTest(this.popSys.POPTVR, data, 2);
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (ThreadStart)delegate()
                    {
                        if (result == 0)
                        {
                            this.futureState_Label.Content = "Good";
                            this.futureState_Rectangle.Fill = greenBrush;
                        }
                        else
                        {
                            this.futureState_Label.Content = "Bad";
                            this.futureState_Rectangle.Fill = redBrush;
                        }
                    }
                );

                Thread.Sleep(new TimeSpan(0, 0, 1));
            }

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (ThreadStart)delegate()
                    {
                        this.time_Label.Content = "Done";
                    }
                );
            this.done = true;
        }

        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
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

        private void fio2_set_Button_Click(object sender, RoutedEventArgs e)
        {
            this.fio2Set = double.TryParse(this.fio2_set_TextBox.Text, out this.fio2);
        }

        private void peep_set_Button_Click(object sender, RoutedEventArgs e)
        {
            this.peepSet = double.TryParse(this.peep_set_TextBox.Text, out this.peep);
        }

        private void setRR_set_Button_Click(object sender, RoutedEventArgs e)
        {
            this.setRRSet = double.TryParse(this.setRR_set_TextBox.Text, out this.setRR);
        }
    }
}
