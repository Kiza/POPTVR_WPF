using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;
using POPTVR_WPF;

namespace POPTVR.Utilities
{
    [Serializable]
    public class AppConfig
    {
        private int InputClusterSize { get; set; }
        private int OutputClusterSize { get; set; }
        private int MaxTrainCycleNumber { get; set; }
        private int PrintOutInterval { get; set; }
        private double WidthConstant { get; set; }
        private double MaxError { get; set; }
        private double LearningRate { get; set; }

        private string TrainingFileAddress { get; set; }
        public string TestingFileAddress { get; set; }
        public string OutputFolderAddress { get; set; }
        private string ClusteringMethod { get; set; }


        public AppConfig(Settings settings)
        {
            InputClusterSize = settings.InputClusterSize;
            OutputClusterSize = settings.OutputClusterSize;
            MaxTrainCycleNumber = settings.MaxTrainCycleNumber;
            PrintOutInterval = settings.PrintOutInterval;
            WidthConstant = settings.WidthConstant;
            MaxError = settings.MaxError;
            LearningRate = settings.LearningRate;
            TrainingFileAddress = settings.TrainingFileAddress;
            TestingFileAddress = settings.TestingFileAddress;
            OutputFolderAddress = settings.OutputFolderAddress;

            ClusteringMethod = settings.ClusterMethod;
        }

        public string getClusteringMethod()
        {
            return ClusteringMethod;
        }

        public int getInputClusterSize()
        {
            return InputClusterSize;
        }

        public int getOutputClusterSize()
        {
            return OutputClusterSize;
        }

        public int getMaxTrainCycleNumber()
        {
            return MaxTrainCycleNumber;
        }

        public int getPrintOutInterval()
        {
            return PrintOutInterval;
        }

        public double getWidthConstant()
        {
            return WidthConstant;
        }

        public double getMaxError()
        {
            return MaxError;
        }

        public double getLearningRate()
        {
            return LearningRate;
        }

        public string getTrainFilename()
        {
            return TrainingFileAddress;
        }

        public string getTestFilename()
        {
            return TestingFileAddress;
        }

        public string getOutputFolder()
        {
            return OutputFolderAddress;
        }
    }
}
