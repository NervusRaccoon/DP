using StackExchange.Redis;
using System.Collections.Generic;

namespace Vaculator.Storage
{
    public class RedisStorage: IStorage
    {
        private IConnectionMultiplexer _connection;
        private IDatabase _db;

        public RedisStorage()
        {
            _connection = ConnectionMultiplexer.Connect("localhost");
            _db = _connection.GetDatabase();
        }

        public void Store(string key, string value)
        {
            _db.StringSet(key, value);
        }

        public string Load(string key)
        {
            return _db.StringGet(key);
        }

        public List<string> GetValues(string prefix)
        {
            var server = _connection.GetServer("localhost", 6379);
            List<string> keys = new List<string>();
            foreach (var key in server.Keys(pattern: prefix + "*"))
            {
                keys.Add(key);
            }
            return keys;
        }
    }
}