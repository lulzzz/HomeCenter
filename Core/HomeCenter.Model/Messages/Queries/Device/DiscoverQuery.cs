﻿using HomeCenter.Model.Core;

namespace HomeCenter.Model.Messages.Queries.Device
{
    public class DiscoverQuery : Query
    {
        public static DiscoverQuery CreateQuery(BaseObject parent)
        {
            var query = new DiscoverQuery();
            foreach (var property in parent.GetProperties())
            {
                query[property.Key] = property.Value;
            }
            return query;
        }
    }
}