using UnityEngine;

namespace _Project.Scripts.Effects
{
    [RequireComponent(typeof(LineRenderer))]
    public class NoiseWave : MonoBehaviour, ITickable
    {
        [SerializeField, Range(0, 10f)] public float amplitude = 10f;
        [SerializeField, Range(0.1f, 10f)] private float speed = 2f;
        [SerializeField] private int points = 128;
        [SerializeField] private float width = 10f;

        [SerializeField, Space] public Gradient flashG;
        
        // ReSharper disable once InconsistentNaming
        [Header("Debug"), Space(5), SerializeField] private float _speed;
        // ReSharper disable once InconsistentNaming
        [SerializeField] public float _amplitude;

        private LineRenderer _line;
        private float _time;

        public void Initialize()
        {
            _line = GetComponent<LineRenderer>();
            _line.positionCount = points;
            _line.startWidth = 0.03f;
            _line.endWidth = 0.03f;
        }

        public void Tick()
        {
            _time += Time.deltaTime * _speed;
            if (!Mathf.Approximately(_speed, speed)) _speed = Mathf.MoveTowards(_speed, speed, Time.deltaTime * 1.8f);
            if (!Mathf.Approximately(_amplitude, amplitude)) _amplitude = Mathf.MoveTowards(_amplitude, amplitude, Time.deltaTime * 10f);

            for (var i = 0; i < points; i++)
            {
                var x = (i / (float)points) * width - width / 2f;
                var y = Mathf.PerlinNoise(_time, i * 0.1f) * _amplitude - _amplitude / 2f;
                _line.SetPosition(i, new Vector3(x, y, 0f));
            }
        }
    }
}