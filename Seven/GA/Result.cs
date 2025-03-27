namespace Seven.GA
{
    public record Result(
        Settings Settings,
        int Generation,
        (double value, int[] gene)[] Population
    );
}
