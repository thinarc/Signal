using System;
using _Project.Scripts.Effects;
using _Project.Scripts.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using DG.Tweening;

namespace _Project.Scripts.Frequency
{
    public class FrequencyController : ITickable
    {
        private readonly Slider _slider;
        private readonly NoiseWave _wave;
        private readonly AudioSource _noiseSource;
        private readonly AudioLowPassFilter _lowPass;
        
        private Frequency _frequency;
        private float _signalStrength;

        private TerminalPool _term;
        
        public FrequencyController(Slider slider, AudioSource noiseSource, AudioLowPassFilter lowPass, NoiseWave wave)
        {
            _slider = slider;
            _noiseSource = noiseSource;
            _lowPass = lowPass;
            _wave = wave;
        }

        public void BindTerminalPool(TerminalPool t)
        {
            _term = t;
        }

        public async UniTask BindAwaitEvent()
        {
            await UniTask.WaitUntil(() => _passed);
        }
        
        public void Initialize()
        {
            Repeat();
            _slider.onValueChanged.AddListener((_) => OnValueChanged());
        }

        public void Repeat(bool twoplusrep = false)
        {
            _frequency = new Frequency()
            {
                target = Random.Range(0.05f, 1f),
                precision = Random.Range(0.02f, 0.05f),
                noiseIntensity = Random.Range(0.3f, 1f)
            };
            
            if (twoplusrep) _wave.amplitude = _startAmplitude;
            _startAmplitude = _wave.amplitude;
            _hitTime = 0;
            _hit = false;
            var line = _wave.GetComponent<LineRenderer>();
            _line = line;
            if (!twoplusrep) _startFlash = line.colorGradient;
            if (_startFlash != null) line.colorGradient = _startFlash;
            _currentHitSliderValue = null;
            _startNoiseVolume = _noiseSource.volume;
            OnValueChanged();
        }
        
        private float _startAmplitude;
        private float? _currentHitSliderValue;
        private float _hitTime;

        private float _startNoiseVolume;
        public void OnValueChanged()
        {
            var current = _slider.value;
            
            var distance = Mathf.Abs(current - _frequency.target);
            
            var baseStrength = Mathf.Clamp01(1f - (distance / _frequency.precision));
            var noise = Mathf.PerlinNoise(Time.time * 2f, 0f) * _frequency.noiseIntensity;
            _signalStrength = Mathf.Clamp01(baseStrength + noise - Random.Range(0f, 0.15f));
            
            _lowPass.cutoffFrequency = Mathf.Lerp(300f, 5000f, _signalStrength);
            _noiseSource.volume = Mathf.Lerp(_noiseSource.volume, _startNoiseVolume * 1.3f * _signalStrength, Time.deltaTime * 100);

            _wave.amplitude = _startAmplitude - _signalStrength * 10;
            if (_signalStrength > 0.91f)
            {
                _currentHitSliderValue = _slider.value;
            }
            else
            {
                _currentHitSliderValue = null;
            }
        }

        private bool _hit;
        private Gradient _startFlash;
        private LineRenderer _line;
        public async void Tick()
        {
            if (_currentHitSliderValue == null)
            {
                _hitTime = 0;
                _hit = false;
                var line = _wave.GetComponent<LineRenderer>();
                if (_startFlash != null) line.colorGradient = _startFlash;
                return;
            }

            _hitTime += Time.deltaTime;

            if (_hitTime >= 1.3f && !_hit)
            {
                _hit = true;
                _line.colorGradient = _wave.flashG;
                await UniTask.Delay(390);
                PassQuest();
            }
        }

        private int _cycle;
        private bool _passed;
        private async void PassQuest()
        {
            Repeat(true);
            _cycle++;

            switch (_cycle)
            {
                case 1: _term.CreateVoiceTerminal("whohears", "who... hears...?"); break;
                case 2: _term.CreateVoiceTerminal("imhere", "I'm... here..."); break;
                case 3: _term.CreateVoiceTerminal("dnthavetolook", "you don't have to look..."); break;
                case 4: await _term.CreateVoiceTerminalAsync("dontswitchoff", "INCOMING TRANSMISSION: lain.exe", "Don't switch off. We are one.");
                    _passed = true; break;
            }
        }
    }
}
