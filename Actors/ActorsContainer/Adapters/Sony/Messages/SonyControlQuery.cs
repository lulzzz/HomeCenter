﻿using HomeCenter.Model.Messages;
using HomeCenter.Model.Messages.Queries.Services;
using System;
using System.Net;

namespace HomeCenter.Adapters.Sony.Messages
{
    public class SonyControlQuery : HttpPostQuery, IFormatableMessage<SonyControlQuery>
    {
        public string Code { get; set; }
        public string AuthorisationKey { get; set; }

        public SonyControlQuery FormatMessage()
        {
            DefaultHeaders.Add("SOAPACTION", "\"urn:schemas-sony-com:service:IRCC:1#X_SendIRCC\"");

            Cookies = new CookieContainer();
            Cookies.Add(new Uri($"http://{Address}/sony/"), new Cookie("auth", AuthorisationKey, "/sony", Address));
            Address = $"http://{Address}/sony/IRCC";
            Body = $@"<?xml version=""1.0""?>
                    <s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"" s:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/"">
                        <s:Body>
                        <u:X_SendIRCC xmlns:u=""urn:schemas-sony-com:service:IRCC:1"">
                            <IRCCCode>{Code}</IRCCCode>
                        </u:X_SendIRCC>
                        </s:Body>
                    </s:Envelope>";

            return this;
        }

        public override object Parse(string rawHttpResult)
        {
            return rawHttpResult;
        }
    }
}