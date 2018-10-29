using System;
using System.Linq;
using System.Linq.Expressions;

namespace quizer_backend.Helpers {
    public static class Randomizer {
        public static T RandomElement<T>(this IQueryable<T> q, Expression<Func<T, bool>> e) {
            var r = new Random();
            q = q.Where(e);
            return q.Skip(r.Next(q.Count())).FirstOrDefault();
        }
        //public static T RandomElement<T>(this IEnumerable<T> q, Func<T, bool> e) {
        //    var r = new Random();
        //    q = q.Where(e);
        //    return q.Skip(r.Next(q.Count())).FirstOrDefault();
        //}
    }
}
