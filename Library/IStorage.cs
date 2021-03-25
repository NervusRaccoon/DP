using System.Collections.Generic;

namespace Library
{
    public interface IStorage
    {
        void Store(string key, string value);
        string Load(string key);
        List<string> GetValues(string prefix);
    }
}