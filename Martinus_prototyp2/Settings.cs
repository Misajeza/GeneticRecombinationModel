using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Martinus_prototyp2
{
    public static class Settings
    {
        //Parameters of mobile elements jumps:
        public const int minJumpSize = 3;
        public const int maxJumpSize = 10;

        //Recombination parameters:
        public const int comparedSequenceSize = 11;
        public const int minRecombinationSize = 5;
        public const int maxRecombinationSize = 40;

        //enviroment parameters:
        public const int populationSize = 100;
        public const int generations = 10000;

        //simulation parametres:
        public const int repetitions = 1000;
        public const int minGeneShift = 0;
        public const int maxGeneShift = 1;

        //genom parameters:
        public const int genomSize = 1000;
        public const float houseKeepingGenePercentil = 70 / 100f;
        public const int houseKeepingGeneCount = 200;


    }
}