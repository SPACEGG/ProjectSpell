using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Multiplay;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProjectSpellGameLobby : MonoBehaviour
{
    public static ProjectSpellGameLobby Singleton { get; private set; }

    public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;

    public class OnLobbyListChangedEventArgs : EventArgs
    {
        public List<Lobby> Lobbies;
    }

    private Lobby _joinedLobby;
    private const float MaxListLobbyTimer = 3f;
    private const int MaxPlayerInLobby = 4;
    private const string RelayJoinCode = "RelayJoinCode";
    private float _listLobbiesTimer = MaxListLobbyTimer;

    private void Awake()
    {
        if (Singleton && Singleton != this)
        {
            Destroy(gameObject);
            return;
        }

        Singleton = this;

        DontDestroyOnLoad(this);

        InitializeUnityAuthentication().Forget();
    }

    private void Update()
    {
        HandlePeriodicListLobbies();
    }

    private void HandlePeriodicListLobbies()
    {
        if (_joinedLobby != null) return;

        if (!AuthenticationService.Instance.IsSignedIn) return;

        _listLobbiesTimer -= Time.deltaTime;
        if (_listLobbiesTimer <= 0f)
        {
            _listLobbiesTimer = MaxListLobbyTimer;
            ListLobbies().Forget();
        }
    }

    private async UniTask<Allocation> AllocateRelay()
    {
        try
        {
            return await RelayService.Instance.CreateAllocationAsync(MaxPlayerInLobby - 1);
        }
        catch (RelayServiceException e)
        {
            Debug.LogException(e);

            return null;
        }
    }

    private async UniTask<string> GetRelayJoinCode(Allocation allocation)
    {
        try
        {
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            Debug.Log($"Relay Join Code: {joinCode}");

            return joinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.LogException(e);

            return null;
        }
    }

    private async UniTask<JoinAllocation> JoinRelay(string joinCode)
    {
        try
        {
            return await RelayService.Instance.JoinAllocationAsync(joinCode);
        }
        catch (RelayServiceException e)
        {
            Debug.LogException(e);

            return null;
        }
    }

    private async UniTaskVoid InitializeUnityAuthentication()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            InitializationOptions options = new InitializationOptions();
            options.SetProfile(Random.Range(10000, 99999).ToString());

            await UnityServices.InitializeAsync();

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    public async UniTaskVoid CreateLobby(string lobbyName)
    {
        try
        {
            _joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, MaxPlayerInLobby);

            var allocation = await AllocateRelay();

            string relayJoinCode = await GetRelayJoinCode(allocation);

            await LobbyService.Instance.UpdateLobbyAsync(_joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { RelayJoinCode, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                }
            });
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(allocation.ToRelayServerData("dtls"));

            ProjectSpellGameMultiplayer.Singleton.StartHost();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
        }
    }

    public async UniTaskVoid QuickJoinLobby()
    {
        try
        {
            _joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            var joinAllocation = await JoinRelay(_joinedLobby.Data[RelayJoinCode].Value);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(joinAllocation.ToRelayServerData("dtls"));

            ProjectSpellGameMultiplayer.Singleton.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
        }
    }

    public async UniTaskVoid JoinLobby(string lobbyId)
    {
        try
        {
            _joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);

            var joinAllocation = await JoinRelay(_joinedLobby.Data[RelayJoinCode].Value);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(joinAllocation.ToRelayServerData("dtls"));

            ProjectSpellGameMultiplayer.Singleton.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
        }
    }

    private async UniTaskVoid ListLobbies()
    {
        var options = new QueryLobbiesOptions
        {
            Count = 10,
            Filters = new List<QueryFilter>
            {
                new(
                    QueryFilter.FieldOptions.AvailableSlots,
                    "0",
                    QueryFilter.OpOptions.GT
                ),
            }
        };

        try
        {
            var response = await LobbyService.Instance.QueryLobbiesAsync(options);

            OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs
            {
                Lobbies = response.Results
            });
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
        }
    }
}