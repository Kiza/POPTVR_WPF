using System;
namespace POPTVR.PoptvrArchitecture.ClusterModel
{
    public interface ClusterInterface
    {
        double[,] Centroids { get; }
        double[,] Widths { get; }

        void computeInitialCentroids(double[,] inputdata, double[,] outputdata, int count);
        void computeFinalCentroids(double[,] inputs, double[,] outputs, int count);
        
    }
}
