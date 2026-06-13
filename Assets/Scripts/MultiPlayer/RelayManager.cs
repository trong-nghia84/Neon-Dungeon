using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayManager : MonoBehaviour
{
    public static RelayManager Instance;

    private void Awake()
    {
        Instance = this;
        
       
    }

    private async void Start()
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        Debug.Log("Unity Services Ready");
    }

    public async Task<string> CreateRelay()
    {
        Allocation allocation =
            await RelayService.Instance.CreateAllocationAsync(1);

        string joinCode =
            await RelayService.Instance.GetJoinCodeAsync(
                allocation.AllocationId);

        NetworkManager.Singleton
            .GetComponent<UnityTransport>()
            .SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData);

        NetworkManager.Singleton.StartHost();

        return joinCode;
    }

    public async Task JoinRelay(string joinCode)
    {
        JoinAllocation allocation =
            await RelayService.Instance.JoinAllocationAsync(
                joinCode);

        NetworkManager.Singleton
            .GetComponent<UnityTransport>()
            .SetClientRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData,
                allocation.HostConnectionData);

        NetworkManager.Singleton.StartClient();
    }
}