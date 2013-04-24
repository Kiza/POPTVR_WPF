using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using POPTVR.Utilities;
using System.Configuration;
using POPTVR_WPF;

namespace POPTVR.Entities
{
    [Serializable]
    public class ClusterSetting
    {
        private int inputClusterSize;
        private int outputClusterSize;
        private int maxTrainCycleNumber;
        private int printOutInterval;
        private double widthConstant;
        private double maxError;
        private double learningRate;

        public ClusterSetting(int inputClusterSize, int outputClusterSize, int maxTrainCycleNumber, int printOutInterval, double widthConstant, double maxError, double learningRate)
        {
            this.inputClusterSize = inputClusterSize;
            this.outputClusterSize = outputClusterSize;
            this.maxTrainCycleNumber = maxTrainCycleNumber;
            this.printOutInterval = printOutInterval;
            this.widthConstant = widthConstant;
            this.maxError = maxError;
            this.learningRate = learningRate;
        }

        public int InputClusterSize
        {
            get
            {
                return this.inputClusterSize;
            }
        }
        public int OutputClusterSize
        {
            get
            {
                return this.outputClusterSize;
            }
        }
        public int MaxTrainCycleNumber
        {
            get
            {
                return this.maxTrainCycleNumber;
            }
        }
        public int PrintOutInterval
        {
            get
            {
                return printOutInterval;
            }
        }
        public double WidthConstant
        {
            get
            {
                return this.widthConstant;
            }
        }
        public double MaxError
        {
            get
            {
                return maxError;
            }
        }
        public double LearningRate
        {
            get
            {
                return learningRate;
            }
        }

        public static ClusterSetting getClusterSettings(Settings settings)
        {
            int inpuClusterSize = settings.InputClusterSize;
            int outputClusterSize = settings.OutputClusterSize;

            int maxTrainCycleNumber = settings.MaxTrainCycleNumber;
            int printOutInterval = settings.PrintOutInterval;
            double widthConstant = settings.WidthConstant;
            double maxError = settings.MaxError;
            double learningRate = settings.LearningRate;

            ClusterSetting clusterSetting = new ClusterSetting(inpuClusterSize, outputClusterSize, maxTrainCycleNumber, printOutInterval, widthConstant, maxError, learningRate);
            return clusterSetting;
        }
    }
}
