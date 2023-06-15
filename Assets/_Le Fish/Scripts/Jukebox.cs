using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Jukebox : MonoBehaviour
{
    AudioClip[] currentPlaylist;
    int indexCurrentPlaylist;
    int indexCurrentMusic;
    string currentPlaylistName;
    string currentMusicName;

    private bool isShuffle, isFinished, isPaused;
    [SerializeField] TextMeshPro textMesh;
    [SerializeField] Music musicData;
    [SerializeField] AudioSource audioSource;
    [SerializeField] Image image;

    private void Start()
    {       
        // We start the jukebox by playing random sound of a random playlist
        indexCurrentPlaylist = 0;
        indexCurrentMusic = Random.Range(0, musicData.ListPlaylist[indexCurrentPlaylist].audioClips.Length);

        currentPlaylist = new AudioClip[musicData.ListPlaylist[indexCurrentPlaylist].audioClips.Length];
        musicData.ListPlaylist[indexCurrentPlaylist].audioClips.CopyTo(currentPlaylist, 0);        

        audioSource.clip = currentPlaylist[indexCurrentMusic];
        audioSource.Play();
        audioSource.loop = false;
        audioSource.volume = 0.2f;

        isShuffle = false;
        isFinished = false;
        isPaused = false;

        TextUpdate();
    }

    private void Update()
    {
        if (!audioSource.isPlaying && !isPaused)
            isFinished = true;

        if (!audioSource.isPlaying && isFinished)
            Skip(); 
    }

    public void PlayPause()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
            isPaused = true;
        }
        else
        {
            audioSource.Play();
            isPaused = false;
        }
    }

    public void Skip()
    {
        if (indexCurrentMusic++ >= currentPlaylist.Length - 1)
            indexCurrentMusic = 0;       

        audioSource.clip = currentPlaylist[indexCurrentMusic];
        audioSource.Play();

        audioSource.loop = false;
        isPaused = false;
        isFinished = false;

        TextUpdate();
    }

    public void Stop()
    {
        audioSource.Stop();
        isPaused = true;
    }

    public void ChangePlaylist()
    {
        if (indexCurrentPlaylist++ >= musicData.ListPlaylist.Count - 1)
            indexCurrentPlaylist = 0;

        currentPlaylist = musicData.ListPlaylist[indexCurrentPlaylist].audioClips;
        indexCurrentMusic = 0;

        audioSource.clip = currentPlaylist[indexCurrentMusic];
        audioSource.Play();

        TextUpdate();
    }

    public void ShufflePlaylist()
    {
        if (!isShuffle)
        {
            isShuffle = true;

            int n = currentPlaylist.Length;
            while (n > 1)
            {
                n--;
                int k = UnityEngine.Random.Range(0, n + 1);
                AudioClip temp = currentPlaylist[k];
                currentPlaylist[k] = currentPlaylist[n];
                currentPlaylist[n] = temp;
            }

            Color originalColor = new(1f, 1f, 1f, 1f);
            image.color = originalColor;
        }
        else
        {
            isShuffle = false;

            currentPlaylist = new AudioClip[musicData.ListPlaylist[indexCurrentPlaylist].audioClips.Length];
            musicData.ListPlaylist[indexCurrentPlaylist].audioClips.CopyTo(currentPlaylist, 0);

            for (int i = 0; i < currentPlaylist.Length; i++)
                Debug.Log("currentPlaylist[" + i + "] :" + currentPlaylist[i].name + "\n");

            Color newColor = new(0.0f, 0.0f, 0.0f, 0.9f);
            image.color = newColor;
        }
    }

    public void IncreaseVolume()
    {
        if (audioSource.volume + 0.1f > 1.0f)
            return;

        audioSource.volume += 0.1f;
        Debug.Log("Volume :" + audioSource.volume);
    }

    public void DecreaseVolume()
    {
        if (audioSource.volume - 0.1f < 0.0f)
            return;

        audioSource.volume -= 0.1f;
        Debug.Log("Volume :" + audioSource.volume);
    }

    private void TextUpdate()
    {
        currentPlaylistName = musicData.ListPlaylist[indexCurrentPlaylist].playlistName;
        currentMusicName = musicData.ListPlaylist[indexCurrentPlaylist].audioClips[indexCurrentMusic].name;

        textMesh.text = "<b>" + currentPlaylistName + "</b>\n" + "<size=1.0>" + currentMusicName + "</size>";
    }
}