using System;
using System.Collections.Generic;
using System.Linq;

namespace Aper_bot.Util
{
    public static class LINQExtensions
    {
        public static IEnumerable<T> SetValue<T>(this IEnumerable<T> items, Action<T> updateMethod)
        {
            foreach (T item in items)
            {
                updateMethod(item);
            }

            return items;
        }
        
        public static IEnumerable<T> ContinuouslyNumber<T>(this IEnumerable<T> items, Action<T,int> updateMethod, int start = 0)
        {
            int i = start;
            foreach (T item in items)
            {
                updateMethod(item,i);
                i++;
            }

            return items;
        }
        
        /// Traverses an object hierarchy and return a flattened list of elements
        /// based on a predicate.
        /// 
        /// TSource: The type of object in your collection.</typeparam>
        /// source: The collection of your topmost TSource objects.</param>
        /// getChildrenFunction: A function that fetches the child collection from an object.
        /// returns: A flattened list of objects which meet the criteria in selectorFunction.
        public static IEnumerable<TSource> Map<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, IEnumerable<TSource>> getChildrenFunction)
        {
            // Add what we have to the stack
            var flattenedList = source;

            // Go through the input enumerable looking for children,
            // and add those if we have them
            foreach (TSource element in source)
            {
                flattenedList = flattenedList.Concat(
                    getChildrenFunction(element).Map(getChildrenFunction)
                );
            }
            return flattenedList;
        }
    }
}