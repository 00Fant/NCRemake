using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class inventory : NetworkBehaviour {
    public Vector2 InventorySize;
    public GameObject ItemPrefab;
    public GameObject ItemBegPrefab;
    public Hud hud;
    public player Player;
    public skills Skills;

    [SerializeField]
    public item[] items;
    public bool isLoaded;

    private GameObject InventoryItems;


    void Start () {
        InitInventory();  
    }

    void Update() {
        if( isLocalPlayer ) {
            if( Input.GetButtonDown( "Use" ) && !Cursor.visible ) {
                RemotePickUp();
            }
        }
        
    }

    //Init
    void InitInventory() {
        if( InventoryItems != null ) {
            DestroyImmediate( InventoryItems );
        }
        InventoryItems = new GameObject( "InventoryItems" );
        InventoryItems.transform.parent = transform;
        InventoryItems.transform.localPosition = Vector3.zero;
        InventoryItems.transform.localRotation = Quaternion.identity;
        int Size = (int)( InventorySize.x * InventorySize.y );
        items = new item[Size];
        for( int i = 0; i < items.Length; i++ ) {
            GameObject NewItem = (GameObject)Instantiate( ItemPrefab, transform.position, transform.rotation, InventoryItems.transform);
            NewItem.name = "Item-" + i.ToString();
            items[i] = NewItem.GetComponent<item>();
        }
        //if( isLocalPlayer ) {
        //    CmdLoadInventory();
        //}
    }

    public string GetInventoryPath( string PlayerName ) {
        if( PlayerName == "" ) {
            PlayerName = "fail";
        }
        if( !Directory.Exists( Application.dataPath + "/StreamingAssets/PlayerData/" ) ) {
            Directory.CreateDirectory( Application.dataPath + "/StreamingAssets/PlayerData/" );
        }
        return Application.dataPath + "/StreamingAssets/PlayerData/" + PlayerName + "_Inventory";
    }


    //Save
    public void RemoteSaveInventory(  ) {
        if( Player.GetPlayerKey() != "" ) {
            CmdSaveInventory( Player.GetPlayerKey() + Skills.Class.ToString() );
        }
    }
   
    [Command]
    public void CmdSaveInventory( string PlayerName ) {
        if( !isServer ) {
            return;
        }
        SaveInventory( PlayerName );
    }

    public string[] SaveInventory( string PlayerName ) {
        string[] PlayerSaveData = new string[items.Length];
        for( int i = 0; i < items.Length; i++ ) {
            PlayerSaveData[i] = items[i].GetItemString();
        }
        File.WriteAllLines( GetInventoryPath( PlayerName ) + ".txt", PlayerSaveData );
        return PlayerSaveData;
    }

   

    //Load
    public void RemoteLoadInventory( ) {
        if( Player.GetPlayerKey() != "" ) {
            CmdLoadInventory( Player.GetPlayerKey() + Skills.Class.ToString() );
            Debug.Log( "RemoteLoadInventory" );
        }
       
    }

    [Command]
    public void CmdLoadInventory( string PlayerName ) {
        if( !isServer ) {
            return;
        }
        Debug.Log( "CmdLoadInventory" );
        if( PlayerName != "" ) {
            string[] invData = LoadInventory( PlayerName );
            if( invData.Length == 0 ) {
                invData = SaveInventory( PlayerName );
                Debug.Log( "NoData for: " + PlayerName + "_Inventory" );
            }
            if( invData.Length > 0 ) {
                StartCoroutine( SendInventoryToClient( invData ) );
            } else {
                RpcInventoryLoaded();
                isLoaded = true;
            }
        }       
    }

    public string[] LoadInventory( string PlayerName ) {
        string IDPath = GetInventoryPath( PlayerName ) + ".txt";
        if( File.Exists( IDPath ) ) {
            return File.ReadAllLines( IDPath );            
        }       
        return new string[0];
    }


    //Send
    IEnumerator SendInventoryToClient( string[] _data ) {
        Debug.Log( "Start Send..." );
        if( isServer ) {
            for( int i = 0; i < _data.Length; i++ ) {
                RpcSetItem( i, _data[i] );
                yield return new WaitForSeconds( .01f );
            }
            RpcInventoryLoaded();
            isLoaded = true;
        }

        yield return null;
    }

    [ClientRpc]
    public void RpcInventoryLoaded() {
        isLoaded = true;
        Debug.Log( "Inventory Sending Finish" );
        if( isLocalPlayer ) {
            hud.Invoke( "LoadHud", 0.2f );
        }
    }

    //SetItem
    public void RemoteSetItem( int _index, string _itemData ) {
        CmdSetItem( _index, _itemData );      
    }

    [Command]
    void CmdSetItem( int _index, string _itemData ) {
        if( _itemData == "" ) {
            items[_index].ResetItem();
        } else {
            items[_index].SetItemFromString( _itemData );
        }
        RpcSetItem( _index, _itemData );
    }

    [ClientRpc]
    void RpcSetItem( int _index, string _itemData ) {
        if( _itemData == "" ) {
            items[_index].ResetItem();
        } else {
            items[_index].SetItemFromString( _itemData );
        }
        if( isLocalPlayer ) {
            hud.Invoke( "LoadHud", 0.2f );
        }
    }


    //Drop
    public void RemoteSpawnItemBeg( string _data ) {
        CmdSpawnItemBeg( _data );
        hud.Invoke( "LoadHud", 0.05f );
    }
    
    [Command]
    public void CmdSpawnItemBeg( string _data ) {
        Vector3 Pos = transform.position + ( transform.forward * 1 );
        GameObject NewItemBeg = Instantiate( ItemBegPrefab, Pos, transform.rotation );
        NetworkServer.Spawn( NewItemBeg );
        itemBeg ItemBeg = NewItemBeg.GetComponent<itemBeg>();
        ItemBeg.SetBeg( _data );
    }



    //Pickup
    public void RemotePickUp() {
        Ray PickUpRay = new Ray( Camera.main.transform.position, Camera.main.transform.forward );
        //Debug.DrawRay( Camera.main.transform.position, Camera.main.transform.forward * 3f, Color.red, 2f );
        RaycastHit PickUpHit;
        if( Physics.Raycast( PickUpRay, out PickUpHit, 3f ) ) {
            if( PickUpHit.collider ) {
                //Debug.Log( PickUpHit.collider.name );
                if( PickUpHit.collider.gameObject != gameObject ) {
                    itemBeg PickUpItem = PickUpHit.collider.gameObject.GetComponentInParent<itemBeg>();
                    if( PickUpItem ) {
                        CmdPickUp( PickUpItem.gameObject );
                        if( isLocalPlayer ) {
                            hud.Invoke( "LoadHud", 0.2f );
                        }
                    }
                }
            }
        }
    }

    [Command]
    public void CmdPickUp( GameObject PickGo ) {
        if( !PickGo ) {
            return;
        }
        itemBeg PickUpItem = PickGo.GetComponent<itemBeg>();
        if( !PickUpItem ) {
            return;
        }
        for( int i = 0; i < items.Length; i++ ) {
            if( items[i].Item_Name == "" ) {
                RemoteSetItem(i, PickUpItem.ItemData );
                NetworkServer.Destroy( PickGo );
                break;
            }
        }
        

    }

}
