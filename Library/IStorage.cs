using System.Collections.Generic;

namespace Library
{
    public interface IStorage
    {
        void StoreShardKey(string id, string shardKey);
        string GetShardKey(string id);
        void Store(string key, string shardKey, string value);
        string Load(string key, string shardKey);  
        void StoreToSet(string setId, string shardKey, string value);
        bool IsValueExist(string setId, string value);
    }
}