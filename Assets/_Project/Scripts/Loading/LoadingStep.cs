using System;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Loading
{
    public class LoadingStep
    {
        public string Description { get; }
        public Func<UniTask> ActionAsync { get; }
        
        public LoadingStep(string description, Func<UniTask> actionAsync, bool isEvent = false, bool isInteract = false)
        {
            Description = description;
            ActionAsync = actionAsync;
            IsEvent = isEvent;
            IsInteract = isInteract;
        }
        
        public bool IsEvent;
        public bool IsInteract;
    }
}