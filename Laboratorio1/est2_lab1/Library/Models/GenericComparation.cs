using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Models
{
    public class GenericComparation<T> where T : IComparable
    {

        public static int Position(T[] data, Delegate comparer, T val)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (!object.Equals(data[i], default(T)))
                {
                    if ((int)comparer.DynamicInvoke(val,data[i]) == -1)
                    {
                        return i;
                    }

                }
                else
                {
                    return i;
                }
            }
            return data.Length;
               
        }
        public static T[] SortedList(T[] data, Delegate comparer)
        {
            for (int i = 0; i < data.Length-1; i++)
            {
                for (int j = i+1; j < data.Length; j++)
                {
                    if (!object.Equals(data[i], default(T)) && !object.Equals(data[j], default(T))  )
                    {
                        if ((int)comparer.DynamicInvoke(data[i], data[j]) == 1 )
                        {
                            var aux = data[i];
                            data[i] = data[j];
                            data[j] = aux;
                        }
                    } 
                }
            }
            return data;
        }
    }

}
