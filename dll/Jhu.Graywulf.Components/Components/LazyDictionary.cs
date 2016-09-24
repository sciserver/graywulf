using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Components
{
    /// <summary>
    /// Implements a dictionary that can load items on demand by firing
    /// and event.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [SerializableAttribute]
    public class LazyDictionary<TKey, TValue>
        : IDictionary<TKey, TValue>,
        ICollection<KeyValuePair<TKey, TValue>>,
        IEnumerable<KeyValuePair<TKey, TValue>>,
        IDictionary,
        ICollection,
        IEnumerable
    {
        private ConcurrentDictionary<TKey, TValue> localStore;
        private bool isAllLoaded;

        public event EventHandler<LazyItemLoadingEventArgs<TKey, TValue>> ItemLoading;
        public event EventHandler<AllItemsLoadingEventArgs<TKey, TValue>> AllItemsLoading;
        public event EventHandler<LazyItemAddedEventArgs<TKey, TValue>> ItemAdded;
        public event EventHandler<LazyItemUpdatedEventArgs<TKey, TValue>> ItemUpdated;
        public event EventHandler<LazyItemRemovedEventArgs<TKey, TValue>> ItemRemoved;

        public bool IsAllLoaded
        {
            get { return isAllLoaded; }
        }

        #region Wrapped properties

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// No lazy loading performed.
        /// </remarks>
        public int Count
        {
            get { return localStore.Count; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// No lazy loading performed.
        /// </remarks>
        public bool IsEmpty
        {
            get { return localStore.IsEmpty; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// No lazy loading performed.
        /// </remarks>
        public ICollection<TKey> Keys
        {
            get { return localStore.Keys; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// No lazy loading performed.
        /// </remarks>
        public ICollection<TValue> Values
        {
            get { return localStore.Values; }
        }

        #endregion
        #region Constructors and initializers

        public LazyDictionary()
        {
            InitializeMembers();
            localStore = new ConcurrentDictionary<TKey, TValue>();
        }

        public LazyDictionary(IEqualityComparer<TKey> comparer)
        {
            InitializeMembers();
            localStore = new ConcurrentDictionary<TKey, TValue>(comparer);
        }

        private void InitializeMembers()
        {
            this.isAllLoaded = false;

            ItemLoading = null;
            ItemAdded = null;
            ItemUpdated = null;
            ItemRemoved = null;
        }

        #endregion
        #region Wrapped functions

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return localStore.GetEnumerator();
        }

        public KeyValuePair<TKey, TValue>[] ToArray()
        {
            return localStore.ToArray();
        }

        #endregion

        public TValue this[TKey key]
        {
            get
            {
                if (key == null)
                {
                    throw new ArgumentNullException("key");
                }

                TValue value;
                if (!TryGetValueInternal(key, out value))
                {
                    throw new KeyNotFoundException();
                }

                return value;
            }
            set
            {
                if (key == null)
                {
                    throw new ArgumentNullException("key");
                }

                TValue res;
                TryAddInternal(key, value, true, out res);
            }
        }

        public void Clear()
        {
            var values = localStore.ToArray();
            localStore.Clear();

            foreach (var i in values)
            {
                OnItemRemoved(i.Key, i.Value);
            }

            isAllLoaded = false;
        }

        public bool ContainsKey(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            TValue value;
            return TryGetValueInternal(key, out value);
        }

        public TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            if (addValueFactory == null)
            {
                throw new ArgumentNullException("addValueFactory");
            }

            if (updateValueFactory == null)
            {
                throw new ArgumentNullException("updateValueFactory");
            }

            TValue oldvalue = default(TValue);
            TValue newvalue = default(TValue);
            bool updated = false;

            TValue res = localStore.AddOrUpdate(key,
                (k) =>
                {
                    newvalue = addValueFactory(k);
                    return newvalue;
                },
                (k, ov) =>
                {
                    newvalue = updateValueFactory(k, ov);
                    oldvalue = ov;
                    updated = true;
                    return newvalue;
                });

            if (updated)
            {
                OnItemUpdated(key, newvalue, oldvalue);
            }
            else
            {
                OnItemAdded(key, newvalue);
            }

            return res;
        }

        public TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
        {
            return AddOrUpdate(key, _ => addValue, updateValueFactory);
        }

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            if (valueFactory == null)
            {
                throw new ArgumentNullException("addValueFactory");
            }

            TValue res;
            if (!TryGetValueInternal(key, out res))
            {
                TryAddInternal(key, valueFactory(key), false, out res);
            }

            return res;
        }

        public TValue GetOrAdd(TKey key, TValue value)
        {
            return GetOrAdd(key, _ => value);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            return TryGetValueInternal(key, out value);
        }

        public bool TryAdd(TKey key, TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            TValue res;
            return TryAddInternal(key, value, false, out res);
        }

        public bool TryUpdate(TKey key, TValue newValue, TValue comparisonValue)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            return TryUpdateInternal(key, newValue, comparisonValue);
        }

        public bool TryRemove(TKey key, out TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            return TryRemoveInternal(key, out value);
        }

        public bool LoadAll(bool forceReload)
        {
            if (forceReload || !isAllLoaded)
            {
                return TryLoadAllInternal();
            }
            else
            {
                return true;
            }
        }

        #region Internal logic

        private bool TryGetValueInternal(TKey key, out TValue value)
        {
            if (!localStore.TryGetValue(key, out value))
            {
                if (!TryLoadValueInternal(key, out value))
                {
                    return false;
                }
            }

            return true;
        }

        private bool TryLoadValueInternal(TKey key, out TValue value)
        {
            if (OnItemLoading(key, out value))
            {
                // Try to add to the collection but it's not a problem
                // if it cannot be added, it just means that the item with
                // the same key already exists (has been added very recently)

                TryAddInternal(key, value, false, out value);

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Raises an event to load all items.
        /// </summary>
        /// <returns></returns>
        private bool TryLoadAllInternal()
        {
            IEnumerable<KeyValuePair<TKey, TValue>> items;
            
            if (OnAllItemsLoading(out items) && items != null)
            {
                foreach (var item in items)
                {
                    this[item.Key] = item.Value;
                }

                isAllLoaded = true;

                return true;
            }

            return false;
        }

        private bool TryAddInternal(TKey key, TValue newValue, bool updateIfExists, out TValue resultingValue)
        {
            if (updateIfExists)
            {
                TValue oldvalue = default(TValue);
                bool updated = false;

                resultingValue = localStore.AddOrUpdate(key,
                    (k) =>
                    {
                        return newValue;
                    },
                    (k, ov) =>
                    {
                        oldvalue = ov;
                        updated = true;
                        return newValue;
                    });

                if (updated)
                {
                    OnItemUpdated(key, newValue, oldvalue);
                }
                else
                {
                    OnItemAdded(key, newValue);
                }

                return true;
            }
            else
            {
                resultingValue = newValue;

                if (localStore.TryAdd(key, newValue))
                {
                    OnItemAdded(key, newValue);
                    return true;
                }
            }

            return false;
        }

        private bool TryUpdateInternal(TKey key, TValue newValue, TValue comparisionValue)
        {
            if (localStore.TryUpdate(key, newValue, comparisionValue))
            {
                OnItemUpdated(key, newValue, comparisionValue);
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool TryRemoveInternal(TKey key, out TValue value)
        {
            if (!localStore.TryRemove(key, out value))
            {
                return false;
            }

            OnItemRemoved(key, value);
            return true;
        }

        #endregion
        #region Events

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <remarks>
        /// Called internally when a key is not found and loading it is necessary.
        /// </remarks>
        protected virtual bool OnItemLoading(TKey key, out TValue value)
        {
            if (ItemLoading != null)
            {
                var e = new LazyItemLoadingEventArgs<TKey, TValue>()
                {
                    Key = key
                };

                // Call event handler
                ItemLoading(this, e);

                if (e.IsCancelled)
                {
                    throw new OperationCanceledException();
                }

                if (e.IsFound)
                {
                    value = e.Value;
                    return true;
                }
            }

            value = default(TValue);
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        /// <remarks>
        /// Called internally when all items are needed.
        /// </remarks>
        protected virtual bool OnAllItemsLoading(out IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            if (!isAllLoaded && AllItemsLoading != null)
            {
                var e = new AllItemsLoadingEventArgs<TKey, TValue>();

                // Call event handler
                AllItemsLoading(this, e);

                if (e.IsCancelled)
                {
                    throw new OperationCanceledException();
                }

                if (e.Items != null)
                {
                    items = e.Items;
                    return true;
                }
            }

            items = null;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <remarks>
        /// Called internally when a new key is added to the collection.
        /// </remarks>
        protected virtual void OnItemAdded(TKey key, TValue value)
        {
            if (ItemAdded != null)
            {
                var e = new LazyItemAddedEventArgs<TKey, TValue>()
                {
                    Key = key,
                    Value = value,
                };

                ItemAdded(this, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <remarks>
        /// Called internally when an item has been updated.
        /// </remarks>
        protected virtual void OnItemUpdated(TKey key, TValue newValue, TValue oldValue)
        {
            if (ItemUpdated != null)
            {
                var e = new LazyItemUpdatedEventArgs<TKey, TValue>()
                {
                    Key = key,
                    OldValue = oldValue,
                    NewValue = newValue,
                };

                ItemUpdated(this, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <remarks>
        /// Called internally when an item has been removed.
        /// </remarks>
        private void OnItemRemoved(TKey key, TValue value)
        {
            if (ItemRemoved != null)
            {
                var e = new LazyItemRemovedEventArgs<TKey, TValue>()
                {
                    Key = key,
                    Value = value,
                };

                ItemRemoved(this, e);
            }
        }

        #endregion
        #region IDictionary<TKey,TValue> Members

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            if (!TryAdd(key, value))
            {
                throw new ArgumentException();
            }
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            TValue value;
            if (!TryRemove(key, out value))
            {
                throw new KeyNotFoundException();
            }

            return true;
        }

        #endregion
        #region ICollection<KeyValuePair<TKey,TValue>> Members

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            ((IDictionary<TKey, TValue>)this).Add(item.Key, item.Value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            TValue value;
            if (TryGetValue(item.Key, out value))
            {
                return EqualityComparer<TValue>.Default.Equals(value, item.Value);
            }
            else
            {
                return false;
            }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            var sa = ToArray();
            Array.Copy(sa, 0, array, arrayIndex, sa.Length);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            return ((IDictionary<TKey, TValue>)this).Remove(item.Key);
        }

        #endregion
        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
        #region IDictionary Members

        void IDictionary.Add(object key, object value)
        {
            if (!(key is TKey))
            {
                throw new ArgumentException();
            }

            if (!(value is TValue))
            {
                throw new ArgumentException();
            }

            ((IDictionary<TKey, TValue>)this).Add((TKey)key, (TValue)value);
        }

        bool IDictionary.Contains(object key)
        {
            if (!(key is TKey))
            {
                throw new ArgumentException();
            }

            return ContainsKey((TKey)key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary)this).GetEnumerator();
        }

        bool IDictionary.IsFixedSize
        {
            get { return false; }
        }

        bool IDictionary.IsReadOnly
        {
            get { return false; }
        }

        ICollection IDictionary.Keys
        {
            get { return (ICollection)this.Keys; }
        }

        void IDictionary.Remove(object key)
        {
            if (!(key is TKey))
            {
                throw new ArgumentException();
            }

            ((IDictionary<TKey, TValue>)this).Remove((TKey)key);
        }

        ICollection IDictionary.Values
        {
            get { return (ICollection)this.Values; }
        }

        object IDictionary.this[object key]
        {
            get
            {
                if (!(key is TKey))
                {
                    throw new ArgumentException();
                }

                return this[(TKey)key];
            }
            set
            {
                if (!(key is TKey))
                {
                    throw new ArgumentException();
                }

                if (!(value is TValue))
                {
                    throw new ArgumentException();
                }

                this[(TKey)key] = (TValue)value;
            }
        }

        #endregion
        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            Array a = ToArray();
            Array.Copy(a, 0, array, index, a.Length);
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { throw new NotSupportedException(); }
        }

        #endregion
    }
}
