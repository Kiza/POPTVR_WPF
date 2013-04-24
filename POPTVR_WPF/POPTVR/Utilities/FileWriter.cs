using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using POPTVR.PoptvrArchitecture;
using POPTVR_WPF;

namespace POPTVR.Utilities
{
    [Serializable]
    public class FileWriter
    {
        private AppConfig appConfig;
        public FileWriter(AppConfig appConfig)
        {
            this.appConfig = appConfig;
        }

        public void WriteToFile(string fileName, string outputData)
        {
            outputData = outputData.Replace("\n", "\r\n");
            System.IO.File.WriteAllText(@fileName, outputData);
        }

        public void WritePopLearnOutput(PoptvrModel poptvr)
        {
            WritePopLearnOutput("", poptvr);
        }

        public void WritePopLearnOutput(string filename, PoptvrModel poptvr)
        {

            Console.Write("Output InitialWeights.txt ... ");
            this.WriteToFile(appConfig.getOutputFolder() + filename + "InitialWeights.txt", poptvr.InitialWeightsString);
            Console.WriteLine("Done!");

            Console.Write("Output SelectedWeights.txt ... ");
            this.WriteToFile(appConfig.getOutputFolder() + filename + "SelectedWeights.txt", poptvr.SelectedWeightsString);
            Console.WriteLine("Done!");

            Console.Write("Output FuzzyRulesString.txt ... ");
            this.WriteToFile(appConfig.getOutputFolder() + filename + "FuzzyRulesString.txt", poptvr.FuzzyRulesString);
            Console.WriteLine("Done!");
        }

        public void WritePopTestOutput(string outputData)
        {
            WritePopTestOutput("", outputData);
        }

        public void WritePopTestOutput(string filename, string outputData)
        {
            Console.Write("Output TestOutput.txt ... ");
            this.WriteToFile(appConfig.getOutputFolder() + filename + "TestOutput.txt", outputData);
            Console.WriteLine("Done!");
        }

        public void WriteDoubleArray(string filename, double[,] data)
        {
            string res = "";
            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    res += ("[" + i + ", " + j + "] = ");
                    res += (data[i, j].ToString() + "\t\t");
                }

                res += "\n";
            }
            this.WriteToFile(appConfig.getOutputFolder() + filename, res);

        }
    }
}
