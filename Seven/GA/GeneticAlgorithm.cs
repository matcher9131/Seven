using Seven.Core.Engines;
using Seven.Core.Random;
using Seven.Core.Rules;
using System.Collections.Frozen;
using System.Linq.Expressions;

namespace Seven.GA
{
    public class GeneticAlgorithm(Rule rule, IRandom random)
    {
        private const int DefaultNumPopulation = 100;
        private const int DefaultNumEvaluationGames = 100000;
        private const int DefaultNumElites = 2;
        private const int DefaultMutationPercent = 10;

        private readonly Rule rule = rule;
        private readonly IRandom random = random;

        private static readonly FrozenDictionary<int, int> basePriorityMap = Enumerable.Range(0, 64).Where(i => !Graph.GetVertexes().Contains(i)).ToFrozenDictionary(x => x, _ => -1);

        private static readonly Comparison<Individual> populationComparison = (a, b) => a.Value.CompareTo(b.Value);
        
        private List<Individual> CreateInitialPopulation()
        {
            InitialPopulation initialPopulation = new(this.random);
            return [.. initialPopulation.Generate(DefaultNumPopulation).Select(gene => new Individual(0.0, gene))];
        }

        public Result Run(Result? prevResult, CancellationToken token)
        {
            #region Local functions
            IEngine createEngine(int[] gene)
            {
                Dictionary<int, int> priorityMap = new(basePriorityMap);
                for (int i = 0; i < gene.Length; ++i)
                {
                    priorityMap[gene[i]] = i;
                }
                return new EngineStandardMyCards(this.rule, priorityMap.AsReadOnly());
            }

            int[] rouletteSelect(List<Individual> population, double sumValues)
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
                return population[^1].Gene;
            }

            Func<IEngine> createOppositeEngineFactory(Type engineType)
            {
                Type[] paramTypes = [typeof(Rule)];
                var ctorInfo = engineType.GetConstructor(paramTypes) ?? throw new InvalidOperationException("Constructor is not found.");
                var paramExpression = Expression.Constant(Rule.Standard);
                var body = Expression.New(ctorInfo, [paramExpression]);
                var lambda = Expression.Lambda<Func<IEngine>>(body);
                var func = lambda.Compile();
                return func;
            }
            #endregion

            if (prevResult == null)
            {
                prevResult = new Result(
                    new Settings(DefaultNumPopulation, DefaultNumEvaluationGames, DefaultNumElites, DefaultMutationPercent, "Seven.Core.Engines.EngineStandardA, Seven.Core"),
                    0,
                    this.CreateInitialPopulation()
                );
            }
            Settings settings = prevResult.Settings;

            Type oppositeEngineType = Type.GetType(settings.OppositeEngineName) ?? throw new InvalidOperationException("Invalid opposite engine name");
            var oppositeEngineFactory = createOppositeEngineFactory(oppositeEngineType);

            Evaluation evaluation = new(this.rule, settings.NumEvaluationGames, () => Seiran.Instance);
            Crossover crossover = new(this.random);
            Mutation mutation = new(this.random);

            List<Individual> currentPopulation = prevResult.Population;
            List<Individual> nextPopulation = new(settings.NumPopulation);

            int iGen = prevResult.Generation;
            for (; ; ++iGen)
            {
                Console.WriteLine($"Generation {iGen}");

                // 評価
                for (int i = 0; i < settings.NumPopulation; ++i)
                {
                    double newValue = evaluation.Evaluate(
                        createEngine(currentPopulation[i].Gene),
                        [oppositeEngineFactory(), oppositeEngineFactory(), oppositeEngineFactory(), oppositeEngineFactory()]
                    );
                    currentPopulation[i] = new Individual(newValue, currentPopulation[i].Gene);
                }
                currentPopulation.Sort(populationComparison);

                // ユーザーによる中断
                if (token.IsCancellationRequested)
                {
                    break;
                }

                double sumEvaluation = currentPopulation.Sum(x => x.Value);
                nextPopulation = new(settings.NumPopulation);

                // エリート選択
                for (int i = 0; i < settings.NumElites; ++i)
                {
                    nextPopulation.Add(currentPopulation[^(i + 1)]);
                }

                // 交叉および突然変異
                while (nextPopulation.Count < settings.NumPopulation)
                {
                    int[] p1 = rouletteSelect(currentPopulation, sumEvaluation);
                    int[] p2 = rouletteSelect(currentPopulation, sumEvaluation);
                    var (c1, c2) = crossover.Cross(p1, p2);
                    if (this.random.Next(100) < settings.MutationPercent)
                    {
                        c1 = mutation.Mutate(c1);
                    }
                    if (this.random.Next(100) < settings.MutationPercent)
                    {
                        c2 = mutation.Mutate(c2);
                    }
                    nextPopulation.Add(new Individual(0, c1));
                    nextPopulation.Add(new Individual(0, c2));
                }

                currentPopulation = nextPopulation;
            }

            return new Result(settings, iGen, currentPopulation);
        }
    }
}
