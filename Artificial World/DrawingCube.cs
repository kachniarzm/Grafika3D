using System.Drawing;
using OpenTK;

namespace Artificial_World
{
    public class DrawingCube
    {
        public DrawingCube(Vector3 offsets, Vector3 scale, uint? textureId) : this()
        {
            Offsets = offsets;
            Scale = scale;
            TextureId = textureId;
        }

        public DrawingCube(Vector3 offsets, Vector3 scale, uint? textureId, float[] textureMapping)
            : this(offsets, scale, textureId)
        {
            TextureMapping = textureMapping;
        }

        public DrawingCube(Vector3 offsets, Vector3 rotations, Vector3 scale, uint? textureId, float[] textureMapping, bool rotationFirst)
            : this(offsets, scale, textureId, textureMapping)
        {
            Rotations = rotations;
            RotationFirst = rotationFirst;
        }

        public DrawingCube(Vector3 offsets, Vector3 scale, Color? color)
            : this()
        {
            Offsets = offsets;   
            Scale = scale;
            Color = color;
        }

        private DrawingCube()
        {
            Rotations = Vector3.Zero;
            Drawing = DrawingType.TriangelesOutside;
            RotationFirst = true;
        }

        public Vector3 Offsets { get; set; }
        
        public Vector3 Rotations { get; set; }

        public Vector3 Scale { get; set; }

        public Color? Color { get; set; }
            
        public uint? TextureId { get; set; }
        
        public DrawingType Drawing { get; set; }
            
        public uint? MarksTextureId { get; set; }

        public uint? HeightTextureId { get; set; }

        public float[] TextureMapping { get; set; }
            
        public bool RotationFirst { get; set; }
    }
}
