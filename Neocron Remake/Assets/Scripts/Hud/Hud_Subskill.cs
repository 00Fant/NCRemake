using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hud_Subskill : MonoBehaviour {
    public Hud hud;

    public Text SubSkillNameText;
    public Text SubSkillValueText;
    public Text SubSkillBonusValueText;

    public void LoadSubSkillValues() {
        if( !hud.Skills ) {
            return;
        }
        SubSkillValueText.text = hud.Skills.GetSubSkillValue( SubSkillNameText.text ).ToString();
    }

    public void AddValuePoint() {
        if( !hud.Skills ) {
            return;
        }
        hud.Skills.AddSubSkillValue( SubSkillNameText.text );
        hud.Stats.CalculateStats();
        hud.Invoke( "LoadHud", 0.05f );
    }

    
}
