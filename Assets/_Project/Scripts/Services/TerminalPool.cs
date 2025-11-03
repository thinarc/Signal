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

        public async UniTask CreateTerminalAsync(params string[] phrases)
        {
            var waiting = true;
            
            var term = CreateTerminal(phrases);
            term.button.onClick.AddListener(() => waiting = false);
            await UniTask.WaitUntil(() => !waiting);
        }
    }
}
