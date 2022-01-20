using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace GenericForHashTable
{
    public class HashTableWrapper<TKey, TValue> : IDictionary<TKey, TValue>,
        IEnumerable<KeyValuePair<TKey, TValue>>,
        ICollection<KeyValuePair<TKey, TValue>>
    {
        public void Add(TKey key, TValue value) => items_.Add(key, value);

        public void Add(KeyValuePair<TKey, TValue> keyValuePair) => items_.Add(keyValuePair.Key, keyValuePair.Value);
       
        public bool ContainsKey(TKey key) => items_.ContainsKey(key);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (DictionaryEntry? item in items_)
                yield return new KeyValuePair<TKey, TValue>((TKey)item.Value.Key, (TValue)item.Value.Value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Remove(TKey key)
        {
            if (ContainsKey(key)) {
                items_.Remove(key);
                return true;
            }

            return false;
        }

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            value = default(TValue);

            if (ContainsKey(key)) 
            {
                value = (TValue)items_[key];
                return true;
            }

            return false;
        }

        public void Clear()
        {
            items_.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return items_.Contains(item.Key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            items_.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (Contains(item)) {
                items_.Remove((TKey)item.Key);
                return true;
            }

           return false;
        }

        public ICollection<TKey> Keys {

            get {
                var buff = new List<TKey>();
                foreach (var item in items_.Keys)
                    buff.Add((TKey)item);

                return buff;
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                var buff = new List<TValue>();
                foreach (var item in items_.Values)
                    buff.Add((TValue)item);

                return buff;
            }
        }

        public int Count => items_.Count;

        public bool IsReadOnly => items_.IsReadOnly;

        public TValue this[TKey key] { get => (TValue)items_[key]; set => items_[key] = value; }
        
        private Hashtable items_ = new Hashtable();
    }
}