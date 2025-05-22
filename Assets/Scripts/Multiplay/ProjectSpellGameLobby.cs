using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Multiplay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
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
            _joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, 4);

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