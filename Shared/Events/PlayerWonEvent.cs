﻿using MBC.Shared;
using System;
using System.Runtime.Serialization;

namespace MBC.Core.Events
{
    /// <summary>
    /// Provides information about a <see cref="Register"/> that had won a <see cref="GameLogic"/>.
    /// </summary>
    public class PlayerWonEvent : PlayerEvent
    {
        /// <summary>
        /// Constructs the event with the player that won.
        /// </summary>
        /// <param name="player"></param>
        public PlayerWonEvent(Player player)
            : base(player)
        {
        }
    }
}