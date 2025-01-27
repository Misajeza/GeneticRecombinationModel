using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Martinus_prototyp2
{
    internal static class RNG
    {
        static Random random = new Random();
        public static int Int() { return random.Next(); }
        public static int Int(int max) { return random.Next(max);}
        public static int Int(int min, int max) { return random.Next(min,max);}
        public static void Shuffle<T>( T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = random.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }
        public static int Exclude(int exclude, int to)
        {
            int outp = exclude;
            while (outp == exclude) outp = random.Next(to);
            return outp;
        }
        public static Genom GenerateGenom(int length, float percentil, int geneNumber, int minSize) 
        {
            string[] nucleotides = { "A", "G", "T", "C" };
            string DNA = "";
            for (int i = 0; i < length; i++) DNA += nucleotides[random.Next(nucleotides.Length)];
            Genom outp = new Genom(DNA, new Gene[geneNumber]);
            for (int i = 0; i < geneNumber; i++) outp.Genes[i] = new Gene(i, (int)Math.Round((length * (1-percentil))/geneNumber), (int)Math.Round((length * percentil)/geneNumber) - minSize);
            

            for (int i = 0; i < geneNumber; i++)
            {
                int moved = random.Next(outp.Genes[i].Stop);
                outp.Genes[i].Stop += minSize - moved;
                outp.Genes[random.Next(geneNumber)].Stop += moved;
            }
            for (int i = 0; i < geneNumber; i++)
            {
                int moved = random.Next(outp.Genes[i].Start);
                outp.Genes[i].Start -= moved;
                int indx = random.Next(geneNumber+1);
                if (indx < geneNumber) outp.Genes[indx].Start += moved;
            }
            int endPos = 0;
            for (int i = 0; i < geneNumber; i++)
            {
                outp.Genes[i].Start += endPos;
                outp.Genes[i].Stop += outp.Genes[i].Start;
                endPos= outp.Genes[i].Stop;
            }
            return outp;
        }
        public static int ChooseWhereToPlaceGene(Genom genom, bool inGenes = false)
        {
            int genesLenInTotal = 0;
            for (int i = 0; i < genom.Genes.Length; i++) genesLenInTotal += genom.Genes[i].Length;
            int spaces = random.Next(genom.DNA.Length-genesLenInTotal);
            int genes = 0;
            for (int i = 0; i < genom.Genes.Length; i++) if (genom.Genes[i].Start < spaces + genes) genes += genom.Genes[i].Length;
            return spaces + genes;
        }
        public static int ChooseWhereToPlaceGene(Sequence sequence, Genom original, bool inGenes = false)
        {
            Gene[] range = sequence.ToNonGeneArray(original);
            int rndCluster = random.Next(range.Length);
            return random.Next(range[rndCluster].Start, range[rndCluster].Stop);


            //Gene[] Genes= sequence.ToGeneArray();
            //int genesLenInTotal = 0;
            //for (int i = 0; i < Genes.Length; i++) genesLenInTotal += Genes[i].Length;
            //int spaces = random.Next(sequence.DNA.Length - genesLenInTotal);
            //int genes = 0;
            //for (int i = 0; i < Genes.Length; i++) if (Genes[i].Start < spaces + genes) genes += Genes[i].Length-1;//-1
            //return spaces + genes;
        }
    }
}
