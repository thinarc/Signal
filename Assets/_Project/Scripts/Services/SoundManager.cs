using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Services
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }
        
        [FormerlySerializedAs("_audioSource")] public AudioSource audioSource;
        [FormerlySerializedAs("_audioSources")] [SerializeField] private AudioSource[] audioSources;
        private float _sourceStartValue;
        private readonly List<float> _sourcesStartValues = new();
        
        public AudioClip hardButtonSound;
        public AudioClip[] lowButtonTypeSound;

        public AudioClip random;
        public AudioClip tone;

        public AudioClip bg2;
        public AudioClip bg3;

        [SerializeField, Space(5)] private TMP_InputField volume;
        [SerializeField, Space(5)] private TMP_InputField nickname;
        public string applyName;

        public void Initialize()
        {
            applyName = "unknown";
            
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

        public async void Play(AudioClip clip)
        {
            while (audioSource.volume > 0.1f)
            {
                audioSource.volume = Mathf.Lerp(audioSource.volume, 0, Time.deltaTime * 4);
                await UniTask.Yield();
            }
            
            audioSource.clip = clip;
            audioSource.Play();
            
            while (audioSource.volume < _sourceStartValue - 0.04f)
            {
                audioSource.volume = Mathf.Lerp(audioSource.volume, _sourceStartValue, Time.deltaTime * 1.5f);
                await UniTask.Yield();
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
            if (nickname.text == "") nickname.text = "unknown";
            applyName = nickname.text;
            nickname.text = "_" + nickname.text.ToLower();
        }

        public void HideAsset()
        {
            Destroy(volume.gameObject);
            Destroy(nickname.gameObject);
        }
    }
}