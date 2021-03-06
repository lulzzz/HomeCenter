﻿using HomeCenter.Model.Capabilities.Constants;
using HomeCenter.Model.Messages.Commands.Device;

namespace HomeCenter.Model.Capabilities
{
    public class InputSourceState : State
    {
        public static string StateName { get; } = nameof(InputSourceState);

        public InputSourceState(string ReadWriteMode = default) : base(ReadWriteMode)
        {
            this[StateProperties.StateName] = nameof(InputSourceState);
            this[StateProperties.CapabilityName] = Constants.Capabilities.InputController;
            SetPropertyList(StateProperties.SupportedCommands, nameof(InputSetCommand));
        }
    }
}