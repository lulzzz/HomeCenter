﻿using HomeCenter.Broker;
using HomeCenter.Model.Messages.Events.Service;
using HomeCenter.Services.Bootstrapper;
using SimpleInjector;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace HomeCenter.Runner
{
    public class WirehomeRunner : Runner
    {
        private readonly List<Runner> _runners = new List<Runner>();
        private Bootstrapper _bootstrapper;

        public WirehomeRunner() : base(nameof(WirehomeRunner))
        {
            _runners = new List<Runner>
            {
                new DenonRunner("DenonComponent"),
                new KodiRunner("KodiComponent"),
                new PcRunner("PcComponent"),
                new SamsungRunner("SamsungComponent"),
                new SonyRunner("SonyComponent"),
                new RemoteSocketRunner("RemoteLamp3"),
                new RaspberryRunner("RaspberryLed"),
                new HSRel8Runner("ButtonLamp")
            };

            _tasks = _runners.Select(x => x.Uid).ToArray();
        }

        public override async Task Run()
        {
            var tcs = new TaskCompletionSource<bool>();
            var container = new Container();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                _bootstrapper = new Bootstrapper(container);
            }
            else
            {
                _bootstrapper = new WirehomeBootstrapper(container);
            }

            var controller = await _bootstrapper.BuildController().ConfigureAwait(false);

            foreach (var runner in _runners)
            {
                runner.SetContainer(container);
            }

            var eventAggregator = container.GetInstance<IEventAggregator>();
            eventAggregator.Subscribe<SystemStartedEvent>(async message =>
            {
                await Task.Delay(500).ConfigureAwait(false);
                tcs.SetResult(true);
            });

            await tcs.Task.ConfigureAwait(false);
            await base.Run().ConfigureAwait(false);
        }

        public override Task RunTask(int taskId)
        {
            return _runners[taskId].Run();
        }
    }
}