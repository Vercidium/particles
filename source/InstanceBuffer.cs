using OpenGL;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Particles
{
    public class InstanceBuffer<T>
    {
        public InstanceBuffer(uint VAO, int initialSize, uint attrib)
        {
            // Instance buffers share a VAO with the ParticleVertexBuffer
            arrayHandle = VAO;
            bufferHandle = Gl.GenBuffer();

            Gl.BindVertexArray(arrayHandle);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, bufferHandle);

            if (typeof(T) == typeof(uint))
            {
                vertexSize = Marshal.SizeOf(typeof(uint));

                Gl.EnableVertexAttribArray(attrib);

                // uint is treated as 4 bytes so it can be used as a colour in the shader
                Gl.VertexAttribPointer(attrib, 4, VertexAttribType.UnsignedByte, true, vertexSize, (IntPtr)0);

                // Must set attribute divisor
                Gl.VertexAttribDivisor(attrib, 1);
            }
            else if (typeof(T) == typeof(Matrix4F))
            {
                var fSize = Marshal.SizeOf(typeof(float));
                vertexSize = fSize * 16;

                for (uint i = 0; i < 4; i++)
                {
                    Gl.EnableVertexAttribArray(attrib + i);

                    // Matrix4F is treated as 4 sets of 4 floats in the shader
                    Gl.VertexAttribPointer(attrib + i, 4, VertexAttribType.Float, false, vertexSize, (IntPtr)(fSize * 4 * i));

                    // Must set attribute divisor
                    Gl.VertexAttribDivisor(attrib + i, 1);
                }
            }

            // Allocate memory on the GPU
            BufferData(initialSize);

            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
            Gl.BindVertexArray(0);
        }

        public uint arrayHandle;
        public uint bufferHandle;
        public int vertexSize;

        public int currentLength;

        public void Bind()
        {
            Gl.BindBuffer(BufferTarget.ArrayBuffer, bufferHandle);
        }

        public void Expand(int newLength)
        {
            // No need to expand
            if (newLength < currentLength)
                return;

            // Ensure we increase thebuffer by a sizeable amount, rather than lots of small increases
            newLength = Math.Max(newLength, currentLength * 2);

            BufferData(newLength);
        }

        public void BufferData(int newLength)
        {
            currentLength = newLength;

            // Bind, buffer and unbind
            Gl.BindBuffer(BufferTarget.ArrayBuffer, bufferHandle);
            Gl.BufferData(BufferTarget.ArrayBuffer, (uint)(currentLength * vertexSize), IntPtr.Zero, BufferUsage.DynamicDraw);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        ~InstanceBuffer()
        {
            if (bufferHandle != 0)
            {
                throw new InvalidOperationException("VertexBuffer not disposed");
            }
        }

        public void Dispose()
        {
            if (bufferHandle != 0)
            {
                Gl.DeleteBuffers(bufferHandle);
                bufferHandle = 0;
            }
        }
    }
}
