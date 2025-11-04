using System.Collections.Generic;
using _Project.Scripts.Input;
using _Project.Scripts.Services;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Cysharp.Threading.Tasks;
using TMPEffects.Components;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Loading
{
    public class LoadingManager : MonoBehaviour
    {
        public static LoadingManager Instance { get; private set; }
        
        [SerializeField] private GameObject loadingUi;
        [SerializeField] private  Slider progressBar;
        [SerializeField] private  TMP_Text statusText;
        
        [SerializeField, Space(5)] private CanvasGroup girl;
        [SerializeField, Space(5)] private CanvasGroup unfadeGirl;
        [SerializeField, Space(5)] private TMPWriter writer;

        public void Initialize()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else Destroy(gameObject);
        }

        private int _repeat;
        public async UniTask LoadSceneAsync(string sceneName, List<LoadingStep> steps)
        {
            var loadingSceneStep = new LoadingStep("Load Scene..", async () => await LoadScene(sceneName));
            steps.Add(loadingSceneStep);
            
            var stepFraction = 1f / steps.Count;
            var currentProgress = 0f;
            
            progressBar.value = currentProgress;
            if (SceneManager.GetActiveScene().buildIndex == 0 || SceneManager.GetActiveScene().buildIndex == 1) loadingUi.SetActive(true);
            else loadingUi.SetActive(false);
        
            foreach (var step in steps)
            {
                if (step.IsEvent && !step.IsInteract) TypeTextM(step.Description);
                else if (step.IsInteract && step.IsEvent)
                {
                    var wait = new InputAwait("Interact", girl, _repeat, unfadeGirl, _repeat >= 2);
                    TypeTextM(step.Description + " [space]");
                    await wait.InitializeAwait();
                    TypeTextM(statusText.text[..^" [space]".Length], true);
                    // statusText.text += "        `";
                    wait.Dispose();
                    _repeat++;
                    var sound = SoundManager.Instance;
                    if (sound != null)
                    {
                        sound.PlayOneShot(sound.hardButtonSound);
                    }
                }
                await step.ActionAsync();
                
                currentProgress += stepFraction;
                progressBar.value = currentProgress;
            }
            
            CloseUi();
            girl.gameObject.SetActive(false);
            unfadeGirl.gameObject.SetActive(false);
            statusText.gameObject.SetActive(false);
            loadingUi.SetActive(false);
        }

        public void CloseUi()
        {
            loadingUi.SetActive(false);
        }
        
        private async UniTask TypeText(string text)
        {
            statusText.text = "";
            foreach (var c in text)
            {
                if (statusText.text == text) return;
                statusText.text += c;
                var sound = SoundManager.Instance;
                if (sound != null && Random.Range(0, 7) == 4)
                {
                    sound.PlayOneShot(sound.lowButtonTypeSound[Random.Range(0, sound.lowButtonTypeSound.Length)]);
                }
                await UniTask.Yield();
            }
        }

        private void TypeTextM(string text, bool fast = false)
        {
            writer.enabled = !fast;
            statusText.text = text;
        }

        private static async UniTask LoadScene(string sceneName)
        {
            var asyncOp = SceneManager.LoadSceneAsync(sceneName);
            if (asyncOp == null) return;
            asyncOp.allowSceneActivation = false;
            
            while (asyncOp.progress < 0.9f)
            {
                await UniTask.Yield();
            }
            
            asyncOp.allowSceneActivation = true;
        }
    }
}