using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Particles
{
    public class InstanceBuffer<T>
    {
        bool mapped;
        int VERTEX_SIZE;
        int used;

        // Expose VAO so this InstanceBuffer can be attached to a ParticleBuffer
        public uint vaoHandle;
        uint vboHandle;

        public InstanceBuffer(uint VAO, int initialLength, uint attrib)
        {
            vaoHandle = VAO;
            vboHandle = Gl.GenBuffer();

            BindVAO();
            BindVBO();


            // Set up vertex attributes and divisors
            if (typeof(T) == typeof(uint))
            {
                var uintSize = Marshal.SizeOf(typeof(uint));
                VERTEX_SIZE = uintSize;


                // 1 uint
                Gl.EnableVertexAttribArray(attrib);
                Gl.VertexAttribPointer(attrib, uintSize, VertexAttribType.UnsignedByte, true, VERTEX_SIZE, (IntPtr)0);
                Gl.VertexAttribDivisor(attrib, 1);
            }
            else if (typeof(T) == typeof(Matrix3F))
            {
                var floatSize = Marshal.SizeOf(typeof(float));
                VERTEX_SIZE = floatSize * 12;


                // 3 rows of 4 floats
                for (uint i = 0; i < 3; i++)
                {
                    Gl.EnableVertexAttribArray(attrib + i);
                    Gl.VertexAttribPointer(attrib + i, 4, VertexAttribType.Float, false, VERTEX_SIZE, (IntPtr)(floatSize * i * floatSize));
                    Gl.VertexAttribDivisor(attrib + i, 1);
                }
            }
            else if (typeof(T) == typeof(Vector3F))
            {
                var fSize = Marshal.SizeOf(typeof(float));
                VERTEX_SIZE = fSize * 3;


                // 3 floats
                Gl.EnableVertexAttribArray(attrib);
                Gl.VertexAttribPointer(attrib, 3, VertexAttribType.Float, false, VERTEX_SIZE, (IntPtr)0);
                Gl.VertexAttribDivisor(attrib, 1);
            }


            // Allocate memory for this buffer
            BufferData(initialLength);


            UnbindVBO();
            UnbindVAO();
        }


        // GL Functions
        public IntPtr MapBuffer(int length)
        {
            Debug.Assert(!mapped);

            if (mapped)
                return IntPtr.Zero;


            // Bind and map the buffer
            BindVBO();
            var result = Gl.MapBufferRange(BufferTarget.ArrayBuffer, IntPtr.Zero, (uint)(length * VERTEX_SIZE), Gl.MAP_WRITE_BIT);
            UnbindVBO();


            // Only set to true if it successfully mapped
            mapped = result != IntPtr.Zero;


            return result;
        }

        public void UnmapBuffer()
        {
            Debug.Assert(mapped);

            if (!mapped)
                return;


            // Bind and unmap the buffer
            BindVBO();
            var success = Gl.UnmapBuffer(BufferTarget.ArrayBuffer);
            UnbindVBO();


            // In the rare case of an unsuccessful mapping, the buffer data must be reinitialised
            // https://www.khronos.org/registry/OpenGL-Refpages/gl4/html/glUnmapBuffer.xhtml
            if (!success)
                BufferData(used);


            // Set to false after successful unmapping
            mapped = false;
        }

        public void Expand(int newLength)
        {
            // Early exit, no need to expand
            if (used >= newLength)
                return;


            // At least double the buffer size, rather than lots of tiny increases
            newLength = Math.Max(newLength, used * 2);
            BufferData(newLength);
        }

        public void BufferData(int newLength)
        {
            used = newLength;


            BindVBO();
            Gl.BufferData(BufferTarget.ArrayBuffer, (uint)(used * VERTEX_SIZE), IntPtr.Zero, BufferUsage.StreamDraw);
            UnbindVBO();
        }


        // Shortcuts
        protected void BindVAO() => Gl.BindVertexArray(vaoHandle);
        protected void UnbindVAO() => Gl.BindVertexArray(0);
        public void BindVBO() => Gl.BindBuffer(BufferTarget.ArrayBuffer, vboHandle);
        public void UnbindVBO() => Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);


        // Disposal
        public void Dispose()
        {
            if (vboHandle != 0)
            {
                Gl.DeleteBuffers(vboHandle);
                vboHandle = 0;
            }

            if (vaoHandle != 0)
            {
                Gl.DeleteVertexArrays(vaoHandle);
                vaoHandle = 0;
            }
        }


        ~InstanceBuffer()
        {
            if (vaoHandle != 0)
                throw new InvalidOperationException("VAO not disposed");

            if (vboHandle != 0)
                throw new InvalidOperationException("VBO not disposed");
        }

    }
}
