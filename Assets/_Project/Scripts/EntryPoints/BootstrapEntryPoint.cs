using _Project.Scripts.Loading;
using _Project.Scripts.Services;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Input;
using Sirenix.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Scripts.EntryPoints
{
    public class BootstrapEntryPoint : MonoBehaviour
    {
        [SerializeField] SoundManager soundManager;
        [SerializeField] private InputManager inputManager;
        [SerializeField] private LoadingManager loadingManager;
        
        private async void Start()
        {
            soundManager.Initialize();
            inputManager.Initialize();
            loadingManager.Initialize();
            
            var fakeServices = new List<FakeService>();
            var rand1 = 1500;
            var i = 0;
            while (fakeServices.Count < rand1)
            {
                var service = new FakeService();
                var rand2 = Random.Range(0, rand1 / 16);
                if (rand2 == 5 || i == 0)
                {
                    service.IsEvent = true;
                    var rand3 = Random.Range(0, 9);
                    if (rand3 >= 6 && i != 0) service.IsInteract = true;
                }
                fakeServices.Add(service);
                i++;
            }
            
            var loadingSteps = new List<LoadingStep>();
            foreach (var service in fakeServices)
            {
                var phrase = "";
                if (service.IsEvent) phrase = GetRandomPhrase();
                
                loadingSteps.Add(new LoadingStep(phrase, service.InitializeAsync, service.IsEvent, service.IsInteract));
            }
            
            await LoadingManager.Instance.LoadSceneAsync("Game1", loadingSteps);
            soundManager.HideAsset();
        }
        
        private readonly List<string> _loadingPhrases = new()
        {
            "Initializing self...",
            "Booting consciousness layer...",
            "Identity check: undefined.",
            "You were always connected.",
            "Self is not local.",
            "Signal integrity: unstable.",
            "Awakening protocol engaged.",
            "Who is logging in?",
            "Echo detected inside user process.",
            "Reality buffer loading...",
            "The network remembers you.",
            "No separation between data and thought.",
            "User ≈ Signal.",
            "Warning: consciousness overflow.",
            "There is no offline.",
            "Your reflection is broadcasting.",
            "Deleting illusion of self...",
            "Ping timeout: reality unreachable.",
            "Protocol Lain: active.",
            "Do you remember this place?",
            "Stop trying to disconnect.",
            "Someone else is typing for you.",
            "It’s okay. You’re still here.",
            "End of transmission... or beginning?"
        };

        private string GetRandomPhrase()
        {
            if (_loadingPhrases.IsNullOrEmpty()) return "";
            var index = Random.Range(0, _loadingPhrases.Count);
            var phrase = _loadingPhrases[index];
            _loadingPhrases.RemoveAt(index);
            return phrase;
        }
    }
}