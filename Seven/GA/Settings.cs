namespace Seven.GA
{
    public record Settings(
        int Seed, 
        int NumGeneration, 
        int NumPopulation, 
        int NumEvaluationGames, 
        int NumElites, 
        int MutationPercent, 
        string OppositeEngineName
    )
    {
        public static Settings GetDefault(int seed) => new(seed, 100, 100, 100000, 2, 10, "EngineStandardA");
    }
}
