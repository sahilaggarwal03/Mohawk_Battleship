﻿using MBC.Shared;
using System;
using System.Runtime.Serialization;

namespace MBC.Core.Events
{
    /// <summary>
    /// Created during a match when a player has been assigned to a team.
    /// </summary>
    public class PlayerTeamAssignEvent : PlayerEvent
    {
        /// <summary>
        /// Constructs the event with the player and the team that they were assigned to.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="teamAssigned"></param>
        public PlayerTeamAssignEvent(Player player, Team teamAssigned)
            : base(player)
        {
            Team = teamAssigned;
        }

        /// <summary>
        /// Gets the team that the player has been assigned to.
        /// </summary>
        public Team Team
        {
            get;
            private set;
        }
    }
}