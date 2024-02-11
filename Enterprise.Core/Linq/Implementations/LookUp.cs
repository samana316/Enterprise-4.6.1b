using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Enterprise.Core.Linq
{
    internal sealed class Lookup<TKey, TElement> : 
        ILookup<TKey, TElement>, IReadOnlyCollection<IGrouping<TKey, TElement>>
    {
        private readonly Map<TKey, IList<TElement>> map;

        private readonly IList<TKey> keys;

        public Lookup(
            IEqualityComparer<TKey> comparer)
        {
            map = new Map<TKey, IList<TElement>>(comparer);
            keys = new List<TKey>();
        }

        public void Add(
            TKey key, 
            TElement element)
        {
            IList<TElement> list;
            if (!map.TryGetValue(key, out list))
            {
                list = new List<TElement>();
                map[key] = list;
                keys.Add(key);
            }
            list.Add(element);
        }

        public int Count
        {
            get { return map.Count; }
        }

        public IEnumerable<TElement> this[TKey key]
        {
            get
            {
                IList<TElement> list;
                if (!map.TryGetValue(key, out list))
                {
                    return Enumerable.Empty<TElement>();
                }
                return list.Select(x => x);
            }
        }

        public bool Contains(
            TKey key)
        {
            return map.ContainsKey(key);
        }

        public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator()
        {
            return keys.Select(key => new Grouping<TKey, TElement>(key, map[key]))
                       .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
