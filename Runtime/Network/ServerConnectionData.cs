using System.Collections.Generic;

namespace Sobia.Utils
{
    /// <summary>
    /// Provides static methods and data for managing the association between client identifiers and their corresponding
    /// names within a server context.
    /// </summary>
    /// <remarks>This class maintains a mapping of client IDs to client names, allowing for efficient
    /// retrieval and updating of client information. All members are static and thread safety is not guaranteed;
    /// external synchronization may be required if accessed concurrently.</remarks>
    public static class ServerConnectionData
    {
        public static readonly Dictionary<ulong, string> ClientIdToName =
            new Dictionary<ulong, string>();

        public static void AddClientName(ulong clientId, string name)
        {
            if (ClientIdToName.ContainsKey(clientId))
            {
                ClientIdToName[clientId] = name;
            }
            else
            {
                ClientIdToName.Add(clientId, name);
            }
        }

        public static bool TryGetClientName(ulong clientId, out string name)
        {
            return ClientIdToName.TryGetValue(clientId, out name);
        }
    }
}