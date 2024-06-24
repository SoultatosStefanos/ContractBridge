namespace ContractBridge.Core.Impl
{
    // FIXME

    public class ScoringSystem : IScoringSystem
    {
        private const int MinorSuitPoints = 20;
        private const int MajorSuitPoints = 30;
        private const int NoTrumpFirstTrickPoints = 40;
        private const int OvertrickMinorSuit = 20;
        private const int OvertrickMajorSuit = 30;
        private const int OvertrickNoTrump = 30;
        private const int DoubledOvertrickNonVulnerable = 100;
        private const int DoubledOvertrickVulnerable = 200;
        private const int UndertrickNonVulnerable = 50;
        private const int UndertrickVulnerable = 100;
        private const int PartScoreBonus = 50;
        private const int GameBonusNonVulnerable = 300;
        private const int GameBonusVulnerable = 500;
        private const int SmallSlamNonVulnerable = 500;
        private const int SmallSlamVulnerable = 750;
        private const int GrandSlamNonVulnerable = 1000;
        private const int GrandSlamVulnerable = 1500;
        private const int DoublingBonus = 50;
        private const int RedoublingBonus = 100;

        public (int DeclarerScore, int DefenderScore) Score(IContract contract, bool vulnerable, int tricksMade)
        {
            var contractPoints = CalculateContractPoints(contract);
            var overtrickPoints = CalculateOvertrickPoints(contract, vulnerable, tricksMade);
            var undertrickPoints = CalculateUndertrickPoints(contract, vulnerable, tricksMade);
            var gameBonus = CalculateGameBonus(contractPoints, vulnerable);
            var partScoreBonus = contractPoints < 100 ? PartScoreBonus : 0;
            var slamBonus = CalculateSlamBonus(contract, vulnerable);
            var doublingBonus = CalculateDoublingBonus(contract);

            // For doubled contracts, we add the doubling bonus separately after calculating the contract points.
            var declarerScore = contractPoints + overtrickPoints + gameBonus + partScoreBonus + slamBonus +
                                doublingBonus;
            var defenderScore = undertrickPoints;

            return (declarerScore, defenderScore);
        }

        private static int CalculateContractPoints(IContract contract)
        {
            var basePointsPerTrick = contract.Denomination switch
            {
                var d when d.IsMajor() => MajorSuitPoints,
                var d when d.IsMinor() => MinorSuitPoints,
                Denomination.NoTrumps => MajorSuitPoints, // Not used directly
                _ => 0
            };

            var contractPoints = contract.Denomination switch
            {
                Denomination.NoTrumps => NoTrumpFirstTrickPoints + ((int)contract.Level - 1) * MajorSuitPoints,
                _ => (int)contract.Level * basePointsPerTrick
            };

            if (contract.Risk == Risk.Doubled)
            {
                contractPoints *= 2;
            }
            else if (contract.Risk == Risk.Redoubled)
            {
                contractPoints *= 4;
            }

            return contractPoints;
        }

        private static int CalculateOvertrickPoints(IContract contract, bool vulnerable, int tricksMade)
        {
            var overtricks = tricksMade - ((int)contract.Level + 6);
            if (overtricks <= 0)
            {
                return 0;
            }

            var overtrickPoints = contract.Risk switch
            {
                Risk.Doubled => overtricks * (vulnerable ? DoubledOvertrickVulnerable : DoubledOvertrickNonVulnerable),
                Risk.Redoubled => overtricks * 2 *
                                  (vulnerable ? DoubledOvertrickVulnerable : DoubledOvertrickNonVulnerable),
                _ => overtricks * contract.Denomination switch
                {
                    Denomination.Spades or Denomination.Hearts or Denomination.NoTrumps => OvertrickMajorSuit,
                    Denomination.Clubs or Denomination.Diamonds => OvertrickMinorSuit,
                    _ => 0
                }
            };

            return overtrickPoints;
        }

        private static int CalculateUndertrickPoints(IContract contract, bool vulnerable, int tricksMade)
        {
            var undertricks = (int)contract.Level + 6 - tricksMade;
            if (undertricks <= 0)
            {
                return 0;
            }

            var undertrickPoints = contract.Risk switch
            {
                Risk.Doubled => vulnerable ? 200 + (undertricks - 1) * 300 : 100 + (undertricks - 1) * 200,
                Risk.Redoubled => vulnerable ? 400 + (undertricks - 1) * 600 : 200 + (undertricks - 1) * 400,
                _ => undertricks * (vulnerable ? UndertrickVulnerable : UndertrickNonVulnerable)
            };

            return undertrickPoints;
        }

        private static int CalculateGameBonus(int contractPoints, bool vulnerable)
        {
            return contractPoints >= 100 ? vulnerable ? GameBonusVulnerable : GameBonusNonVulnerable : 0;
        }

        private static int CalculateSlamBonus(IContract contract, bool vulnerable)
        {
            return contract.Level switch
            {
                Level.Six => vulnerable ? SmallSlamVulnerable : SmallSlamNonVulnerable,
                Level.Seven => vulnerable ? GrandSlamVulnerable : GrandSlamNonVulnerable,
                _ => 0
            };
        }

        private static int CalculateDoublingBonus(IContract contract)
        {
            return contract.Risk switch
            {
                Risk.Doubled => DoublingBonus,
                Risk.Redoubled => RedoublingBonus,
                _ => 0
            };
        }
    }
}