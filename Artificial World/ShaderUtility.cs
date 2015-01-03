using OpenTK.Graphics.OpenGL;
using System;
using System.IO;

namespace Artificial_World
{
    public static class ShaderUtility
    {
        public static int CreateShader(string filename, ShaderType type)
        {
            string infoMsg;
            string source = ReadFile(filename);
            int handler = GL.CreateShader(type);
            const string version = "#version 120\n";
            string combined = String.Format("{0} {1}", version, source);
            GL.ShaderSource(handler, combined);
            GL.CompileShader(handler);
            GL.GetShaderInfoLog(handler, out infoMsg);
            Console.WriteLine(infoMsg);
            return handler;
        }

        private static string ReadFile(string fileName)
        {
            using (FileStream stream = File.Open("Shaders/" + fileName, FileMode.Open))
            {
                var reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
        }
    }
}
