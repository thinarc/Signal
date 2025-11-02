using System;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Services
{
    public class FakeService
    {
        public bool IsEvent;
        public bool IsInteract;
        
        public UniTask InitializeAsync()
        {
            return UniTask.Delay(10);
        }
    }
}