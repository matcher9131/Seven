using Seven.Core;
using Seven.Core.Engines;
using Seven.Core.Models;
using Seven.Core.Rules;
using System.Collections.Frozen;

namespace Seven.GA
{
    public class GeneticAlgorithm(Rule rule, IRandom random)
    {
        private const int NumGeneration = 2;
        private const int NumPopulation = 100;
        private const int NumElite = 2;
        private const int MutationPercent = 10;

        private readonly Rule rule = rule;
        private readonly IRandom random = random;

        private static readonly FrozenDictionary<int, int> basePriorityMap = Enumerable.Range(0, 64).Where(i => !Graph.GetVertexes().Contains(i)).ToFrozenDictionary(x => x, _ => -1);

        private static readonly Comparison<(double value, int[] gene)> populationComparison = (a, b) => a.value.CompareTo(b.value);

        public void Run(Func<IEngine> oppositeEngineFactory)
        {
            IEngine createEngine(int[] gene)
            {
                Dictionary<int, int> priorityMap = new(basePriorityMap);
                for (int i = 0; i < gene.Length; ++i)
                {
                    priorityMap[gene[i]] = i;
                }
                return new EngineStandardMyCards(this.rule, this.random, priorityMap.AsReadOnly());
            }

            int[] rouletteSelect(List<(double value, int[] gene)> population, double sumValues)
            {
                double rand = this.random.NextDouble();
                double current = 0;
                foreach (var (value, gene) in population)
                {
                    current += value;
                    if (rand < current)
                    {
                        return gene;
                    }
                }
                return population[^1].gene;
            }

            DateTime startTime = DateTime.Now;

            InitialPopulation initialPopulation = new(this.random);
            Evaluation evaluation = new(this.rule, new Dealer(this.random));
            Crossover crossover = new(this.random);
            Mutation mutation = new(this.random);

            List<(double value, int[] gene)> currentPopulation = new(NumPopulation);
            List<(double value, int[] gene)> nextPopulation = new(NumPopulation);

            foreach (var gene in initialPopulation.Generate(NumPopulation))
            {
                currentPopulation.Add((0, gene));
            }

            //
            Console.WriteLine("foo");
            //

            for (int iGen = 0; iGen < NumGeneration; ++iGen)
            {
                Console.WriteLine($"Generation: {iGen}");

                // 評価
                for (int i = 0; i < NumPopulation; ++i)
                {
                    double newValue = evaluation.Evaluate(
                        createEngine(currentPopulation[i].gene),
                        [oppositeEngineFactory(), oppositeEngineFactory(), oppositeEngineFactory(), oppositeEngineFactory()]
                    );
                    currentPopulation[i] = (newValue, currentPopulation[i].gene);
                }
                currentPopulation.Sort(populationComparison);

                // 終了判定
                if (iGen == NumGeneration - 1)
                {
                    break;
                }

                double sumEvaluation = currentPopulation.Sum(x => x.value);
                nextPopulation = new(NumPopulation);
                // エリート選択
                for (int i = 0; i < NumElite; ++i)
                {
                    nextPopulation.Add(currentPopulation[^(i + 1)]);
                }
                
                // 交叉および突然変異
                while (nextPopulation.Count < NumPopulation)
                {
                    int[] p1 = rouletteSelect(currentPopulation, sumEvaluation);
                    int[] p2 = rouletteSelect(currentPopulation, sumEvaluation);
                    var (c1, c2) = crossover.Cross(p1, p2);
                    if (this.random.Next(100) < MutationPercent)
                    {
                        c1 = mutation.Mutate(c1);
                    }
                    if (this.random.Next(100) < MutationPercent)
                    {
                        c2 = mutation.Mutate(c2);
                    }
                    nextPopulation.Add((0, c1));
                    nextPopulation.Add((0, c2));
                }

                currentPopulation = nextPopulation;
            }

            DateTime endTime = DateTime.Now;
            TimeSpan span = endTime - startTime;

            string logContent = $"NumGeneration = {NumGeneration}\nNumPopulation = {NumPopulation}\nNumElite = {NumElite}\nMutationPercent = {MutationPercent}\nOppositeEngine: {oppositeEngineFactory().GetType().Name}\n";
            for (int i = 0; i < 5; ++i)
            {
                var (value, gene) = currentPopulation[^(i + 1)];
                logContent += $"Population 0:\n  gene: [{string.Join(", ", gene)}]\n  value: {value:0.000000}\n";
            }
            logContent += $"Elapsed: {span:hh\\:mm\\:ss}\n";
            Logger logger = new();
            logger.Log(logContent);

            Console.WriteLine(logContent);
        }
    }
}
