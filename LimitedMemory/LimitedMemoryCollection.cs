namespace LimitedMemory
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class LimitedMemoryCollection<TK, TV> : ILimitedMemoryCollection<TK, TV>
    {
        private readonly Dictionary<TK, TV> storage;
        private readonly Dictionary<TK, LinkedListNode<Pair<TK, TV>>> links;
        private readonly LinkedList<Pair<TK, TV>> order;

        private int capacity;

        public LimitedMemoryCollection(int capacity)
        {
            this.Capacity = capacity;

            this.storage = new Dictionary<TK, TV>();
            this.links = new Dictionary<TK, LinkedListNode<Pair<TK, TV>>>();
            this.order = new LinkedList<Pair<TK, TV>>();
        }

        public IEnumerator<Pair<TK, TV>> GetEnumerator()
        {
            foreach (var pair in this.order)
            {
                yield return pair;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public int Capacity
        {
            get
            {
                return this.capacity;
            }

            private set
            {
                if (value < 4 || value > 200000)
                {
                    throw new InvalidOperationException();
                }

                this.capacity = value;
            }
        }

        public int Count
        {
            get
            {
                return this.storage.Count;
            }
        }

        public void Set(TK key, TV value)
        {
            if (!this.storage.ContainsKey(key))
            {
                if (this.Count >= this.capacity)
                {
                    //  Remove oldest
                    var elementToRemove = this.order.Last;
                    this.links.Remove(elementToRemove.Value.Key);
                    this.storage.Remove(elementToRemove.Value.Key);
                    this.order.RemoveLast();
                }

                // Add new
                this.storage[key] = value;
                var pair = new Pair<TK, TV>
                {
                    Key = key,
                    Value = value
                };
                
                this.links[key] = this.order.AddFirst(pair);
            }
            else
            {
                // Update existing
                this.storage[key] = value;

                this.ReorderElement(key);

                this.links[key].Value.Value = value;
            }
        }

        public TV Get(TK key)
        {
            if (!this.storage.ContainsKey(key))
            {
                throw new KeyNotFoundException();
            }

            this.ReorderElement(key);

            return this.storage[key];
        }

        private void ReorderElement(TK key)
        {
            var elementToUpdate = this.links[key];
            this.order.Remove(elementToUpdate);
            this.order.AddFirst(elementToUpdate);
        }
    }
}
