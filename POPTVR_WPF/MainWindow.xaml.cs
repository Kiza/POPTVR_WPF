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
using System.Windows.Navigation;
using System.Windows.Shapes;
using POPTVR.PoptvrArchitecture;
using System.IO;
using POPTVR_WPF.UIComponent;

namespace POPTVR_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SystemOutputWindow sysOutputWindow = new SystemOutputWindow();
        private Settings settings = Settings.Default;
        private bool init = true;

        public MainWindow()
        {
            InitializeComponent();      
            this.init = false;

            LoadSettingValues();      
        }

        private void LoadSettingValues()
        {
            this.inputClusterSize_Slider.Value = this.settings.InputClusterSize;
            this.outputClusterSize_Slider.Value = this.settings.OutputClusterSize;

            this.trainingFile_Textbox.Text = this.settings.TrainingFileAddress;
            this.testingFile_Textbox.Text = this.settings.TestingFileAddress;
            this.outputFolder_Textbox.Text = this.settings.OutputFolderAddress;

            this.maxError_Texbox.Text = this.settings.MaxError.ToString();
            this.maxNumberTrainingCycles_Textbox.Text = this.settings.MaxTrainCycleNumber.ToString();
            this.printOutInterval_TextBox.Text = this.settings.PrintOutInterval.ToString();
            this.widthConstant_Textbox.Text = this.settings.WidthConstant.ToString();
            this.learingRate_Textbox.Text = this.settings.LearningRate.ToString();

            if (this.settings.ClusterMethod.Equals(this.clusterMethod_MLVQ.Name))
            {
                this.clusterMethod_MLVQ.IsChecked = true;
            }
            else if (this.settings.ClusterMethod.Equals(this.clusterMethod_RCT.Name))
            {
                this.clusterMethod_RCT.IsChecked = true;
            }
            else if (this.settings.ClusterMethod.Equals(this.clusterMethod_SO.Name))
            {
                this.clusterMethod_SO.IsChecked = true;
            }
            else if (this.settings.ClusterMethod.Equals(this.clusterMethod_SRCT.Name))
            {
                this.clusterMethod_SRCT.IsChecked = true;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.settings.TraingDone = false;
            this.settings.Save();
            this.sysOutputWindow.Kill = true;
            this.sysOutputWindow.Close();
        }

        private string selectFile()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Configure open file dialog box
            dlg.DefaultExt = ".txt"; // Default file extension
            dlg.Filter = "Text files (*.txt)|*.txt" +
                         "|" + "Data (*.dat)|*.dat" +
                         "|" + "All files (*.*)|*.*";
            dlg.Multiselect = false;

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                return dlg.FileName;
            }

            return null;
        }

        private void ShowSystemOutput_Click(object sender, RoutedEventArgs e)
        {
            sysOutputWindow.addOutput("Click on " + sender.ToString() + "\n");
            sysOutputWindow.Visibility = System.Windows.Visibility.Visible;
        }


        #region Training Tab
        private TrainingProgressWindow trainingProgressWindow;
        private PoptvrSystem popSystem;

        private void trainingFile_Button_Click(object sender, RoutedEventArgs e)
        {
            string temp = selectFile();
            if (!string.IsNullOrWhiteSpace(temp))
            {
                this.settings.TrainingFileAddress = temp;
                this.trainingFile_Textbox.Text = temp;
                this.sysOutputWindow.addOutput("Selected training file: " + this.settings.TrainingFileAddress + "\n");
                settings.Save();
            }
        }

        private void testingFile_Button_Click(object sender, RoutedEventArgs e)
        {
            string temp = selectFile();
            if (!string.IsNullOrWhiteSpace(temp))
            {
                this.settings.TestingFileAddress = temp;
                this.testingFile_Textbox.Text = temp;
                this.sysOutputWindow.addOutput("Selected testing file: " + this.settings.TestingFileAddress + "\n");
                settings.Save();
            }
        }

        private void outputFolder_Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();

            System.Windows.Forms.DialogResult result = dlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                this.settings.OutputFolderAddress = dlg.SelectedPath + "\\";
                this.sysOutputWindow.addOutput("Output folder: " + this.settings.OutputFolderAddress + "\n");
                this.outputFolder_Textbox.Text = this.settings.OutputFolderAddress;
                settings.Save();
            }

        }

        private void maxNumberTrainingCycles_Textbox_LostFocus(object sender, RoutedEventArgs e)
        {
            int temp;
            bool parsed = int.TryParse(((TextBox)sender).Text, out temp);
            if (parsed)
            {
                this.settings.MaxTrainCycleNumber = temp;
                this.settings.Save();
            }
            else
            {
                MessageBox.Show("Invalid data for Max Number of Training Cycles!\nIt should be a int.","Error!",MessageBoxButton.OK,MessageBoxImage.Error);
                ((TextBox)sender).Text = this.settings.MaxTrainCycleNumber.ToString();
            }
        }

        private void widthConstant_Textbox_LostFocus(object sender, RoutedEventArgs e)
        {
            double temp;
            bool parsed = double.TryParse(((TextBox)sender).Text, out temp);
            if (parsed)
            {
                this.settings.WidthConstant = temp;
                this.settings.Save();
            }
            else
            {
                MessageBox.Show("Invalid data for Width Constant!\nIt should be a double.", "Error!",MessageBoxButton.OK,MessageBoxImage.Error);
                ((TextBox)sender).Text = this.settings.WidthConstant.ToString();
            }
        }

        private void maxError_Texbox_LostFocus(object sender, RoutedEventArgs e)
        {
            double temp;
            bool parsed = double.TryParse(((TextBox)sender).Text, out temp);
            if (parsed && temp<1 && temp >=0)
            {
                this.settings.MaxError = temp;
                this.settings.Save();
            }
            else
            {
                MessageBox.Show("Invalid data for Max Error!\nIt should be a double from [0,1)", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                ((TextBox)sender).Text = this.settings.MaxError.ToString();
            }
        }

        private void learingRate_Textbox_LostFocus(object sender, RoutedEventArgs e)
        {
            double temp;
            bool parsed = double.TryParse(((TextBox)sender).Text, out temp);
            if (parsed && temp < 1 && temp >= 0)
            {
                this.settings.LearningRate = temp;
                this.settings.Save();
            }
            else
            {
                MessageBox.Show("Invalid data for Learning Rate!\nIt should be a double from [0,1)", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                ((TextBox)sender).Text = this.settings.LearningRate.ToString();
            }
        }

        private void printOutInterval_TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            int temp;
            bool parsed = int.TryParse(((TextBox)sender).Text, out temp);
            if (parsed)
            {
                this.settings.PrintOutInterval = temp;
                this.settings.Save();
            }
            else
            {
                MessageBox.Show("Invalid data for Print Out Interval!\nIt should be a int.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                ((TextBox)sender).Text = this.settings.PrintOutInterval.ToString();
            }
        }

        private void clusterMethod_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            this.settings.ClusterMethod = radioButton.Name;
            this.settings.Save();
        }

        private void inputClusterSize_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!this.init)
            {
                this.settings.InputClusterSize = (int)((Slider)sender).Value;
                this.settings.Save();
            }
        }

        private void outputClusterSize_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!this.init)
            {
                this.settings.OutputClusterSize = (int)((Slider)sender).Value;
                this.settings.Save();
            }
        }

        private void StartTrain_Button_Click(object sender, RoutedEventArgs e)
        {     
            trainingProgressWindow = new TrainingProgressWindow();
            trainingProgressWindow.setVariables(this.popSystem, this.settings);

            trainingProgressWindow.ShowDialog();
        }

        private void ExportNetwork_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!this.settings.TraingDone)
            {
                MessageBox.Show("Cannot export network.\nThere is not completed training.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                this.popSystem = this.trainingProgressWindow.PopSystem;
                byte[] byteStream = POPTVR.Utilities.SystemFunctions.ObjectToByteArray(this.popSystem);
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();

                saveFileDialog.Filter = "POPTVR Network (*.poptvr)|*.poptvr" + "|" + "All File (*.*)| *.*";
                saveFileDialog.Title = "Save a POPTVR Network";
                saveFileDialog.ShowDialog();

                if (saveFileDialog.FileName != "")
                {
                    // Create a new stream to write to the file
                    BinaryWriter Writer = new BinaryWriter(File.OpenWrite(saveFileDialog.FileName));
                    Writer.Write(byteStream);
                    Writer.Flush();
                    Writer.Close();

                    MessageBox.Show(saveFileDialog.FileName + " has been saved.", "File Saved", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
        #endregion

        #region Simulation Tab
        private PoptvrSystem simPopSystem = null;
        private string simDataFile = null;
        private string simOutputFolder = null;

        private bool isSimReady()
        {
            if ((bool)this.batchMode_Simulator.IsChecked)
            {
                this.outputFolder_sim_Button.IsEnabled = true;
            }
            else
            {
                this.outputFolder_sim_Button.IsEnabled = false;
            }

            if ((bool)this.batchMode_Simulator.IsChecked && this.simOutputFolder == null)
            {
                this.start_sim_Button.IsEnabled = false;
                return false;
            }

            if (this.simPopSystem == null)
            {
                this.start_sim_Button.IsEnabled = false;
                return false;
            }

            if (!(bool)this.batchMode_Simulator.IsChecked && !(bool)this.interactiveMode_Simulator.IsChecked)
            {
                this.start_sim_Button.IsEnabled = false;
                return false;
            }

            if (this.simDataFile == null)
            {
                this.start_sim_Button.IsEnabled = false;
                return false;
            }

            this.start_sim_Button.IsEnabled = true;
            return true;
        }

        private void trained_Simulator_Checked(object sender, RoutedEventArgs e)
        {
            if (!this.settings.TraingDone)
            {
                MessageBox.Show("Cannot use trained network.\nThere is not completed training.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                ((RadioButton)sender).IsChecked = false;
            }
            else
            {
                this.popSystem = this.trainingProgressWindow.PopSystem;
                this.type_sim_Label.Content = ((RadioButton) sender).Content;
                displaySimulatorInfo(this.popSystem);
                this.loadNetwork_sim_Button.Visibility = System.Windows.Visibility.Hidden;

                this.simPopSystem = this.popSystem;
                this.fileCreatedTime_sim_Label.Content = "Just Now.";
            }

            isSimReady();
        }

        private void displaySimulatorInfo(PoptvrSystem popSys)
        {
            this.inputClusterSize_sim_Label.Content = popSys.AppConfig.getInputClusterSize();
            this.outputtClusterSize_sim_Label.Content = popSys.AppConfig.getOutputClusterSize();
            this.clusteringMethod_sim_Label.Content = popSys.AppConfig.getClusteringMethod();
        }

        private void imported_Simulator_Checked(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Configure open file dialog box
            dlg.DefaultExt = ".poptvr"; // Default file extension
            dlg.Filter = "POPTVR Network (*.poptvr)|*.poptvr" +
                         "|" + "All files (*.*)|*.*";
            dlg.Multiselect = false;

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();
            PoptvrSystem popSys;
            if (result == true)
            {
                FileStream fs = File.OpenRead(dlg.FileName);

                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                fs.Close();

                popSys = (PoptvrSystem)POPTVR.Utilities.SystemFunctions.ByteArrayToObject(bytes);

                this.type_sim_Label.Content = ((RadioButton)sender).Content;
                this.fileCreatedTime_sim_Label.Content = File.GetCreationTime(dlg.FileName).ToLongDateString() + " - " + File.GetCreationTime(dlg.FileName).ToLongTimeString();
                displaySimulatorInfo(popSys);

                this.simPopSystem = popSys;
                this.loadNetwork_sim_Button.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                ((RadioButton)sender).IsChecked = false;
            }

            isSimReady();
        }

        private void loadNetwork_sim_Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Configure open file dialog box
            dlg.DefaultExt = ".poptvr"; // Default file extension
            dlg.Filter = "POPTVR Network (*.poptvr)|*.poptvr" +
                         "|" + "All files (*.*)|*.*";
            dlg.Multiselect = false;

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();
            PoptvrSystem popSys;
            if (result == true)
            {
                FileStream fs = File.OpenRead(dlg.FileName);
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                fs.Close();

                popSys = (PoptvrSystem)POPTVR.Utilities.SystemFunctions.ByteArrayToObject(bytes);

                this.fileCreatedTime_sim_Label.Content = File.GetCreationTime(dlg.FileName).ToLongDateString() + " - " + File.GetCreationTime(dlg.FileName).ToLongTimeString();
                displaySimulatorInfo(popSys);

                this.simPopSystem = popSys;
                this.loadNetwork_sim_Button.Visibility = System.Windows.Visibility.Visible;
            }

            isSimReady();
        }

        private void selectDataFile_sim_Button_Click(object sender, RoutedEventArgs e)
        {
            string temp = selectFile();
            if (temp != null)
            {
                this.simDataFile = temp;
            }

            this.selectDataFile_sim_Textbox.Text = this.simDataFile;
            isSimReady();
        }

        private void batchMode_Simulator_Checked(object sender, RoutedEventArgs e)
        {
            isSimReady();
        }

        private void interactiveMode_Simulator_Checked(object sender, RoutedEventArgs e)
        {
            isSimReady();
        }

        private void outputFolder_sim_Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();

            System.Windows.Forms.DialogResult result = dlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                this.simOutputFolder = dlg.SelectedPath + "\\";
                this.outputFolder_sim_Textbox.Text = this.simOutputFolder;
            }

            isSimReady();
        }

        private void start_sim_Button_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)this.batchMode_Simulator.IsChecked)
            {
                this.simPopSystem.AppConfig.OutputFolderAddress = this.simOutputFolder;
                this.simPopSystem.AppConfig.TestingFileAddress = this.simDataFile;

                BatchSimulationWindow batchSimWindow = new BatchSimulationWindow();
                batchSimWindow.setVariables(this.simPopSystem);
                batchSimWindow.ShowDialog();
            }
            else
            {
                this.simPopSystem.AppConfig.TestingFileAddress = this.simDataFile;
                InteractiveSimulationWindow interactiveSimWindow = new InteractiveSimulationWindow();
                interactiveSimWindow.setVariables(this.simPopSystem);
                interactiveSimWindow.ShowDialog();
            }
        }

        
        #endregion

        

    }
}
