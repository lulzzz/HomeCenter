﻿using HomeCenter.Adapters.Kodi.Messages.JsonModels;
using HomeCenter.Model.Messages;
using HomeCenter.Model.Messages.Queries.Services;
using Newtonsoft.Json;

namespace HomeCenter.Adapters.Kodi.Messages
{
    //https://github.com/FabienLavocat/kodi-remote/tree/master/src/KodiRemote.Core
    //https://github.com/akshay2000/XBMCRemoteRT/blob/master/XBMCRemoteRT/XBMCRemoteRT.Shared/RPCWrappers/Player.cs
    //http://kodi.wiki/view/JSON-RPC_API/Examples
    //http://kodi.wiki/view/JSON-RPC_API/v8#Notifications_2

    public class KodiCommand : HttpPostQuery, IFormatableMessage<KodiCommand>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Method { get; set; }
        public int Port { get; set; }
        public object Parameters { get; set; }

        public KodiCommand()
        {
            ContentType = "application/json-rpc";
        }

        public KodiCommand FormatMessage()
        {
            Creditionals = new System.Net.NetworkCredential(UserName, Password);
            Address = $"http://{Address}:{Port}/jsonrpc";

            var jsonRpcRequest = new JsonRpcRequest
            {
                Method = Method,
                Parameters = Parameters
            };

            Body = JsonConvert.SerializeObject(jsonRpcRequest);

            return this;
        }

        public override object Parse(string rawHttpResult)
        {
            var result = JsonConvert.DeserializeObject<JsonRpcResponse>(rawHttpResult);

            return result;
        }
    }
}