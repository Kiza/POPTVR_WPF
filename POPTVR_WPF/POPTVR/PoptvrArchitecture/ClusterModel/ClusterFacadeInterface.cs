using System;
using POPTVR.Entities;
using POPTVR.Utilities;
namespace POPTVR.PoptvrArchitecture.ClusterModel
{
    public interface ClusterFacadeInterface
    {
        ClusterSetting ClusterSetting { set; }
        DataSet DataSet { set; }

        ClusterInterface getInputCluster(AppConfig appConfig);
        ClusterInterface getOutputCluster(AppConfig appConfig);
    }
}
