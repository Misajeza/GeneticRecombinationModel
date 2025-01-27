using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Martinus_prototyp2
{
    internal class CSV
    {
        string path;
        int[,] data;
        public CSV(string path, int[,] data)
        {
            this.path = path;
            this.data = data;
        }
        public void Save()
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                string startLine = "number of tralslocated genes ";
                for (int i = 0; i < data.GetLength(1); i++)
                {
                    startLine += $",{i}";
                }
                sw.WriteLine(startLine);
                for (int i = 0; i < data.GetLength(0); i++)
                {
                    string line = $"{i}";
                    for (int j = 0; j < data.GetLength(1); j++)
                    {
                        line += "," + data[i, j].ToString();
                    }
                    sw.WriteLine(line);
                }
                sw.Flush();
            }
        }
    }
}
