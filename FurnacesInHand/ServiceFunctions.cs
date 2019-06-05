using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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
        public static string FindExePath(string exe)
        {
            exe = Environment.ExpandEnvironmentVariables(exe);
            if (!File.Exists(exe))
            {
                if (Path.GetDirectoryName(exe) == String.Empty)
                {
                    foreach (string test in (Environment.GetEnvironmentVariable("PATH") ?? "").Split(';'))
                    {
                        string path = test.Trim();
                        if (!String.IsNullOrEmpty(path) && File.Exists(path = Path.Combine(path, exe)))
                            return Path.GetFullPath(path);
                    }
                }
                throw new FileNotFoundException(new FileNotFoundException().Message, exe);
            }
            return Path.GetFullPath(exe);
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
