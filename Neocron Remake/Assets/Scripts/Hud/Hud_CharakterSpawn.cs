using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Hud_CharakterSpawn : MonoBehaviour {

    public Dropdown ClassDropDown;

    public int ClassIndex;

    private void Start() {
        ClassIndex = PlayerPrefs.GetInt( "LastClass" );
        ClassDropDown.value = ClassIndex;
    }

    public void SetClassIndex() {
        ClassIndex = ClassDropDown.value;
        PlayerPrefs.SetInt( "LastClass" , ClassIndex );
    }

}
