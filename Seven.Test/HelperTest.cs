namespace Seven.Test
{
    public class HelperTest
    {
        public class HelperTestData : TheoryData<int[], int, int, int, int[]>
        {
            public HelperTestData()
            {
                this.Add([0, 1, 2, 3, 4, 5, 6], 2, 6, 1, [0, 2, 3, 4, 5, 1, 6]);
                this.Add([0, 1, 2, 3, 4, 5, 6], 2, 6, 3, [0, 1, 6, 2, 3, 4, 5]);
                this.Add([0, 1, 2, 3, 4, 5, 6], 2, 6, 0, [2, 3, 4, 5, 0, 1, 6]);
                this.Add([0, 1, 2, 3, 4, 5, 6], 0, 3, 0, [0, 1, 2, 3, 4, 5, 6]);
                this.Add([0, 1, 2, 3, 4, 5, 6], 0, 3, 2, [3, 4, 0, 1, 2, 5, 6]);
            }
        }

        [Theory]
        [ClassData(typeof(HelperTestData))]
        public void CutAndInsertTest(int[] perm, int l, int r, int i, int[] expected)
        {
            int[] actual = [.. perm];
            Helper.CutAndInsert(actual, l, r, i);
            Assert.Equal(actual, expected);
        }
    }
}
