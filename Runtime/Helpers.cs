namespace Sobia.Utils
{
    public static class PrototypeUtils
    {
        public static void QuickLog(string message)
        {
            UnityEngine.Debug.Log($"[Prototype] {message}");
        }

        public static void PrintDumb()
        {
            UnityEngine.Debug.Log($"I am Dumb!");
        }
    }
}