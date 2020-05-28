using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RosterSave
{
    List<MinionSave> roster;

    public RosterSave(List<MinionSave> inRoster)
    {
        roster = inRoster;
    }

    public List<MinionSave> getMinionSaveList()
    {
        return roster;
    }
}
