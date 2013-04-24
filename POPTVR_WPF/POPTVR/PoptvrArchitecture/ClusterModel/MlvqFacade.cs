using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using POPTVR.Entities;
using POPTVR.Utilities;

namespace POPTVR.PoptvrArchitecture.ClusterModel
{
    [Serializable]
    public class MlvqFacade : ClusterFacadeInterface
    {
        private ClusterSetting clusterSetting;
        private DataSet dataset;

        public MlvqFacade()
        {
        }

        public MlvqFacade(DataSet dataset, ClusterSetting clusterSetting)
        {
            this.clusterSetting = clusterSetting;
            this.dataset = dataset;
        }

        public ClusterSetting ClusterSetting
        {
            set
            {
                this.clusterSetting = value;
            }
        }

        public DataSet DataSet
        {
            set
            {
                this.dataset = value;
            }
        }

        public ClusterInterface getInputCluster(AppConfig appConfig)
        {
            ClusterInterface cluster = new MLVQ(this.dataset.NumberOfInputNodes, this.clusterSetting.InputClusterSize);
            cluster.computeInitialCentroids(this.dataset.Inputdata, new double[0, 0], this.dataset.TotalNumberOfRecords);

            FileWriter fileWriter = new FileWriter(appConfig);

            fileWriter.WriteDoubleArray("inputIniCent.txt", cluster.Centroids);
            fileWriter.WriteDoubleArray("inputIniWidths.txt", cluster.Widths);

            cluster.computeFinalCentroids(this.dataset.Inputdata, this.dataset.DesiredOutputs, this.dataset.TotalNumberOfRecords);
            fileWriter.WriteDoubleArray("inputFinCent.txt", cluster.Centroids);
            fileWriter.WriteDoubleArray("inputFinWidths.txt", cluster.Widths);

            return cluster;
        }

        public ClusterInterface getOutputCluster(AppConfig appConfig)
        {
            ClusterInterface cluster = new MLVQ(this.dataset.NumberOfOutputNodes, this.clusterSetting.OutputClusterSize);
            cluster.computeInitialCentroids(this.dataset.DesiredOutputs, new double[0, 0], this.dataset.TotalNumberOfRecords);

            FileWriter fileWriter = new FileWriter(appConfig);

            fileWriter.WriteDoubleArray("outputIniCent.txt", cluster.Centroids);
            fileWriter.WriteDoubleArray("outputIniWidths.txt", cluster.Widths);
            
            cluster.computeFinalCentroids(this.dataset.DesiredOutputs, this.dataset.DesiredOutputs, this.dataset.TotalNumberOfRecords);

            fileWriter.WriteDoubleArray("outputFinCent.txt", cluster.Centroids);
            fileWriter.WriteDoubleArray("outputFinWidths.txt", cluster.Widths);

            return cluster;
        }
    }
}
