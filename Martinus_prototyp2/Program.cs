using System.Text.Json;

namespace Martinus_prototyp2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Data data = Experiments.Experiment1();
            CSV viableCSV = new CSV("ViableHybrides.csv", data.Viables);
            CSV hybridesCSV = new CSV("Hybrides.csv", data.Hybrides);
            viableCSV.Save();
            hybridesCSV.Save();

            Console.WriteLine(data);


            ////string fileName = "C:\\Users\\42073\\source\\repos\\Martinus_prototyp2\\Martinus_prototyp2\\jsconfig1.json";

            //string fileName = "C:\\Users\\42073\\source\\repos\\Martinus_prototyp2\\Martinus_prototyp2\\Genom.json";
            //string Jstring = File.ReadAllText(fileName);
            //Genom original = JsonSerializer.Deserialize<Genom>(Jstring)!;//RNG.GenerateGenom(100, 0.7f, 5, 6);//JsonSerializer.Deserialize<Genom>(Jstring)!;
            //Sequence sequence = original.ToSequence();
            //foreach (var item in sequence.ToGeneArray()) { Console.WriteLine(item); }
            //Console.WriteLine(sequence);
            //foreach (var item in sequence.ToNonGeneArray(original)) { Console.WriteLine(item); }
            //Console.WriteLine(RNG.ChooseWhereToPlaceGene(sequence, original));
            //sequence.MoveGene(0, 60);
            //sequence.MoveGene(5, 0);
            //sequence.MoveGene(2, 68);
            //Console.WriteLine(sequence);
            //for (int i = 0; i < 1000; i++)
            //{
            //    //Console.WriteLine(RNG.ChooseWhereToPlaceGene(sequence));
            //    Gene[] nongene = sequence.ToNonGeneArray(original);
            //    int where = RNG.ChooseWhereToPlaceGene(sequence,original);
            //    int what = RNG.Int(sequence.Start.Length);
            //    foreach (Gene gene in nongene) { Console.WriteLine(gene); }
            //    Console.WriteLine($"what:{what},where:{where}");
            //    sequence.MoveGene(what, where);
            //    Console.WriteLine(sequence);
            //    ////foreach (Gene gene in sequence.ToGeneArray()) Console.WriteLine(gene);
            //    Console.WriteLine(sequence.IsViable(original));
            //}

            ////Sequence sequence = JsonSerializer.Deserialize<Sequence>(Jstring)!;
            ////Console.WriteLine(sequence.ToString());
            ////Interface.WriteSequence(sequence);
            ////Console.WriteLine(sequence);
            ////sequence.MoveGene(0, 11);
            ////Console.WriteLine(sequence);
        }
    }
}