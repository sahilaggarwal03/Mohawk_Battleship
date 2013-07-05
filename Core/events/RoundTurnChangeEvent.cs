﻿using MBC.Core.Rounds;
using MBC.Shared;

namespace MBC.Core.Events
{
    public class RoundTurnChangeEvent : RoundEvent
    {
        public RoundTurnChangeEvent(Round rnd, ControllerID last, ControllerID next) :
            base(rnd)
        {
            NextTurn = next;
            PreviousTurn = last;
        }

        public ControllerID NextTurn
        {
            get;
            private set;
        }

        public ControllerID PreviousTurn
        {
            get;
            private set;
        }

        internal override void ProcBackward()
        {
            Round.CurrentTurn = PreviousTurn;
        }

        internal override void ProcForward()
        {
            Round.CurrentTurn = NextTurn;
        }
    }
}