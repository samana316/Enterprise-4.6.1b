using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Enterprise.Core.Linq
{
    internal sealed class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
    {
        private readonly TKey key;

        private readonly IEnumerable<TElement> elements;

        public Grouping(
            TKey key, 
            IEnumerable<TElement> elements)
        {
            this.key = key;
            this.elements = elements;
        }

        public TKey Key { get { return key; } }

        public IEnumerator<TElement> GetEnumerator()
        {
            return elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
