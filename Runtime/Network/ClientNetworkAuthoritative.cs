using Unity.Netcode.Components;

namespace Sobia.Utils
{
    public class ClientNetworkAnimator : NetworkAnimator
    {
        /// <summary>
        /// The Client is the onwer of this object.
        /// The Client send data to server (can also cheat).
        /// Cheat not a problem, cause casual games.
        /// Better for Performance
        /// <remark>
        /// Assign to the object you want to control.
        /// </remark>
        /// </summary>
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}