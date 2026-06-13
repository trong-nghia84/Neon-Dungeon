using Unity.Netcode.Components;
using UnityEngine;

namespace Unity.Netcode.Samples
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Netcode/Client Network Transform")]
    public class ClientNetworkTransform : NetworkTransform
    {
      
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}