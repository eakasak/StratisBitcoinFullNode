﻿using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NBitcoin.Rules;
using Stratis.Bitcoin.Features.Consensus.Rules.CommonRules;
using Xunit;

namespace Stratis.Bitcoin.Features.Consensus.Tests
{
    public class PosConsensusRulesRegistrationTests
    {
        private readonly IEnumerable<IConsensusRule> rules;

        public PosConsensusRulesRegistrationTests()
        {
            this.rules = new FullNodeBuilderConsensusExtension.PosConsensusRulesRegistration().GetRules();
        }

        [Fact(Skip = "This should be activated when rules move to network")]
        public void GetRules_ForPOS_ReturnsListOfRegisteredPowRules()
        {
            this.rules.Count().Should().Be(26);

            this.rules.ElementAt(0).Should().BeOfType<HeaderTimeChecksRule>();
            this.rules.ElementAt(1).Should().BeOfType<HeaderTimeChecksPosRule>();
            this.rules.ElementAt(2).Should().BeOfType<PosTimeMaskRule>();
            this.rules.ElementAt(3).Should().BeOfType<StratisBigFixPosFutureDriftRule>();
            this.rules.ElementAt(4).Should().BeOfType<StratisHeaderVersionRule>();
            this.rules.ElementAt(5).Should().BeOfType<BlockMerkleRootRule>();
            this.rules.ElementAt(6).Should().BeOfType<PosBlockSignatureRule>();
            this.rules.ElementAt(7).Should().BeOfType<SetActivationDeploymentsRule>();
            this.rules.ElementAt(8).Should().BeOfType<CheckDifficultyPosRule>();
            this.rules.ElementAt(9).Should().BeOfType<CheckpointsRule>();
            this.rules.ElementAt(10).Should().BeOfType<AssumeValidRule>();
            this.rules.ElementAt(11).Should().BeOfType<TransactionLocktimeActivationRule>();
            this.rules.ElementAt(12).Should().BeOfType<CoinbaseHeightActivationRule>();
            this.rules.ElementAt(13).Should().BeOfType<WitnessCommitmentsRule>();
            this.rules.ElementAt(14).Should().BeOfType<BlockSizeRule>();
            this.rules.ElementAt(15).Should().BeOfType<PosBlockContextRule>();
            this.rules.ElementAt(16).Should().BeOfType<EnsureCoinbaseRule>();
            this.rules.ElementAt(17).Should().BeOfType<CheckPowTransactionRule>();
            this.rules.ElementAt(18).Should().BeOfType<CheckPosTransactionRule>();
            this.rules.ElementAt(19).Should().BeOfType<CheckSigOpsRule>();
            this.rules.ElementAt(20).Should().BeOfType<PosCoinstakeRule>();
            this.rules.ElementAt(21).Should().BeOfType<LoadCoinviewRule>();
            this.rules.ElementAt(22).Should().BeOfType<TransactionDuplicationActivationRule>();
            this.rules.ElementAt(23).Should().BeOfType<PosCoinviewRule>();
            this.rules.ElementAt(24).Should().BeOfType<SaveCoinviewRule>();
        }
    }
}