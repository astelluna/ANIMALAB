using System;
using System.Collections.Generic;
using UnityEngine;

public enum eSoundID
{
    backgroundSound,
    ButtonClick,
    PootStep,
    DoorOpenClose,
}

public class SoundDatabase : MonoBehaviour
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
            foreach(var k in entries)
            {
                if(!soundDataDict.ContainsKey(k.id))
                {
                    soundDataDict.Add(id, k.data);
                }
            }
        }
        return soundDataDict.TryGetValue(id, out var data) ? data : null;
    }
}
