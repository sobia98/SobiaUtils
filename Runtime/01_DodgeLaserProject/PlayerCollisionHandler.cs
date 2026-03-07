using Sobia.Utils;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

namespace Sobia.LaserProject
{
    public class PlayerCollisionHandler : NetworkBehaviour
    {
        private CinemachineImpulseSource ImpulseSource;

        private void Awake()
        {
            ImpulseSource = GetComponent<CinemachineImpulseSource>();
        }

        private void Start()
        {
            SobiaUtils.IsAssigned(ImpulseSource, nameof(ImpulseSource), gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsOwner) return;

            if (other.gameObject.CompareTag("Laser"))
            {
                HandleLaserHit();
            }
            else if (other.gameObject.CompareTag("KillZone"))
            {
                HandleFallDeath();
            }
        }

        private void HandleLaserHit()
        {
            ImpulseSource.GenerateImpulse();
            //AudioManager.Instance.PlayHitClip();
            ReportHitServerRpc();
        }

        private void HandleFallDeath()
        {
            ImpulseSource.GenerateImpulse();
            //AudioManager.Instance.PlayGroundClip(); // Assuming you have a less severe "fall" clip
            ReportHitServerRpc();
        }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
        private void ReportHitServerRpc()
        {
            ulong targetId = OwnerClientId;

            GameUIManager.Instance.ReportNewHighScoreServerRpc(targetId);
            GameManager.Instance.RequestPlayerAfterHitServerRpc(targetId);
        }
    }
}