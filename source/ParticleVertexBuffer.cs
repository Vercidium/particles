using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Particles
{

    public class ParticleVertexBuffer : IDisposable
    {
        public ParticleVertexBuffer()
        {
            data = new ParticleVertex[36];

            // Generate VAO and VBO
            arrayHandle = Gl.GenVertexArray();
            bufferHandle = Gl.GenBuffer();

            // Bind VAO and VBO
            Gl.BindVertexArray(arrayHandle);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, bufferHandle);

            // 4 bytes per ParticleVertex
            vertexSize = 4;

            Gl.EnableVertexAttribArray(0);
            Gl.VertexAttribPointer(0, 4, VertexAttribType.Byte, false, vertexSize, IntPtr.Zero);

            // Divisor must be set as we are using instance buffers
            Gl.VertexAttribDivisor(0, 0);

            // Unbind VAO and VBO
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
            Gl.BindVertexArray(0);
        }
        
        public ParticleVertex[] data;

        public uint arrayHandle;  // VAO
        public uint bufferHandle; // VBO

        protected int vertexSize;
        public int used;

        public void BufferData()
        {
            Gl.BindBuffer(BufferTarget.ArrayBuffer, bufferHandle);

            unsafe
            {
                fixed (ParticleVertex* p = data)
                {
                    Gl.BufferData(BufferTarget.ArrayBuffer, (uint)(used * vertexSize), (IntPtr)p, BufferUsage.StaticDraw);
                }
            }

            // No need to store data here anymore as it has been buffered
            data = null;

            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void Dispose()
        {
            if (bufferHandle != 0)
            {
                Gl.DeleteBuffers(bufferHandle);
                Gl.DeleteVertexArrays(arrayHandle);
                bufferHandle = 0;
                arrayHandle = 0;
            }
        }

        ~ParticleVertexBuffer()
        {
            if (bufferHandle != 0)
            {
                throw new InvalidOperationException("ParticleVertexBuffer not disposed");
            }
        }
    }   
}
