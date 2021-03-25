using StackExchange.Redis;
using System.Collections.Generic;

namespace Library
{
    public class RedisStorage: IStorage
    {
        private IConnectionMultiplexer _connection;
        private IDatabase _db;
        private string _host = "localhost";
        private int _port = 6379;


        public RedisStorage()
        {
            _connection = ConnectionMultiplexer.Connect(_host);
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
            var server = _connection.GetServer(_host, _port);
            List<string> keys = new List<string>();
            foreach (var key in server.Keys(pattern: prefix + "*"))
            {
                keys.Add(key);
            }
            return keys;
        }
    }
}