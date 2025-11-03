using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Input
{
    public class InputAwait : IDisposable
    {
        private readonly string _actionName;
        private CanvasGroup _canvasGroup;

        private bool _disabled = false;
        private int _repeat = 0;

        private bool _exp;
        private CanvasGroup _girl2;
        
        public InputAwait(string actionName, CanvasGroup canvasGroup, int rep, CanvasGroup girl2, bool exp)
        {
            _actionName = actionName;
            _canvasGroup = canvasGroup;
            _repeat = rep;
            
            _girl2 = girl2;
            _exp = exp;
        }

        public async UniTask InitializeAwait()
        {
            SubscribeCallback();
            
            while (!_click)
            {
                await UniTask.Yield();
            }
            await UniTask.DelayFrame(1);
        }
        
        private bool _click;
        

        private void SubscribeCallback()
        {
            InputManager.Instance?.GetInput(_actionName, ClickEvent, true, false);
        }
        
        private void ClickEvent(InputAction.CallbackContext context)
        {
            if (_disabled) return;
            _click = true;
            
            if (_repeat >= 1)
            {
                DOTween.Kill("unfadegirl");
                DOTween.Kill("unfadedurat");
                _canvasGroup.DOFade(1f, 0.2f).SetEase(Ease.InFlash);
                if (!_exp) return;
            }

            if (_exp) _canvasGroup = _girl2;
            var unfadedurat = 0.7f;
            var tw = "unfadegirl";
            if (_exp) tw = "unfadedurat";
            if (_exp) unfadedurat = 1.1f;

            if (_canvasGroup == null)
            {
                Dispose();
                return;
            }
            _canvasGroup.DOFade(1f, 0.2f).SetEase(Ease.InFlash).OnComplete((() => _canvasGroup.DOFade(0, unfadedurat).SetEase(Ease.OutFlash))).SetId(tw);
            Dispose();
        }

        public void Dispose()
        {
            _disabled = true;
            InputManager.Instance?.ClearSubscribers();
        }
    }
}