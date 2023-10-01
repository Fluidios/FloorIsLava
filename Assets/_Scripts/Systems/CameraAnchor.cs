using Fusion;
using Game.SystemsManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems
{
    public class CameraAnchor : NetworkBehaviour
    {
        private void Awake()
        {
            StartCoroutine(WaitForSelfNetworkObjectInitialization(TryAttachCameraToMe));
        }
        private void TryAttachCameraToMe()
        {
            if (Object.HasInputAuthority)
            {
                SystemsManager.GetSystemOfType<CameraSystem>().SetTarget(transform);
            }
        }

        IEnumerator WaitForSelfNetworkObjectInitialization(Action actionAfterInitialization)
        {
            yield return new WaitWhile(() => Object == null);
            actionAfterInitialization();
        }
    }
}
