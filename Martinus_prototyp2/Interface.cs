using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Martinus_prototyp2
{
    internal class Interface
    {
        List <ProgressBar> progress;
        DateTime StartTime = DateTime.Now;
        public bool Finished = false;
        public Interface() 
        {
            progress = new List <ProgressBar>();
        }
        public static void WriteSequence(Sequence seq) { //neníí funkční

            Gene[] unSorted = seq.ToGeneArray(); 
            Gene[] genes = new Gene[unSorted.Length];
            int last = -1;
            for (int i = 0; i < unSorted.Length; i++)
            {
                int best = int.MaxValue;
                int indx = 0;
                for (int j = 0; j < unSorted.Length; j++)
                {
                    if (unSorted[j].Start < best && last < unSorted[j].Start) { best = unSorted[j].Start; indx = j;}
                }
                last = best;
                genes[i] = unSorted[indx];
            }
            foreach (Gene gene in genes) Console.WriteLine(gene.ToString());
            for (int i = 0; i < genes.Length; i++)
            {
                if (i==0) Console.Write(seq.DNA.Substring(0, genes[i].Start));
                Console.BackgroundColor = ConsoleColor.Red;
                Console.Write(seq.DNA.Substring(genes[i].Start, genes[i].Stop- genes[i].Start));
                Console.BackgroundColor = ConsoleColor.Black;
                if (i+1<genes.Length && genes[i + 1].Start - genes[i].Stop>0) Console.Write(seq.DNA.Substring(genes[i].Stop, genes[i+1].Start-genes[i].Stop));
                else Console.Write(seq.DNA.Substring(genes[i].Stop, seq.DNA.Length- genes[i].Stop));
            }
        }//potřeba opravit
        public void NewProgressbar(ProgressBar pb)
        {
            progress.Add(pb);
        }
        public void UpdateBar (int indx, int val) 
        {
            progress[indx].Actual = val;
        }
        public void Write()
        {
            string outp = "";
            outp += $"Start:{StartTime}   Run:{DateTime.Now - StartTime}\n" ;
            for (int i = 0; i < progress.Count; i++)
            {
                outp += $"{progress[i]}\n";
            }
            if (Finished) { outp += $"End:{DateTime.Now}   Run:{DateTime.Now - StartTime}\n\n";  }
            Console.SetCursorPosition(0, 0);
            Console.WriteLine(outp);
            
        }
    }
   
    internal class ProgressBar
    {
        int Rank;
        public string Title = "";
        public int Actual;
        int Max;
        int Length;
        public ProgressBar(string Title, int Max, int Rank = 0, int Length = 20)
        {  
            this.Title = Title;
            this.Max = Max-1;
            this.Rank = Rank;
            this.Length = Length;
            Console.CursorVisible = false;

        }
        public override string ToString()
        {
            string tab = "";
            for (int i = 0; i < Rank; i++) tab += "  ";
            string outp = $"{tab}{Title}\n{tab}|";
            for (int i = 0; i < Length * ((float)Actual/Max); i++) outp += "#";
            for (int i = 0; i <= (Length * (1- (float)Actual /Max))-1; i++) outp += " ";
            outp += $"|{Math.Round(100*((float)Actual/Max))}%  ";
            return outp;
        }
    }
}
