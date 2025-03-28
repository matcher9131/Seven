using Seven.GA;

namespace Seven.Test
{
    public class HelperTest
    {
        public class OrderBasedCrossoverTestData : TheoryData<int[], int[], int[], int[]>
        {
            public OrderBasedCrossoverTestData()
            {
                this.Add(
                    [3, 4, 1, 2, 5, 8, 7, 6],
                    [2, 5, 4, 6, 1, 7, 3, 8],
                    [0, 2, 5, 7],
                    [2, 5, 4, 3, 1, 7, 8, 6]
                );
                this.Add(
                    [4, 5, 1, 7, 6, 3, 2, 0],
                    [5, 4, 0, 1, 2, 6, 7, 3],
                    [2, 5, 6, 7],
                    [5, 4, 1, 3, 2, 6, 7, 0]
                );
            }
        }

        [Theory]
        [ClassData(typeof(OrderBasedCrossoverTestData))]
        public void OrderBasedCrossoverTest(int[] x, int[] y, int[] indices, int[] expected)
        {
            var actual = Helper.OrderBasedCrossover(x, y, indices);
            Assert.Equal(expected, actual);
        }

        public class CutAndInsertTestData : TheoryData<int[], int, int, int, int[]>
        {
            public CutAndInsertTestData()
            {
                this.Add([0, 1, 2, 3, 4, 5, 6], 2, 6, 1, [0, 2, 3, 4, 5, 1, 6]);
                this.Add([0, 1, 2, 3, 4, 5, 6], 2, 6, 3, [0, 1, 6, 2, 3, 4, 5]);
                this.Add([0, 1, 2, 3, 4, 5, 6], 2, 6, 0, [2, 3, 4, 5, 0, 1, 6]);
                this.Add([0, 1, 2, 3, 4, 5, 6], 0, 3, 0, [0, 1, 2, 3, 4, 5, 6]);
                this.Add([0, 1, 2, 3, 4, 5, 6], 0, 3, 2, [3, 4, 0, 1, 2, 5, 6]);
            }
        }

        [Theory]
        [ClassData(typeof(CutAndInsertTestData))]
        public void CutAndInsertTest(int[] perm, int l, int r, int i, int[] expected)
        {
            var actual = Helper.CutAndInsert(perm, l, r, i);
            Assert.Equal(expected, actual);
        }
    }
}
