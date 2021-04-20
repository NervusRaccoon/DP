using StackExchange.Redis;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Logging;

namespace Library
{
    public class RedisStorage: IStorage
    {
        private readonly ILogger<RedisStorage> _logger;
        private IConnectionMultiplexer _connection;
        private IDatabase _db;
        private string _host = "localhost";
        private readonly string _ru = Environment.GetEnvironmentVariable("DB_RUS", EnvironmentVariableTarget.User);
        private readonly string _eu = Environment.GetEnvironmentVariable("DB_EU", EnvironmentVariableTarget.User);
        private readonly string _other = Environment.GetEnvironmentVariable("DB_OTHER", EnvironmentVariableTarget.User);
        private readonly Dictionary<string, IDatabase> _shardConnections;

        public RedisStorage(ILogger<RedisStorage> logger)
        {
            _logger = logger;
            _connection = ConnectionMultiplexer.Connect(_host);
            _db = _connection.GetDatabase();
            _shardConnections = new Dictionary<string, IDatabase>();
            _shardConnections.Add(Constants.RuShardKey, ConnectionMultiplexer.Connect(_ru).GetDatabase());
            _shardConnections.Add(Constants.EuShardKey, ConnectionMultiplexer.Connect(_eu).GetDatabase());
            _shardConnections.Add(Constants.OtherShardKey, ConnectionMultiplexer.Connect(_other).GetDatabase());
        }
        public void StoreShardKey(string id, string shardKey)
        {
            _db.StringSet(id, shardKey);
        }
        public string GetShardKey(string id)
        {   
            return _db.StringGet(id);
        }
        public void Store(string key, string shardKey, string value)
        {
            IDatabase shardConnection = _shardConnections[shardKey];
            shardConnection.StringSet(key, value);
        }
        public string Load(string key, string shardKey)
        {
            IDatabase shardConnection = _shardConnections[shardKey];
            return shardConnection.StringGet(key);
        }
        public void StoreToSet(string setId, string shardKey, string value)
        {
            IDatabase shardConnection = _shardConnections[shardKey];
            shardConnection.SetAdd(setId, value);
        }
        public bool IsValueExist(string setId, string value)
        {
            foreach (var connection in _shardConnections)
            {
                if (connection.Value.SetContains(setId, value))
                {
                    return true;
                }
            }
            return false;
        }
    }
}