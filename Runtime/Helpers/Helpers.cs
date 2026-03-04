using UnityEngine;
namespace Sobia.Utils
{
    public static class SobiaUtils
    {
        /// <summary>
        /// Checks if a Unity object is null and logs a formatted error.
        /// Returns true if valid, false if null.
        /// obj => which refernce, fieldName => nameof(obj), holder => gameboject
        /// </summary>
        public static void IsAssigned(Object obj, string fieldName, GameObject holder)
        {
            if (obj == null)
            {
                Debug.LogError($"<b>[{holder.name}]</b> <color=red>Missing Reference:</color> '{fieldName}' is not assigned!", holder);
            }
        }
    }
}