using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hud_Inventory : MonoBehaviour {
    public Hud hud;
    public Vector2 Size;
    public Transform HudSlots;
    public GameObject InventorySlotPrefab;
    public Slider SlotScroleSlider;
    public RawImage DragItem;

    private inventory Inventory;
    private GameObject SlotsGroup;
    private GameObject[] SubSlots;
    private Hud_InventorySlot[] InvSlots;
    private int DragIndex;
    
	void Start () {
        DragItem.enabled = false;
        InventorySlotPrefab.SetActive( false );
        InitHudInventory();
        gameObject.SetActive( false );
    }

    void Update() {
        if( DragItem.texture != null ) {
            DragItem.transform.position = Input.mousePosition;
        }
        if( Inventory ) {
            if( gameObject.activeSelf && !Inventory.isLoaded ) {
                gameObject.SetActive( false );
            }
        }
        
    }

    public void InitHudInventory() {
        Size = new Vector3( 10, 10 );
        if( SlotsGroup ) {
            DestroyImmediate( SlotsGroup );
        }
        SlotsGroup = new GameObject( "Slots" );
        SlotsGroup.transform.SetParent( HudSlots );
        SlotsGroup.transform.localScale = Vector3.one;

        SubSlots = new GameObject[(int)Size.y];
        InvSlots = new Hud_InventorySlot[(int)( Size.x * Size.y )];
        int index = 0;
        for( int y = 0; y < Size.y; y++ ) {
            SubSlots[y] = new GameObject( "SubSlot: " + y.ToString() );
            SubSlots[y].transform.SetParent( SlotsGroup.transform );
            SubSlots[y].transform.localScale = Vector3.one;
            SubSlots[y].transform.localPosition = ( new Vector3( 0, -y, 0 ) * 36 );
            for( int x = 0; x < Size.x; x++ ) {
                Vector3 Pos = ( new Vector3( x, 0, 0 ) * 36 );
                GameObject NewSlot = (GameObject)Instantiate( InventorySlotPrefab, Pos, Quaternion.identity, SubSlots[y].transform );
                NewSlot.transform.localPosition = Pos;
                NewSlot.name = "Slot: " + (int)( x + ( y * Size.y ) );
                NewSlot.SetActive( true );
                InvSlots[index] = NewSlot.GetComponent<Hud_InventorySlot>();
                index++;
            }
        }
        ScroleInventory();
    }

    public void LoadHudInventory( inventory _inventory ) {
        if( !_inventory ) {
            return;
        }
        Inventory = _inventory;

        for( int i = 0; i < Inventory.items.Length; i++ ) {
            item Item = Inventory.items[i];
            Hud_InventorySlot Slot = InvSlots[i];
            Slot.SlotIcon.texture = Item.Icon_Image;
        }
    }

    public void ScroleInventory() {
        if( !SlotsGroup ) {
            return;
        }
        SlotsGroup.transform.localPosition = new Vector3( 0, SlotScroleSlider.value * 36, 0 );
        for( int i = 0; i < SubSlots.Length; i++ ) {
            if( i >= SlotScroleSlider.value && i <= SlotScroleSlider.value + 3 ) {
                SubSlots[i].SetActive( true );
            } else {
                SubSlots[i].SetActive( false );
            }
        }
    }

    public void InventorySlotClick( int _index ) {
        if( !Inventory ) {
            return;
        }
        if( DragItem.texture == null ) {
            //Grab
            if( Inventory.items[_index].name == "" || Inventory.items[_index].Icon_Image == null ) {
                return;
            }
            DragItem.enabled = true;
            DragItem.texture = Inventory.items[_index].Icon_Image;
            DragIndex = _index;
        } else {
            //Release
            string ItemData = Inventory.items[DragIndex].GetItemString();
            Inventory.RemoteSetItem( DragIndex, Inventory.items[_index].GetItemString() );
            Inventory.RemoteSetItem( _index, ItemData );
            DragItem.texture = null;
            DragItem.enabled = false;
            LoadHudInventory( Inventory );
        }
    }

    public void DropItem() {
        if( DragItem.texture != null ) {
            string ItemData = Inventory.items[DragIndex].GetItemString();
            Inventory.RemoteSpawnItemBeg( ItemData );
            Inventory.RemoteSetItem( DragIndex, "" );
            DragItem.texture = null;
            DragItem.enabled = false;
            LoadHudInventory( Inventory );
        }
    }
}
