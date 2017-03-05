using System;
using System.Collections;
using System.Collections.Generic;

namespace Introspection.Analysis.Models.Introspection.Analysis.Models.Collections
{
    [Serializable]
    public abstract class BaseCollection<T> : BaseClearHash, IEnumerable<T> // where T : IBaseElement
    {
        // ReSharper disable once InconsistentNaming
        // ReSharper disable once StaticMemberInGenericType
        private static readonly List<BaseClearHash> _list = new List<BaseClearHash>();
        // ReSharper disable once InconsistentNaming
        protected readonly List<T> _data = new List<T>();
        // ReSharper disable once InconsistentNaming
        private HashSet<T> _hash = new HashSet<T>();
        // ReSharper disable once InconsistentNaming
        //protected int[] _ids;

        internal BaseCollection()
        {
            _list.Add(this);
        }

        public int Count => _data.Count;

        public T this[int index] => _data[index];

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        public static void Dispose()
        {
            _list.ForEach(item => item.Clear());
            _list.Clear();
        }

        internal override void Clear()
        {
            _hash = null;
        }

        internal void Add(T item)
        {
            if (item != null && _hash.Add(item))
            {
                _data.Add(item);
            }
        }

        internal void AddRange(IEnumerable<T> items)
        {
            items.ForEach(Add);
        }

        internal void Remove(T item)
        {
            if (item != null && _hash.Remove(item))
            {
                _data.Remove(item);
            }
        }
    }
}