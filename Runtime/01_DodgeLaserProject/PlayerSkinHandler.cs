using Sobia.Utils;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Sobia.LaserProject
{
    [Serializable]
    public class SkinEntry
    {
        public int SkinID;
        public string SkinName;
        public GameObject SkinPrefab;
    }

    public class PlayerSkinHandler : NetworkBehaviour
    {
        [SerializeField] private SkinEntry DefaultSkin;
        [SerializeField] private List<SkinEntry> AvailableSkins = new List<SkinEntry>();

        //Skins Variables
        private GameObject CurrentVisual;

        private NetworkVariable<int> CurrentVisualsId = new NetworkVariable<int>(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

        private void Start()
        {
            AvailableSkins.Add(DefaultSkin);
            ButtonManagerUI.Instance.MaleSkin.onClick.AddListener(() => SetSkinByName(0));
            ButtonManagerUI.Instance.FemaleSkin.onClick.AddListener(() => SetSkinByName(1));
            ButtonManagerUI.Instance.GoblinSkin.onClick.AddListener(() => SetSkinByName(2));
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsServer)
            {
                CurrentVisualsId.Value = DefaultSkin != null ? DefaultSkin.SkinID : 0;
            }

            CurrentVisualsId.OnValueChanged += (oldValue, newValue) => ChangeSkin(newValue);
            ChangeSkin(CurrentVisualsId.Value);
        }

        public override void OnNetworkDespawn()
        {
            CurrentVisualsId.OnValueChanged -= (oldValue, newValue) => ChangeSkin(newValue);
            base.OnNetworkDespawn();
        }

        public void SetSkinByName(int skinId)
        {
            if (!IsOwner) return;
            SetSkinServerRpc(skinId);
        }

        public void ChangeSkin(int skinID)
        {
            GameObject skinPrefab = AvailableSkins.Find(e => e.SkinID == skinID)?.SkinPrefab;

            if (skinPrefab != null)
            {
                if (CurrentVisual != null)
                {
                    Destroy(CurrentVisual);
                }

                // sec argument indicates parent, instaniate as child of attached object(player)
                GameObject newVisuals = Instantiate(skinPrefab, transform);
                newVisuals.name = skinPrefab.name + "_Visuals_Instance";
                CurrentVisual = newVisuals;
            }
            else
            {
                Debug.LogError($"Skin not found skin id: '{skinID}'.");
                return;
            }
        }

        [ServerRpc]
        private void SetSkinServerRpc(int newVisualId)
        {
            CurrentVisualsId.Value = newVisualId;
        }
    }
}