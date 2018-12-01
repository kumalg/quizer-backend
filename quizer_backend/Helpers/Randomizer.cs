using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace quizer_backend.Helpers {
    public static class Randomizer {

        /// <summary>
        /// Returns an random element of sequence
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="q"></param>
        /// <returns></returns>
        public static T RandomElement<T>(this IQueryable<T> q) {
            var r = new Random();
            return q.Skip(r.Next(q.Count())).FirstOrDefault();
        }


        /// <summary>
        /// Returns an random element of filtered sequence
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="q"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static T RandomElement<T>(this IQueryable<T> q, Expression<Func<T, bool>> e) {
            q = q.Where(e);
            return RandomElement(q);
        }

        /// <summary>
        /// Returns an sequence of shuffled elements using a time-dependent default seed value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="q"></param>
        /// <returns></returns>
        public static IQueryable<T> Shuffle<T>(this IQueryable<T> q) {
            var r = new Random();
            return q.OrderBy(i => r.Next());
        }

        /// <summary>
        /// Returns an sequence of shuffled elements using a time-dependent default seed value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="q"></param>
        /// <returns></returns>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> q) {
            var r = new Random();
            return q.OrderBy(i => r.Next());
        }

    }
}
