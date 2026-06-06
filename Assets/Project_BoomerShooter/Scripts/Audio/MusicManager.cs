using UnityEngine;
using System.Collections.Generic;

namespace BoomerShooter.Audio
{
    public class MusicManager : MonoBehaviour
    {
        private static MusicManager _instance;

        public static MusicManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<MusicManager>();
                }
                return _instance;
            }
        }

        [Header("Audio Source")]
        [SerializeField] private AudioSource musicSource;

        [Header("Default Playlist")]
        [SerializeField] private List<AudioClip> defaultPlaylist = new List<AudioClip>();

        private List<AudioClip> _currentPlaylist = new List<AudioClip>();
        private int _currentTrackIndex = 0;
        private bool _isSpecialMusicPlaying = false;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        private void Start()
        {
            if (defaultPlaylist.Count > 0)
            {
                PlayPlaylist(defaultPlaylist);
            }
        }

        private void Update()
        {
            if (musicSource != null && !musicSource.isPlaying && _currentPlaylist.Count > 0 && !_isSpecialMusicPlaying)
            {
                NextTrack();
            }
        }

        public void PlayPlaylist(List<AudioClip> newList)
        {
            _currentPlaylist = new List<AudioClip>(newList);
            _currentTrackIndex = 0;
            _isSpecialMusicPlaying = false;
            PlayTrack(_currentTrackIndex);
        }

        public void PlaySpecialMusic(AudioClip specialClip, bool loop = true)
        {
            if (specialClip == null || musicSource == null) return;

            _isSpecialMusicPlaying = true;
            musicSource.Stop();
            musicSource.clip = specialClip;
            musicSource.loop = loop;
            musicSource.Play();
        }

        public void ReturnToDefaultPlaylist()
        {
            if (defaultPlaylist.Count > 0)
            {
                PlayPlaylist(defaultPlaylist);
            }
        }

        private void PlayTrack(int index)
        {
            if (musicSource == null || _currentPlaylist.Count == 0) return;

            musicSource.Stop();
            musicSource.clip = _currentPlaylist[index];
            musicSource.loop = false;
            musicSource.Play();
        }

        private void NextTrack()
        {
            _currentTrackIndex = (_currentTrackIndex + 1) % _currentPlaylist.Count;
            PlayTrack(_currentTrackIndex);
        }
    }
}