using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hud_Skillbar : MonoBehaviour {
    public Hud hud;
    public string SkillName;
    public RectTransform skillbar;
    public Text skillExp;
    public Text skillLevel;
    public Text skillpoints;
    
    public void LoadSkillBar() {
        if( !hud.Skills ) {
            return;
        }

        skillLevel.text = hud.Skills.GetSkillLevel( SkillName ).ToString();
        int current_Exp = hud.Skills.GetSkillCurrentExp( SkillName );
        int next_Exp = hud.Skills.GetSkillNextLevelExp( SkillName );

        float skillbarScale = 0f;
        if( next_Exp > 0 ) {
            skillbarScale = (float)current_Exp / (float)next_Exp;
        }
        skillbar.localScale = new Vector3( skillbarScale, 1,1);

        skillExp.text = current_Exp.ToString() + " / " + next_Exp.ToString();
        skillpoints.text = hud.Skills.GetSkillPoints( SkillName ).ToString();
    }

}
