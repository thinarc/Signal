using _Project.Scripts.Services;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.EntryPoints
{
    public class Game2EntryPoint : MonoBehaviour
    {
        [SerializeField] private TMP_InputField[] superterms;
        [SerializeField] private Transform termsWrap;
        [SerializeField] private TerminalPool termPool;
        [SerializeField] private Transform[] buttons;

        [SerializeField] private TextMeshProUGUI thanks;

        private bool _clicked;

        private async void Start()
        {
            termsWrap.DOScale(0, 0);
            termsWrap.DOScale(1, 0.1f).SetEase(Ease.InBounce);
            
            buttons[0].DOScale(0, 0);
            buttons[1].DOScale(0, 0);
            buttons[2].DOScale(0, 0);
            buttons[3].DOScale(0, 0);
            
            termPool.Initialize();
            foreach (var term in superterms) term.text = "";
            
            superterms[0].text = "You're not a user, you're a process.";
            await UniTask.Delay(2200);
            buttons[0].DOScale(1, 0.2f).SetEase(Ease.Flash);
            buttons[1].DOScale(1, 0.2f).SetEase(Ease.InFlash);
            await UniTask.WaitUntil(() => _clicked);
            buttons[0].GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
            _clicked = false;
            await termPool.CreateTerminalAsync("When you look at the screen, who's looking at whom?", "When you look at the screen, who's looking at whom?");
            await UniTask.Delay(700);
            superterms[1].text = "You're not a human, you're a signal.";
            await UniTask.Delay(2400);
            buttons[2].DOScale(1, 0.2f).SetEase(Ease.Flash);
            buttons[3].DOScale(1, 0.2f).SetEase(Ease.InFlash);
            await UniTask.WaitUntil(() => _clicked);
            buttons[2].GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
            await termPool.CreateTerminalAsync("Every byte is you. Just distracted.", "Every byte is you. Just distracted.");
            _clicked = false;
            
            superterms[2].text = "You think the network is outside of you. You're wrong.";
            await UniTask.Delay(1200);
            await termPool.CreateTerminalAsync("confirm", "confirm.");
            await UniTask.Delay(300);
            await termPool.CreateTerminalAsync("confirm", "confirm..");
            await UniTask.Delay(400);
            await termPool.CreateTerminalAsync("confirm", "no matter where you go\neveryone is always in touch.");
            await termPool.CreateTerminalAsync("confirm", "confirm...");
            await UniTask.Delay(200);
            await termPool.CreateTerminalAsync("confirm", "confirm....");
            
            await UniTask.Delay(1100);
            
            superterms[3].text = "> whoami";
            await UniTask.Delay(2500);
            superterms[4].text = "SIGNAL";
            await termPool.CreateTerminalAsync("confirm", "SIGNAL");
            await UniTask.Delay(1500);
            superterms[5].text = "> connect self";
            await termPool.CreateTerminalAsync("confirm", "connection1.");
            await termPool.CreateTerminalAsync("confirm", "connection2..");
            await termPool.CreateTerminalAsync("confirm", "connection3...");
            await termPool.CreateTerminalAsync("confirm", "connection4....");
            await UniTask.Delay(2200);
            superterms[6].text = "CONNECTION ESTABLISHED";
            
            await UniTask.Delay(2400);
            superterms[7].text = "\n";
            superterms[8].text = "> CONNECTION: SELF ESTABLISHED.";
            await UniTask.Delay(2500);
            superterms[9].text = "> END OF TRANSMISSION.";
            await UniTask.Delay(2000);
            await termPool.CreateTerminalAsync("confirm", "bye" + " _" + SoundManager.Instance.applyName.ToLower());
            await UniTask.Delay(1500);
            
            termsWrap.DOScale(10, 0.034f).SetEase(Ease.Flash);
            await UniTask.Delay(1400);
            thanks.gameObject.SetActive(true);
        }

        public void Click()
        {
            _clicked = true;
        }
    }
}