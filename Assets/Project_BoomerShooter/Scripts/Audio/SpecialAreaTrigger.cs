using UnityEngine;
using BoomerShooter.Audio;

public class SpecialAreaTrigger : MonoBehaviour
{
    [SerializeField] private AudioClip specialMusicClip;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MusicManager.Instance.PlaySpecialMusic(specialMusicClip, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MusicManager.Instance.ReturnToDefaultPlaylist();
        }
    }
}