using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;


public class ServerManager : MonoBehaviour {
	public NetworkManager manager;
    public Hud hud;

    public int selectedClass;
    private bool IsRunning;

    void Start() {
        Load();
    }

    void Update() {
        if( !hud ) {
            hud = Camera.main.GetComponentInChildren<Hud>();
            return;
        }
        //if( !manager.isNetworkActive && IsRunning ) {
        //    StopServer();
        //}
        if( hud.ServerIP.text == "" || hud.ServerPort.text == "" || hud.ServerPort.text == "0" ) {
            Load();
        }
    }

    void Load(){
        hud.Name.text = PlayerPrefs.GetString( "Name" );
        hud.Password.text = PlayerPrefs.GetString( "Password" );
        hud.ServerIP.text = PlayerPrefs.GetString( "LastAddress" );
        hud.ServerPort.text = PlayerPrefs.GetString( "LastPort" );
        if ( hud.ServerIP.text == "") {
            hud.ServerIP.text = "localhost";
		}
		if ( hud.ServerPort.text == "" || hud.ServerPort.text == "0" ) {
            hud.ServerPort.text = "7777";
		}
        Save();
    }

    void Save() {
        PlayerPrefs.SetString( "Name", hud.Name.text );
        PlayerPrefs.SetString( "Password", hud.Password.text );
        PlayerPrefs.SetString( "LastAddress", hud.ServerIP.text );
        PlayerPrefs.SetString( "LastPort", hud.ServerPort.text );
    }

    public void Start_Server() {
        if( !hud.HasCorrectData() ) {
            return;
        }
        if (!NetworkClient.active && !NetworkServer.active && !IsRunning ) {
            manager.StopHost();
            NetworkServer.Reset();
            manager.networkAddress = hud.ServerIP.text;
            manager.networkPort = int.Parse( hud.ServerPort.text);
            manager.maxConnections = 32;
            Save();
            try {
                manager.StartHost();
                IsRunning = true;
                hud.Button_StartServer.SetActive( false );
                hud.Button_JoinServer.SetActive( false );
                hud.Button_StopServer.SetActive( true );
            } catch( System.Exception ) {
                throw;
            }
        }
    }

    public void ConnectToServer() {
        if( !hud.HasCorrectData() ) {
            return;
        }
        if (!NetworkClient.active && !NetworkServer.active && !IsRunning ) {
            manager.StopHost();
            NetworkServer.Reset();
            manager.networkAddress = hud.ServerIP.text;
            manager.networkPort = int.Parse( hud.ServerPort.text);
            Save();
            manager.StartClient();
            IsRunning = true;
            hud.Button_StartServer.SetActive( false );
            hud.Button_JoinServer.SetActive( false );
            hud.Button_StopServer.SetActive( true );
        }
    }

    public void Stop_Server(){
        if( hud.Skills ) {
            hud.Skills.RemoteSaveSkills();
        }
        if( hud.Inventory ) {
            hud.Inventory.RemoteSaveInventory();
        }
        Network.RemoveRPCs( Network.player );
        Network.Disconnect();
        manager.StopHost();
        NetworkServer.Reset();
        IsRunning = false;
        if( hud.PlayerGo ) {
            Destroy( hud.PlayerGo );
        }
        hud.Button_StartServer.SetActive( true );
        hud.Button_JoinServer.SetActive( true );
        hud.Button_StopServer.SetActive( false );
        SceneManager.LoadScene( 0 );
    }
}
