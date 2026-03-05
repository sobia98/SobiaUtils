using System;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace Sobia.Utils
{
    /// <summary>
    /// Manages the lifecycle of Unity Gaming Services authentication.
    /// Provides an event for other systems to initialize once login is successful.
    /// Other Methods listen to OnLoginComplete to start working with the Player.
    /// Assign to Networkmanager
    /// </summary>
    public class AuthenticationServiceFacade : MonoBehaviour
    {
        //Sub to this Action
        public static event Action OnLoginComplete;

        private async void Start()
        {
            try
            {
                await UnityServices.InitializeAsync();

                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                    Debug.Log($"[Auth] Signed in! Player ID: {AuthenticationService.Instance.PlayerId}");
                }

                OnLoginComplete?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError($"[Auth] Initialization failed: {e.Message}");
            }
        }
    }
}