using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Hud : NetworkBehaviour {

    public GameObject Windows;
    public bool HUDActive;
    public GameObject PlayerGo;
    public GameObject ServerMenu;

    public skills Skills;
    public stats Stats;
    public inventory Inventory;
    public player Player;
    public Hud_Inventory hud_Inventory;

    public InputField ServerIP;
    public InputField ServerPort;
    public InputField Name;
    public InputField Password;
    public GameObject Button_StartServer;
    public GameObject Button_JoinServer;
    public GameObject Button_StopServer;

    private void Start() {
        ToggleHUD();
        Button_StartServer.SetActive( true );
        Button_JoinServer.SetActive( true );
        Button_StopServer.SetActive( false );
    }

    void Update() {
        if( !PlayerGo ) {
            GetLocalPlayer();
            if( PlayerGo ) {
                LoadHud();
            }
            
            if( !PlayerGo && !ServerMenu.activeSelf ) {
                ToggleWindow( ServerMenu );
            }
        }
        if( Input.GetButtonDown( "ToggleHUD" ) ) {
            ToggleHUD();
        }
        if( Input.GetKeyDown( KeyCode.Escape ) ) {
            ToggleWindow( ServerMenu );
        }
    }

    void GetLocalPlayer() {
        foreach( GameObject Ply in GameObject.FindGameObjectsWithTag("Player") ) {
            if( Ply.GetComponent<NetworkIdentity>().isLocalPlayer ) {
                PlayerGo = Ply;
                if( HUDActive ) {
                    ToggleHUD();
                }
                return;
            }
        }
    }

    public void ToggleHUD() {
        if( !HUDActive ) {
            HUDActive = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            ShowWindows();
        } else {
            if( !PlayerGo ) {
                return;
            }
            HUDActive = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            HideWindows();
        }
    }

    public void ShowWindows() {
        Windows.SetActive( true );
        LoadHud();
    }

    public void HideWindows() {
        Windows.SetActive( false );
    }

    public void LoadHud() {
        if( !PlayerGo ) {
            return;
        }
        Player = PlayerGo.GetComponent<player>();
        if( !Player ) {
            return;
        }
        Skills = PlayerGo.GetComponent<skills>();
        if( !Skills ) {
            return;
        }
        Stats = PlayerGo.GetComponent<stats>();
        if( !Stats ) {
            return;
        }
        Inventory = PlayerGo.GetComponent<inventory>();
        if( !Inventory ) {
            return;
        }
        Inventory.hud = this;
        Stats.hud = this;
        Stats.getStats();
        foreach( Hud_Skillbar _hud_Skillbar in GetComponentsInChildren<Hud_Skillbar>( true ) ) {
            _hud_Skillbar.LoadSkillBar();
        }
        foreach( Hud_Subskill _hud_subSkillbar in GetComponentsInChildren<Hud_Subskill>( true ) ) {
            _hud_subSkillbar.LoadSubSkillValues();
        }
        
        hud_Inventory.LoadHudInventory( Inventory );
    }
    
    public void ToggleWindow( GameObject _Window ) {
        if( _Window.activeSelf ) {
            _Window.SetActive( false );
        } else {
            LoadHud();
            _Window.SetActive( true );
        }
    }

    public void DebugAddExpToAll() {
        if( !Skills ) {
            return;
        }
        Skills.DebugAddExpToAll();
        foreach( Hud_Skillbar _hud_Skillbar in GetComponentsInChildren<Hud_Skillbar>( true ) ) {
            _hud_Skillbar.LoadSkillBar();
        }
        foreach( Hud_Subskill _hud_subSkillbar in GetComponentsInChildren<Hud_Subskill>( true ) ) {
            _hud_subSkillbar.LoadSubSkillValues();
        }
        Invoke( "LoadHud", 0.2f );
    }

    public void ExitGame() {
        Application.Quit();
    }

    public void CorrectedName(  ) {
        string newName = Name.text;
        newName = newName.Replace( "&", "" ).Replace( "$", "" ).Replace( "%", "" ).Replace( "|", "" ).Replace( "/", "" ).Replace( ",", "" ).Replace( "§", "" );
        newName = newName.Replace( "Player", "" ).Replace( "(", "" ).Replace( ")", "" ).Replace( "[", "" ).Replace( "]", "" ).Replace( "{", "" ).Replace( "}", "" );
        newName = newName.Replace( "#", "" ).Replace( "!", "" ).Replace( "?", "" ).Replace( ",", "" ).Replace( ".", "" ).Replace( "-", "" ).Replace( "_", "" );
        Name.text = newName;
    }
    public void CorrectedPassword() {
        string newName = Password.text;
        newName = newName.Replace( "&", "" ).Replace( "$", "" ).Replace( "%", "" ).Replace( "|", "" ).Replace( "/", "" ).Replace( ",", "" ).Replace( "§", "" );
        newName = newName.Replace( "Player", "" ).Replace( "(", "" ).Replace( ")", "" ).Replace( "[", "" ).Replace( "]", "" ).Replace( "{", "" ).Replace( "}", "" );
        newName = newName.Replace( "#", "" ).Replace( "!", "" ).Replace( "?", "" ).Replace( ",", "" ).Replace( ".", "" ).Replace( "-", "" ).Replace( "_", "" );
        Password.text = newName;
    }
    public bool HasCorrectData() {
        CorrectedName();
        CorrectedPassword();
        if( Name.text.Length >= 4 ) {
            if( Password.text.Length >= 4 ) {
                if( ServerIP.text.Length > 0 ) {
                    if( ServerPort.text.Length > 0 ) {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
