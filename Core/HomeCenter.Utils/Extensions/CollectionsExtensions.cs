﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace HomeCenter.Utils.Extensions
{
    public static class CollectionsExtensions
    {
        public static ICollection<T> AddChained<T>(this ICollection<T> collection, T item)
        {
            collection.Add(item);
            return collection;
        }

        public static ICollection<T> RemoveChained<T>(this ICollection<T> collection, T item)
        {
            collection.Remove(item);
            return collection;
        }

        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (T item in collection) action(item);
        }

        public static string ToFormatedString<T>(this IEnumerable<T> collection, string separator = ",")
        {
            return string.Join($"{separator} ", collection);
        }

        public static IEnumerable<T> Expand<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> elementSelector)
        {
            var stack = new Stack<IEnumerator<T>>();
            var e = source.GetEnumerator();
            try
            {
                while (true)
                {
                    while (e.MoveNext())
                    {
                        var item = e.Current;
                        yield return item;
                        var elements = elementSelector(item);
                        if (elements == null) continue;
                        stack.Push(e);
                        e = elements.GetEnumerator();
                    }
                    if (stack.Count == 0) break;
                    e.Dispose();
                    e = stack.Pop();
                }
            }
            finally
            {
                e.Dispose();
                while (stack.Count != 0) stack.Pop().Dispose();
            }
        }

        public static void RemoveRange<TKey, TValue>(this IDictionary<TKey, TValue> dic, IEnumerable<TKey> toRemove)
        {
            foreach (var el in toRemove)
            {
                dic.Remove(el);
            }
        }

        public static string ToFormatedString<TKey, TValue>(this IDictionary<TKey, TValue> dic)
        {
            if (dic.Count == 0) return string.Empty;
            return dic?.Select(x => x.Key + "=" + x.Value)?.Aggregate((s1, s2) => s1 + ";" + s2);
        }

        public static void AddRangeNewOnly<TKey, TValue>(this IDictionary<TKey, TValue> dic, IDictionary<TKey, TValue> dicToAdd)
        {
            dicToAdd.ForEach(x => { if (!dic.ContainsKey(x.Key)) dic.Add(x.Key, x.Value); });
        }

        public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dictionary) => new ReadOnlyDictionary<TKey, TValue>(dictionary);

        public static bool IsEqual(this Dictionary<string, string> source, Dictionary<string, string> dest)
        {
            if (ReferenceEquals(source, dest)) return true;
            if (source.Count != dest?.Count) return false;

            foreach (var attribute in source)
            {
                if (!dest.ContainsKey(attribute.Key)) return false;

                if (dest[attribute.Key].Compare(attribute.Value) != 0) return false;
            }
            return true;
        }

        public static void ForEach<T>(this IReadOnlyCollection<T> collection, Action<T> action)
        {
            foreach (T item in collection) action(item);
        }

        public static bool LeftEqual<T, K>(this IReadOnlyDictionary<T, K> source, IReadOnlyDictionary<T, K> dest) where T : class where K : class
        {
            if (ReferenceEquals(source, dest)) return true;

            foreach (var attribute in source)
            {
                if (!dest.ContainsKey(attribute.Key)) return false;
                if (!dest[attribute.Key].Equals(attribute.Value)) return false;
            }
            return true;
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dict) => dict.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        
    }
}