using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Seven.Core.Random
{
    /// <summary>
    /// Seiran128 pseudorandom number generator.
    /// </summary>
    /// <seealso cref="https://github.com/andanteyk/prng-seiran"/>
    public sealed class Seiran : IRandom
    {
        private ulong state0;
        private ulong state1;

        [ThreadStatic]
        private static Seiran? staticInstance;

        /// <summary>
        /// Gets a static instance of <see cref="Seiran"/>.
        /// </summary>
        /// <remarks>
        /// If you don't need reproducibility, we recommend using this.
        /// This instance is thread static.
        /// </remarks>
        public static Seiran Instance => staticInstance ??= new Seiran();

        /// <summary>
        /// Initializes a new instance of the <see cref="Seiran"/> class using a cryptographically secure pseudo-random number generator.
        /// </summary>
        /// <remarks>
        /// Basically you should use this constructor.
        /// </remarks>
        public Seiran()
        {
            Span<ulong> state = stackalloc ulong[2];
            var stateBytes = MemoryMarshal.Cast<ulong, byte>(state);

            using var rng = RandomNumberGenerator.Create();
            do
            {
                rng.GetBytes(stateBytes);
            }
            while (state[0] == 0 && state[1] == 0);

            state0 = state[0];
            state1 = state[1];
        }
        

        /// <summary>
        /// Initializes a new instance of the <see cref="Seiran"/> class using the specified <paramref name="state"/>.
        /// </summary>
        /// <param name="state">
        /// Internal state obtained by <see cref="GetState"/>.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The length of <paramref name="state"/> is less than 2, or all elements of <paramref name="state"/> are 0.
        /// </exception>
        public Seiran(ReadOnlySpan<ulong> state)
        {
            if (state.Length < 2) throw new ArgumentOutOfRangeException(nameof(state));
            if (state[0] == 0 && state[1] == 0) throw new ArgumentOutOfRangeException(nameof(state));

            state0 = state[0];
            state1 = state[1];
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="Seiran"/> class that will generate the same sequence of random numbers as the <paramref name="original"/> instance.
        /// </summary>
        /// <param name="original">Copy source instance.</param>
        /// <exception cref="ArgumentNullException"><paramref name="original"/> is null.</exception>
        public Seiran(Seiran original)
        {
            ArgumentNullException.ThrowIfNull(original);

            state0 = original.state0;
            state1 = original.state1;
        }

        /// <summary>
        /// Gets the internal state of this <see cref="Seiran"/> instance.
        /// </summary>
        /// <returns>Internal state. The element length is 2.</returns>
        /// <remarks>
        /// You can use <see cref="Seiran(ReadOnlySpan{ulong})"/> to restore the current state of this instance.
        /// </remarks>
        public ulong[] GetState() => [state0, state1];



        /// <summary>
        /// Generates a 64-bit random number.
        /// </summary>
        /// <returns>[0, <see cref="ulong.MaxValue"/>]</returns>
        public ulong NextULong()
        {
            static ulong rotl(ulong x, int k) => x << k | x >> -k;

            ulong s0 = state0, s1 = state1;
            ulong result = rotl((s0 + s1) * 9, 29) + s0;

            state0 = s0 ^ rotl(s1, 29);
            state1 = s0 ^ s1 << 9;

            return result;
        }

        /// <summary>
        /// Generates a random number smaller than the specified maximum.
        /// </summary>
        /// <param name="maxExclusive">An exclusive upper limit for the generated random numbers.</param>
        /// <returns>[0, <paramref name="maxExclusive"/>)</returns>
        public uint Next(uint maxExclusive)
        {
            var hi = Math.BigMul(NextULong(), maxExclusive, out var lo);
            if (lo < maxExclusive)
            {
                ulong lowerBound = (0ul - maxExclusive) % maxExclusive;

                while (lo < lowerBound)
                {
                    hi = Math.BigMul(NextULong(), maxExclusive, out lo);
                }
            }
            return (uint)hi;
        }


        /// <summary>
        /// Generates a floating-point random number greater than or equal to 0.0 and less than 1.0.
        /// </summary>
        /// <returns>[0.0, 1.0)</returns>
        /// <remarks>
        /// The actual minimum value greater than 0 is 1.1102230246251565E-16 and the actual maximum value is 0.99999999999999989.
        /// </remarks>
        public double NextDouble() => (NextULong() >> 11) * (1.0 / (1ul << 53));


        /// <summary>
        /// Fills the elements of the specified array with random numbers.
        /// </summary>
        /// <param name="bytes">An array to be filled with random numbers.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="bytes"/> is null.
        /// </exception>
        /// <remarks>
        /// The range of random numbers is [0x00, 0xff].
        /// </remarks>
        public void NextBytes(byte[] bytes)
        {
            ArgumentNullException.ThrowIfNull(bytes);

            NextBytes(bytes.AsSpan());
        }

        /// <summary>
        /// Fills the elements of the specified span with random numbers.
        /// </summary>
        /// <param name="buffer">A span to be filled with random numbers.</param>
        /// <remarks>
        /// The range of random numbers is [0x00, 0xff].
        /// If you want to apply to another type, use <see cref="MemoryMarshal.Cast{TFrom, TTo}(Span{TFrom})"/>.
        /// </remarks>
        public void NextBytes(Span<byte> buffer)
        {
            var ulongSpan = MemoryMarshal.Cast<byte, ulong>(buffer);
            for (int i = 0; i < ulongSpan.Length; i++)
                ulongSpan[i] = NextULong();

            if ((buffer.Length & 7) != 0)
            {
                ulong r = NextULong();
                for (int i = buffer.Length & ~7; i < buffer.Length; i++)
                {
                    buffer[i] = (byte)r;
                    r >>= 8;
                }
            }
        }


        /// <summary>
        /// Shuffles the order of the elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="list"/>.</typeparam>
        /// <param name="list">List to be shuffled.</param>
        /// <exception cref="ArgumentNullException"> <paramref name="list"/> is null. </exception>
        public void ShuffleList<T>(IList<T> list)
        {
            ArgumentNullException.ThrowIfNull(list);

            for (int i = list.Count - 1; i > 0; i--)
            {
                int r = (int)Next((uint)(i + 1));
                (list[i], list[r]) = (list[r], list[i]);
            }
        }

        /// <summary>
        /// Shuffles the order of the elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="span"/>.</typeparam>
        /// <param name="span">List to be shuffled.</param>
        public void ShuffleSpan<T>(Span<T> span)
        {
            for (int i = span.Length - 1; i > 0; i--)
            {
                int r = (int)Next((uint)(i + 1));
                (span[i], span[r]) = (span[r], span[i]);
            }
        }



        private void Jump(ReadOnlySpan<ulong> jumpPolynomial)
        {
            ulong t0 = 0, t1 = 0;

            for (int i = 0; i < 2; i++)
            {
                for (int b = 0; b < 64; b++)
                {
                    if (((jumpPolynomial[i] >> b) & 1) != 0)
                    {
                        t0 ^= state0;
                        t1 ^= state1;
                    }
                    NextULong();
                }
            }

            state0 = t0;
            state1 = t1;
        }

        /// <summary>
        /// Advance the internal state by 2^32 steps.
        /// </summary>
        /// <remarks>
        /// This method is equivalent to 2^32 <see cref="NextULong"/> calls.
        /// It can be executed in the same amount of time as 128 <see cref="NextULong"/> calls.
        /// </remarks>
        public void Jump32() => Jump([0x40165CBAE9CA6DEB, 0x688E6BFC19485AB1]);

        /// <summary>
        /// Advance the internal state by 2^64 steps.
        /// </summary>
        /// <remarks>
        /// This method is equivalent to 2^64 <see cref="NextULong"/> calls.
        /// It can be executed in the same amount of time as 128 <see cref="NextULong"/> calls.
        /// </remarks>
        public void Jump64() => Jump([0xF4DF34E424CA5C56, 0x2FE2DE5C2E12F601]);

        /// <summary>
        /// Advance the internal state by 2^96 steps.
        /// </summary>
        /// <remarks>
        /// This method is equivalent to 2^96 <see cref="NextULong"/> calls.
        /// It can be executed in the same amount of time as 128 <see cref="NextULong"/> calls.
        /// </remarks>
        public void Jump96() => Jump([0x185F4DF8B7634607, 0x95A98C7025F908B2]);


        /// <summary>
        /// Rewinds the internal state to the previous state.
        /// </summary>
        public void Prev()
        {
            static ulong rotl(ulong x, int k) => x << k | x >> -k;

            ulong t1 = rotl(state0 ^ state1, 64 - 29);
            t1 ^= t1 << 44 ^ (t1 & ~0xffffful) << 24;
            t1 ^= (t1 & (0x7ffful << 40)) << 4;
            t1 ^= (t1 & (0x7fful << 40)) << 8;
            t1 ^= (t1 & (0x7ul << 40)) << 16;
            t1 ^= (t1 & (0xffffful << 35)) >> 20;
            t1 ^= (t1 & (0x7ffful << 20)) >> 20;

            state0 ^= rotl(t1, 29);
            state1 = t1;
        }
    }
}
