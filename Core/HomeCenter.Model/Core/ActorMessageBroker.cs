﻿using ConcurrentCollections;
using HomeCenter.Broker;
using HomeCenter.Model.Contracts;
using HomeCenter.Model.Messages;
using HomeCenter.Model.Messages.Commands;
using HomeCenter.Model.Messages.Events;
using HomeCenter.Model.Messages.Queries;
using HomeCenter.Model.Messages.Queries.Services;
using Newtonsoft.Json;
using Proto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomeCenter.Model.Core
{
    public class ActorMessageBroker : IActorMessageBroker
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IActorFactory _actorFactory;

        private readonly ConcurrentHashSet<SubscriptionCache> _subscriptionCahce = new ConcurrentHashSet<SubscriptionCache>();

        public ActorMessageBroker(IEventAggregator eventAggregator, IActorFactory actorFactory)
        {
            _eventAggregator = eventAggregator;
            _actorFactory = actorFactory;
        }

        public SubscriptionToken SubscribeForMessage<T>(PID subscriber, RoutingFilter filter = null) where T : ActorMessage
        {
            var sub = GetSubscription<T>(subscriber);

            if (!_subscriptionCahce.Add(sub)) return SubscriptionToken.Empty;

            return _eventAggregator.Subscribe<T>(message => _actorFactory.Context.Send(subscriber, message.Message), filter);
        }

        public SubscriptionToken SubscribeForQuery<T, R>(PID subscriber, RoutingFilter filter = null) where T : Query
        {
            var sub = GetSubscription<T>(subscriber);

            if (!_subscriptionCahce.Add(sub)) return SubscriptionToken.Empty;

            return _eventAggregator.SubscribeForAsyncResult<T>(async message => await _actorFactory.Context.RequestAsync<R>(subscriber, message.Message).ConfigureAwait(false), filter);
        }

        private SubscriptionCache GetSubscription<T>(PID subscriber)
        {
            var rootActor = _actorFactory.GetRootActor(subscriber);
            var sub = new SubscriptionCache(rootActor, typeof(T));
            return sub;
        }

        public Task<R> QueryService<T, R>(T query, RoutingFilter filter = null) where T : Query
                                                                                where R : class
        {
            query = FormatMessage(query);

            return _eventAggregator.QueryAsync<T, R>(query, filter);
        }

        public async Task<R> QueryJsonService<T, R>(T query, RoutingFilter filter = null) where T : Query
                                                                                          where R : class
        {
            query = FormatMessage(query);

            var json = await _eventAggregator.QueryAsync<T, string>(query, filter).ConfigureAwait(false);
            var result = JsonConvert.DeserializeObject<R>(json);
            return result;
        }

        public Task SendToService<T>(T command, RoutingFilter filter = null) where T : Command
        {
            command = FormatMessage(command);

            return _eventAggregator.Publish(command, filter);
        }

        public async Task<bool> QueryServiceWithVerify<T, Q, R>(T query, R expectedResult, RoutingFilter filter = null) where T : Query, IMessageResult<Q, R>
                                                                                                                        where Q : class
                                                                                                                        where R : class
        {
            var result = await QueryService<T, Q>(query, filter).ConfigureAwait(false);
            return query.Verify(result, expectedResult);
        }

        public Task PublisEvent<T>(T message, IEnumerable<string> routerAttributes = null) where T : Event
        {
            return _eventAggregator.Publish(message, message.GetRoutingFilter(routerAttributes));
        }

        public void Send(object message, PID destination)
        {
            _actorFactory.Context.Send(destination, message);
        }

        public void Send(object message, string uid, string address = null)
        {
            var pid = _actorFactory.GetActor(uid, address);
            _actorFactory.Context.Send(pid, message);
        }

        public Task<R> Request<T, R>(T message, PID actor) where T : ActorMessage => _actorFactory.Context.RequestAsync<R>(actor, message);

        public Task<R> Request<T, R>(T message, string uid) where T : ActorMessage
        {
            var pid = _actorFactory.GetActor(uid);

            return Request<T, R>(message, pid);
        }

        private static T FormatMessage<T>(T query) where T : ActorMessage
        {
            if (query is IFormatableMessage<T> formatableMessage)
            {
                query = formatableMessage.FormatMessage();
            }

            return query;
        }
    }
}