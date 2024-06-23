namespace ContractBridge.Core.Impl
{
    public class ScoringSystem : IScoringSystem
    {
        private const int MinorSuitPoints = 20;
        private const int MajorSuitPoints = 30;
        private const int NoTrumpFirstTrickPoints = 40;
        private const int RedoubledOvertrickNonVulnerable = 200;
        private const int RedoubledOvertrickVulnerable = 400;
        private const int DoubledOvertrickNonVulnerable = 100;
        private const int DoubledOvertrickVulnerable = 200;
        private const int OvertrickPoints = 30;
        private const int RedoubledUndertrickNonVulnerable = 400;
        private const int RedoubledUndertrickVulnerable = 800;
        private const int DoubledUndertrickNonVulnerable = 100;
        private const int DoubledUndertrickVulnerable = 200;
        private const int UndertrickNonVulnerable = 50;
        private const int UndertrickVulnerable = 100;
        private const int PartScoreBonus = 50;
        private const int GameBonusNonVulnerable = 300;
        private const int GameBonusVulnerable = 500;
        private const int SmallSlamNonVulnerable = 500;
        private const int SmallSlamVulnerable = 750;
        private const int GrandSlamNonVulnerable = 1000;
        private const int GrandSlamVulnerable = 1500;
        private const int RedoublingBonus = 100;
        private const int DoublingBonus = 50;

        public (int DeclarerScore, int DefenderScore) Score(IContract contract, bool vulnerable, int tricksMade)
        {
            var baseContractPoints = CalculateBaseContractPoints(contract);
            var overtrickPoints = CalculateOvertrickPoints(contract, vulnerable, tricksMade);
            var undertrickPenalties = CalculateUndertrickPenalties(contract, vulnerable, tricksMade);
            var gameBonus = CalculateGameBonus(baseContractPoints, vulnerable);
            var partScoreBonus = CalculatePartScoreBonus(baseContractPoints);
            var slamBonus = CalculateSlamBonus(contract, vulnerable);
            var doublingBonus = CalculateDoublingBonus(contract);

            var declarerScore = DeclarerScore();
            var defenderScore = DefenderScore();

            return (declarerScore, defenderScore);

            int DeclarerScore()
            {
                var ret = baseContractPoints + overtrickPoints + gameBonus + partScoreBonus + slamBonus + doublingBonus;
                return contract.Tricks() - tricksMade > 0 ? ret : 0;
            }

            int DefenderScore()
            {
                return contract.Tricks() - tricksMade <= 0 ? undertrickPenalties : 0;
            }
        }

        private static int CalculateBaseContractPoints(IContract contract)
        {
            var basePointsPerTrick = contract.Denomination switch
            {
                var d when d.IsMajor() => MajorSuitPoints,
                var d when d.IsMinor() => MinorSuitPoints,
                _ => MajorSuitPoints
            };

            if (contract.Denomination == Denomination.NoTrumps)
            {
                return NoTrumpFirstTrickPoints + ((int)contract.Level - 1) * MajorSuitPoints;
            }

            return (int)contract.Level * basePointsPerTrick;
        }

        private static int CalculateOvertrickPoints(IContract contract, bool vulnerable, int tricksMade)
        {
            var overtricks = tricksMade - contract.Tricks();

            if (overtricks <= 0)
            {
                return 0;
            }

            return contract.Risk switch
            {
                Risk.Redoubled => overtricks * RedoubledOvertrick(),
                Risk.Doubled => overtricks * DoubledOvertrick(),
                _ => overtricks * OvertrickPoints
            };

            int RedoubledOvertrick()
            {
                return vulnerable ? RedoubledOvertrickVulnerable : RedoubledOvertrickNonVulnerable;
            }

            int DoubledOvertrick()
            {
                return vulnerable ? DoubledOvertrickVulnerable : DoubledOvertrickNonVulnerable;
            }
        }

        private static int CalculateUndertrickPenalties(IContract contract, bool vulnerable, int tricksMade)
        {
            var undertricks = contract.Tricks() - tricksMade;

            if (undertricks <= 0)
            {
                return 0;
            }

            return contract.Risk switch
            {
                Risk.Redoubled => undertricks * RedoubledUndertrick(),
                Risk.Doubled => undertricks * DoubledUnderTrick(),
                _ => undertricks * Undertrick()
            };

            int RedoubledUndertrick()
            {
                return vulnerable ? RedoubledUndertrickVulnerable : RedoubledUndertrickNonVulnerable;
            }

            int DoubledUnderTrick()
            {
                return vulnerable ? DoubledUndertrickVulnerable : DoubledUndertrickNonVulnerable;
            }

            int Undertrick()
            {
                return vulnerable ? UndertrickVulnerable : UndertrickNonVulnerable;
            }
        }

        private static int CalculateGameBonus(int baseContractPoints, bool vulnerable)
        {
            return baseContractPoints >= 100 ? vulnerable ? GameBonusVulnerable : GameBonusNonVulnerable : 0;
        }

        private static int CalculatePartScoreBonus(int baseContractPoints)
        {
            return baseContractPoints < 100 ? PartScoreBonus : 0;
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
                Risk.Redoubled => RedoublingBonus,
                Risk.Doubled => DoublingBonus,
                _ => 0
            };
        }
    }
}