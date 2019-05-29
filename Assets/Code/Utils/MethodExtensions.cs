using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Utility
{
    public static class MethodExtensions
    {
        public static string RemoveQuotes(this string Value)
        {
            return Value.Replace("\"", "");
        }
    }
}