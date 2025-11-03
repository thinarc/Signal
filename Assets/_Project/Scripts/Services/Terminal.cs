using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace _Project.Scripts.Services
{
    public class Terminal : MonoBehaviour
    {
        [SerializeField] public Button button;

        public void Initialize(Action callback)
        {
            button.onClick.AddListener((() => callback()));
            button.onClick.AddListener(TriggerAnim);
            var startScale = transform.localScale;
            transform.localScale = Vector3.zero;
            transform.DOScale(startScale, 0.2f).SetEase(Ease.InQuad);
            SoundManager.Instance?.PlayOneShot(SoundManager.Instance?.tone);
        }

        public void TriggerAnim()
        {
            transform.DOScale(0, 0.2f).SetEase(Ease.Flash);
            SoundManager.Instance?.PlayOneShot(SoundManager.Instance?.random);
        }

        private void OnDisable()
        {
            button.onClick.RemoveAllListeners();
        }
    }
}
