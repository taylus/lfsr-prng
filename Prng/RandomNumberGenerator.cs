namespace Prng
{
    /// <summary>
    /// Implements a random number generator using a linear feedback shift register.
    /// This approach can be applied to the Game Boy given its simple shift and xor requirements.
    /// </summary>
    /// <remarks>
    /// This class is adapted from the PRNG described here: 
    /// https://wiki.nesdev.com/w/index.php/Random_number_generator,
    /// which is why it wound up implementing some aspects of the NES 6502-based CPU.
    /// </remarks>
    public class LinearFeedbackShiftRegisterPrng
    {
        private bool carry;                 //CPU's carry flag
        public ushort Seed { get; set; }    //TODO: disallow zero? seeding with zero will always return zero

        public LinearFeedbackShiftRegisterPrng(ushort seed)
        {
            Seed = seed;
        }

        /// <summary>
        /// Generates and returns a random byte.
        /// Requires a 16-bit seed value initialized to any value except zero
        /// (a seed of zero will cause the PRNG to always return zero.)
        /// </summary>
        /// <remarks>
        /// "This is a 16-bit Galois linear feedback shift register with polynomial 0x2D.
        /// The sequence of numbers it generates will repeat after 65535 calls."
        /// </remarks>
        /// <see cref="https://wiki.nesdev.com/w/index.php/Random_number_generator"/>
        public byte Generate()
        {
            byte x = 8;             //ldx #8
            byte a = Low(Seed);     //lda seed+0

            while (x > 0)
            {
                //asl
                a = ShiftLeftWithCarry(a, ref carry);

                //rol seed+1
                Seed = (ushort)(RotateLeftWithCarry(High(Seed), ref carry) << 8 | Low(Seed));

                //"apply XOR feedback whenever a 1 bit is shifted out"
                if (carry) a ^= 0x2D;
                x--;
            }

            Seed = (ushort)((ushort)(High(Seed) << 8) | a);     //sta seed+0
            return a;   //rts
        }

        /// <summary>
        /// Returns the high byte of the given 16-bit value.
        /// </summary>
        private static byte High(ushort value) => (byte)(value >> 8);

        /// <summary>
        /// Returns the low byte of the given 16-bit value.
        /// </summary>
        private static byte Low(ushort value) => (byte)(value & 0xFF);

        /// <summary>
        /// Returns true if the most significant bit of the given byte
        /// is 1, false otherwise. Useful for determining if a carry
        /// occurred while executing an instruction.
        /// </summary>
        private static bool IsMostSignicantBitSet(byte value) => (value & 0b1000_0000) != 0;

        /// <summary>
        /// Implements the 6502's ASL (arithmetic shift left) instruction.
        /// All bits are shifted left one position w/ 0 shifted into bit 0
        /// and bit 7 shifted into the carry.
        /// </summary>
        private static byte ShiftLeftWithCarry(byte value, ref bool carry)
        {
            carry = IsMostSignicantBitSet(value);
            return (byte)(value << 1);
        }

        /// <summary>
        /// Implements the 6502's ROL (rotate left) instruction.
        /// All bits are shifted left one position w/ the carry shifted
        /// into bit 0 and the original bit 7 shifted into the carry.
        /// </summary>
        private static byte RotateLeftWithCarry(byte value, ref bool carry)
        {
            bool newCarry = IsMostSignicantBitSet(value);
            value <<= 1;
            if (carry) value |= 1;
            carry = newCarry;
            return value;
        }
    }
}
