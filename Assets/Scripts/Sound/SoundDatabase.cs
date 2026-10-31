using System;
using System.Collections.Generic;
using UnityEngine;

public enum eSoundID
{
    none,
    ButtonClick,
    FootStep,
    DoorOpenClose,
    Background_Floor1,
    Background_Floor2,
    Background_Floor3,
    Background_Floor4
}

[CreateAssetMenu(menuName = "Sound/SoundDatabase")]
public class SoundDatabase : ScriptableObject
{
    [Serializable]
    public class Entry
    {
        public eSoundID id;
        public SoundData data;
    }
    public List<Entry> entries;
    private Dictionary<eSoundID, SoundData> soundDataDict;
    public SoundData Get(eSoundID id)
    {
        if(soundDataDict == null)
        {
            soundDataDict = new Dictionary<eSoundID, SoundData>();
            foreach(var k in entries)
            {
                if(!soundDataDict.ContainsKey(k.id))
                {
                    soundDataDict.Add(k.id, k.data);
                }
            }
        }
        return soundDataDict.TryGetValue(id, out var data) ? data : null;
    }
}
