using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static System.Formats.Asn1.AsnWriter;

namespace Martinus_prototyp2
{
    internal class Sequence
    {
        public string DNA {  get; set; }
        public Mark[] Start { get; set; }
        public Mark[] Stop { get; set; }
        public Sequence(string DNA, Mark[] Start, Mark[] Stop)
        {
            this.DNA = DNA;
            this.Start = Start;
            this.Stop = Stop;
        }
        public Sequence Subsequence(int pos, int len) {
            Mark[] tempStart = new Mark[Start.Length];
            int lastStart = 0;
            for (int i = 0; i < Start.Length; i++)
            {
                if (Start[i].Position >= pos && Start[i].Position < pos + len) tempStart[lastStart++] = Start[i];
            }

            Mark[] tempStop = new Mark[Stop.Length];
            int lastStop = 0;
            for (int i = 0; i < Stop.Length; i++)
            {
                if (Stop[i].Position >= pos && Stop[i].Position < pos + len) tempStop[lastStop++] = Stop[i];
            }
            Sequence outp = new Sequence(DNA.Substring(pos, len), new Mark[lastStart], new Mark[lastStop]);

            for (int i = 0; i < lastStart; i++) { outp.Start[i] = tempStart[i]; outp.Start[i].Position -= pos; }
            for (int i = 0; i < lastStop; i++)  { outp.Stop[i]  = tempStop[i];  outp.Stop[i].Position  -= pos; }
            return outp;
        }
        public Sequence Clone()
        {
            return new Sequence(DNA, (Mark[])Start.Clone(), (Mark[])Stop.Clone());
        }
        public void MoveGene(int indx, int pos)
        {
            int stopIndx = FindStopIndx(indx);
            if (stopIndx == -1) { return; }
            string moved = DNA.Substring(Start[indx].Position, Stop[stopIndx].Position - Start[indx].Position);
            if (pos > Start[indx].Position) if (pos < Stop[stopIndx].Position) pos = Start[indx].Position;
            else pos -= Stop[stopIndx].Position - Start[indx].Position;

            DNA = DNA.Substring(0, Start[indx].Position) + DNA.Substring(Stop[stopIndx].Position);
            DNA = DNA.Substring(0, pos) + moved + DNA.Substring(pos); //-44

            Corection(Start, indx, indx, stopIndx, pos);
            Corection(Stop, stopIndx, indx, stopIndx, pos+1);

            Stop[stopIndx].Position = Stop[stopIndx].Position - Start[indx].Position + pos;
            Start[indx].Position = pos;
        }
        public bool Integrate(Sequence seq)
        {
            string strt = seq.DNA.Substring(0, Settings.comparedSequenceSize);
            string end = seq.DNA.Substring(seq.DNA.Length - Settings.comparedSequenceSize);

            int[] possibleStarts = new int[0];
            int lastIndex = 0;
            while (true)
            {
                lastIndex = DNA.IndexOf(strt, lastIndex + 1);
                if (lastIndex == -1) break;
                possibleStarts = possibleStarts.Concat(new int[] { lastIndex }).ToArray();
            }
            if (possibleStarts.Length <= 0) return false;//Status.UNSUCCES;

            RNG.Shuffle(possibleStarts);

            int startIndx = -1;
            int endIndx = -1;
            for (int i = 0; i < possibleStarts.Length; i++)
            {
                endIndx = DNA.IndexOf(end, possibleStarts[i]);
                if (endIndx != -1) { startIndx = possibleStarts[i]; break; }
            }
            if (startIndx == -1) return false;//Status.UNSUCCES;
            Overwrite(seq, startIndx, endIndx + Settings.comparedSequenceSize);
            return true;//Status.SUCCES;

        }
        public bool IsViable(Genom original) 
        {
            bool[] isGene = new bool[original.Genes.Length];
            for (int i = 0; i < Start.Length; i++)
            {
                for (int j = 0; j < Stop.Length; j++)
                {
                    if (Start[i].Index != Stop[j].Index) continue;
                    if (Stop[j].Position - Start[i].Position == original.Genes[Start[i].Index].Length) isGene[Start[i].Index] = true;
                }
            }
            for (int i = 0; i < isGene.Length; i++) if (!isGene[i]) return false; 
            return true;
        }
        public bool IsViable2(Genom original)
        {
            bool[] isGene = new bool[original.Genes.Length];
            for (int i = 0; i < Start.Length; i++)
            {
                for (int j = 0; j < Stop.Length; j++)
                {
                    if (Start[i].Index != Stop[j].Index) continue;
                    if (Stop[j].Position - Start[i].Position == original.Genes[Start[i].Index].Length && DNA.Substring(Start[i].Position, Stop[j].Position - Start[i].Position) == original.DNA.Substring(original.Genes[Start[i].Index].Start, original.Genes[Start[i].Index].Stop - original.Genes[Start[i].Index].Start)) isGene[Start[i].Index] = true;
                }
            }
            for (int i = 0; i < isGene.Length; i++) if (!isGene[i]) return false;
            return true;
        }
        public enum Status
        {
            UNSUCCES,
            SUCCES
        }
        public Gene[] ToGeneArray()
        {
            Gene[] genes = new Gene[Start.Length];
            for (int i = 0; i < Start.Length; i++) genes[i] = new Gene(Start[i].Index, Start[i].Position, FindStopPos(Stop,i));
            Gene[] outp = genes.RemoveInvalid();
            return outp;
            {//int valid = 0;
            //Gene[] genes = new Gene[Start.Length];
            //for (int i = 0; i < Start.Length; i++) genes[i] = new Gene(Start[i].Index, Start[i].Position, -1);
            //for (int i = 0; i < Stop.Length; i++)
            //{
            //    for (int j = 0; j < genes.Length; j++)
            //    {
            //        if (genes[j].Index == Stop[i].Index && genes[j].Start <= Stop[i].Position)
            //        {
            //            if (genes[j].Stop == -1) genes[j].Stop = Stop[i].Position;
            //            else if (genes[j].Stop - genes[j].Start > Stop[i].Position - genes[j].Start) genes[j].Stop = Stop[i].Position;
            //            if (genes[i].Start <= genes[i].Stop) valid++;
            //        }
            //    }
            //}
            //Gene[] outp = new Gene[valid];
            //int indx = 0;
            //for (int i = 0; i < genes.Length; i++) if (genes[i].Start <= genes[i].Stop) outp[indx++] = genes[i];
            //return outp;
            }
        }
        public Gene[] ToNonGeneArray(Genom genom) 
        {
            Gene[] genes = ToGeneArray().OnlyHKG(genom);
            Gene[] outp = new Gene[genes.Length+1];
            outp[0] = new Gene(-1, 0, DNA.Length);
            int length = genes.Length;
            int candidatIndex = -1;
            for (int i = 0; i < outp.Length-1; i++) {
                for (int j = 0; j < length; j++) {
                    if (genes[j].Start >= outp[i].Start && genes[j].Start < outp[i].Stop) {
                        candidatIndex = j;
                        outp[i].Stop = genes[j].Start;
                    }  
                }
                outp[i + 1] = new Gene(-1, genes[candidatIndex].Stop, DNA.Length);
                genes[candidatIndex] = genes[--length];
            }
            return outp;
            {
                //Gene[] genes = ToGeneArray();
                //Gene[] spaces = new Gene[Start.Length + 1];

                //int end = 0;
                //for (int i = 0; i < genes.Length; i++)
                //{
                //    spaces[i] = new Gene(0, end, genes[i].Start);
                //    end = genes[i].Stop;
                //}
                //spaces[spaces.Length - 1] = new Gene(0, end, DNA.Length);

                //int lastValid = 0;
                //for (int i = 0; i < spaces.Length; i++)
                //{
                //    spaces[lastValid] = spaces[i];
                //    if (spaces[i].Start <= spaces[i].Stop)
                //    {
                //        lastValid++;
                //    }
                //}
                //Gene[] outp = new Gene[lastValid];
                //for (int i = 0; i < outp.Length; i++)
                //{
                //    outp[i] = spaces[i];
                //}
                //return outp;
            }
        }
        public override string ToString()
        {
            Gene[] genes = new Gene[Start.Length];
            for (int i = 0; i < Start.Length; i++) genes[i] = new Gene(Start[i].Index, Start[i].Position, -1);
            for (int i = 0; i < Stop.Length; i++)
            {
                for (int j = 0; j < genes.Length; j++)
                {
                    if (genes[j].Index == Stop[i].Index && genes[j].Start <= Stop[i].Position)
                    {
                        if (genes[j].Stop == -1) genes[j].Stop = Stop[i].Position;
                        else if (genes[j].Stop - genes[j].Start > Stop[i].Position - genes[j].Start) genes[j].Stop = Stop[i].Position;
                    }
                }
            }
            string outp = DNA + "\n";
            for (int i = 0; i < genes.Length; i++)
            {
                if (genes[i].Start <= genes[i].Stop) outp += $"<{genes[i].Index}>({genes[i].Start},{genes[i].Stop}){DNA.Substring(genes[i].Start, genes[i].Stop - genes[i].Start)}\n";
            }
            return outp;
        }
        void Corection(Mark[] mark, int compIndx, int indx, int stopIndx, int pos)
        {
            for (int i = 0; i < mark.Length; i++)
            {
                if (mark[i].Position == mark[compIndx].Position && i == compIndx) continue;
                if (mark[i].Position == mark[compIndx].Position && i != compIndx) mark[i].Position += pos - Start[indx].Position;
                else if (mark[i].Position > Start[indx].Position && mark[i].Position < Stop[stopIndx].Position) mark[i].Position += pos - Start[indx].Position;
                else if (pos < Start[indx].Position)
                {
                    if (mark[i].Position >= pos && mark[i].Position <= Start[indx].Position) { mark[i].Position += Stop[stopIndx].Position - Start[indx].Position; }
                }
                else if (pos > Start[indx].Position)
                {
                    if (mark[i].Position < pos + Stop[stopIndx].Position - Start[indx].Position && mark[i].Position >= Stop[stopIndx].Position) { mark[i].Position -= Stop[stopIndx].Position - Start[indx].Position; }
                }
            }
        }
        Mark[] Corection(Mark[] mark, Mark[] seqMark, int start, int stop, int len)
        {

            int lastIndx = 0;
            Mark[] temp = new Mark[mark.Length + seqMark.Length];
            for (int i = 0; i < mark.Length; i++)
            {
                if (mark[i].Position >= stop)
                {
                    temp[lastIndx] = mark[i];
                    temp[lastIndx++].Position += (len - (stop - start));
                }
                else if (mark[i].Position <= start)
                {
                    temp[lastIndx] = mark[i];
                    temp[lastIndx++].Position = mark[i].Position;
                }
            }
            for (int i = 0; i < seqMark.Length; i++)
            {
                temp[lastIndx] = seqMark[i];
                temp[lastIndx++].Position += start;
            }
            Mark[] outp = new Mark[lastIndx];
            for (int i = 0; i < outp.Length; i++)
            {
                outp[i] = temp[i];
            }
            return outp;
        }
        void Overwrite(Sequence seq, int start, int stop)
        {
            DNA = DNA.Substring(0, start) + seq.DNA + DNA.Substring(stop);
            Start = Corection(Start, seq.Start, start, stop, seq.DNA.Length);
            Stop = Corection(Stop, seq.Stop, start, stop, seq.DNA.Length);


        }
        Gene GetGene(int i)
        {
            return new Gene(Start[i].Index, Start[i].Position, Stop[FindStopIndx(i)].Position);
        }
        int FindStopIndx(int i)
        {
            int best = -1;
            int bestValue = int.MaxValue;
            for (int j = 0; j < Stop.Length; j++)
            {
                if (Stop[j].Index == Start[i].Index && Stop[j].Position < bestValue && Stop[j].Position >= Start[i].Position)
                {
                    bestValue = Stop[j].Position;
                    best = j;
                }
            }
            return best;
        }
        int FindStopPos(Mark[] mark, int i)
        {
            int indx = FindStopIndx(i);
            if (indx == -1) return -1;
            return mark[i].Position;
        }
    }
    internal struct Mark
    {
        public Mark(int Index, int Position)
        {
            this.Index = Index;
            this.Position = Position;
        }
        public int Index { get; set; }
        public int Position { get; set; }
    }

