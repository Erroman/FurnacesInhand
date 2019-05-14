using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FurnacesInHand
{
    static class ServiceFunctions
    {
        public static string StringToLowerCase(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            char[] a = s.ToCharArray();
            a[0] = char.ToLower(a[0]);
            return new string(a);
        }
    }
   // a collection of extra extension methods off IEnumerable<T>
   public static class EnumerableExtensions
   {
       // Finds an item in the collection, similar to List<T>.FindIndex()
       public static int FindIndex<T>(this IEnumerable<T> list, Predicate<T> finder)
       {
           // note if item not found, result is length and not -1!
           return list.TakeWhile(i => !finder(i)).Count();
       }
   }
}
