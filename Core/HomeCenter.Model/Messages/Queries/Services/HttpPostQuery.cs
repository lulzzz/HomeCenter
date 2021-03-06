﻿using HomeCenter.Model.Messages.Queries;
using System.Collections.Generic;
using System.Net;

namespace HomeCenter.Model.Messages.Queries.Services
{
    public abstract class HttpPostQuery : Query
    {
        public virtual string Address { get; set; }
        public virtual string Body { get; set; }

        public abstract object Parse(string rawHttpResult);

        public string RequestType { get; set; } = "POST";
        public string ContentType { get; set; }
        public CookieContainer Cookies { get; set; }
        public Dictionary<string, string> DefaultHeaders { get; set; } = new Dictionary<string, string>();
        public KeyValuePair<string, string> AuthorisationHeader { get; set; } = new KeyValuePair<string, string>("", "");
        public NetworkCredential Creditionals { get; set; }
        public bool IgnoreReturnStatus { get; set; }
    }
}