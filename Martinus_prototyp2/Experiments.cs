using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Martinus_prototyp2
{//test
    public static class Experiments
    {
        public static Data Experiment1()
        {
            //------SETTINGS------
            const int GENOM_LENGTH = 10_000;
            const int GENE_NUMBER = 200;
            const int GENE_MINIMUM_SIZE = 22;
            const float GENE_DENSITY = 0.7f;

            const int POPULATION_SIZE = 100;

            const int MAX_GENERATION_NUMBER = 100;
            const int MAX_SHIFT_GENES_NUMBER = 70;
            const int MAX_REPETITIONS_NUMBER = 100;

            const float ACTIVE_FRACTION_OF_SEQUENCES = 0.1f;

            const int MIN_TRANSFER_SIZE = 100;
            const int MAX_TRANSFER_SIZE = 1000;

            //-------SETUP--------
            Data data = new Data("Experiment1", MAX_SHIFT_GENES_NUMBER, MAX_GENERATION_NUMBER);
            Interface UI = new Interface();
            UI.NewProgressbar(new ProgressBar("Repetitions:", MAX_REPETITIONS_NUMBER, 1, 20));
            UI.NewProgressbar(new ProgressBar("Translocations:", MAX_SHIFT_GENES_NUMBER, 1));
            UI.NewProgressbar(new ProgressBar("Generations:", MAX_GENERATION_NUMBER, 1));
            //-------LOOP---------
            for (int repetition = 0; repetition < MAX_REPETITIONS_NUMBER; repetition++)
            {
                UI.UpdateBar(0, repetition);
                Genom originalGenom = RNG.GenerateGenom(GENOM_LENGTH, GENE_DENSITY, GENE_NUMBER, GENE_MINIMUM_SIZE);
                Sequence originalSeq = originalGenom.ToSequence();
                Sequence MovedSeq = originalSeq.Clone();
                for (int shift = 0; shift < MAX_SHIFT_GENES_NUMBER; shift++)
                {
                    UI.UpdateBar(1, shift);
                    //Console.WriteLine(MovedSeq);

                    Sequence[] population1 = new Sequence[POPULATION_SIZE];
                    Sequence[] population2 = new Sequence[POPULATION_SIZE];

                    for (int i = 0; i < population1.Length; i++)
                    {
                        population1[i] = originalSeq.Clone();
                        population2[i] = MovedSeq.Clone();
                    }

                    for (int generation = 0; generation < MAX_GENERATION_NUMBER; generation++)
                    {
                        UI.UpdateBar(2, generation);
                        UI.Write();
                        for (int i = 0; i < population1.Length * ACTIVE_FRACTION_OF_SEQUENCES; i++)
                        {
                            int acceptor = RNG.Int(population1.Length);
                            int donor = RNG.Int(population2.Length);
                            int seqPos = RNG.Int(population2[donor].DNA.Length - MIN_TRANSFER_SIZE);
                            int seqLen = RNG.Int(MIN_TRANSFER_SIZE, MAX_TRANSFER_SIZE);
                            if (seqLen > population2[donor].DNA.Length - seqPos) seqPos = population2[donor].DNA.Length - seqPos;
                            bool isSucces = population1[acceptor].Integrate(population2[donor].Subsequence(seqPos, seqLen));
                            bool isViable = population1[acceptor].IsViable(originalGenom);
                            if (!isViable) population1[acceptor] = population1[RNG.Exclude(acceptor, population1.Length)].Clone();
                            if (isSucces) data.Hybrides[shift, generation]++;
                            if (isViable && isSucces) data.Viables[shift, generation]++;
                        }
                    }
                    MovedSeq.MoveGene(RNG.Int(MovedSeq.Start.Length), RNG.ChooseWhereToPlaceGene(MovedSeq, originalGenom));
                    //foreach (Sequence seq in population2) Console.WriteLine(seq.IsViable(originalGenom));
                    //foreach (Sequence seq in population2) Interface.WriteSequence(seq);
                }
            }
            UI.Finished = true;
            UI.Write();
            return data;
        }
        public static Data ExperimentMultiCore()
        {
            //------SETTINGS------
            const int GENOM_LENGTH = 10_000;
            const int GENE_NUMBER = 200;
            const int GENE_MINIMUM_SIZE = 22;
            const float GENE_DENSITY = 0.7f;

            const int POPULATION_SIZE = 100;

            const int MAX_GENERATION_NUMBER = 100;
            const int MAX_SHIFT_GENES_NUMBER = 70;
            const int MAX_REPETITIONS_NUMBER = 100;

            const float ACTIVE_FRACTION_OF_SEQUENCES = 0.1f;

            const int MIN_TRANSFER_SIZE = 300;
            const int MAX_TRANSFER_SIZE = 1_000;

            //-------SETUP--------
            Data data = new Data("Experiment1", MAX_SHIFT_GENES_NUMBER, MAX_GENERATION_NUMBER);
            Interface UI = new Interface();
            UI.NewProgressbar(new ProgressBar("Finished:", MAX_REPETITIONS_NUMBER * MAX_SHIFT_GENES_NUMBER, 1, 20)); ;
            //-------LOOP---------
            //for (int repetition = 0; repetition < MAX_REPETITIONS_NUMBER; repetition++)
            //{
            int translocation = 0;
            UI.AsyncRun();
            Parallel.For(0, MAX_REPETITIONS_NUMBER, repetition =>
            {
                Genom originalGenom = RNG.GenerateGenom(GENOM_LENGTH, GENE_DENSITY, GENE_NUMBER, GENE_MINIMUM_SIZE);
                Sequence originalSeq = originalGenom.ToSequence();
                Sequence MovedSeq = originalSeq.Clone();
                for (int shift = 0; shift < MAX_SHIFT_GENES_NUMBER; shift++)
                {
                    UI.UpdateBar(0, translocation++);
                    //Console.WriteLine(MovedSeq);

                    Sequence[] population1 = new Sequence[POPULATION_SIZE];
                    Sequence[] population2 = new Sequence[POPULATION_SIZE];

                    for (int i = 0; i < population1.Length; i++)
                    {
                        population1[i] = originalSeq.Clone();
                        population2[i] = MovedSeq.Clone();
                    }
                    for (int generation = 0; generation < MAX_GENERATION_NUMBER; generation++)
                    {
                        for (int i = 0; i < population1.Length * ACTIVE_FRACTION_OF_SEQUENCES; i++)
                        {
                            int acceptor = RNG.Int(population1.Length);
                            int donor = RNG.Int(population2.Length);
                            int seqPos = RNG.Int(population2[donor].DNA.Length - MIN_TRANSFER_SIZE);
                            int seqLen = RNG.Int(MIN_TRANSFER_SIZE, MAX_TRANSFER_SIZE);
                            if (seqLen > population2[donor].DNA.Length - seqPos) seqLen = population2[donor].DNA.Length - seqPos;
                            bool isSucces = population1[acceptor].Integrate(population2[donor].Subsequence(seqPos, seqLen));
                            bool isViable = population1[acceptor].IsViable(originalGenom);
                            if (!isViable) population1[acceptor] = population1[RNG.Exclude(acceptor, population1.Length)].Clone();
                            if (isSucces) data.Hybrides[shift, generation]++;
                            if (isViable && isSucces) data.Viables[shift, generation]++;
                        }
                    }
                    MovedSeq.MoveGene(RNG.Int(MovedSeq.Start.Length), RNG.ChooseWhereToPlaceGene(MovedSeq, originalGenom));
                    //foreach (Sequence seq in population2) Console.WriteLine(seq.IsViable(originalGenom));
                    //foreach (Sequence seq in population2) Interface.WriteSequence(seq);
                }
                //UI.UpdateBar(0, finishedCounter++);
            });
            UI.Finished = true;
            //UI.Finished = true;
            //UI.Write();
            return data;
        }
        public static Data[] VlivMinTransferSize()
        {
            const int MIN_MIN_TRANSFER_SIZE = 22;
            const int MAX_MIN_TRANSFER_SIZE = 50_022;
            const int STEP_MIN_TRANSFER_SIZE = 10_000;
            //------SETTINGS------
            const int GENOM_LENGTH = 100_000;
            const int GENE_NUMBER = 200;
            const int GENE_MINIMUM_SIZE = 22;
            const float GENE_DENSITY = 0.7f;

            const int POPULATION_SIZE = 100;

            const int MAX_GENERATION_NUMBER = 100;
            const int MAX_SHIFT_GENES_NUMBER = 70;
            const int MAX_REPETITIONS_NUMBER = 100;

            const float ACTIVE_FRACTION_OF_SEQUENCES = 0.1f;

            const int MAX_TRANSFER_SIZE = 50_022;

            int dataIndex = 0;
            Data[] data = new Data[(MAX_MIN_TRANSFER_SIZE- MIN_MIN_TRANSFER_SIZE)/ STEP_MIN_TRANSFER_SIZE];
            for (int MIN_TRANSFER_SIZE = MIN_MIN_TRANSFER_SIZE; MIN_TRANSFER_SIZE < MAX_MIN_TRANSFER_SIZE; MIN_TRANSFER_SIZE+= STEP_MIN_TRANSFER_SIZE)
            {
                //-------SETUP--------
                data[dataIndex] = new Data("Experiment1", MAX_SHIFT_GENES_NUMBER, MAX_GENERATION_NUMBER);
                Interface UI = new Interface();
                UI.NewProgressbar(new ProgressBar("Finished:", MAX_REPETITIONS_NUMBER * MAX_SHIFT_GENES_NUMBER, 1, 20)); ;
                //-------LOOP---------
                //for (int repetition = 0; repetition < MAX_REPETITIONS_NUMBER; repetition++)
                //{
                int translocation = 0;
                UI.AsyncRun();
                Parallel.For(0, MAX_REPETITIONS_NUMBER, repetition =>
                {
                    Genom originalGenom = RNG.GenerateGenom(GENOM_LENGTH, GENE_DENSITY, GENE_NUMBER, GENE_MINIMUM_SIZE);
                    Sequence originalSeq = originalGenom.ToSequence();
                    Sequence MovedSeq = originalSeq.Clone();
                    for (int shift = 0; shift < MAX_SHIFT_GENES_NUMBER; shift++)
                    {
                        UI.UpdateBar(0, translocation++);
                        //Console.WriteLine(MovedSeq);

                        Sequence[] population1 = new Sequence[POPULATION_SIZE];
                        Sequence[] population2 = new Sequence[POPULATION_SIZE];

                        for (int i = 0; i < population1.Length; i++)
                        {
                            population1[i] = originalSeq.Clone();
                            population2[i] = MovedSeq.Clone();
                        }
                        for (int generation = 0; generation < MAX_GENERATION_NUMBER; generation++)
                        {
                            for (int i = 0; i < population1.Length * ACTIVE_FRACTION_OF_SEQUENCES; i++)
                            {
                                int acceptor = RNG.Int(population1.Length);
                                int donor = RNG.Int(population2.Length);
                                int seqPos = RNG.Int(population2[donor].DNA.Length - MIN_TRANSFER_SIZE);
                                int seqLen = RNG.Int(MIN_TRANSFER_SIZE, MAX_TRANSFER_SIZE);
                                if (seqLen > population2[donor].DNA.Length - seqPos) seqLen = population2[donor].DNA.Length - seqPos;
                                bool isSucces = population1[acceptor].Integrate(population2[donor].Subsequence(seqPos, seqLen));
                                bool isViable = population1[acceptor].IsViable(originalGenom);
                                if (!isViable) population1[acceptor] = population1[RNG.Exclude(acceptor, population1.Length)].Clone();
                                if (isSucces) data[dataIndex].Hybrides[shift, generation]++;
                                if (isViable && isSucces) data[dataIndex].Viables[shift, generation]++;
                            }
                        }
                        MovedSeq.MoveGene(RNG.Int(MovedSeq.Start.Length), RNG.ChooseWhereToPlaceGene(MovedSeq, originalGenom));
                        //foreach (Sequence seq in population2) Console.WriteLine(seq.IsViable(originalGenom));
                        //foreach (Sequence seq in population2) Interface.WriteSequence(seq);
                    }
                    //UI.UpdateBar(0, finishedCounter++);
                });
                UI.Finished = true;
                dataIndex++;
            }
            //UI.Finished = true;
            //UI.Write();
            return data;
        }
        public static Data LongRun()//tohle dodělat a dneska večer spustit
        {
            //------SETTINGS------
            const int GENOM_LENGTH = 10_000;
            const int GENE_NUMBER = 200;
            const int GENE_MINIMUM_SIZE = 22;
            const float GENE_DENSITY = 0.7f;

            const int POPULATION_SIZE = 100;

            const int MAX_GENERATION_NUMBER = 10_000;
            const int WRITE_DATA_PERIOD = 100;
            const int SHIFT_GENES_NUMBER = 70;
            const int MAX_REPETITIONS_NUMBER = 100;

            const float ACTIVE_FRACTION_OF_SEQUENCES = 0.1f;

            const int MIN_TRANSFER_SIZE = 500;
            const int MAX_TRANSFER_SIZE = 1_000;

            //-------SETUP--------
            Data data = new Data("Experiment1", 1, MAX_GENERATION_NUMBER/WRITE_DATA_PERIOD);
            Interface UI = new Interface();
            UI.NewProgressbar(new ProgressBar("Finished:", MAX_REPETITIONS_NUMBER * (MAX_GENERATION_NUMBER/WRITE_DATA_PERIOD), 1, 20)); ;
            //-------LOOP---------
            //for (int repetition = 0; repetition < MAX_REPETITIONS_NUMBER; repetition++)
            //{
            int counter = 0;
            UI.AsyncRun();
            Parallel.For(0, MAX_REPETITIONS_NUMBER, repetition =>
            {
                Genom originalGenom = RNG.GenerateGenom(GENOM_LENGTH, GENE_DENSITY, GENE_NUMBER, GENE_MINIMUM_SIZE);
                Sequence originalSeq = originalGenom.ToSequence();
                Sequence MovedSeq = originalSeq.Clone();
                
                //Console.WriteLine(MovedSeq);

                Sequence[] population1 = new Sequence[POPULATION_SIZE];
                Sequence[] population2 = new Sequence[POPULATION_SIZE];
                for (int i  = 0; i < SHIFT_GENES_NUMBER; i++) 
                    MovedSeq.MoveGene(RNG.Int(MovedSeq.Start.Length), RNG.ChooseWhereToPlaceGene(MovedSeq, originalGenom));
                for (int i = 0; i < population1.Length; i++)
                {
                    population1[i] = originalSeq.Clone();
                    population2[i] = MovedSeq.Clone();
                }
                int dataIndex = -1;
                for (int generation = 0; generation < MAX_GENERATION_NUMBER; generation++)
                {
                    if (generation % WRITE_DATA_PERIOD == 0)
                    {
                        UI.UpdateBar(0, counter);
                        dataIndex++;
                        counter++;
                    }
                    for (int i = 0; i < population1.Length * ACTIVE_FRACTION_OF_SEQUENCES; i++)
                    {
                        int acceptor = RNG.Int(population1.Length);
                        int donor = RNG.Int(population2.Length);
                        int seqPos = RNG.Int(population2[donor].DNA.Length - MIN_TRANSFER_SIZE);
                        int seqLen = RNG.Int(MIN_TRANSFER_SIZE, MAX_TRANSFER_SIZE);
                        if (seqLen > population2[donor].DNA.Length - seqPos) seqLen = population2[donor].DNA.Length - seqPos;
                        bool isSucces = population1[acceptor].Integrate(population2[donor].Subsequence(seqPos, seqLen));
                        bool isViable = population1[acceptor].IsViable(originalGenom);
                        if (!isViable) population1[acceptor] = population1[RNG.Exclude(acceptor, population1.Length)].Clone();
                        if (isSucces) data.Hybrides[0, dataIndex]++;
                        if (isViable && isSucces) data.Viables[0, dataIndex]++;
                    }
                    //Console.WriteLine("\n\n\n\n\n\ngfkutfdktuf");
                }
                //MovedSeq.MoveGene(RNG.Int(MovedSeq.Start.Length), RNG.ChooseWhereToPlaceGene(MovedSeq, originalGenom));
                //foreach (Sequence seq in population2) Console.WriteLine(seq.IsViable(originalGenom));
                //foreach (Sequence seq in population2) Interface.WriteSequence(seq);
            //UI.UpdateBar(0, finishedCounter++);
            });
            UI.Finished = true;
            //UI.Finished = true;
            //UI.Write();
            return data;
        }
        public static Data LongRunWithCominucationInPopulation()//tohle dodělat a dneska večer spustit
        {
            //------SETTINGS------
            const int GENOM_LENGTH = 10_000;
            const int GENE_NUMBER = 200;
            const int GENE_MINIMUM_SIZE = 22;
            const float GENE_DENSITY = 0.7f;

            const int POPULATION_SIZE = 100;

            const int MAX_GENERATION_NUMBER = 10_000;
            const int WRITE_DATA_PERIOD = 100;
            const int SHIFT_GENES_NUMBER = 70;
            const int MAX_REPETITIONS_NUMBER = 100;

            const float ACTIVE_FRACTION_OF_SEQUENCES = 0.1f;
            const float ACTIVE_FRACTION_OF_SEQUENCES_IN_POPULATION = 0.1f;

            const int MIN_TRANSFER_SIZE = 500;
            const int MAX_TRANSFER_SIZE = 1_000;

            //-------SETUP--------
            Data data = new Data("Experiment1", 1, MAX_GENERATION_NUMBER / WRITE_DATA_PERIOD);
            Interface UI = new Interface();
            UI.NewProgressbar(new ProgressBar("Finished:", MAX_REPETITIONS_NUMBER * (MAX_GENERATION_NUMBER / WRITE_DATA_PERIOD), 1, 20)); ;
            //-------LOOP---------
            //for (int repetition = 0; repetition < MAX_REPETITIONS_NUMBER; repetition++)
            //{
            int counter = 0;
            UI.AsyncRun();
            Parallel.For(0, MAX_REPETITIONS_NUMBER, repetition =>
            {
                Genom originalGenom = RNG.GenerateGenom(GENOM_LENGTH, GENE_DENSITY, GENE_NUMBER, GENE_MINIMUM_SIZE);
                Sequence originalSeq = originalGenom.ToSequence();
                Sequence MovedSeq = originalSeq.Clone();

                //Console.WriteLine(MovedSeq);

                Sequence[] population1 = new Sequence[POPULATION_SIZE];
                Sequence[] population2 = new Sequence[POPULATION_SIZE];
                for (int i = 0; i < SHIFT_GENES_NUMBER; i++)
                    MovedSeq.MoveGene(RNG.Int(MovedSeq.Start.Length), RNG.ChooseWhereToPlaceGene(MovedSeq, originalGenom));
                for (int i = 0; i < population1.Length; i++)
                {
                    population1[i] = originalSeq.Clone();
                    population2[i] = MovedSeq.Clone();
                }
                int dataIndex = -1;
                for (int generation = 0; generation < MAX_GENERATION_NUMBER; generation++)
                {
                    if (generation % WRITE_DATA_PERIOD == 0)
                    {
                        UI.UpdateBar(0, counter);
                        dataIndex++;
                        counter++;
                    }
                    for (int i = 0; i < population1.Length * ACTIVE_FRACTION_OF_SEQUENCES; i++)
                    {
                        int acceptor = RNG.Int(population1.Length);
                        int donor = RNG.Int(population2.Length);
                        int seqPos = RNG.Int(population2[donor].DNA.Length - MIN_TRANSFER_SIZE);
                        int seqLen = RNG.Int(MIN_TRANSFER_SIZE, MAX_TRANSFER_SIZE);
                        if (seqLen > population2[donor].DNA.Length - seqPos) seqLen = population2[donor].DNA.Length - seqPos;
                        bool isSucces = population1[acceptor].Integrate(population2[donor].Subsequence(seqPos, seqLen));
                        bool isViable = population1[acceptor].IsViable(originalGenom);
                        if (!isViable) population1[acceptor] = population1[RNG.Exclude(acceptor, population1.Length)].Clone();
                        if (isSucces) data.Hybrides[0, dataIndex]++;
                        if (isViable && isSucces) data.Viables[0, dataIndex]++;
                    }
                    for (int i = 0; i < population1.Length * ACTIVE_FRACTION_OF_SEQUENCES_IN_POPULATION; i++)
                    {
                        int acceptor = RNG.Int(population1.Length);
                        int donor = RNG.Int(population1.Length);
                        int seqPos = RNG.Int(population1[donor].DNA.Length - MIN_TRANSFER_SIZE);
                        int seqLen = RNG.Int(MIN_TRANSFER_SIZE, MAX_TRANSFER_SIZE);
                        if (seqLen > population1[donor].DNA.Length - seqPos) seqLen = population1[donor].DNA.Length - seqPos;
                        bool isSucces = population1[acceptor].Integrate(population1[donor].Subsequence(seqPos, seqLen));
                        bool isViable = population1[acceptor].IsViable(originalGenom);
                        if (!isViable) population1[acceptor] = population1[RNG.Exclude(acceptor, population1.Length)].Clone();
                    }
                    //Console.WriteLine("\n\n\n\n\n\ngfkutfdktuf");
                }
                //MovedSeq.MoveGene(RNG.Int(MovedSeq.Start.Length), RNG.ChooseWhereToPlaceGene(MovedSeq, originalGenom));
                //foreach (Sequence seq in population2) Console.WriteLine(seq.IsViable(originalGenom));
                //foreach (Sequence seq in population2) Interface.WriteSequence(seq);
                //UI.UpdateBar(0, finishedCounter++);
            });
            UI.Finished = true;
            //UI.Finished = true;
            //UI.Write();
            return data;
        }
        public static Data[] NightLongRunWithCominucationInPopulation1(int[] minTransfersizes)//tohle dodělat a dneska večer spustit
        {
            //------SETTINGS------
            const int GENOM_LENGTH = 10_000;
            const int GENE_NUMBER = 200;
            const int GENE_MINIMUM_SIZE = 22;
            const float GENE_DENSITY = 0.7f;

            const int POPULATION_SIZE = 100;

            const int MAX_GENERATION_NUMBER = 10_000;
            const int WRITE_DATA_PERIOD = 100;
            const int SHIFT_GENES_NUMBER = 70;
            const int MAX_REPETITIONS_NUMBER = 100;

            const float ACTIVE_FRACTION_OF_SEQUENCES = 0.1f;
            const float ACTIVE_FRACTION_OF_SEQUENCES_IN_POPULATION = 0.1f;

            //const int MIN_TRANSFER_SIZE = 500;
            const int MAX_TRANSFER_SIZE = 1000;
            Data[] data = new Data[minTransfersizes.Length];
            for (int indexOfMT = 0; indexOfMT < minTransfersizes.Length; indexOfMT++) 
            {
                int MIN_TRANSFER_SIZE = minTransfersizes[indexOfMT];
                data[indexOfMT] = new Data("Experiment1", MAX_REPETITIONS_NUMBER, MAX_GENERATION_NUMBER / WRITE_DATA_PERIOD, MAX_REPETITIONS_NUMBER, 12);
                Interface UI = new Interface();
                UI.NewProgressbar(new ProgressBar("Finished:", MAX_REPETITIONS_NUMBER * (MAX_GENERATION_NUMBER / WRITE_DATA_PERIOD), 1, 20)); ;
                //-------LOOP---------
                //for (int repetition = 0; repetition < MAX_REPETITIONS_NUMBER; repetition++)
                //{
                int counter = 0;
                UI.AsyncRun();
                Parallel.For(0, MAX_REPETITIONS_NUMBER, repetition =>
                {
                    Genom originalGenom = RNG.GenerateGenom(GENOM_LENGTH, GENE_DENSITY, GENE_NUMBER, GENE_MINIMUM_SIZE);
                    Sequence originalSeq = originalGenom.ToSequence();
                    Sequence MovedSeq = originalSeq.Clone();

                    //Console.WriteLine(MovedSeq);

                    Sequence[] population1 = new Sequence[POPULATION_SIZE];
                    Sequence[] population2 = new Sequence[POPULATION_SIZE];
                    for (int i = 0; i < SHIFT_GENES_NUMBER; i++)
                        MovedSeq.MoveGene(RNG.Int(MovedSeq.Start.Length), RNG.ChooseWhereToPlaceGene(MovedSeq, originalGenom));
                    data[indexOfMT].Genoms[repetition, 0] = originalGenom;
                    data[indexOfMT].Genoms[repetition, 1] = MovedSeq.ToGenom();
                    for (int i = 0; i < population1.Length; i++)
                    {
                        population1[i] = originalSeq.Clone();
                        population2[i] = MovedSeq.Clone();
                    }
                    int dataIndex = -1;
                    for (int generation = 0; generation < MAX_GENERATION_NUMBER; generation++)
                    {
                        if (generation % WRITE_DATA_PERIOD == 0)
                        {
                            UI.UpdateBar(0, counter);
                            dataIndex++;
                            counter++;
                        }
                        for (int i = 0; i < population1.Length * ACTIVE_FRACTION_OF_SEQUENCES; i++)
                        {
                            int acceptor = RNG.Int(population1.Length);
                            int donor = RNG.Int(population2.Length);
                            int seqPos = RNG.Int(population2[donor].DNA.Length - MIN_TRANSFER_SIZE);
                            int seqLen = RNG.Int(MIN_TRANSFER_SIZE, MAX_TRANSFER_SIZE);
                            if (seqLen > population2[donor].DNA.Length - seqPos) seqLen = population2[donor].DNA.Length - seqPos;
                            bool isSucces = population1[acceptor].Integrate(population2[donor].Subsequence(seqPos, seqLen));
                            bool isViable = population1[acceptor].IsViable(originalGenom);
                            if (!isViable) population1[acceptor] = population1[RNG.Exclude(acceptor, population1.Length)].Clone();
                            if (isSucces) data[indexOfMT].Hybrides[repetition, dataIndex]++;
                            if (isViable && isSucces) data[indexOfMT].Viables[repetition, dataIndex]++;
                        }
                        for (int i = 0; i < population1.Length * ACTIVE_FRACTION_OF_SEQUENCES_IN_POPULATION; i++)
                        {
                            int acceptor = RNG.Int(population1.Length);
                            int donor = RNG.Int(population1.Length);
                            int seqPos = RNG.Int(population1[donor].DNA.Length - MIN_TRANSFER_SIZE);
                            int seqLen = RNG.Int(MIN_TRANSFER_SIZE, MAX_TRANSFER_SIZE);
                            if (seqLen > population1[donor].DNA.Length - seqPos) seqLen = population1[donor].DNA.Length - seqPos;
                            bool isSucces = population1[acceptor].Integrate(population1[donor].Subsequence(seqPos, seqLen));
                            bool isViable = population1[acceptor].IsViable(originalGenom);
                            if (!isViable) population1[acceptor] = population1[RNG.Exclude(acceptor, population1.Length)].Clone();
                        }
                        for (int i = 2; i < 12; i++)
                        {
                            data[indexOfMT].Genoms[repetition,i] = population1[RNG.Int(population1.Length)].ToGenom();
                            
                        }
                        //Console.WriteLine("\n\n\n\n\n\ngfkutfdktuf");
                    }
                    //MovedSeq.MoveGene(RNG.Int(MovedSeq.Start.Length), RNG.ChooseWhereToPlaceGene(MovedSeq, originalGenom));
                    //foreach (Sequence seq in population2) Console.WriteLine(seq.IsViable(originalGenom));
                    //foreach (Sequence seq in population2) Interface.WriteSequence(seq);
                    //UI.UpdateBar(0, finishedCounter++);
                });
                UI.Finished = true;
            }
            //-------SETUP--------
            //UI.Finished = true;
            //UI.Write();
            return data;
        }
        public static Data Experimanet2(int minTransfersize)//tohle dodělat a dneska večer spustit
        {
            //------SETTINGS------
            const int GENOM_LENGTH = 10_000;
            const int GENE_NUMBER = 200;
            const int GENE_MINIMUM_SIZE = 22;
            const float GENE_DENSITY = 0.7f;

            const int POPULATION_SIZE = 100;

            const int MAX_GENERATION_NUMBER = 10_000;
            const int WRITE_DATA_PERIOD = 100;
            const int SHIFT_GENES_NUMBER = 70;
            const int MAX_REPETITIONS_NUMBER = 100;

            const float ACTIVE_FRACTION_OF_SEQUENCES = 0.01f;
            const float ACTIVE_FRACTION_OF_SEQUENCES_IN_POPULATION = 0.1f;

            //const int MIN_TRANSFER_SIZE = 500;
            const int MAX_TRANSFER_SIZE = 1000;
                int MIN_TRANSFER_SIZE = minTransfersize;
                Data data = new Data("Experiment1", MAX_REPETITIONS_NUMBER, MAX_GENERATION_NUMBER / WRITE_DATA_PERIOD, MAX_REPETITIONS_NUMBER, 12);
                Interface UI = new Interface();
                UI.NewProgressbar(new ProgressBar("Finished:", MAX_REPETITIONS_NUMBER * (MAX_GENERATION_NUMBER / WRITE_DATA_PERIOD), 1, 20)); ;
                //-------LOOP---------
                //for (int repetition = 0; repetition < MAX_REPETITIONS_NUMBER; repetition++)
                //{
                int counter = 0;
                UI.AsyncRun();
                Parallel.For(0, MAX_REPETITIONS_NUMBER, repetition =>
                {
                    Genom originalGenom = RNG.GenerateGenom(GENOM_LENGTH, GENE_DENSITY, GENE_NUMBER, GENE_MINIMUM_SIZE);
                    Sequence originalSeq = originalGenom.ToSequence();
                    Sequence MovedSeq = originalSeq.Clone();

                    //Console.WriteLine(MovedSeq);

                    Sequence[] population1 = new Sequence[POPULATION_SIZE];
                    Sequence[] population2 = new Sequence[POPULATION_SIZE];
                    for (int i = 0; i < SHIFT_GENES_NUMBER; i++)
                        MovedSeq.MoveGene(RNG.Int(MovedSeq.Start.Length), RNG.ChooseWhereToPlaceGene(MovedSeq, originalGenom));
                    data.Genoms[repetition, 0] = originalGenom;
                    data.Genoms[repetition, 1] = MovedSeq.ToGenom();
                    for (int i = 0; i < population1.Length; i++)
                    {
                        population1[i] = originalSeq.Clone();
                        population2[i] = MovedSeq.Clone();
                    }
                    int dataIndex = -1;
                    for (int generation = 0; generation < MAX_GENERATION_NUMBER; generation++)
                    {
                        if (generation % WRITE_DATA_PERIOD == 0)
                        {
                            UI.UpdateBar(0, counter);
                            dataIndex++;
                            counter++;
                        }
                        for (int i = 0; i < population1.Length * ACTIVE_FRACTION_OF_SEQUENCES; i++)
                        {
                            int acceptor = RNG.Int(population1.Length);
                            int donor = RNG.Int(population2.Length);
                            int seqPos = RNG.Int(population2[donor].DNA.Length - MIN_TRANSFER_SIZE);
                            int seqLen = RNG.Int(MIN_TRANSFER_SIZE, MAX_TRANSFER_SIZE);
                            if (seqLen > population2[donor].DNA.Length - seqPos) seqLen = population2[donor].DNA.Length - seqPos;
                            bool isSucces = population1[acceptor].Integrate(population2[donor].Subsequence(seqPos, seqLen));
                            bool isViable = population1[acceptor].IsViable(originalGenom);
                            if (!isViable) population1[acceptor] = population1[RNG.Exclude(acceptor, population1.Length)].Clone();
                            if (isSucces) data.Hybrides[repetition, dataIndex]++;
                            if (isViable && isSucces) data.Viables[repetition, dataIndex]++;
                        }
                        for (int i = 0; i < population1.Length * ACTIVE_FRACTION_OF_SEQUENCES_IN_POPULATION; i++)
                        {
                            int acceptor = RNG.Int(population1.Length);
                            int donor = RNG.Int(population1.Length);
                            int seqPos = RNG.Int(population1[donor].DNA.Length - MIN_TRANSFER_SIZE);
                            int seqLen = RNG.Int(MIN_TRANSFER_SIZE, MAX_TRANSFER_SIZE);
                            if (seqLen > population1[donor].DNA.Length - seqPos) seqLen = population1[donor].DNA.Length - seqPos;
                            bool isSucces = population1[acceptor].Integrate(population1[donor].Subsequence(seqPos, seqLen));
                            bool isViable = population1[acceptor].IsViable(originalGenom);
                            if (!isViable) population1[acceptor] = population1[RNG.Exclude(acceptor, population1.Length)].Clone();
                        }
                        for (int i = 2; i < 12; i++)
                        {
                            data.Genoms[repetition, i] = population1[RNG.Int(population1.Length)].ToGenom();

                        }
                        //Console.WriteLine("\n\n\n\n\n\ngfkutfdktuf");
                    }
                    //MovedSeq.MoveGene(RNG.Int(MovedSeq.Start.Length), RNG.ChooseWhereToPlaceGene(MovedSeq, originalGenom));
                    //foreach (Sequence seq in population2) Console.WriteLine(seq.IsViable(originalGenom));
                    //foreach (Sequence seq in population2) Interface.WriteSequence(seq);
                    //UI.UpdateBar(0, finishedCounter++);
                });
                UI.Finished = true;
            //-------SETUP--------
            //UI.Finished = true;
            //UI.Write();
            return data;
        }
        public static Data Experimanet3(int minTransfersize)//tohle dodělat a dneska večer spustit
        {
            //------SETTINGS------
            const int GENOM_LENGTH = 10_000;
            const int GENE_NUMBER = 200;
            const int GENE_MINIMUM_SIZE = 22;
            const float GENE_DENSITY = 0.7f;

            const int POPULATION_SIZE = 100;

            const int MAX_GENERATION_NUMBER = 10_000;
            const int WRITE_DATA_PERIOD = 100;
            const int SHIFT_GENES_NUMBER = 20;
            const int MAX_REPETITIONS_NUMBER = 100;

            const float ACTIVE_FRACTION_OF_SEQUENCES = 0.01f;
            const float ACTIVE_FRACTION_OF_SEQUENCES_IN_POPULATION = 0.1f;

            //const int MIN_TRANSFER_SIZE = 500;
            const int MAX_TRANSFER_SIZE = 1000;
            int MIN_TRANSFER_SIZE = minTransfersize;
            Data data = new Data("Experiment1", MAX_REPETITIONS_NUMBER, MAX_GENERATION_NUMBER / WRITE_DATA_PERIOD, MAX_REPETITIONS_NUMBER, 12);
            Interface UI = new Interface();
            UI.NewProgressbar(new ProgressBar("Finished:", MAX_REPETITIONS_NUMBER * (MAX_GENERATION_NUMBER / WRITE_DATA_PERIOD), 1, 20)); ;
            //-------LOOP---------
            //for (int repetition = 0; repetition < MAX_REPETITIONS_NUMBER; repetition++)
            //{
            int counter = 0;
            UI.AsyncRun();
            Parallel.For(0, MAX_REPETITIONS_NUMBER, repetition =>
            {
                Genom originalGenom = RNG.GenerateGenom(GENOM_LENGTH, GENE_DENSITY, GENE_NUMBER, GENE_MINIMUM_SIZE);
                Sequence originalSeq = originalGenom.ToSequence();
                Sequence MovedSeq = originalSeq.Clone();

                //Console.WriteLine(MovedSeq);

                Sequence[] population1 = new Sequence[POPULATION_SIZE];
                Sequence[] population2 = new Sequence[POPULATION_SIZE];
                for (int i = 0; i < SHIFT_GENES_NUMBER; i++)
                    MovedSeq.MoveGene(RNG.Int(MovedSeq.Start.Length), RNG.ChooseWhereToPlaceGene(MovedSeq, originalGenom));
                data.Genoms[repetition, 0] = originalGenom;
                data.Genoms[repetition, 1] = MovedSeq.ToGenom();
                for (int i = 0; i < population1.Length; i++)
                {
                    population1[i] = originalSeq.Clone();
                    population2[i] = MovedSeq.Clone();
                }
                int dataIndex = -1;
                for (int generation = 0; generation < MAX_GENERATION_NUMBER; generation++)
                {
                    if (generation % WRITE_DATA_PERIOD == 0)
                    {
                        UI.UpdateBar(0, counter);
                        dataIndex++;
                        counter++;
                    }
                    for (int i = 0; i < population1.Length * ACTIVE_FRACTION_OF_SEQUENCES; i++)
                    {
                        int acceptor = RNG.Int(population1.Length);
                        int donor = RNG.Int(population2.Length);
                        int seqPos = RNG.Int(population2[donor].DNA.Length - MIN_TRANSFER_SIZE);
                        int seqLen = RNG.Int(MIN_TRANSFER_SIZE, MAX_TRANSFER_SIZE);
                        if (seqLen > population2[donor].DNA.Length - seqPos) seqLen = population2[donor].DNA.Length - seqPos;
                        bool isSucces = population1[acceptor].Integrate(population2[donor].Subsequence(seqPos, seqLen));
                        bool isViable = population1[acceptor].IsViable(originalGenom);
                        if (!isViable) population1[acceptor] = population1[RNG.Exclude(acceptor, population1.Length)].Clone();
                        if (isSucces) data.Hybrides[repetition, dataIndex]++;
                        if (isViable && isSucces) data.Viables[repetition, dataIndex]++;
                    }
                    for (int i = 0; i < population1.Length * ACTIVE_FRACTION_OF_SEQUENCES_IN_POPULATION; i++)
                    {
                        int acceptor = RNG.Int(population1.Length);
                        int donor = RNG.Int(population1.Length);
                        int seqPos = RNG.Int(population1[donor].DNA.Length - MIN_TRANSFER_SIZE);
                        int seqLen = RNG.Int(MIN_TRANSFER_SIZE, MAX_TRANSFER_SIZE);
                        if (seqLen > population1[donor].DNA.Length - seqPos) seqLen = population1[donor].DNA.Length - seqPos;
                        bool isSucces = population1[acceptor].Integrate(population1[donor].Subsequence(seqPos, seqLen));
                        bool isViable = population1[acceptor].IsViable(originalGenom);
                        if (!isViable) population1[acceptor] = population1[RNG.Exclude(acceptor, population1.Length)].Clone();
                    }
                    for (int i = 2; i < 12; i++)
                    {
                        data.Genoms[repetition, i] = population1[RNG.Int(population1.Length)].ToGenom();

                    }
                    //Console.WriteLine("\n\n\n\n\n\ngfkutfdktuf");
                }
                //MovedSeq.MoveGene(RNG.Int(MovedSeq.Start.Length), RNG.ChooseWhereToPlaceGene(MovedSeq, originalGenom));
                //foreach (Sequence seq in population2) Console.WriteLine(seq.IsViable(originalGenom));
                //foreach (Sequence seq in population2) Interface.WriteSequence(seq);
                //UI.UpdateBar(0, finishedCounter++);
            });
            UI.Finished = true;
            //-------SETUP--------
            //UI.Finished = true;
            //UI.Write();
            return data;
        }
        public static Data Test1()
        {
            //------SETTINGS------
            const int GENOM_LENGTH = 1_000;
            const int GENE_NUMBER = 20;
            const int GENE_MINIMUM_SIZE = 22;
            const float GENE_DENSITY = 0.7f;

            const int POPULATION_SIZE = 1_000;

            const int MAX_GENERATION_NUMBER = 100;

            const float ACTIVE_FRACTION_OF_SEQUENCES = 0.1f;

            const int MIN_TRANSFER_SIZE = 30;
            Data data = new Data("Test1",1, MAX_GENERATION_NUMBER);
            //-------SETUP--------
            bool run = true;
            while (run) {
                data = new Data("Test1",1, MAX_GENERATION_NUMBER);

                //-------LOOP---------
                Genom originalGenom = RNG.GenerateGenom(GENOM_LENGTH, GENE_DENSITY, GENE_NUMBER, GENE_MINIMUM_SIZE);
                Sequence originalSeq = originalGenom.ToSequence();
                Sequence MovedSeq = originalSeq.Clone();
                int where = RNG.ChooseWhereToPlaceGene(MovedSeq, originalGenom); //tohle je jen pro debug, pak vepsat přímo do MoveGene
                int what = RNG.Int(MovedSeq.Start.Length); //tohle je jen pro debug, pak vepsat přímo do MoveGene
                Console.WriteLine(where);
                Console.WriteLine(what);
                MovedSeq.MoveGene(what, where); //tohle je potřeba předělat
                                                //MovedSeq.MoveGene(0, 50);

                Sequence[] population1 = new Sequence[POPULATION_SIZE];
                Sequence[] population2 = new Sequence[POPULATION_SIZE];

                for (int i = 0; i < population1.Length; i++)
                {
                    population1[i] = originalSeq.Clone();
                    population2[i] = MovedSeq.Clone();
                }

                for (int generation = 0; generation < MAX_GENERATION_NUMBER; generation++)
                {
                    Console.WriteLine(generation);
                    for (int i = 0; i < population1.Length * ACTIVE_FRACTION_OF_SEQUENCES; i++)
                    {
                        int acceptor = RNG.Int(population1.Length);
                        int donor = RNG.Int(population2.Length);
                        int seqPos = RNG.Int(population2[donor].DNA.Length - MIN_TRANSFER_SIZE);
                        int seqLen = RNG.Int(MIN_TRANSFER_SIZE, population2[donor].DNA.Length - seqPos);
                        bool isSucces = population1[acceptor].Integrate(population2[donor].Subsequence(seqPos, seqLen));
                        bool isViable = population1[acceptor].IsViable2(originalGenom); //odkomentovat až class Data hotovo;
                        if (!isViable) population1[acceptor] = population1[RNG.Exclude(acceptor, population1.Length)];
                        if (isSucces) data.Hybrides[0, generation]++;
                        if (isViable && isSucces) data.Viables[0, generation]++;
                    }
                    //for (int i = 0; i < population2.Length * ACTIVE_FRACTION_OF_SEQUENCES; i++)
                    //{
                    //    int selected = RNG.Int(population2.Length);
                    //    population2[selected].Integrate(population1[RNG.Int(population1.Length)].Subsequence(RNG.Int(MIN_TRANSFER_SIZE,)));
                    //    //population2[selected].IsViable(originalGenom); //odkomentovat až class Data hotovo;
                    //}
                }
                //foreach (Sequence seq in population1) Console.WriteLine(seq);
                Console.WriteLine(population2[0]);
                Console.WriteLine(population1[0]);
                Console.WriteLine(population2[0].DNA == population1[0].DNA);
                Console.WriteLine(population2[0].IsViable2(originalGenom));
                foreach (Sequence sequence in population1) if (!sequence.IsViable2(originalGenom)) run = false; ;
            }
            return data;
        }
        public static void test2()
        {
            const int GENOM_LENGTH = 100;
            const int GENE_NUMBER = 5;
            const int GENE_MINIMUM_SIZE = 5;
            const float GENE_DENSITY = 0.7f;

            const int POPULATION_SIZE = 1_000;

            const int MAX_GENERATION_NUMBER = 100;

            const float ACTIVE_FRACTION_OF_SEQUENCES = 0.1f;

            const int MIN_TRANSFER_SIZE = 30;

            Genom originalGenom = RNG.GenerateGenom(GENOM_LENGTH, GENE_DENSITY, GENE_NUMBER, GENE_MINIMUM_SIZE);
            Sequence originalSeq = originalGenom.ToSequence();
            Sequence MovedSeq = originalSeq.Clone();
            Console.WriteLine(originalSeq);
            Console.WriteLine(originalGenom);
            Console.WriteLine(MovedSeq.IsViable(originalGenom));
            Console.WriteLine(originalSeq.IsViable(originalGenom));
            for (int i = 0; i < 1000; i++) 
            {
                int where = RNG.ChooseWhereToPlaceGene(MovedSeq, originalGenom); 
                int what = RNG.Int(MovedSeq.Start.Length);
                Console.WriteLine(where);
                Console.WriteLine(what);
                foreach (Gene gen in MovedSeq.ToNonGeneArray(originalGenom)) Console.WriteLine(gen);
                MovedSeq.MoveGene(what, where);
                Console.WriteLine(MovedSeq);
                Console.WriteLine(MovedSeq.IsViable(originalGenom));
            }
            
        }
    }
}
