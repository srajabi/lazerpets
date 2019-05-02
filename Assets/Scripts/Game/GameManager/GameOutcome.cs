using System;
using System.Collections.Generic;

namespace Game
{
    public class GameOutcomeEventArgs : EventArgs
    {
        public GameOutcome Outcome { get; private set; }

        public GameOutcomeEventArgs(GameOutcome outcome)
        {
            this.Outcome = outcome;
        }
    }

    public class GameOutcome
    {
        public Player Winner { get; private set; }
        public List<Player> Losers { get; private set; } //noobs

        public GameOutcome(Player winner, List<Player> losers)
        {
            Winner = winner;
            Losers = losers;
        }
    }
}