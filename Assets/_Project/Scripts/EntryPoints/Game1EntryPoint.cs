using System.Collections.Generic;
using _Project.Scripts.Effects;
using _Project.Scripts.Frequency;
using _Project.Scripts.Loading;
using _Project.Scripts.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.EntryPoints
{
    public class Game1EntryPoint : MonoBehaviour
    {
        [SerializeField] private NoiseWave wave;
        [SerializeField] private GameObject prefabsFuturePreview;
        
        [SerializeField, Space(5)] private Slider frequencySlider;
        private FrequencyController _frequencyController;

        [SerializeField, Space(5)] private TerminalPool termPool;
        [SerializeField] private Animator background;
        
        private async void Start()
        {
            SoundManager.Instance.Play(SoundManager.Instance.bg2);
            background = GameObject.FindWithTag("Background").GetComponent<Animator>();
            
            wave.Initialize();
            prefabsFuturePreview.SetActive(false);

            var noiseSource = GameObject.FindWithTag("SoundManager").GetComponent<AudioSource>();
            var lowPass = noiseSource.gameObject.GetComponent<AudioLowPassFilter>();
            _frequencyController = new FrequencyController(frequencySlider, noiseSource, lowPass, wave);
            _frequencyController.Initialize();
            
            termPool.Initialize();
            termPool.CreateTerminal("Use this sphere to find the frequency.", "w is movable. can closed.");
            termPool.CreateTerminal("sound then reacts.");
            _frequencyController.BindTerminalPool(termPool);
            await _frequencyController.BindAwaitEvent();
            await UniTask.Delay(800);
            frequencySlider.interactable = false;
            await termPool.CreateTerminalAsync("passed", "passed");
            await LoadingManager.Instance.LoadSceneAsync("Game2", new List<LoadingStep>());
            ClearUi();
        }

        private void ClearUi()
        {
            background.SetTrigger("Out2");
        }

        private void Update()
        {
            wave.Tick();
            _frequencyController.Tick();
        }

        public void SkipScene()
        {
            ClearUi();
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Game2");
        }
    }
}