using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ExtraSerialization
{
    public abstract class DrawableDictionary
    {

    }

    [Serializable()]
    public class SerializableDictionaryBase<TKey, TValue> : DrawableDictionary, IDictionary<TKey, TValue>, UnityEngine.ISerializationCallbackReceiver
    {

        #region Fields

        [System.NonSerialized()]
        private Dictionary<TKey, TValue> _dict;
        [System.NonSerialized()]
        private IEqualityComparer<TKey> _comparer;

        #endregion

        #region CONSTRUCTOR

        public SerializableDictionaryBase()
        {

        }

        public SerializableDictionaryBase(IEqualityComparer<TKey> comparer)
        {
            _comparer = comparer;
        }

        #endregion

        #region Properties

        public IEqualityComparer<TKey> Comparer
        {
            get { return _comparer; }
        }

        #endregion

        #region IDictionary Interface

        public int Count
        {
            get { return (_dict != null) ? _dict.Count : 0; }
        }

        public void Add(TKey key, TValue value)
        {
            if (_dict == null) _dict = new Dictionary<TKey, TValue>(_comparer);
            _dict.Add(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            if (_dict == null) return false;
            return _dict.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get
            {
                if (_dict == null) _dict = new Dictionary<TKey, TValue>(_comparer);
                return _dict.Keys;
            }
        }

        public bool Remove(TKey key)
        {
            if (_dict == null) return false;
            return _dict.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (_dict == null)
            {
                value = default(TValue);
                return false;
            }
            return _dict.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values
        {
            get
            {
                if (_dict == null) _dict = new Dictionary<TKey, TValue>(_comparer);
                return _dict.Values;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                if (_dict == null) throw new KeyNotFoundException();
                return _dict[key];
            }
            set
            {
                if (_dict == null) _dict = new Dictionary<TKey, TValue>(_comparer);
                _dict[key] = value;
            }
        }

        public void Clear()
        {
            if (_dict != null) _dict.Clear();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            if (_dict == null) _dict = new Dictionary<TKey, TValue>(_comparer);
            (_dict as ICollection<KeyValuePair<TKey, TValue>>).Add(item);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            if (_dict == null) return false;
            return (_dict as ICollection<KeyValuePair<TKey, TValue>>).Contains(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (_dict == null) return;
            (_dict as ICollection<KeyValuePair<TKey, TValue>>).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            if (_dict == null) return false;
            return (_dict as ICollection<KeyValuePair<TKey, TValue>>).Remove(item);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return false; }
        }

        public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
        {
            if (_dict == null) return default(Dictionary<TKey, TValue>.Enumerator);
            return _dict.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (_dict == null) return Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();
            return _dict.GetEnumerator();
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            if (_dict == null) return Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();
            return _dict.GetEnumerator();
        }

        #endregion

        #region ISerializationCallbackReceiver

        [UnityEngine.SerializeField()]
        private TKey[] _keys;
        [UnityEngine.SerializeField()]
        private TValue[] _values;

        void UnityEngine.ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (_keys != null && _values != null)
            {
                if (_dict == null) _dict = new Dictionary<TKey, TValue>(_keys.Length, _comparer);
                else _dict.Clear();
                for (int i = 0; i < _keys.Length; i++)
                {
                    if (i < _values.Length)
                        _dict[_keys[i]] = _values[i];
                    else
                        _dict[_keys[i]] = default(TValue);
                }
            }

            _keys = null;
            _values = null;
        }

        void UnityEngine.ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (_dict == null || _dict.Count == 0)
            {
                _keys = null;
                _values = null;
            }
            else
            {
                int cnt = _dict.Count;
                _keys = new TKey[cnt];
                _values = new TValue[cnt];
                int i = 0;
                var e = _dict.GetEnumerator();
                while (e.MoveNext())
                {
                    _keys[i] = e.Current.Key;
                    _values[i] = e.Current.Value;
                    i++;
                }
            }
        }

        #endregion

    }

    [Serializable]
    public class SerializeableDictionaryIntInt : SerializableDictionaryBase<int, int> { }

    [Serializable]
    public class SerializeableDictionaryIntString : SerializableDictionaryBase<int, string> { }

    [Serializable]
    public class SerializeableDictionaryStringInt : SerializableDictionaryBase<string, int> { }
    [Serializable]
    public class SerializeableDictionaryIntDictIntInt : SerializableDictionaryBase<int, SerializeableDictionaryIntInt> { }
    [Serializable]
    public class Collection<T>
    {
        public List<T> values;

        public Collection()
        {
            values = new List<T>();
        }
        public void Add(T value)
        {
            values.Add(value);
        }
        public void Remove(T value)
        {
            values.Remove(value);
        }
        public bool Contains(T value)
        {
            return values.Contains(value);
        }
        public void Clear()
        {
            values.Clear();
        }
        public T this[int i]
        {
            get { return values[i]; }
            set { values[i] = value; }
        }
    }
    [Serializable]
    public class IntCollection : Collection<int>
    {

    }
    [Serializable]
    public class StringCollection : Collection<string>
    {

    }

    public class Converter
    {
        public static int ToInt(Color color)
        {
            return (Mathf.RoundToInt(color.a * 255) << 24) +
                   (Mathf.RoundToInt(color.r * 255) << 16) +
                   (Mathf.RoundToInt(color.g * 255) << 8) +
                   Mathf.RoundToInt(color.b * 255);
        }

        public static Color ToColor(int value)
        {
            var a = (float)(value >> 24 & 0xFF) / 255f;
            var r = (float)(value >> 16 & 0xFF) / 255f;
            var g = (float)(value >> 8 & 0xFF) / 255f;
            var b = (float)(value & 0xFF) / 255f;
            return new Color(r, g, b, a);
        }
    }

    [Serializable]
    public struct Date
    {
        [Range(1, 100)] public int Year;
        [Range(1, 12)] public int Month;
        [Range(1, 30)] public int Day;
        [Range(0, 23)] public int Hour;
        [Range(0, 59)] public int Minute;

        string result;

        public Date(int year, int month, int day, int hour, int minute)
        {
            Year = year;
            Month = month;
            Day = day;
            Hour = hour;
            Minute = minute;
            result = default;
        }

        public Date(Date reference)
        {
            Year = reference.Year;
            Month = reference.Month;
            Day = reference.Day;
            Hour = reference.Hour;
            Minute = reference.Minute;
            result = default;
        }

        public string AddMinutes(int value)
        {
            if (Minute + value > 59)
            {
                Minute = (Minute + value - 60);
                AddHours(1);
            }
            else
            {
                Minute += value;
            }
            return GetDayHourMinute();
        }
        public void AddHours(int value)
        {
            if (Hour + value > 23)
            {
                Hour = (Hour + value - 24);
                AddDays(1);
            }
            else
            {
                Hour += value;
            }
        }
        public void AddDays(int value)
        {
            if (Day + value > 30)
            {
                Day = (Day + value - 30);
                AddMonths(1);
            }
            else
            {
                Day += value;
            }
        }
        public void AddMonths(int value)
        {
            if (Month + value > 12)
            {
                Month = (Month + value - 12);
                AddYear(1);
            }
            else
            {
                Month += value;
            }
        }
        public void AddYear(int value)
        {
            if (Year + value >= 100)
            {

                Debug.Log("Поздравляем, вы прожили больше 100 лет!");
            }
            else
            {
                Year += value;
            }
        }

        public string GetDayHourMinute()
        {
            result = Day + ":" + Hour + ":" + Minute;
            return result;
        }
    }
}
