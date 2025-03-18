using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Martinus_prototyp2
{
    static class DataManagement
    {
        public static List<Genom[]> ToListOfMultipleGenoms(Genom[,] genoms)
        {
            List<Genom[]> outp = new List<Genom[]>();
            for (int i = 0; i < genoms.GetLength(0); i++)
            {
                outp.Add(new Genom[genoms.GetLength(1)]);
                for (int j = 0; j < genoms.GetLength(1); j++)
                {
                    outp[i][j] = genoms[i, j];
                }
            }
            return outp;
        }
    }
}
