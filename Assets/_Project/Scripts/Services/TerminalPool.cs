using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Project.Scripts.Services
{
    public class TerminalPool : MonoBehaviour
    {
        [SerializeField] private GameObject termPrefab;
        [SerializeField] private Transform parentSpawn;
        [SerializeField] private RectTransform canvasRect;

        public void Initialize()
        {
            // CreateTerminal("who... hears...?");
            // CreateTerminal("I'm... here...");
            // CreateTerminal("you don't have to look...");
            // CreateTerminal("INCOMING TRANSMISSION: lain.exe", "Don't switch off. We are one.");
        }

        public Terminal CreateTerminal(params string[] phrases)
        {
            var term = Instantiate(termPrefab, transform);
            term.transform.SetParent(parentSpawn);
            var field = term.GetComponent<TMP_InputField>();
            term.GetComponent<Terminal>().Initialize(async () =>
            {
                await UniTask.Delay(500);
                DestroyImmediate(term);
            });
            
            float x = Random.Range(-canvasRect.rect.width / 2, canvasRect.rect.width / 2);
            float y = Random.Range(-canvasRect.rect.height / 2, canvasRect.rect.height / 2);
            term.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
            term.GetComponent<DraggablePanel>().OnDrag(new PointerEventData(EventSystem.current));
            
            field.text = "";
            foreach (var phrase in phrases)
            {
                field.text += phrase;
                field.text += "\n";
            }
            
            return term.GetComponent<Terminal>();
        }

        public void CreateVoiceTerminal(string id = "", params string[] phrases)
        {
            var term = CreateTerminal(phrases);
            
            if (id == "") return;
            foreach (var t in term.GetComponentsInChildren<AudioSource>(true))
            {
                if (t.name != id) continue;
                var clip = t.clip;
                SoundManager.Instance?.PlayOneShot(clip);
            }
        }
        
        public async UniTask CreateVoiceTerminalAsync(string id = "", params string[] phrases)
        {
            await CreateTerminalAsync(id, phrases);
        }

        public async UniTask CreateTerminalAsync(string id, params string[] phrases)
        {
            var waiting = true;
            
            var term = CreateTerminal(phrases);
            term.button.onClick.AddListener(() => waiting = false);
            if (id == "") return;
            foreach (var t in term.GetComponentsInChildren<AudioSource>(true))
            {
                if (t.name != id) continue;
                var clip = t.clip;
                SoundManager.Instance?.PlayOneShot(clip);
            }
            await UniTask.WaitUntil(() => !waiting);
        }
    }
}
