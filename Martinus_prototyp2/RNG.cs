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
                outp.Genes[random.Next(geneNumber)].Stop += moved-1;
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
        //public static int ChooseWhereToPlaceGene(Genom genom, bool inGenes = false)
        //{
        //    int genesLenInTotal = 0;
        //    for (int i = 0; i < genom.Genes.Length; i++) genesLenInTotal += genom.Genes[i].Length;
        //    int spaces = random.Next(genom.DNA.Length-genesLenInTotal);
        //    int genes = 0;
        //    for (int i = 0; i < genom.Genes.Length; i++) if (genom.Genes[i].Start < spaces + genes) genes += genom.Genes[i].Length;
        //    return spaces + genes;
        //}
        public static int ChooseWhereToPlaceGene(Sequence sequence, Genom HKGenes,PlaceMode mode = PlaceMode.OutOfGenes)
        {
            //Gene[] range = sequence.ToNonGeneArray(original);
            //int rndCluster = random.Next(range.Length);
            //return random.Next(range[rndCluster].Start, range[rndCluster].Stop);
            Gene[] spaces;
            int[] weights;
            Gene selectedSpace;
            switch (mode)
            {
                case PlaceMode.OutOfGenes:
                    spaces = sequence.ToNonGeneArray(HKGenes);
                    weights = new int[spaces.Length];
                    for (int i = 0; i < weights.Length; i++) { weights[i] = spaces[i].Length-1; if (weights[i] == 0) weights[i] = 1; }
                    selectedSpace = spaces[WeightedChoice(weights)];
                    //Gene selectedSpace = spaces[random.Next(spaces.Length)];
                    return random.Next(selectedSpace.Start, selectedSpace.Stop);

                //case PlaceMode.Cluster://dodělat
                //    int ClustringIndex = 2;
                //    spaces = sequence.ToNonGeneArray(HKGenes);
                //    weights = new int[spaces.Length];
                //    for (int i = 0; i < weights.Length; i++) { weights[i] = spaces[i].Length - 1; if (weights[i] == 0) weights[i] = 1; }
                //    selectedSpace = spaces[WeightedChoice(weights)];
                //    //Gene selectedSpace = spaces[random.Next(spaces.Length)];
                //    int[] indivWeidgt = new int[selectedSpace.Length];
                //    return random.Next(selectedSpace.Start, selectedSpace.Stop);
            }
            return -1;
            //Gene[] Genes= sequence.ToGeneArray();
            //int genesLenInTotal = 0;
            //for (int i = 0; i < Genes.Length; i++) genesLenInTotal += Genes[i].Length;
            //int spaces = random.Next(sequence.DNA.Length - genesLenInTotal);
            //int genes = 0;
            //for (int i = 0; i < Genes.Length; i++) if (Genes[i].Start < spaces + genes) genes += Genes[i].Length-1;//-1
            //return spaces + genes;
        }
        public static int WeightedChoice(int[] weights)
        {
            int[] indexes = Enumerable.Range(0, weights.Length).ToArray();
            int totalWeight = weights.Sum();
            int choice = random.Next(totalWeight);
            int sum = 0;
            foreach (int i in indexes)
            {
                sum += weights[i];
                if (choice < sum)
                {
                    return i;
                }
            }

            return 0;
            //Random random = new Random();
            //int[] indexes = Enumerable.Range(0,weights.Length).ToArray();
            //int selected = -1;
            //int bestScore = 0;
            //Shuffle(indexes);
            //foreach(int i in indexes)
            //{
            //    //Gene gene = genes[i];
            //    int s = random.Next(0, weights[i]);
            //    if (bestScore <= s)
            //    {
            //        selected = i;
            //        bestScore = s;
            //    }
            //}
            //return selected;
        }

        public enum PlaceMode
        {
            OutOfGenes,
            InGenes,
            Cluster,
            Random
        }
    }
}
