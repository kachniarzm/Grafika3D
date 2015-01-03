using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Artificial_World
{
    class Particle
    {
        private static Random rand = new Random((int)DateTime.Now.Ticks);

        private Vector3 _startPosition;
        private Vector3 _endPosition;
        private Vector3 _startScale;
        private int _currentFrame;
        private int _lastFrame;
        private int _maxScaleRatio;
        private float _rotation;

        public Particle(Vector3 startPosition, Vector3 startScale)
        {
            _startPosition = startPosition;
            _endPosition = startPosition + new Vector3((float)(rand.NextDouble() * 6 - 3), (float)(rand.NextDouble() * 6 - 3), (float)(rand.NextDouble() * 6 - 3));
            _maxScaleRatio = 10;
            _currentFrame = 0;
            _lastFrame = 600;
            _startScale = startScale;
            _rotation = rand.Next(180);
        }

        public Vector3 GetParticleOffsets()
        {
            Vector3 offsets = (_endPosition - _startPosition) * GetRatio() + _startPosition;

            return offsets;
        }

        public Vector3 GetParticleScale()
        {
            Vector3 scale = _startScale * (_maxScaleRatio - 1) * GetRatio() + _startScale;

            return scale;
        }

        public float GetRotation()
        {
            return _rotation;
        }

        public float GetParticleOpacity()
        {
            float opacity = (1f - GetRatio()) * 0.3f;

            return opacity;
        }

        public void Update()
        {
            _currentFrame++;
            if (_currentFrame > _lastFrame)
            {
                _currentFrame = 0;
                _endPosition = _startPosition + new Vector3((float)(rand.NextDouble() * 6 - 3), (float)(rand.NextDouble() * 6 - 3), (float)(rand.NextDouble() * 6 - 3));
            }
                
        }

        // 0 -> 1
        private float GetRatio()
        {
            //return ((float) _currentFrame/_lastFrame);
            var pow = (float)Math.Pow(((float)(_lastFrame - _currentFrame) / _lastFrame), 4d);
            return (1 - pow);
        }
    }
}
