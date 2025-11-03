using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Services
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }
        
        [FormerlySerializedAs("_audioSource")] [SerializeField] private AudioSource audioSource;
        [FormerlySerializedAs("_audioSources")] [SerializeField] private AudioSource[] audioSources;
        private float _sourceStartValue;
        private readonly List<float> _sourcesStartValues = new();
        
        public AudioClip hardButtonSound;
        public AudioClip[] lowButtonTypeSound;

        public AudioClip random;
        public AudioClip tone;

        [SerializeField, Space(5)] private TMP_InputField volume;
        [SerializeField, Space(5)] private TMP_InputField nickname;
        private string _applyName;

        public void Initialize()
        {
            if (Instance == null)
            {
                Instance = this;
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
            }
            else Destroy(gameObject);
            
            _sourceStartValue = audioSource.volume;
            _sourcesStartValues.Add(audioSources[0].volume);
            
            audioSource.volume = _sourceStartValue * 1f;
            for (var i = 0; i < audioSources.Length; i++)
            {
                audioSources[i].volume = _sourcesStartValues[0] * 0.75f;
            }
            
        }

        public void PlayOneShot(AudioClip clip)
        {
            audioSources[Random.Range(0, audioSources.Length)].PlayOneShot(clip);
        }

        public void OnEndEditVolume()
        {
            try
            {
                var value = decimal.Parse(volume.text);

                var background = (int)Math.Truncate(value);
                var sfx = (int)Math.Round((value - background) * 100);
                
                if (background > 100 || background < 0 || sfx > 100 || sfx < 0) throw new Exception();
                
                var back = background / 100f;
                var sfx2 = sfx / 100f;
                if (volume.text.EndsWith("100")) sfx2 = 1;
                audioSource.volume = _sourceStartValue * back;
                for (var i = 0; i < audioSources.Length; i++)
                {
                    audioSources[i].volume = _sourcesStartValues[0] * sfx2;
                }
            }
            catch
            {
                volume.text = "100.75";
                audioSource.volume = _sourceStartValue * 1f;
                for (var i = 0; i < audioSources.Length; i++)
                {
                    audioSources[i].volume = _sourcesStartValues[0] * 0.75f;
                }
            }

            volume.text = "=> " + volume.text;
        }

        public void OnEndEditNickname()
        {
            nickname.text = nickname.text.Replace("_", "");
            if (nickname.text == "") nickname.text = "wired";
            _applyName = nickname.text;
            nickname.text = "_" + nickname.text;
        }

        public void HideAsset()
        {
            Destroy(volume.gameObject);
            Destroy(nickname.gameObject);
        }
    }
}