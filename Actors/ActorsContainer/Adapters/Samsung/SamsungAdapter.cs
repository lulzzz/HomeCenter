﻿using HomeCenter.Adapters.Samsung.Messages;
using HomeCenter.CodeGeneration;
using HomeCenter.Model.Adapters;
using HomeCenter.Model.Capabilities;
using HomeCenter.Model.Capabilities.Constants;
using HomeCenter.Model.Exceptions;
using HomeCenter.Model.Messages;
using HomeCenter.Model.Messages.Commands.Device;
using HomeCenter.Model.Messages.Queries.Device;
using Proto;
using System.Threading.Tasks;

namespace HomeCenter.Adapters.Samsung
{
    [ProxyCodeGenerator]
    public abstract class SamsungAdapter : Adapter
    {
        private const uint TURN_ON_IR_CODE = 3772793023;
        private string _hostname;
        private bool _powerState;
        private string _input;
        private string _infraredAdaperName;

        protected override async Task OnStarted(IContext context)
        {
            await base.OnStarted(context).ConfigureAwait(false);

            _hostname = AsString(MessageProperties.Hostname);
            _infraredAdaperName = AsString(MessageProperties.InfraredAdapter);
        }

        protected DiscoveryResponse Discover(DiscoverQuery message)
        {
            return new DiscoveryResponse(RequierdProperties(), new PowerState(ReadWriteMode.Write),
                                                               new VolumeState(ReadWriteMode.Write),
                                                               new MuteState(ReadWriteMode.Write),
                                                               new InputSourceState(ReadWriteMode.Write)
                                          );
        }

        private SamsungControlCommand GetCommand(string code)
        {
            return new SamsungControlCommand
            {
                Address = _hostname,
                Code = code
            };
        }

        protected Task Handle(TurnOnCommand message)
        {
            MessageBroker.Send(SendCodeCommand.Create(TURN_ON_IR_CODE), _infraredAdaperName);
            return Task.CompletedTask;
        }

        protected async Task Handle(TurnOffCommand message)
        {
            var cmd = GetCommand("KEY_POWEROFF");
            await MessageBroker.SendToService(cmd).ConfigureAwait(false);

            _powerState = await UpdateState(PowerState.StateName, _powerState, false).ConfigureAwait(false);
        }

        protected Task Handle(VolumeUpCommand command)
        {
            var cmd = GetCommand("KEY_VOLUP");
            return MessageBroker.SendToService(cmd);
        }

        protected Task Handle(VolumeDownCommand command)
        {
            var cmd = GetCommand("KEY_VOLDOWN");
            return MessageBroker.SendToService(cmd);
        }

        protected Task Handle(MuteCommand message)
        {
            var cmd = GetCommand("KEY_MUTE");
            return MessageBroker.SendToService(cmd);
        }

        protected async Task Handle(InputSetCommand message)
        {
            var inputName = message.AsString(MessageProperties.InputSource);

            var source = "";
            if (inputName == "HDMI")
            {
                source = "KEY_HDMI";
            }
            else if (inputName == "AV")
            {
                source = "KEY_AV1";
            }
            else if (inputName == "COMPONENT")
            {
                source = "KEY_COMPONENT1";
            }
            else if (inputName == "TV")
            {
                source = "KEY_TV";
            }

            if (source?.Length == 0) throw new UnsupportedPropertyStateException($"Input {inputName} was not found on Samsung available device input sources");

            var cmd = GetCommand(source);
            await MessageBroker.SendToService(cmd).ConfigureAwait(false);

            _input = await UpdateState(InputSourceState.StateName, _input, inputName).ConfigureAwait(false);
        }
    }
}