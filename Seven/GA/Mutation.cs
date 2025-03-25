using Seven.Core;

namespace Seven.GA
{
    public class Mutation(IRandom random)
    {
        private readonly IRandom random = random;

        public int[] Mutate(int[] genes)
        {
            int l = random.Next(genes.Length);
            int r = random.Next(genes.Length);
            if (l > r)
            {
                (l, r) = (r, l);
            }
            ++r;
            int i = random.Next(genes.Length - (r - l));
            return Helper.CutAndInsert(genes, l, r, i);
        }
    }
}
