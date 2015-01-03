using System;
using System.Collections.Generic;
using OpenTK;

namespace Artificial_World
{
    public class BilboardsComparer : IComparer<DrawingCube>
    {
        private Vector3 _camera;
        private readonly uint _particleTextureId;

        public BilboardsComparer(Vector3 camera, uint particleTextureId)
        {
            _camera = camera;
            _particleTextureId = particleTextureId;
        }

        public int Compare(DrawingCube x, DrawingCube y)
        {
            //if (x.TextureId == _particleTextureId && y.TextureId == _particleTextureId)
            //{
            //    return 0;
            //}

            var value1 = (x.Offsets - _camera).Length;
            var value2 = (y.Offsets - _camera).Length;
            if (value1 > value2) return -1;
            else if (value1 < value2) return 1;
            else return 0;
        }
    }
}
