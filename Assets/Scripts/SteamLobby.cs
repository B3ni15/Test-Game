using UnityEngine;
using Steamworks;
using Mirror;

public class SteamLobby : MonoBehaviour
{
    [SerializeField] private GameObject lobbyUI;
    [SerializeField] private NetworkManager networkManager;

    private Callback<LobbyCreated_t> lobbyCreated;
    private Callback<GameLobbyJoinRequested_t> lobbyJoinRequested;
    private Callback<LobbyEnter_t> lobbyEntered;

    private void Start()
    {
        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        lobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    // Lobby létrehozása
    public void CreateLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, networkManager.maxConnections);
    }

    // Ha létrejött a lobby
    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            Debug.LogError("Lobby creation failed!");
            return;
        }

        // Beállítjuk a lobby adatait
        SteamMatchmaking.SetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            "HostAddress",
            SteamUser.GetSteamID().ToString()
        );

        // Host indítása Mirror-rel
        networkManager.StartHost();
        lobbyUI.SetActive(false);
    }

    // Ha valaki csatlakozni akar
    private void OnLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    // Ha beléptünk a lobbyba
    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        if (NetworkServer.active) return; // Host esetén nem kell csatlakozni

        // Megkapjuk a hoszt IP-jét és csatlakozunk
        string hostAddress = SteamMatchmaking.GetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            "HostAddress"
        );

        networkManager.networkAddress = hostAddress;
        networkManager.StartClient();
        lobbyUI.SetActive(false);
    }
}