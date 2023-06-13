using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Music", menuName = "ScriptableObjects/Music")]
public class Music : ScriptableObject
{
    [Serializable]
    public class Playlist
    {
        public string playlistName;
        public AudioClip[] audioClips;
    }

    public List<Playlist> ListPlaylist = new();
}
