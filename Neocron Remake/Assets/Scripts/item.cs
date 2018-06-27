using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class item : MonoBehaviour {
    public string Item_Name;
    public int Tech_Level;
    public float Weight;
    public Texture Icon_Image;
    public Texture[] Icons;
    // Stats
    public int Stats_Condition_current;
    public int Stats_Condition_maximum = 120;
    public int Stats_Damage;
    public int Stats_Frequency;
    public int Stats_Handling;
    public int Stats_Range;
    // Details
    public enum Types {
        Melee = 0,
        Pistol = 1,
        Rifle = 2,
        Cannon = 3,
        Psi = 4,
        Useable = 5,
        Armor = 6
    }
    public Types Details_Item_Type;
    public enum FireModes {
        Single = 0,
        Burst = 1
    }
    public FireModes Details_Fire_Mode;
    public int Details_Damage;
    public int Details_Aiming;
    public int Details_Shot_Frequency;
    public int Details_Recoil;
    public int Details_Range;
    public bool Details_Mod_Flashlight;
    public bool Details_Mod_Scope;
    public bool Details_Mod_Silencer;
    public bool Details_Mod_Laserpointer;

    public string GetItemString() {
        string returnData = "";
        if( Item_Name == "" ) {
            return returnData;
        }
        returnData += Item_Name + "|";
        returnData += Tech_Level.ToString() + "|";
        returnData += Weight.ToString() + "|";

        returnData += Stats_Condition_current.ToString() + "|";
        returnData += Stats_Damage.ToString() + "|";
        returnData += Stats_Frequency.ToString() + "|";
        returnData += Stats_Handling.ToString() + "|";
        returnData += Stats_Range.ToString() + "|";
        
        returnData += Details_Item_Type + "|";
        returnData += Details_Fire_Mode + "|";
        returnData += Details_Damage.ToString() + "|";
        returnData += Details_Aiming.ToString() + "|";
        returnData += Details_Shot_Frequency.ToString() + "|";
        returnData += Details_Recoil.ToString() + "|";
        returnData += Details_Range.ToString() + "|";

        returnData += Details_Mod_Flashlight.ToString() + "|";
        returnData += Details_Mod_Scope.ToString() + "|";
        returnData += Details_Mod_Silencer.ToString() + "|";
        returnData += Details_Mod_Laserpointer.ToString() + "|";

        if( Icon_Image != null ) {
            returnData += Icon_Image.name.ToLower() + "|";
            //Debug.Log( Icon_Image.name.ToLower() );
        } else {
            returnData += "none" + "|";
        }
        return returnData;
    }

    public void SetItemFromString( string ItemData ) {
        string[] ItemDataSplit = ItemData.Split( '|' );
        if( ItemDataSplit.Length < 19 ) {
            ResetItem();
            //Debug.Log( "ItemDataSplit.Length < 20" );
            return;
        }
        Item_Name = ItemDataSplit[0];
        Tech_Level = int.Parse( ItemDataSplit[1] );
        Weight = float.Parse( ItemDataSplit[2] );

        Stats_Condition_current = int.Parse( ItemDataSplit[3] );
        Stats_Damage = int.Parse( ItemDataSplit[4] );
        Stats_Frequency = int.Parse( ItemDataSplit[5] );
        Stats_Handling = int.Parse( ItemDataSplit[6] );
        Stats_Range = int.Parse( ItemDataSplit[7] );

        string ItemTypeIndex = ItemDataSplit[8].ToLower();
        if( ItemTypeIndex.Contains("melee") ) {
            Details_Item_Type = Types.Melee;
        }
        if( ItemTypeIndex.Contains( "pistol" ) ) {
            Details_Item_Type = Types.Pistol;
        }
        if( ItemTypeIndex.Contains( "rifle" ) ) {
            Details_Item_Type = Types.Rifle;
        }
        if( ItemTypeIndex.Contains( "cannon" ) ) {
            Details_Item_Type = Types.Cannon;
        }
        if( ItemTypeIndex.Contains( "psi" ) ) {
            Details_Item_Type = Types.Psi;
        }
        if( ItemTypeIndex.Contains( "useable" ) ) {
            Details_Item_Type = Types.Useable;
        }
        if( ItemTypeIndex.Contains( "armor" ) ) {
            Details_Item_Type = Types.Armor;
        }

        string FireModesIndex = ItemDataSplit[9].ToLower();
        if( FireModesIndex.Contains( "single" ) ) {
            Details_Fire_Mode = FireModes.Single;
        }
        if( FireModesIndex.Contains( "burst" ) ) {
            Details_Fire_Mode = FireModes.Burst;
        }

        Details_Damage = int.Parse( ItemDataSplit[10] );
        Details_Aiming = int.Parse( ItemDataSplit[11] );
        Details_Shot_Frequency = int.Parse( ItemDataSplit[12] );
        Details_Recoil = int.Parse( ItemDataSplit[13] );
        Details_Range = int.Parse( ItemDataSplit[14] );

        if( ItemDataSplit[15].ToLower().Contains( "true" ) ) {
            Details_Mod_Flashlight = true;
        } else {
            Details_Mod_Flashlight = false;
        }

        if( ItemDataSplit[16].ToLower().Contains( "true" ) ) {
            Details_Mod_Scope = true;
        } else {
            Details_Mod_Scope = false;
        }

        if( ItemDataSplit[17].ToLower().Contains( "true" ) ) {
            Details_Mod_Silencer = true;
        } else {
            Details_Mod_Silencer = false;
        }

        if( ItemDataSplit[18].ToLower().Contains( "true" ) ) {
            Details_Mod_Laserpointer = true;
        } else {
            Details_Mod_Laserpointer = false;
        }

        for( int i = 0; i < Icons.Length; i++ ) {
            if( Icons[i].name.ToLower() == ItemDataSplit[19].ToLower() ) {
                Icon_Image = Icons[i];
                //Debug.Log( ItemDataSplit[19].ToLower() );
                break;
            }
        }
    }


    public void ResetItem() {     
        Item_Name = "";
        Tech_Level = 0;
        Weight = 0;

        Stats_Condition_current = 0;
        Stats_Damage = 0;
        Stats_Frequency = 0;
        Stats_Handling = 0;
        Stats_Range = 0;

        Details_Item_Type = Types.Melee;

        Details_Fire_Mode = FireModes.Single;

        Details_Damage = 0;
        Details_Aiming = 0;
        Details_Shot_Frequency = 0;
        Details_Recoil = 0;
        Details_Range = 0;

        Details_Mod_Flashlight = false;

        Details_Mod_Scope = false;

        Details_Mod_Silencer = false;

        Details_Mod_Laserpointer = false;

        Icon_Image = null;
    }

    private void Start() {

    }
}
