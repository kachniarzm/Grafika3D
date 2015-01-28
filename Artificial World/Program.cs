using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Drawing;
using System.Runtime.CompilerServices;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace Artificial_World
{
    class Program : GameWindow
    {
        #region Cube fields

        readonly ushort[] _cubeTrianglesOutside =
		{
			0, 1, 2,    2, 3, 0,      // front
            4, 5, 6,    6, 7, 4,      // right
            8, 9,10,   10,11, 8,      // top
            12,13,14,  14,15,12,      // left
            16,17,18,  18,19,16,      // bottom
            20,21,22,  22,23,20 };    // back

        readonly ushort[] _cubeTrianglesInside =
		{
			0, 2, 1,     2, 0, 3,      // front
            4, 6, 5,     6, 4, 7,      // right
            8, 10, 9,   10, 8,11,      // top
            12,14,13,   14,12,15,      // left
            16,18,17,   18,16,19,      // bottom
            20,22,21,   22,20,23 };    // back

        readonly ushort[] _cubeLines =
		{
			0, 1, 1,     2, 2, 3,      // front
            5, 4, 4,     6, 6, 7,      // right
            9, 10, 10,   8, 8,11,      // top
            13,12,12,   14,14,15,      // left
            17,18,18,   16,16,19,      // bottom
            21,20,20,   22,22,23 };    // back

        readonly float[] _cubeVertices = {    
                        1, 1, 1,   0, 1, 1,   0, 0, 1,   1, 0, 1,   // v0,v1,v2,v3 (front)
                        1, 1, 1,   1, 0, 1,   1, 0, 0,   1, 1, 0,   // v0,v3,v4,v5 (right)
                        1, 1, 1,   1, 1, 0,   0, 1, 0,   0, 1, 1,   // v0,v5,v6,v1 (top)
                        0, 1, 1,   0, 1, 0,   0, 0, 0,   0, 0, 1,   // v1,v6,v7,v2 (left)
                        0, 0, 0,   1, 0, 0,   1, 0, 1,   0, 0, 1,   // v7,v4,v3,v2 (bottom)
                        1, 0, 0,   0, 0, 0,   0, 1, 0,   1, 1, 0 }; // v4,v7,v6,v5 (back)

        readonly float[] _cubeVerticesMinus = {    
                        1, 1, 1,  -1, 1, 1,  -1, 0, 1,   1, 0, 1,   // v0,v1,v2,v3 (front)
                        1, 1, 1,   1, 0, 1,   1, 0,-1,   1, 1,-1,   // v0,v3,v4,v5 (right)
                        1, 1, 1,   1, 1,-1,  -1, 1,-1,  -1, 1, 1,   // v0,v5,v6,v1 (top)
                       -1, 1, 1,  -1, 1,-1,  -1, 0,-1,  -1, 0, 1,   // v1,v6,v7,v2 (left)
                       -1, 0,-1,   1, 0,-1,   1, 0, 1,  -1, 0, 1,   // v7,v4,v3,v2 (bottom)
                        1, 0,-1,  -1, 0,-1,  -1, 1,-1,   1, 1,-1 }; // v4,v7,v6,v5 (back)

        readonly float[] _cubeNormalsOutside = {
                        0, 0, 1,   0, 0, 1,   0, 0, 1,   0, 0, 1,   // v0,v1,v2,v3 (front)
                        1, 0, 0,   1, 0, 0,   1, 0, 0,   1, 0, 0,   // v0,v3,v4,v5 (right)
                        0, 1, 0,   0, 1, 0,   0, 1, 0,   0, 1, 0,   // v0,v5,v6,v1 (top)
                       -1, 0, 0,  -1, 0, 0,  -1, 0, 0,  -1, 0, 0,   // v1,v6,v7,v2 (left)
                        0,-1, 0,   0,-1, 0,   0,-1, 0,   0,-1, 0,   // v7,v4,v3,v2 (bottom)
                        0, 0,-1,   0, 0,-1,   0, 0,-1,   0, 0,-1 }; // v4,v7,v6,v5 (back)

        readonly float[] _cubeNormalsInside = {
                       0, 0,-1,   0, 0,-1,   0, 0,-1,   0, 0,-1,   // v0,v1,v2,v3 (front)
                      -1, 0, 0,  -1, 0, 0,  -1, 0, 0,  -1, 0, 0,   // v0,v3,v4,v5 (right)
                       0,-1, 0,   0,-1, 0,   0,-1, 0,   0,-1, 0,   // v0,v5,v6,v1 (top)
                       1, 0, 0,   1, 0, 0,   1, 0, 0,   1, 0, 0,   // v1,v6,v7,v2 (left)
                       0, 1, 0,   0, 1, 0,   0, 1, 0,   0, 1, 0,   // v7,v4,v3,v2 (bottom)
                       0, 0, 1,   0, 0, 1,   0, 0, 1,   0, 0, 1 }; // v4,v7,v6,v5 (back)

        readonly float[] _cubeTangents =
        {
            1, 0, 0,   1, 0, 0,   1, 0, 0,   1, 0, 0,
            0, 1, 1,   0, 1, 1,   0, 1, 1,   0, 1, 1,   
            1, 0, 1,   1, 0, 1,   1, 0, 1,   1, 0, 1,   
            0,-1,-1,   0,-1,-1,   0,-1,-1,   0,-1,-1,
           -1, 0,-1,  -1, 0,-1,  -1, 0,-1,  -1, 0,-1,
           -1,-1, 0,  -1,-1, 0,  -1,-1, 0,  -1,-1, 0  
        };
        
        readonly float[] _cubeStandardTexcoords =
        {
            0, 0,  0, 0,   0, 0,  0, 0, // front (from light side)
            0, 0,  0, 0,   0, 0,  0, 0, // right (from start persp)
            0, 0,  6, 0,   6, 6,  0, 6, // top
            0, 0,  0, 0,   0, 0,  0, 0, // left
            0, 0,  6, 0,   6, 6,  0, 6, // bottom
            0, 0,  0, 0,   0, 0,  0, 0  // back
        };

        readonly float[] _cubeRockTexcoords =
        {
            0, 0,  0, 1,   1, 1,  1, 0, // right (from start persp)
            0, 0,  1, 0,   1, 1,  0, 1, // top
            1, 0,  0, 0,   0, 1,  1, 1, // left
            0, 0,  1, 0,   1, 1,  0, 1, // bottom
            0, 1,  1, 1,   1, 0,  0, 0  // back
            1, 1,  0, 1,   0, 0,  1, 0, // front (from light side)
        };

        readonly float[] _cubeAdvTexcoords =
        {
            3, 0,  0, 0,   0, 1,  3, 1, // front (from light side)
            0, 0,  0, 1,   1.66f, 1,  1.66f, 0, // right (from start persp)
            0, 0,  0, 0,   0, 0,  0, 0, // top
            1.66f, 0,  0, 0,   0, 1,  1.66f, 1, // left
            0, 0,  0, 0,   0, 0,  0, 0, // bottom
            0, 1,  3, 1,   3, 0,  0, 0  // back
        };

        readonly float[] _cubeMarkTexcoords =
        {
            0, 0,  0, 0,   0, 0,  0, 0, // front (from light side)
            0, 0,  0, 0,   0, 0,  0, 0, // right (from start persp)
            1, 1,  1, 0,   0, 0,  0, 1, // top
            0, 0,  0, 0,   0, 0,  0, 0, // left
            0, 0,  0, 0,   0, 0,  0, 0, // bottom
            0, 0,  0, 0,   0, 0,  0, 0  // back
        };

        float[] _cubeTelebimTexcoords =
        {
            1, 1,  0, 1,   0, 0,  1, 0, // front (from light side)
            0, 0,  0, 0,   0, 0,  0, 0, // right (from start persp)
            0, 0,  0, 0,   0, 0,  0, 0, // top
            0, 0,  0, 0,   0, 0,  0, 0, // left
            0, 0,  0, 0,   0, 0,  0, 0, // bottom
            0, 0,  1, 0,   1, 1,  0, 1, // back
        };

        float[] _cubeManTexcoords =
        {
            1, 0,  0, 0,   0, 1,  1, 1, // front (from light side)
            0, 0,  0, 0,   0, 0,  0, 0, // right (from start persp)
            0, 0,  0, 0,   0, 0,  0, 0, // top
            0, 0,  0, 0,   0, 0,  0, 0, // left
            0, 0,  0, 0,   0, 0,  0, 0, // bottom
            0, 1,  1, 1,   1, 0,  0, 0, // back
        };



        //readonly float[] _cubeTexcoords = new float[6 * 4 * 2];
        readonly float[] _cubeColors = new float[4 * 24];
        readonly ushort[] _cubeTriangles = new ushort[3 * 2 * 6];
        readonly float[] _cubeNormals = new float[3 * 24];

        #endregion

        #region .obj Models fields

        //Meshomatic.MeshData ballModel;
        //Meshomatic.MeshData cubeModel;
        //Meshomatic.MeshData chairModel;
        //Meshomatic.MeshData benchModel;
        Meshomatic.MeshData _manModel;
        Meshomatic.MeshData _areaLightModel;

        Dictionary<int, Matrix4> _objectModelMatrixDionary;
        Dictionary<int, Matrix3> _objectNormalMatrixDionary;
        Dictionary<int, uint> _vboObjectVerticesDictionary;
        Dictionary<int, uint> _vboObjectNormalsDictionary;
        Dictionary<int, uint> _vboObjectColorsDictionary;
        Dictionary<int, uint> _iboObjectTrianglesDictionary;

        #endregion

        #region Camera fields

        // Camera position
        Vector3[] _eye = new Vector3[] { new Vector3(41, 9, 28), new Vector3(7, 7, 23) };
        // Camera watching directionvector
        Vector3[] _eyeDirection = new Vector3[] { new Vector3(-0.8f, -0.4f, -0.45f), new Vector3(0.65f, -0.5f, -0.6f) };
        // Camera vector orienting where is up
        Vector3[] _upCameraVector = new Vector3[] { Vector3.UnitY, Vector3.UnitY };
        // Camera vector orienting where is right
        Vector3[] _rightCameraVector = new Vector3[2];

        int currentCameraIndex = 0;

        private const float CameraMoveSpeed = 0.1f;
        private const float CameraRotationSpeed = 0.02f;

        float _lastMouseX = float.NaN;
        float _lastMouseY = float.NaN;

        #endregion

        #region Matrixes fields

        Matrix4 _matrixProjection;
        Matrix4 _matrixView;

        #endregion

        #region Shaders fields

        int _program;

        uint _vboCubeVertices;
        uint _vboCubeNormals;
        uint _vboCubeTangents;
        uint _vboCubeColors;
        uint _vboCubeTexcoords;
        uint _vboCubeMarkTexcoords;
        uint _iboCubeTriangles;

        int _attributeCoord3;
        int _attributeColor;
        int _attributeNormal;
        int _attributeTangent;
        int _attributeTexcoord;
        int _attributeMarkTexcoord;

        int _uniformTick;
        int _uniformModelMatix;
        int _uniformViewMatix;
        int _uniformProjectionMatix;
        int _uniformNormalMatix;
        int _uniformViewInvertedMatix;
        int _uniformTexture;
        int _uniformMarkTexture;
        int _uniformHeightTexture;
        int _uniformifTextured;
        int _uniformifMarkTextured;
        int _uniformifHeightTextured;
        int _uniformCameraPosition;
        int _uniformFogRatio;
        int _uniformIfMaxLight;
        int _uniformOpacity;

        #endregion

        #region Texture fields

        int frameBufferName;
        uint renderedTexture;

        uint _floor1TextureId;
        uint _floor2TextureId;
        uint _floorMarkId;
        uint _advTextureId;
        uint _fireTextureId;
        uint _rockTextureId;
        uint _rockHeightTextureId;

        uint[] _manTextureId = new uint[13];

        #endregion

        #region Visual options fileds

        FloorType _floorType = FloorType.LightWood;
        float _fogRatio = 0;

        #endregion

        #region Particles fileds

        List<Particle> particles = new List<Particle>(); 

        #endregion

        #region Events

        protected override void OnLoad(EventArgs e)
        {
            InitShaders();
            InitAttributes();
            InitUniforms();
            InitVbos();
            InitFramebuffer();
            //InitParticles();

            _advTextureId = LoadTexture("Textures/adv.jpg");
            _floor1TextureId = LoadTexture("Textures/floor_1.jpg");
            _floor2TextureId = LoadTexture("Textures/floor_2.jpg");
            _floorMarkId = LoadTexture("Textures/field.png");
            _fireTextureId = LoadTexture("Textures/fire.png");
            _rockTextureId = LoadTexture("Textures/cube_color.png");
            _rockHeightTextureId = LoadTexture("Textures/cube_heightmap.png");

            for (int i = 0; i < _manTextureId.Length; i++)
            {
                _manTextureId[i] = LoadTexture(String.Format("Textures/man{0}.png", (i + 1).ToString("D2")));
            }

            _manModel = new Meshomatic.ObjLoader().LoadFile("graphics models/man.obj");
            _areaLightModel = new Meshomatic.ObjLoader().LoadFile("graphics models/area_light.obj");

            _objectModelMatrixDionary = new Dictionary<int, Matrix4>();
            _objectNormalMatrixDionary = new Dictionary<int, Matrix3>();
            _vboObjectVerticesDictionary = new Dictionary<int, uint>();
            _vboObjectNormalsDictionary = new Dictionary<int, uint>();
            _vboObjectColorsDictionary = new Dictionary<int, uint>();
            _iboObjectTrianglesDictionary = new Dictionary<int, uint>();

            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.DepthTest);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.PointSmooth);

            WindowState = WindowState.Fullscreen;
        }

        protected override void OnResize(EventArgs e)
        {
            ResizeScreen(Width, Height);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            Draw();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            var keyboardState = OpenTK.Input.Keyboard.GetState();
            if (keyboardState[Key.Escape])
            {
                Close();
                //WindowState = WindowState.Normal;
            }
            else if (keyboardState[Key.F5])
            {
                WindowState = WindowState.Fullscreen;
            }
            else if (keyboardState[Key.W])
            {
                _eye[currentCameraIndex] += _eyeDirection[currentCameraIndex] * CameraMoveSpeed;
            }
            else if (keyboardState[Key.S])
            {
                _eye[currentCameraIndex] -= _eyeDirection[currentCameraIndex] * CameraMoveSpeed;
            }
            else if (keyboardState[Key.A])
            {
                Vector3 result = -Vector3.Cross(_eyeDirection[currentCameraIndex], _upCameraVector[currentCameraIndex]);
                result.Normalize();
                _eye[currentCameraIndex] += result * CameraMoveSpeed;
            }
            else if (keyboardState[Key.D])
            {
                Vector3 result = Vector3.Cross(_eyeDirection[currentCameraIndex], _upCameraVector[currentCameraIndex]);
                result.Normalize();
                _eye[currentCameraIndex] += result * CameraMoveSpeed;
            }
            else if (keyboardState[Key.Q])
            {
                _eye[currentCameraIndex] += _upCameraVector[currentCameraIndex] * CameraMoveSpeed;
            }
            else if (keyboardState[Key.E])
            {
                _eye[currentCameraIndex] -= _upCameraVector[currentCameraIndex] * CameraMoveSpeed;
            }
            else if (keyboardState[Key.R])
            {
                _cubeTelebimTexcoords = new float[]
                {
                    1, 1,  0, 1,   0, 0,  1, 0, // front (from light side)
                    0, 0,  0, 0,   0, 0,  0, 0, // right (from start persp)
                    0, 0,  0, 0,   0, 0,  0, 0, // top
                    0, 0,  0, 0,   0, 0,  0, 0, // left
                    0, 0,  0, 0,   0, 0,  0, 0, // bottom
                    0, 0,  1, 0,   1, 1,  0, 1, // back
                };

                _eye[currentCameraIndex] = new Vector3(41, 9, 28);
                _eyeDirection[currentCameraIndex] = new Vector3(-0.8f, -0.4f, -0.45f);
                CalculateCameraVectors();

                _fogRatio = 0;
            }
            else if (keyboardState[Key.BracketLeft])
            {
                _cubeTelebimTexcoords = ScaleArray(_cubeTelebimTexcoords, 0.99);
            }
            else if (keyboardState[Key.BracketRight])
            {
                _cubeTelebimTexcoords = ScaleArray(_cubeTelebimTexcoords, 1.01);
            }

            var mouseState = OpenTK.Input.Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed && !float.IsNaN(_lastMouseX) && !float.IsNaN(_lastMouseY))
            {
                CalculateCameraVectors();

                float cameraRotationY = -(mouseState.X - _lastMouseX) * CameraRotationSpeed * _upCameraVector[currentCameraIndex].Y;

                float cameraRotationX = -(mouseState.Y - _lastMouseY) * CameraRotationSpeed * _rightCameraVector[currentCameraIndex].X;
                float cameraRotationZ = -(mouseState.Y - _lastMouseY) * CameraRotationSpeed * _rightCameraVector[currentCameraIndex].Z;

                _eyeDirection[currentCameraIndex] = Vector3.Transform(_eyeDirection[currentCameraIndex], Matrix4.CreateRotationY(cameraRotationY));
                _eyeDirection[currentCameraIndex] = Vector3.Transform(_eyeDirection[currentCameraIndex], Matrix4.CreateRotationX(cameraRotationX));
                _eyeDirection[currentCameraIndex] = Vector3.Transform(_eyeDirection[currentCameraIndex], Matrix4.CreateRotationZ(cameraRotationZ));
                _eyeDirection[currentCameraIndex].Normalize();

                CalculateCameraVectors();
            }

            _lastMouseX = mouseState.X;
            _lastMouseY = mouseState.Y;

        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == 'f')
                _floorType = (FloorType)(((int)_floorType + 1) % Enum.GetNames(typeof(FloorType)).Count());
            else if (e.KeyChar == 'z')
                _fogRatio += 0.01f;
            else if (e.KeyChar == 'x')
                _fogRatio -= 0.01f;
            else if (e.KeyChar == 'c')
                currentCameraIndex = (currentCameraIndex + 1) % 2;
        }

        #endregion

        #region Camera Methods

        private void SetCamera()
        {
            _matrixView = Matrix4.Identity;
            _matrixView *= Matrix4.LookAt(_eye[currentCameraIndex], _eye[currentCameraIndex] + _eyeDirection[currentCameraIndex].Normalized(), _upCameraVector[currentCameraIndex].Normalized());
        }

        private void CalculateCameraVectors()
        {
            _rightCameraVector[currentCameraIndex] = Vector3.Cross(_eyeDirection[currentCameraIndex], Vector3.UnitY);
            _upCameraVector[currentCameraIndex] = Vector3.Cross(_rightCameraVector[currentCameraIndex], _eyeDirection[currentCameraIndex]);
            _rightCameraVector[currentCameraIndex].Normalize();
            _upCameraVector[currentCameraIndex].Normalize();
        }

        private void ResizeScreen(int width, int height)
        {
            GL.Viewport(0, 0, width, height);
            _matrixProjection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, width / (float)height, 0.1f, 100f);
        }

        #endregion

        #region Init Methods

        private void InitShaders()
        {
            int vs = ShaderUtility.CreateShader("triangle.v.glsl", ShaderType.VertexShader);
            int fs = ShaderUtility.CreateShader("triangle.f.glsl", ShaderType.FragmentShader);

            _program = GL.CreateProgram();
            GL.AttachShader(_program, vs);
            GL.AttachShader(_program, fs);
            GL.LinkProgram(_program);
        }

        private void InitVbos()
        {
            GL.GenBuffers(1, out _vboCubeVertices);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboCubeVertices);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(_cubeVertices.Length * sizeof(float)), _cubeVertices, BufferUsageHint.DynamicDraw);

            GL.GenBuffers(1, out _iboCubeTriangles);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _iboCubeTriangles);
            GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(_cubeTriangles.Length * sizeof(ushort)), _cubeTriangles, BufferUsageHint.DynamicDraw);

            GL.GenBuffers(1, out _vboCubeNormals);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboCubeNormals);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(_cubeNormals.Length * sizeof(float)), _cubeNormals, BufferUsageHint.DynamicDraw);

            GL.GenBuffers(1, out _vboCubeTangents);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboCubeTangents);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(_cubeTangents.Length * sizeof(float)), _cubeTangents, BufferUsageHint.DynamicDraw);

            GL.GenBuffers(1, out _vboCubeColors);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboCubeColors);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(_cubeColors.Length * sizeof(float)), _cubeColors, BufferUsageHint.DynamicDraw);

            GL.GenBuffers(1, out _vboCubeTexcoords);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboCubeTexcoords);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(_cubeStandardTexcoords.Length * sizeof(float)), _cubeStandardTexcoords, BufferUsageHint.DynamicDraw);

            GL.GenBuffers(1, out _vboCubeMarkTexcoords);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboCubeMarkTexcoords);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(_cubeMarkTexcoords.Length * sizeof(float)), _cubeMarkTexcoords, BufferUsageHint.DynamicDraw);
        }

        private void InitUniforms()
        {
            _uniformTick = GL.GetUniformLocation(_program, "tick");

            _uniformModelMatix = GL.GetUniformLocation(_program, "m_transform");

            _uniformViewMatix = GL.GetUniformLocation(_program, "m_view");

            _uniformProjectionMatix = GL.GetUniformLocation(_program, "m_projection");

            _uniformNormalMatix = GL.GetUniformLocation(_program, "m_normal");

            _uniformViewInvertedMatix = GL.GetUniformLocation(_program, "m_view_inv");

            _uniformTexture = GL.GetUniformLocation(_program, "myTexture");

            _uniformMarkTexture = GL.GetUniformLocation(_program, "myMarkTexture");

            _uniformHeightTexture = GL.GetUniformLocation(_program, "myHeightTexture");

            _uniformifTextured = GL.GetUniformLocation(_program, "ifTextured");

            _uniformifMarkTextured = GL.GetUniformLocation(_program, "ifMarkTextured");

            _uniformifHeightTextured = GL.GetUniformLocation(_program, "ifHeightTextured");

            _uniformCameraPosition = GL.GetUniformLocation(_program, "cameraPosition");

            _uniformFogRatio = GL.GetUniformLocation(_program, "fogRatio");

            _uniformIfMaxLight = GL.GetUniformLocation(_program, "ifMaxLight");

            _uniformOpacity = GL.GetUniformLocation(_program, "opacity");
        }

        private void InitAttributes()
        {
            _attributeCoord3 = GL.GetAttribLocation(_program, "coord3d");

            _attributeColor = GL.GetAttribLocation(_program, "v_color");

            _attributeNormal = GL.GetAttribLocation(_program, "v_normal");

            _attributeTangent = GL.GetAttribLocation(_program, "v_tangent");

            _attributeTexcoord = GL.GetAttribLocation(_program, "texcoord");

            _attributeMarkTexcoord = GL.GetAttribLocation(_program, "marktexcoord");
        }

        public uint LoadTexture(string file)
        {
            uint textureId;
            Bitmap bitmap = new Bitmap(file);

            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            GL.GenTextures(1, out textureId);
            GL.BindTexture(TextureTarget.Texture2D, textureId);

            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bitmap.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            return textureId;
        }

        public void InitFramebuffer()
        {
            GL.Enable(EnableCap.Texture2D);

            GL.GenFramebuffers(1, out frameBufferName);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBufferName);

            GL.GenTextures(1, out renderedTexture);
            GL.BindTexture(TextureTarget.Texture2D, renderedTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, 1024, 768, 0, PixelFormat.Rgb, PixelType.UnsignedByte, new IntPtr());
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);

            int depthRenderBuffer;
            GL.GenRenderbuffers(1, out depthRenderBuffer);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthRenderBuffer);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent, 1024, 768);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, depthRenderBuffer);

            GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, renderedTexture, 0);
            DrawBuffersEnum drawBuffers = DrawBuffersEnum.ColorAttachment0;
            GL.DrawBuffers(1, ref drawBuffers);

            var result = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
        }

        public void InitParticles()
        {
            for (int i=0; i<10; i++)
                particles.Add(new Particle(new Vector3(0, 10, 14.5f), new Vector3(0.5f, 0.5f, 0.001f)));
        }

        #endregion

        #region Draw Methods

        private void Draw()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBufferName);
            ResizeScreen(1024, 768);

            var old = currentCameraIndex;
            currentCameraIndex = 1;
            SetCamera();

            DrawScene();

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            ResizeScreen(Width, Height);

            currentCameraIndex = 0;
            SetCamera();

            DrawScene(true);

            currentCameraIndex = old;
        }

        private void DrawScene(bool ifAgain = false)
        {
            GL.ClearColor(Color.CornflowerBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            DateTime now = DateTime.Now;
            GL.Uniform1(_uniformTick, (float)(Math.Abs((float)0.5 - ((float)now.Millisecond / 10 + (float)(now.Second % 10) * 100) / 1000.0)) * 2);
            GL.UniformMatrix4(_uniformViewMatix, false, ref _matrixView);
            GL.UniformMatrix4(_uniformProjectionMatix, false, ref _matrixProjection);
            Matrix4 viewInverted = _matrixView.Inverted();
            GL.UniformMatrix4(_uniformViewInvertedMatix, false, ref viewInverted);
            GL.Uniform3(_uniformCameraPosition, _eye[ifAgain ? 0 : 1]);
            GL.Uniform1(_uniformFogRatio, _fogRatio);
            GL.Uniform1(_uniformIfMaxLight, 0);
            GL.Uniform1(_uniformOpacity, 1f);

            //DrawModelWithShader(benchModel, Color.SandyBrown, new Vector3(25, 0, 9), new Vector3(0, 90, 0), new Vector3(0.02f, 0.02f, 0.02f));
            //DrawModelWithShader(benchModel, Color.SandyBrown, new Vector3(21, 0, 9), Vector3.Zero, new Vector3(0.02f, 0.02f, 0.02f));

            //DrawModelWithShader(benchModel, Color.SandyBrown, new Vector3(16, 0, 9), new Vector3(0, 90, 0), new Vector3(0.02f, 0.02f, 0.02f));
            //DrawModelWithShader(benchModel, Color.SandyBrown, new Vector3(12, 0, 9), Vector3.Zero, new Vector3(0.02f, 0.02f, 0.02f));

            DrawModelWithShader(_manModel, Color.Yellow, new Vector3(13, 0, 13), new Vector3(0, 90, 0), new Vector3(0.01f, 0.01f, 0.01f));
            DrawModelWithShader(_manModel, Color.Yellow, new Vector3(16, 0, 16), new Vector3(0, 90, 0), new Vector3(0.01f, 0.01f, 0.01f));

            DrawModelWithShader(_manModel, Color.LightPink, new Vector3(22, 0, 13), new Vector3(0, -90, 0), new Vector3(0.01f, 0.01f, 0.01f));
            DrawModelWithShader(_manModel, Color.LightPink, new Vector3(25, 0, 16), new Vector3(0, -90, 0), new Vector3(0.01f, 0.01f, 0.01f));

            DrawModelWithShader(_areaLightModel, Color.Silver, new Vector3(36, 0.1f, 27 - 3), new Vector3(0, -45, 0), new Vector3(0.03f, 0.03f, 0.03f));
            DrawModelWithShader(_areaLightModel, Color.Silver, new Vector3(2 + 3, 0.1f, 27), new Vector3(0, -135, 0), new Vector3(0.03f, 0.03f, 0.03f));

            uint floorId = 0;
            if (FloorType.DarkWood == _floorType)
            {
                floorId = _floor2TextureId;
            }
            else if (FloorType.LightWood == _floorType)
            {
                floorId = _floor1TextureId;
            }

            // court
            DrawCubeWithShader(new DrawingCube(new Vector3(10, 0, 10), new Vector3(18, 0.01f, 9), floorId) { MarksTextureId = _floorMarkId });

            // adv
            DrawCubeWithShader(new DrawingCube(new Vector3(5, 0, 7), new Vector3(28, 1f, 0.1f), _advTextureId, _cubeAdvTexcoords));
            DrawCubeWithShader(new DrawingCube(new Vector3(5, 0, 22), new Vector3(28, 1f, 0.1f), _advTextureId, _cubeAdvTexcoords));
            DrawCubeWithShader(new DrawingCube(new Vector3(5, 0, 7), new Vector3(0.1f, 1f, 15.1f), _advTextureId, _cubeAdvTexcoords));
            DrawCubeWithShader(new DrawingCube(new Vector3(33, 0, 7), new Vector3(0.1f, 1f, 15.1f), _advTextureId, _cubeAdvTexcoords));

            // sticks
            DrawCubeWithShader(new DrawingCube(new Vector3(19, 0, 9), new Vector3(0.1f, 2.43f, 0.1f), Color.PapayaWhip));
            DrawCubeWithShader(new DrawingCube(new Vector3(19, 0, 20), new Vector3(0.1f, 2.43f, 0.1f), Color.PapayaWhip));
            // room
            DrawCubeWithShader(new DrawingCube(new Vector3(-10, 0, -10), new Vector3(58, 20, 49), Color.LightGray)
            {
                Drawing = DrawingType.TriangelesInside
            });

            // relief map cube
            DrawCubeWithShader(new DrawingCube(new Vector3(19, 3, 15), new Vector3(2, 2, 2), _rockTextureId, _cubeRockTexcoords)
            {
                HeightTextureId = _rockHeightTextureId
            });
            
            // telebim
            if (ifAgain)
            {
                GL.Uniform1(_uniformIfMaxLight, 1);
                DrawCubeWithShader(new DrawingCube(new Vector3(19 - 6, 12, 14.5f), new Vector3(12, 8, 0.01f), renderedTexture, _cubeTelebimTexcoords));
                GL.Uniform1(_uniformIfMaxLight, 0);
            }

            DrawBilboards();

            // net
            DrawCubeWithShader(new DrawingCube(new Vector3(19, 2.43f - 1.5f, 9), new Vector3(0.02f, 1.5f, 11), Color.FromArgb(128, Color.Gray)));

            SwapBuffers();
        }

        private void DrawBilboards()
        {
            List<DrawingCube> bilboards = new List<DrawingCube>();

            bilboards.Add(new DrawingCube(new Vector3(7, 0, 6), RotateBilboard(new Vector3(7, 0, 6)), new Vector3(0.5f, 1.8f, 0.01f), _manTextureId[7], _cubeManTexcoords, false));
            bilboards.Add(new DrawingCube(new Vector3(18, 0, 6), RotateBilboard(new Vector3(18, 0, 6)), new Vector3(0.5f, 1.8f, 0.01f), _manTextureId[8], _cubeManTexcoords, false));
            bilboards.Add(new DrawingCube(new Vector3(12, 0, 5), RotateBilboard(new Vector3(12, 0, 5)), new Vector3(0.5f, 1.8f, 0.01f), _manTextureId[2],_cubeManTexcoords, false));
            bilboards.Add(new DrawingCube(new Vector3(16, 0, 5), RotateBilboard(new Vector3(16, 0, 5)), new Vector3(0.5f, 1.8f, 0.01f), _manTextureId[3], _cubeManTexcoords, false));
            bilboards.Add(new DrawingCube(new Vector3(25, 0, 4), RotateBilboard(new Vector3(25, 0, 4)), new Vector3(0.5f, 1.8f, 0.01f), _manTextureId[4],  _cubeManTexcoords, false));
            bilboards.Add(new DrawingCube(new Vector3(17, 0, 4), RotateBilboard(new Vector3(17, 0, 4)), new Vector3(0.5f, 1.8f, 0.01f), _manTextureId[5], _cubeManTexcoords, false));
            bilboards.Add(new DrawingCube(new Vector3(29, 0, 3), RotateBilboard(new Vector3(29, 0, 3)), new Vector3(0.5f, 1.8f, 0.01f), _manTextureId[6], _cubeManTexcoords, false));
            bilboards.Add(new DrawingCube(new Vector3(30, 0, 23), RotateBilboard(new Vector3(30, 0, 23)), new Vector3(0.5f, 1.8f, 0.01f),  _manTextureId[0], _cubeManTexcoords, false));
            bilboards.Add(new DrawingCube(new Vector3(15, 0, 23), RotateBilboard(new Vector3(15, 0, 23)), new Vector3(0.5f, 1.8f, 0.01f), _manTextureId[3], _cubeManTexcoords, false));
            bilboards.Add(new DrawingCube(new Vector3(6, 0, 24), RotateBilboard(new Vector3(6, 0, 24)), new Vector3(0.5f, 1.8f, 0.01f), _manTextureId[9], _cubeManTexcoords, false));
            bilboards.Add(new DrawingCube(new Vector3(16, 0, 24), RotateBilboard(new Vector3(16, 0, 24)), new Vector3(0.5f, 1.8f, 0.01f), _manTextureId[10], _cubeManTexcoords, false));
            bilboards.Add(new DrawingCube(new Vector3(8, 0, 25), RotateBilboard(new Vector3(8, 0, 25)), new Vector3(0.5f, 1.8f, 0.01f), _manTextureId[11], _cubeManTexcoords, false));
            bilboards.Add(new DrawingCube(new Vector3(22, 0, 25), RotateBilboard(new Vector3(22, 0, 25)), new Vector3(0.5f, 1.8f, 0.01f), _manTextureId[1], _cubeManTexcoords, false));

            bilboards.Add(new DrawingCube(new Vector3(8.5f, 0, 4.5f), RotateBilboard(new Vector3(8.5f, 0, 4.5f)), new Vector3(0.5f, 1.8f, 0.01f), _manTextureId[0], _cubeManTexcoords, false));
            bilboards.Add(new DrawingCube(new Vector3(11.5f, 0, 3.5f), RotateBilboard(new Vector3(11.5f, 0, 3.5f)), new Vector3(0.5f, 1.8f, 0.01f), _manTextureId[11], _cubeManTexcoords, false));
            bilboards.Add(new DrawingCube(new Vector3(13.5f, 0, 5.5f), RotateBilboard(new Vector3(13.5f, 0, 5.5f)), new Vector3(0.5f, 1.8f, 0.01f), _manTextureId[10], _cubeManTexcoords, false));
            bilboards.Add(new DrawingCube(new Vector3(17.5f, 0, 5.5f), RotateBilboard(new Vector3(17.5f, 0, 5.5f)), new Vector3(0.5f, 1.8f, 0.01f), _manTextureId[9], _cubeManTexcoords, false));
            bilboards.Add(new DrawingCube(new Vector3(21.5f, 0, 4.5f), RotateBilboard(new Vector3(21.5f, 0, 4.5f)), new Vector3(0.5f, 1.8f, 0.01f), _manTextureId[8], _cubeManTexcoords, false));
            bilboards.Add(new DrawingCube(new Vector3(23.5f, 0, 4.5f), RotateBilboard(new Vector3(23.5f, 0, 4.5f)), new Vector3(0.5f, 1.8f, 0.01f), _manTextureId[7], _cubeManTexcoords, false));
            bilboards.Add(new DrawingCube(new Vector3(28.5f, 0, 3.5f), RotateBilboard(new Vector3(28.5f, 0, 3.5f)), new Vector3(0.5f, 1.8f, 0.01f), _manTextureId[0], _cubeManTexcoords, false));
            bilboards.Add(new DrawingCube(new Vector3(7.5f, 0, 23.5f), RotateBilboard(new Vector3(7.5f, 0, 23.5f)), new Vector3(0.5f, 1.8f, 0.01f), _manTextureId[5], _cubeManTexcoords, false));
            bilboards.Add(new DrawingCube(new Vector3(13.5f, 0, 23.5f), RotateBilboard(new Vector3(13.5f, 0, 23.5f)), new Vector3(0.5f, 1.8f, 0.01f), _manTextureId[4], _cubeManTexcoords, false));
            bilboards.Add(new DrawingCube(new Vector3(12.5f, 0, 24.5f), RotateBilboard(new Vector3(12.5f, 0, 24.5f)), new Vector3(0.5f, 1.8f, 0.01f), _manTextureId[3], _cubeManTexcoords, false));
            bilboards.Add(new DrawingCube(new Vector3(25.5f, 0, 24.5f), RotateBilboard(new Vector3(25.5f, 0, 24.5f)), new Vector3(0.5f, 1.8f, 0.01f), _manTextureId[2], _cubeManTexcoords, false));
            bilboards.Add(new DrawingCube(new Vector3(27.5f, 0, 25.5f), RotateBilboard(new Vector3(27.5f, 0, 25.5f)), new Vector3(0.5f, 1.8f, 0.01f), _manTextureId[5], _cubeManTexcoords, false));
            bilboards.Add(new DrawingCube(new Vector3(30.5f, 0, 25.5f), RotateBilboard(new Vector3(30.5f, 0, 25.5f)), new Vector3(0.5f, 1.8f, 0.01f), _manTextureId[6], _cubeManTexcoords, false));

            bilboards.Add(new DrawingCube(new Vector3(19, 0, 8), RotateBilboard(new Vector3(19, 0, 8)), new Vector3(0.5f, 1.8f, 0.01f), _manTextureId[12], _cubeManTexcoords, false));

            for (int i = 0; i < particles.Count; i++)
            {
                var rowRotation = RotateBilboard(particles[i].GetParticleOffsets());

                bilboards.Add(new DrawingCube(particles[i].GetParticleOffsets(),
                    rowRotation, 
                    particles[i].GetParticleScale(),
                    _fireTextureId,
                    _cubeManTexcoords,
                    false)); 
            }

            bilboards.Sort(new BilboardsComparer(_eye[currentCameraIndex], _fireTextureId));

            foreach (var item in bilboards)
            {
                if (item.TextureId == _fireTextureId)
                {
                    GL.Uniform1(_uniformIfMaxLight, 1);
                    GL.Uniform1(_uniformOpacity, particles[0].GetParticleOpacity());
                    //GL.DepthMask(true);
                }
                DrawCubeWithShader(item);
                if (item.TextureId == _fireTextureId)
                {
                    GL.Uniform1(_uniformIfMaxLight, 0);
                    GL.Uniform1(_uniformOpacity, 1f);
                    //GL.DepthMask(false);
                }
            }

            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Update();
            }
        }

        private void DrawModelWithShader(Meshomatic.MeshData model, Color color, Vector3 offsets, Vector3 rotations, Vector3 scale)
        {
            GL.Uniform1(_uniformifTextured, 0);
            GL.Uniform1(_uniformifMarkTextured, 0);

            int hash = model.GetHashCode() ^ color.GetHashCode() ^ offsets.GetHashCode() ^ rotations.GetHashCode() ^ scale.GetHashCode() * (int)offsets.X;

            SetModelNormalMatrix(hash, offsets, rotations, scale);
            uint vboColors = GetVboColors(hash, model, color);
            uint vboVertices = GetVboVertices(hash, model);
            uint vboNormals = GetVboNormals(hash, model);
            uint iboTriangles = GetIboTriangles(hash, model);

            GL.UseProgram(_program);
            GL.EnableVertexAttribArray(_attributeColor);
            GL.EnableVertexAttribArray(_attributeCoord3);
            GL.EnableVertexAttribArray(_attributeNormal);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboVertices);

            GL.VertexAttribPointer(
                _attributeCoord3,                   // attribute
                3,                                  // number of elements per vertex, here (x,y)
                VertexAttribPointerType.Float,      // the type of each element
                false,                              // take our values as-is
                0,                                  // no extra data between each position
                0);                                 // offset of first element

            GL.BindBuffer(BufferTarget.ArrayBuffer, vboColors);

            GL.VertexAttribPointer(
                _attributeColor,                     // attribute
                4,                                  // number of elements per vertex, here (x,y)
                VertexAttribPointerType.Float,      // the type of each element
                false,                              // take our values as-is
                0,                                  // no extra data between each position
                0);                                 // offset of first element

            GL.BindBuffer(BufferTarget.ArrayBuffer, vboNormals);

            GL.VertexAttribPointer(
                _attributeNormal,                     // attribute
                3,                                  // number of elements per vertex, here (x,y)
                VertexAttribPointerType.Float,      // the type of each element
                false,                              // take our values as-is
                0,                                  // no extra data between each position
                0);                                 // offset of first element

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, iboTriangles);
            int size;
            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);
            GL.DrawElements(BeginMode.Triangles, size / sizeof(ushort), DrawElementsType.UnsignedShort, 0);

            GL.DisableVertexAttribArray(_attributeCoord3);
            GL.DisableVertexAttribArray(_attributeColor);
            GL.DisableVertexAttribArray(_attributeNormal);
        }

        private uint GetVboNormals(int hash, Meshomatic.MeshData model)
        {
            uint vboNormals;

            if (_vboObjectNormalsDictionary.Any(x => x.Key == hash))
            {
                vboNormals = _vboObjectNormalsDictionary[hash];
            }
            else
            {
                float[] actualModelNormals = model.NormalArrayFloat;

                GL.GenBuffers(1, out vboNormals);
                GL.BindBuffer(BufferTarget.ArrayBuffer, vboNormals);
                GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(actualModelNormals.Length * sizeof(float)), actualModelNormals, BufferUsageHint.StaticDraw);

                _vboObjectNormalsDictionary.Add(hash, vboNormals);
            }

            return vboNormals;
        }

        private uint GetIboTriangles(int hash, Meshomatic.MeshData model)
        {
            uint iboTriangles;
            if (_iboObjectTrianglesDictionary.Any(x => x.Key == hash))
            {
                iboTriangles = _iboObjectTrianglesDictionary[hash];
            }
            else
            {
                var actualModelTriangles = new ushort[3 * model.Tris.Count()];

                for (int i = 0; i < model.Tris.Count(); i++)
                {
                    actualModelTriangles[i * 3 + 0] = (ushort)model.Tris[i].P1.Vertex;
                    actualModelTriangles[i * 3 + 1] = (ushort)model.Tris[i].P2.Vertex;
                    actualModelTriangles[i * 3 + 2] = (ushort)model.Tris[i].P3.Vertex;
                }

                GL.GenBuffers(1, out iboTriangles);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, iboTriangles);
                GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(actualModelTriangles.Length * sizeof(ushort)), actualModelTriangles, BufferUsageHint.StaticDraw);

                _iboObjectTrianglesDictionary.Add(hash, iboTriangles);
            }
            return iboTriangles;
        }

        private uint GetVboVertices(int hash, Meshomatic.MeshData model)
        {
            uint vboVertices;

            if (_vboObjectVerticesDictionary.Any(x => x.Key == hash))
            {
                vboVertices = _vboObjectVerticesDictionary[hash];
            }
            else
            {
                float[] actualModelVertices = model.VertexArrayFloat;

                GL.GenBuffers(1, out vboVertices);
                GL.BindBuffer(BufferTarget.ArrayBuffer, vboVertices);
                GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(actualModelVertices.Length * sizeof(float)), actualModelVertices, BufferUsageHint.StaticDraw);

                _vboObjectVerticesDictionary.Add(hash, vboVertices);
            }

            return vboVertices;
        }

        private uint GetVboColors(int hash, Meshomatic.MeshData model, Color color)
        {
            uint vboColors;

            if (_vboObjectColorsDictionary.Any(x => x.Key == hash))
            {
                vboColors = _vboObjectColorsDictionary[hash];
            }
            else
            {
                var actualModelColor = new float[4 * model.Vertices.Count()];
                for (int i = 0; i < model.Vertices.Count(); i++)
                {
                    actualModelColor[i * 4 + 0] = color.R / 255f;
                    actualModelColor[i * 4 + 1] = color.G / 255f;
                    actualModelColor[i * 4 + 2] = color.B / 255f;
                    actualModelColor[i * 4 + 3] = color.A / 255f;
                }

                GL.GenBuffers(1, out vboColors);
                GL.BindBuffer(BufferTarget.ArrayBuffer, vboColors);
                GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(actualModelColor.Length * sizeof(float)), actualModelColor, BufferUsageHint.StaticDraw);

                _vboObjectColorsDictionary.Add(hash, vboColors);
            }

            return vboColors;
        }

        private void SetModelNormalMatrix(int hash, Vector3 offsets, Vector3 rotations, Vector3 scale)
        {
            Matrix4 transformMatrix;
            if (_objectModelMatrixDionary.Any(x => x.Key == hash))
            {
                transformMatrix = _objectModelMatrixDionary[hash];
            }
            else
            {
                transformMatrix = CreateTransformationMatrix(offsets, rotations, scale);
                _objectModelMatrixDionary.Add(hash, transformMatrix);
            }
            GL.UniformMatrix4(_uniformModelMatix, false, ref transformMatrix);

            Matrix3 normalMatrix;
            if (_objectNormalMatrixDionary.Any(x => x.Key == hash))
            {
                normalMatrix = _objectNormalMatrixDionary[hash];
            }
            else
            {
                normalMatrix = new Matrix3(transformMatrix);
                normalMatrix.Invert();
                normalMatrix.Transpose();
                _objectNormalMatrixDionary.Add(hash, normalMatrix);
            }
            GL.UniformMatrix3(_uniformNormalMatix, false, ref normalMatrix);
        }

        private void DrawCubeWithShader(DrawingCube cube)
        {
            Matrix4 transformMatrix = CreateTransformationMatrix(cube.Offsets, cube.Rotations, cube.Scale, cube.RotationFirst);
            GL.UniformMatrix4(_uniformModelMatix, false, ref transformMatrix);

            var normalMatrix = new Matrix3(transformMatrix);
            normalMatrix.Invert();
            normalMatrix.Transpose();
            GL.UniformMatrix3(_uniformNormalMatix, false, ref normalMatrix);

            if (!cube.RotationFirst)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vboCubeVertices);
                GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(_cubeVerticesMinus.Length * sizeof(float)), _cubeVerticesMinus, BufferUsageHint.DynamicDraw);
            }
            else
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vboCubeVertices);
                GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(_cubeVertices.Length * sizeof(float)), _cubeVertices, BufferUsageHint.DynamicDraw);
            }

            if (cube.TextureId == null || cube.Drawing == DrawingType.Lines)
            {
                if (cube.Drawing == DrawingType.Lines)
                    cube.Color = Color.Black;

                GL.Uniform1(_uniformifTextured, 0);
                GL.Uniform1(_uniformifMarkTextured, 0);

                for (int i = 0; i < _cubeColors.Length / 4; i++)
                {
                    _cubeColors[i * 4 + 0] = ((Color)cube.Color).R / 255f;
                    _cubeColors[i * 4 + 1] = ((Color)cube.Color).G / 255f;
                    _cubeColors[i * 4 + 2] = ((Color)cube.Color).B / 255f;
                    _cubeColors[i * 4 + 3] = ((Color)cube.Color).A / 255f;
                }

                GL.BindBuffer(BufferTarget.ArrayBuffer, _vboCubeColors);
                GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(_cubeColors.Length * sizeof(float)), _cubeColors, BufferUsageHint.DynamicDraw);
            }
            else
            {
                GL.Uniform1(_uniformifTextured, 1);

                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, (uint)cube.TextureId);
                _uniformTexture = GL.GetUniformLocation(_program, "myTexture");
                GL.Uniform1(_uniformTexture, 0);

                if (cube.TextureMapping == null)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, _vboCubeTexcoords);
                    GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(_cubeStandardTexcoords.Length * sizeof(float)), _cubeStandardTexcoords, BufferUsageHint.DynamicDraw);
                }
                else
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, _vboCubeTexcoords);
                    GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(cube.TextureMapping.Length * sizeof(float)), cube.TextureMapping, BufferUsageHint.DynamicDraw);
                }

                if (cube.MarksTextureId != null)
                {
                    GL.Uniform1(_uniformifMarkTextured, 1);

                    GL.ActiveTexture(TextureUnit.Texture1);
                    GL.BindTexture(TextureTarget.Texture2D, (uint)cube.MarksTextureId);
                    _uniformMarkTexture = GL.GetUniformLocation(_program, "myMarkTexture");
                    GL.Uniform1(_uniformMarkTexture, 1);
                }
                else
                {
                    GL.Uniform1(_uniformifMarkTextured, 0);
                }

                if (cube.HeightTextureId != null)
                {
                    GL.Uniform1(_uniformifHeightTextured, 1);

                    GL.ActiveTexture(TextureUnit.Texture2);
                    GL.BindTexture(TextureTarget.Texture2D, (uint)cube.HeightTextureId);
                    _uniformHeightTexture = GL.GetUniformLocation(_program, "myHeightTexture");
                    GL.Uniform1(_uniformHeightTexture, 2);
                }
                else
                {
                    GL.Uniform1(_uniformifHeightTextured, 0);
                }
            }

            for (int i = 0; i < _cubeTriangles.Length; i++)
            {
                if (cube.Drawing == DrawingType.TriangelesOutside)
                    _cubeTriangles[i] = _cubeTrianglesOutside[i];
                else if (cube.Drawing == DrawingType.TriangelesInside)
                    _cubeTriangles[i] = _cubeTrianglesInside[i];
                else if (cube.Drawing == DrawingType.Lines)
                    _cubeTriangles[i] = _cubeLines[i];
            }

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _iboCubeTriangles);
            GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(_cubeTriangles.Length * sizeof(ushort)), _cubeTriangles, BufferUsageHint.DynamicDraw);

            for (int i = 0; i < _cubeNormals.Length; i++)
            {
                if (cube.Drawing == DrawingType.TriangelesOutside)
                    _cubeNormals[i] = _cubeNormalsOutside[i];
                else if (cube.Drawing == DrawingType.TriangelesInside)
                    _cubeNormals[i] = _cubeNormalsInside[i];
                else if (cube.Drawing == DrawingType.Lines)
                    _cubeNormals[i] = _cubeNormalsOutside[i];
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboCubeNormals);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(_cubeNormals.Length * sizeof(float)), _cubeNormals, BufferUsageHint.DynamicDraw);

            GL.UseProgram(_program);
            GL.EnableVertexAttribArray(_attributeColor);
            GL.EnableVertexAttribArray(_attributeCoord3);
            GL.EnableVertexAttribArray(_attributeNormal);
            GL.EnableVertexAttribArray(_attributeTangent);
            GL.EnableVertexAttribArray(_attributeTexcoord);
            GL.EnableVertexAttribArray(_attributeMarkTexcoord);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboCubeVertices);

            GL.VertexAttribPointer(
                _attributeCoord3,                   // attribute
                3,                                  // number of elements per vertex, here (x,y)
                VertexAttribPointerType.Float,      // the type of each element
                false,                              // take our values as-is
                0,                                  // no extra data between each position
                0);                                 // offset of first element

            if (cube.TextureId == null)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vboCubeColors);

                GL.VertexAttribPointer(
                    _attributeColor, // attribute
                    4, // number of elements per vertex, here (x,y)
                    VertexAttribPointerType.Float, // the type of each element
                    false, // take our values as-is
                    0, // no extra data between each position
                    0); // offset of first element
            }
            else
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vboCubeTexcoords);

                GL.VertexAttribPointer(
                  _attributeTexcoord,               // attribute
                  2,                                // number of elements per vertex, here (x,y)
                  VertexAttribPointerType.Float,    // the type of each element
                  false,                            // take our values as-is
                  0,                                // no extra data between each position
                  0                                 // offset of first element
                );

                if (cube.MarksTextureId != null)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, _vboCubeMarkTexcoords);

                    GL.VertexAttribPointer(
                      _attributeMarkTexcoord,           // attribute
                      2,                                // number of elements per vertex, here (x,y)
                      VertexAttribPointerType.Float,    // the type of each element
                      false,                            // take our values as-is
                      0,                                // no extra data between each position
                      0                                 // offset of first element
                    );
                }
            }

            //if (cube.HeightTextureId != null)
            //{
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vboCubeTangents);

                GL.VertexAttribPointer(
                    _attributeTangent, // attribute
                    3, // number of elements per vertex, here (x,y)
                    VertexAttribPointerType.Float, // the type of each element
                    false, // take our values as-is
                    0, // no extra data between each position
                    0); // offset of first element
            //}

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboCubeNormals);

            GL.VertexAttribPointer(
                _attributeNormal,                   // attribute
                3,                                  // number of elements per vertex, here (x,y)
                VertexAttribPointerType.Float,      // the type of each element
                false,                              // take our values as-is
                0,                                  // no extra data between each position
                0);                                 // offset of first element

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _iboCubeTriangles);
            int size;
            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);
            GL.DrawElements(cube.Drawing == DrawingType.Lines ? BeginMode.Lines : BeginMode.Triangles,
                size / sizeof(ushort), DrawElementsType.UnsignedShort, 0);

            GL.DisableVertexAttribArray(_attributeCoord3);
            GL.DisableVertexAttribArray(_attributeColor);
            GL.DisableVertexAttribArray(_attributeNormal);
            GL.DisableVertexAttribArray(_attributeTangent);
            GL.DisableVertexAttribArray(_attributeTexcoord);
            GL.DisableVertexAttribArray(_attributeMarkTexcoord);
        }

        private Matrix4 CreateTransformationMatrix(Vector3 offsets, Vector3 rotations, Vector3 scale, bool rotationFirst = true)
        {
            Matrix4 transformMatrix = Matrix4.Identity;

            if (rotations != Vector3.Zero && rotationFirst)
            {
                transformMatrix *= Matrix4.CreateRotationX(rotations.X * (float)Math.PI / 180f);
                transformMatrix *= Matrix4.CreateRotationY(rotations.Y * (float)Math.PI / 180f);
                transformMatrix *= Matrix4.CreateRotationZ(rotations.Z * (float)Math.PI / 180f);
            }

            if (scale != Vector3.One)
            {
                transformMatrix *= Matrix4.CreateScale(scale);
            }

            if (rotations != Vector3.Zero && !rotationFirst)
            {
                transformMatrix *= Matrix4.CreateRotationX(rotations.X * (float)Math.PI / 180f);
                transformMatrix *= Matrix4.CreateRotationY(rotations.Y * (float)Math.PI / 180f);
                transformMatrix *= Matrix4.CreateRotationZ(rotations.Z * (float)Math.PI / 180f);
            }

            if (offsets != Vector3.Zero)
            {
                transformMatrix *= Matrix4.CreateTranslation(offsets);
            }

            return transformMatrix;
        }

        #endregion

        #region Helpers Methods

        private float[] ScaleArray(float[] array, double p)
        {
            float[] newArray = new float[array.Length];
            for (int i = 0; i < newArray.Length; i++)
            {
                newArray[i] = (float)(array[i] * p);
            }

            return newArray;
        }

        private Vector3 RotateBilboard(Vector3 position)
        {
            var camera = _eye[currentCameraIndex];
            float angle = (float)Math.Atan((camera.X - position.X) / (camera.Z - position.Z)) / (float)Math.PI * 180f;
            return new Vector3(0, angle, 0);
        }

        #endregion

        [STAThread]
        private static void Main()
        {
            using (Program p = new Program())
            {
                p.Run(60d);
            }
        }
    }
}