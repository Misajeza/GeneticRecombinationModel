using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Martinus_prototyp2
{
    public class Data
    {
        public string Name { get; set; }
        public int[,] Viables { get; set; }
        public int[,] Hybrides { get; set; }
        public Genom[,] Genoms { get; set; }
        public Data(string Name, int tabliceHight, int tabliceWidth)
        {
            this.Name = Name;
            this.Viables = new int[tabliceHight, tabliceWidth];
            this.Hybrides = new int[tabliceHight, tabliceWidth];
            this.Genoms = new Genom[0, 0];
        }
        public Data(string Name, int tabliceHight, int tabliceWidth, int genomHeight, int genomWidth)
        {
            this.Name = Name;
            this.Viables = new int[tabliceHight, tabliceWidth];
            this.Hybrides = new int[tabliceHight, tabliceWidth];
            this.Genoms = new Genom[genomHeight, genomWidth];
        }
        string TabliceToString(int[,] tab) 
        {
            string outp = "";
            for (int i = 0; i < tab.GetLength(0); i++)
            {
                for (int j = 0; j < tab.GetLength(1); j++)
                {
                    outp += tab[i, j];
                    if (j < tab.GetLength(1) - 1) outp += ",";
                }
                if (i < tab.GetLength(0) - 1) outp += "\n";
            }
            return outp;
        }
        public override string ToString()
        {
            return $"Hybridisations{TabliceToString(Hybrides)}\nViables:{TabliceToString(Viables)}";
        }
    }
}