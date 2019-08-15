using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("RSM", "RustServerManager", "1.0.0")]
    [Description("Allows RSM to have more control over the server.")]
    public class RSM : RustPlugin
    {
        private void Init()
        {
            cmd.AddConsoleCommand("joinlist", this, nameof(GetJoiningPlayers));
            cmd.AddConsoleCommand("queuelist", this, nameof(GetQueuedPlayers));
            cmd.AddConsoleCommand("removeconnection", this, nameof(RemovePlayerFromQueue));
        }

        private void RemovePlayerFromQueue(ConsoleSystem.Arg arg)
        {
            if (arg.IsAdmin == false)
            {
                return;
            }

            if (arg.Args == null || arg.Args?.Length == 0)
            {
                return;
            }

            var targetID = 0ul;
            if (ulong.TryParse(arg.Args[0], out targetID) == false)
            {
                Puts($"removeconnection steamID");
                return;
            }

            var connection = ServerMgr.Instance.connectionQueue.queue.FirstOrDefault(x => x.userid == targetID);
            if (connection == null)
            {
                Puts($"Player {targetID} not exists in queue!");
                return;
            }
            
            ServerMgr.Instance.connectionQueue.RemoveConnection(connection);
            Puts($"Connection {connection.username}[{connection.userid}] was removed from queue");
        }

        private void GetJoiningPlayers(ConsoleSystem.Arg arg)
        {
            if (arg.IsAdmin == false)
            {
                return;
            }
            
            var players = ServerMgr.Instance.connectionQueue.joining;
            var list = new List<PlayerInfo>();

            foreach (var player in players)
            {
                list.Add(new PlayerInfo
                {
                    authLevel = player.authLevel,
                    userid = player.userid,
                    ownerid = player.ownerid,
                    username = player.username,
                    os = player.os,
                    connectionTime = player.connectionTime,
                    ipaddress = player.ipaddress
                });
            }
            
            Debug.Log(JsonConvert.SerializeObject(list));
        }

        private void GetQueuedPlayers(ConsoleSystem.Arg arg)
        {
            if (arg.IsAdmin == false)
            {
                return;
            }
            
            var players = ServerMgr.Instance.connectionQueue.queue;
            var list = new List<PlayerInfo>();
            
            foreach (var player in players)
            {
                list.Add(new PlayerInfo
                {
                    authLevel = player.authLevel,
                    userid = player.userid,
                    ownerid = player.ownerid,
                    username = player.username,
                    os = player.os,
                    connectionTime = player.connectionTime,
                    ipaddress = player.ipaddress
                });
            }
            
            Debug.Log(JsonConvert.SerializeObject(list));
        }

        private class PlayerInfo
        {
            public uint authLevel;
            public ulong userid;
            public ulong ownerid;
            public string username;
            public string os;
            public double connectionTime;
            public string ipaddress;
        }
    }
}