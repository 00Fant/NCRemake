using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hud_InventorySlot : MonoBehaviour {

    public Hud_Inventory hud_inventory;
    public RawImage SlotIcon;

	void Start () {
		
	}

    public void SlotClick() {
        if( !hud_inventory ) {
            return;
        }
        int index = int.Parse( name.Replace( "Slot: ", "" ) );
        //Debug.Log( index );
        hud_inventory.InventorySlotClick( index );
    }

}
