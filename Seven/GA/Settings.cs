namespace Seven.GA
{
    public record Settings(
        int NumPopulation, 
        int NumEvaluationGames, 
        int NumElites, 
        int MutationPercent, 
        string OppositeEngineName
    )
    {
        public static Settings GetDefault() => new(100, 100000, 2, 10, "EngineStandardA");
    }
}
