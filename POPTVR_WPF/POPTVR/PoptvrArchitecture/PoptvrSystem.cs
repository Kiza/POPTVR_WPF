using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using POPTVR.Entities;
using POPTVR.PoptvrArchitecture.ClusterModel;
using POPTVR.Utilities;
using POPTVR_WPF;

namespace POPTVR.PoptvrArchitecture
{
    [Serializable]
    public class PoptvrSystem
    {
        private DataSet dataset;
        private ClusterSetting clusterSetting;
        private PoptvrModel poptvr;

        private ClusterFacadeInterface clusterFacade;
        public ClusterFacadeInterface ClusterFacade
        {
            get
            {
                return clusterFacade;
            }
        }

        private ClusterInterface inputCluster;
        private ClusterInterface outputCluster;

        private AppConfig appConfig;
        public AppConfig AppConfig
        {
            get
            {
                return this.appConfig;
            }
        }

        public PoptvrModel POPTVR
        {
            get
            {
                return poptvr;
            }

            set
            {
                poptvr = value;
            }
        }

        public PoptvrSystem(DataSet dataset, ClusterSetting clusterSetting, ClusterFacadeInterface clusterFacade, Settings settings)
        {
            this.dataset = dataset;
            this.clusterSetting = clusterSetting;
            this.clusterFacade = clusterFacade;
            this.appConfig = new AppConfig(settings);
        }

        public void InitClusters(ClusterFacadeInterface clusterFacade)
        {
            this.clusterFacade = clusterFacade;
            this.InitClusters();
        }
        public void InitClusters()
        {
            this.clusterFacade.ClusterSetting = this.clusterSetting;
            this.clusterFacade.DataSet = this.dataset;

            this.inputCluster = this.clusterFacade.getInputCluster(this.appConfig);
            this.outputCluster = this.clusterFacade.getOutputCluster(this.appConfig);
        }

        public PoptvrModel PopLearn()
        {
            this.poptvr = new PoptvrModel(this.dataset, this.clusterSetting);
            this.poptvr.initMembership(inputCluster, outputCluster);

            this.poptvr.popLearn();
            FileWriter fileWriter = new FileWriter(this.appConfig);
            fileWriter.WritePopLearnOutput(this.poptvr);

            return this.poptvr;
        }
        public void PopTest(DataSet dataset)
        {
            string result = PoptvrModel.POPTest(this.poptvr, dataset);
            FileWriter fileWriter = new FileWriter(this.appConfig);
            fileWriter.WritePopTestOutput(result);
        }

    }
}
