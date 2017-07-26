using System;

namespace UITests.Model.Common
{   
    public static class Util
    {
         public static T RandomValue<T>(T[] values)
        {
            return values[new Random().Next(values.Length - 1)];
        }                
    }
}