    public class Gene
    {
        public int Index { get; set; }
        public int Start { get; set; }
        public int Stop { get; set; }
        public int Length { get { return Stop - Start; } }
        public Gene(int Index, int Start, int Stop)
        {
            this.Index = Index;
            this.Start = Start;
            this.Stop = Stop;
        }
        public override string ToString()
        {
            return $"<{Index}>({Start},{Stop})";
        }
    }
    public static class GeneExtension
    {
        public static Gene[] RemoveInvalid(this Gene[] genes)
        {
            int invalidCounter = 0;
            for (int i = 0; i < genes.Length; i++)
            {
                if (genes[i].Start == -1 || genes[i].Stop == -1)
                {
                    genes[i] = genes[genes.Length - invalidCounter - 1];
                    invalidCounter++;
                }
            }
            if (invalidCounter == 0) return genes;

            Gene[] outp = new Gene[genes.Length - invalidCounter];
            for (int i = 0; i < outp.Length; i++) outp[i] = genes[i];
            return outp;
        }
        public static Gene[] OnlyHKG(this Gene[] genes, Genom orig)
        {
            int invalidCounter = 0;
            for (int i = 0; i < genes.Length; i++)
            {
                if (genes[i].Length != orig.Genes[genes[i].Index].Length)
                {
                    genes[i] = genes[genes.Length - invalidCounter - 1];
                    invalidCounter++;
                }
            }
            if (invalidCounter == 0) return genes;

            Gene[] outp = new Gene[genes.Length - invalidCounter];
            for (int i = 0; i < outp.Length; i++) outp[i] = genes[i];
            return outp;
        }
    }
    public class Genom 
    {
        public string DNA { get; set; }
        public Gene[] Genes { get; set; }
        public Genom(string DNA, Gene[] Genes)
        {
            this.DNA = DNA;
            this.Genes = Genes;
        }
        internal Sequence ToSequence() 
        {
            Sequence outp = new Sequence(DNA, new Mark[Genes.Length], new Mark[Genes.Length]);
            for (int i = 0; i < Genes.Length; i++)
            {
                outp.Start[i].Position = Genes[i].Start;
                outp.Start[i].Index = Genes[i].Index;
                outp.Stop[i].Position = Genes[i].Stop;
                outp.Stop[i].Index = Genes[i].Index;
            }
            return outp;
        }
    }
}
