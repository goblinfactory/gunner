using System.Collections.Generic;

namespace Gunner.Engine
{
    public static class DictionaryHelper
    {
        public static void Increment(this Dictionary<int, int> dictionary, int key, int increment = 1)
        {
            int i;
            dictionary.TryGetValue(key, out i);
            dictionary[key] = i + increment;
        }
    }
}