using Seven.Core.Random;

namespace Seven.GA
{
    public class Mutation(IRandom random)
    {
        private readonly IRandom random = random;

        public int[] Mutate(int[] genes)
        {
            int l = (int)random.Next((uint)genes.Length);
            int r = (int)random.Next((uint)genes.Length);
            if (l > r)
            {
                (l, r) = (r, l);
            }
            ++r;
            int i = (int)random.Next((uint)(genes.Length - (r - l)));
            return Helper.CutAndInsert(genes, l, r, i);
        }
    }
}
