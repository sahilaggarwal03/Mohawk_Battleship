﻿using MBC.Core.Util;
using MBC.Shared;
using System;
using System.Diagnostics;
using System.Threading;
using MBC.Core.Events;
using System.Collections.Generic;

namespace MBC.Core
{
    /// <summary>
    /// Loads a <see cref="Controller"/> from <see cref="ControllerInformation"/>. Wraps the <see cref="Controller"/>
    /// to invoke its methods in a different thread and prevent hang-ups due to
    /// <see cref="Controller"/>s taking too long to complete method calls.
    /// </summary>
    public abstract class Player : ControllerRegister
    {

        public ShipList Ships { get; internal set; }
        public ShotList Shots { get; internal set; }
        public ShipList ShipsLeft { get; internal set; }
        public ShotList ShotsAgainst { get; internal set; }
        public Shot LastShot { get; internal set; }
        public List<PlayerEvent> events;
    }
}