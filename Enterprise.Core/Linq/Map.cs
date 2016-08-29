using System.Collections.Generic;

namespace Enterprise.Core.Linq
{
    internal sealed class Map<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> map;

        private bool haveNullKey;

        private TValue valueForNullKey;

        public Map(
            IEqualityComparer<TKey> comparer)
        {
            this.map = new Dictionary<TKey, TValue>(comparer);
        }

        public int Count
        {
            get
            {
                if (this.haveNullKey)
                {
                    return this.map.Count + 1;
                }

                return this.map.Count;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                if (ReferenceEquals(key, null))
                {
                    return this.valueForNullKey;
                }

                return this.map[key];
            }

            set
            {
                if (ReferenceEquals(key, null))
                {
                    this.haveNullKey = true;
                    this.valueForNullKey = value;
                }
                else
                {
                    this.map[key] = value;
                }
            }
        }

        public bool ContainsKey(
            TKey key)
        {
            if (ReferenceEquals(key, null))
            {
                return this.haveNullKey;
            }

            return this.map.ContainsKey(key);
        }

        public bool TryGetValue(
            TKey key, 
            out TValue value)
        {
            if (ReferenceEquals(key, null))
            {
                value = this.valueForNullKey;
                return this.haveNullKey;
            }

            return this.map.TryGetValue(key, out value);
        }
    }
}
