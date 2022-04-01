using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var items = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var result = items.Mwhere(x => x==1);
            Console.WriteLine(result.ToList()[0]);
        }
    }

    public static class Extend
    {
        public static IEnumerable<T> Mwhere <T> (this IEnumerable<T> items,Func<T,bool> func)
        {
            List<T> datalist = new List<T>();
            foreach(var item in items)
            {
                if (func(item))
                {
                    yield return item;
                }
            }
        }

    }
}
