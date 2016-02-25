using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aphysoft.Common
{
    /// <summary>
    /// Provides a set of static methods that provide support for any other object.
    /// </summary>
    public static class ObjectHelper
    {
        /// <summary>
        /// Compares specified object with 'compare' object, returns 'iftrue' if true otherwise if false returns the specified object.
        /// </summary>
        public static object Format(object obj, object compare, object iftrue)
        {
            if (obj == compare)
                return iftrue;
            else
                return obj;
        }
        /// <summary>
        /// Returns null if specified string is a empty string, otherwise returns itself.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Format(string str)
        {
            return (string)Format(str, string.Empty, null);
        }

        /// <summary>
        /// Compare two bytes sequence.
        /// </summary>
        public static bool ByteArrayCompare(byte[] a1, byte[] a2)
        {
            IStructuralEquatable eqa1 = a1;
            return eqa1.Equals(a2, StructuralComparisons.StructuralEqualityComparer);
        }
    }

    [Serializable]
    public abstract class Ageable
    {
        protected DateTime lastTouch = DateTime.Now;

        public Ageable()
        {
            lastTouch = DateTime.Now;

            
        }

        public void Touch()
        {
            lastTouch = DateTime.Now;
        }
    }
}
