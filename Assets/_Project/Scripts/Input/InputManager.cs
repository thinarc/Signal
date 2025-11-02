using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Input
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }
        private PlayerInput _input;
        
        private List<BindedAction> _actions;
        
        private bool _initialized;

        public void Initialize()
        {
            if (Instance == null)
            {
                Instance = this;
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
            }
            else Destroy(gameObject);
            
            _input = GetComponent<PlayerInput>();
            _actions = new List<BindedAction>();
            
            _initialized = true;
        }

        public InputAction GetInput(string actionName, Action<InputAction.CallbackContext> callback, bool performed = true, bool saveToDispose = true)
        {
            var action = _input.actions[actionName];
            if (action != null)
            {
                if (performed) action.performed += callback;
                else action.canceled += callback;
                
                if (saveToDispose) _actions.Add(new BindedAction
                {
                    Action = action,
                    Callback = callback,
                    Performed = performed
                });

                return action;
            }
            else Debug.LogWarning($"Action '{actionName}' not found.");

            return null;
        }
        
        public void GetAllInput(string actionName, Action<InputAction.CallbackContext> callback)
        {
            GetInput(actionName, callback);
            GetInput(actionName, callback, false);
        }
        
        private void OnEnable()
        {
            if (!_initialized) return;
            _input.enabled = true;
        }
        
        private void OnDisable()
        {
            _input.enabled = false;
            ClearSubscribers();
        }

        public void ClearSubscribers()
        {
            foreach (var bindedAction in _actions)
            {
                if (bindedAction.Performed) bindedAction.Action.performed -= bindedAction.Callback;
                else bindedAction.Action.canceled -= bindedAction.Callback;
            }
            _actions.Clear();
        }
    }
    
    public struct BindedAction
    {
        public InputAction Action;
        public Action<InputAction.CallbackContext> Callback;
        public bool Performed;
    }
}
