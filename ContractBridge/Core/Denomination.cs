namespace ContractBridge.Core
{
    public enum Denomination : byte
    {
        Clubs = 1,
        Diamonds,
        Hearts,
        Spades,
        NoTrumps
    }

    public static class DenominationExtensions
    {
        public static bool IsMajor(this Denomination denomination)
        {
            return denomination is Denomination.Hearts or Denomination.Spades;
        }

        public static bool IsMinor(this Denomination denomination)
        {
            return denomination is Denomination.Clubs or Denomination.Diamonds;
        }
    }
}