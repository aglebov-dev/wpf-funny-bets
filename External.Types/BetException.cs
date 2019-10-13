using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace External.Types
{
    public class BetException : Exception
    {
        public static Type[] GetKnownTypes()
        {
            List<Type> result = new List<Type>();
            result.Add(Type.GetType("System.Collections.ListDictionaryInternal"));
            return result.ToArray();
        }
        protected BetException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public BetException(string message) : base(message) { }
    }
}
