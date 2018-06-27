using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class skills : NetworkBehaviour {
    /*
     * 
     * 
       Class        STR 	    DEX	        CON	        INT	        PSI	
                MAX	MIN 	MAX	MIN	    MAX	MIN	    MAX	MIN	    MAX	MIN
Private Eye	    60	3	    80	3	    65	3	    60	3	    35	1	
        Spy	    40	2	    100	3	    40	2	    100	5	    20	1	
       Tank     100	5	    75	2	    100	4	    25	1	    0   0
    PSI Monk    20	1	    35	2	    45	1	    100	4	    100	5	

        intelligence
            Hacking
            Barter
            Psi Use
            Weapon Lore
            Construction
            Research
            Implant
            Willpower

        strength
            Melee Combat
            Heavy Combat
            Transport
            Resist Piercing
            Resist Force

        constitution
            Athletics
            Body Health
            Endurance
            Resist Energy
            Resist Fire
            Resist X-Ray
            Resist Poisen

        dexterity
            Pistol Combat
            Rifle Combat
            Hightech Combat
            Vehicle Use
            Agility
            Repair
            Recycle
            Remote Control

        psychic power
            Passive Psi Use
            Agressive Psi Use
            Focussing
            Psi Power
            Psi Resist
     */

    public player Player;

    public enum _Class {
        Spy = 0,
        PrivateEye = 1,
        Tank = 2,
        PsiMonk = 3
    };

    public _Class Class;
    public skill[] Skills;

    private void Start() {

        //        Class     STR          DEX         CON         INT         PSI
        //                MAX MIN     MAX MIN     MAX MIN     MAX MIN     MAX MIN
        //Private Eye     60  3       80  3       65  3       60  3       35  1
        //        Spy     40  2       100 3       40  2       100 5       20  1
        //       Tank     100 5       75  2       100 4       25  1       0   0
        //    PSI Monk    20  1       35  2       45  1       100 4       100 5
        //if( isLocalPlayer ) {
        //    CmdLoadSkills();
        //}
        
    }

    //Save
    public void RemoteSaveSkills() {
        if( Player.GetPlayerKey() != "" ) {
            CmdRemoteSaveSkills( Player.GetPlayerKey() + Class.ToString() );
        }
    }

    [Command]
    public void CmdRemoteSaveSkills( string PlayerName ) {
        if( !isServer ) {
            return;
        }
        SaveSkills( PlayerName );
    }

    public void SaveSkills( string PlayerName ) {
        int ValueCount = 0;
        if( Skills.Length > 0 ) {
            for( int i = 0; i < Skills.Length; i++ ) {
                if( Skills[i] != null ) {
                    ValueCount += 3;
                    if( Skills[i].SubSkill.Length > 0 ) {
                        ValueCount += Skills[i].SubSkill.Length;
                    }
                }
            }
        }

        string[] PlayerSaveData = new string[ValueCount];
        int saveIndex = 0;
        if( Skills.Length > 0 ) {
            for( int i = 0; i < Skills.Length; i++ ) {
                if( Skills[i] != null ) {
                    PlayerSaveData[saveIndex] = Skills[i].level.ToString();
                    saveIndex++;
                    PlayerSaveData[saveIndex] = Skills[i].skillpoints.ToString();
                    saveIndex++;
                    PlayerSaveData[saveIndex] = Skills[i].current_exp.ToString();
                    saveIndex++;
                    if( Skills[i].SubSkill.Length > 0 ) {
                        for( int o = 0; o < Skills[i].SubSkill.Length; o++ ) {
                            PlayerSaveData[saveIndex] = Skills[i].SubSkill[o].value.ToString();
                            saveIndex++;
                        }
                    }
                }
            }
        }

        File.WriteAllLines( GetSkillsPath( PlayerName ) + ".txt", PlayerSaveData );
    }


    // Load
    public void RemoteLoadSkills( ) {
        if( Player.GetPlayerKey() != "" ) {
            CmdLoadSkills( Player.GetPlayerKey() + Class.ToString() );
        }
    }

    [Command]
    public void CmdLoadSkills( string PlayerName ) {
        if( !isServer ) {
            return;
        }
        LoadSkills( PlayerName );
    }

    public void LoadSkills( string PlayerName ) {
        string IDPath = GetSkillsPath( PlayerName ) + ".txt";
        string[] PlayerLoadData = null;
        if( File.Exists( IDPath ) ) {
            PlayerLoadData = File.ReadAllLines( IDPath );
        } else {
            if( _Class.Spy == Class ) {
                SetSkillLevel( "Intelligence", 5, true );
                SetSkillLevel( "Strength", 2, true );
                SetSkillLevel( "Constitution", 2, true );
                SetSkillLevel( "Dexterity", 3, true );
                SetSkillLevel( "Psi Power", 1, true );
            }
            if( _Class.PrivateEye == Class ) {
                SetSkillLevel( "Intelligence", 3, true );
                SetSkillLevel( "Strength", 3, true );
                SetSkillLevel( "Constitution", 3, true );
                SetSkillLevel( "Dexterity", 3, true );
                SetSkillLevel( "Psi Power", 1, true );
            }
            if( _Class.Tank == Class ) {
                SetSkillLevel( "Intelligence", 1, true );
                SetSkillLevel( "Strength", 5, true );
                SetSkillLevel( "Constitution", 4, true );
                SetSkillLevel( "Dexterity", 2, true );
                SetSkillLevel( "Psi Power", 0, true );
            }
            if( _Class.PsiMonk == Class ) {
                SetSkillLevel( "Intelligence", 4, true );
                SetSkillLevel( "Strength", 1, true );
                SetSkillLevel( "Constitution", 1, true );
                SetSkillLevel( "Dexterity", 2, true );
                SetSkillLevel( "Psi Power", 5, true );
            }
            SaveSkills( PlayerName );
            return;
        }
       
        int ValueCount = 0;
        if( Skills.Length > 0 ) {
            for( int i = 0; i < Skills.Length; i++ ) {
                if( Skills[i] != null ) {
                    SetSkillLevel(Skills[i].name, int.Parse( PlayerLoadData[ValueCount] ), false );
                    ValueCount++;
                    SetSkillPoints( Skills[i].name, int.Parse( PlayerLoadData[ValueCount] ) );
                    ValueCount++;
                    SetSkillCurrentExp( Skills[i].name, int.Parse( PlayerLoadData[ValueCount] ) );
                    ValueCount++;
                    if( Skills[i].SubSkill.Length > 0 ) {
                        for( int o = 0; o < Skills[i].SubSkill.Length; o++ ) {
                            SetSubSkillValue( Skills[i].SubSkill[o].name, int.Parse( PlayerLoadData[ValueCount] ) );
                            ValueCount++;
                        }
                    }
                }
            }
        }
    }


    public string GetSkillsPath( string PlayerName ) {
        if( PlayerName == "" ) {
            PlayerName = "fail";
        }
        if( !Directory.Exists( Application.dataPath + "/StreamingAssets/PlayerData/" ) ) {
            Directory.CreateDirectory( Application.dataPath + "/StreamingAssets/PlayerData/" );
        }
        return Application.dataPath + "/StreamingAssets/PlayerData/" + PlayerName + "_Skills";
    }

    public int GetSkillLevel( string SkillName ) {
        if( Skills.Length > 0 ) {
            for( int i = 0; i < Skills.Length; i++ ) {
                if( Skills[i].name.ToLower() == SkillName.ToLower() ) {
                    return Skills[i].level;
                }
            }
        }
        return -1;
    }

    public int GetSkillCurrentExp( string SkillName ) {
        if( Skills.Length > 0 ) {
            for( int i = 0; i < Skills.Length; i++ ) {
                if( Skills[i].name.ToLower() == SkillName.ToLower() ) {
                    return Skills[i].current_exp;
                }
            }
        }
        return -1;
    }

    public int GetSkillNextLevelExp( string SkillName ) {
        if( Skills.Length > 0 ) {
            for( int i = 0; i < Skills.Length; i++ ) {
                if( Skills[i].name.ToLower() == SkillName.ToLower() ) {
                    return Skills[i].next_level_exp;
                }
            }
        }
        return -1;
    }

    public int GetSkillPoints( string SkillName ) {
        if( Skills.Length > 0 ) {
            for( int i = 0; i < Skills.Length; i++ ) {
                if( Skills[i].name.ToLower() == SkillName.ToLower() ) {
                    return Skills[i].skillpoints;
                }
            }
        }
        return -1;
    }

    public string GetSkillDescription( string SkillName ) {
        if( Skills.Length > 0 ) {
            for( int i = 0; i < Skills.Length; i++ ) {
                if( Skills[i].name.ToLower() == SkillName.ToLower() ) {
                    return Skills[i].description;
                }
            }
        }
        return "";
    }

    public void AddExpToSkill( string SkillName, int AddExp ) {
        CmdAddExpToSkill( SkillName, AddExp );
    }
    [Command]
    void CmdAddExpToSkill( string SkillName, int AddExp ) {
        RpcAddExpToSkill( SkillName, AddExp );
    }
    [ClientRpc]
    void RpcAddExpToSkill( string SkillName, int AddExp ) {
        if( Skills.Length > 0 ) {
            for( int i = 0; i < Skills.Length; i++ ) {
                if( Skills[i].name.ToLower() == SkillName.ToLower() ) {
                    Skills[i].addExp( AddExp );
                }
            }
        }
    }

    public void SetSkillLevel( string SkillName, int Level, bool addSkillPoints ) {
        CmdSetSkillLevel( SkillName, Level, addSkillPoints );
    }
    [Command]
    void CmdSetSkillLevel( string SkillName, int Level, bool addSkillPoints ) {
        RpcSetSkillLevel( SkillName, Level, addSkillPoints );
    }
    [ClientRpc]
    void RpcSetSkillLevel( string SkillName, int Level, bool addSkillPoints ) {
        if( Skills.Length > 0 ) {
            for( int i = 0; i < Skills.Length; i++ ) {
                if( Skills[i].name.ToLower() == SkillName.ToLower() ) {
                    Skills[i].level = Level;
                    if( addSkillPoints ) {
                        Skills[i].skillpoints = Level * 5;
                    }                    
                    Skills[i].next_level_exp = Skills[i].next_level_exp_base * Level;
                }
            }
        }
    }

    public void SetSkillPoints( string SkillName, int points ) {
        CmdSetSkillPoints( SkillName, points );
    }
    [Command]
    public void CmdSetSkillPoints( string SkillName, int points ) {
        RpcSetSkillPoints( SkillName, points );
    }
    [ClientRpc]
    public void RpcSetSkillPoints( string SkillName, int points ) {
        if( Skills.Length > 0 ) {
            for( int i = 0; i < Skills.Length; i++ ) {
                if( Skills[i].name.ToLower() == SkillName.ToLower() ) {
                    Skills[i].skillpoints = points;
                    return;
                }
            }
        }
    }

    public int GetSubSkillValue( string SubSkillName ) {
        if( Skills.Length > 0 ) {
            for( int i = 0; i < Skills.Length; i++ ) {
                if( Skills[i] != null ) {
                    if( Skills[i].SubSkill.Length > 0 ) {
                        for( int o = 0; o < Skills[i].SubSkill.Length; o++ ) {
                            if( Skills[i].SubSkill[o].name.ToLower() == SubSkillName.ToLower() ) {
                                return Skills[i].SubSkill[o].value;
                            }
                        }
                    }
                }
            }
        }
        return -1;
    }

    public void AddSubSkillValue( string SubSkillName ) {
        CmdAddSubSkillValue( SubSkillName );
    }
    [Command]
    void CmdAddSubSkillValue( string SubSkillName ) {
        RpcAddSubSkillValue( SubSkillName );
    }
    [ClientRpc]
    void RpcAddSubSkillValue( string SubSkillName ) {
        if( Skills.Length > 0 ) {
            for( int i = 0; i < Skills.Length; i++ ) {
                if( Skills[i] != null ) {
                    if( Skills[i].SubSkill.Length > 0 ) {
                        for( int o = 0; o < Skills[i].SubSkill.Length; o++ ) {
                            if( Skills[i].SubSkill[o].name.ToLower() == SubSkillName.ToLower() ) {
                                int _SubSkillValue = Skills[i].SubSkill[o].value;
                                if( _SubSkillValue <= 50 ) {
                                    if( Skills[i].skillpoints < 1 ) {
                                        return;
                                    }
                                    Skills[i].skillpoints -= 1;
                                }
                                if( _SubSkillValue >= 51 && _SubSkillValue <= 75 ) {
                                    if( Skills[i].skillpoints < 2 ) {
                                        return;
                                    }
                                    Skills[i].skillpoints -= 2;
                                }
                                if( _SubSkillValue >= 76 && _SubSkillValue <= 100 ) {
                                    if( Skills[i].skillpoints < 3 ) {
                                        return;
                                    }
                                    Skills[i].skillpoints -= 3;
                                }
                                if( _SubSkillValue >= 101 ) {
                                    if( Skills[i].skillpoints < 5 ) {
                                        return;
                                    }
                                    Skills[i].skillpoints -= 5;
                                }
                                Skills[i].SubSkill[o].value++;
                            }
                        }
                    }
                }
            }
        }
    }

    public void SetSkillCurrentExp( string SkillName, int points ) {
        CmdSetSkillCurrentExp( SkillName, points );
    }
    [Command]
    public void CmdSetSkillCurrentExp( string SkillName, int points ) {
        RpcSetSkillCurrentExp( SkillName, points );
    }
    [ClientRpc]
    public void RpcSetSkillCurrentExp( string SkillName, int points ) {
        if( Skills.Length > 0 ) {
            for( int i = 0; i < Skills.Length; i++ ) {
                if( Skills[i].name.ToLower() == SkillName.ToLower() ) {
                    Skills[i].current_exp = points;
                    return;
                }
            }
        }
    }


    public void SetSubSkillValue( string SubSkillName, int Value ) {
        CmdSetSubSkillValue( SubSkillName, Value );
    }
    [Command]
    void CmdSetSubSkillValue( string SubSkillName, int Value ) {
        RpcSetSubSkillValue( SubSkillName, Value );
    }
    [ClientRpc]
    void RpcSetSubSkillValue( string SubSkillName, int Value ) {
        if( Skills.Length > 0 ) {
            for( int i = 0; i < Skills.Length; i++ ) {
                if( Skills[i] != null ) {
                    if( Skills[i].SubSkill.Length > 0 ) {
                        for( int o = 0; o < Skills[i].SubSkill.Length; o++ ) {
                            if( Skills[i].SubSkill[o].name.ToLower() == SubSkillName.ToLower() ) {
                                Skills[i].SubSkill[o].value = Value;
                            }
                        }
                    }
                }
            }
        }
    }

    
    public void DebugAddExpToAll() {
        AddExpToSkill( "Intelligence", Random.Range(100,300) );
        AddExpToSkill( "Strength", Random.Range( 100, 300 ) );
        AddExpToSkill( "Constitution", Random.Range( 100, 300 ) );
        AddExpToSkill( "Dexterity", Random.Range( 100, 300 ) );
        AddExpToSkill( "Psi Power", Random.Range( 100, 300 ) );
    }
   
}

    [System.Serializable]
public class skill {
    public string name;
    public int level;
    public int current_exp;
    public int next_level_exp;
    public int next_level_exp_base;
    public int skillpoints;
    public string description;
    public subSkill[] SubSkill;

    public void addExp( int exp ) {
        if( level <= 0 ) {
            return;
        }
        current_exp += Mathf.Abs( exp );
        if( current_exp > next_level_exp ) {
            current_exp = 0;
            level++;
            skillpoints += 5;
            next_level_exp = next_level_exp_base * level;
        }
    }
}

[System.Serializable]
public class subSkill {
    public string name;
    public int value;
}

