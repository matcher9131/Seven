using Seven.Core;
using Seven.Core.Engines;
using Seven.Core.Rules;
using Seven.GA;

Rule rule = Rule.Standard;
GeneticAlgorithm ga = new(rule, new StandardRandom());

Func<IEngine> oppositeEngineFactory = () => new EngineStandardA(rule, new StandardRandom());
ga.Run(oppositeEngineFactory);