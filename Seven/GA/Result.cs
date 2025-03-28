namespace Seven.GA
{
    public record Result(
        Settings Settings,
        int Generation,
        List<Individual> Population
    );
}
