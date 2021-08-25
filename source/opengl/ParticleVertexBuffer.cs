using System;

namespace Particles
{
    public class ParticleVertexBuffer : IDisposable
    {
        public ParticleVertexBuffer()
        {
            data = new ParticleVertex[ModelHelper.CUBE_STRIP_LENGTH];

            // Create VAO
            vaoHandle = Gl.GenVertexArray();
            Gl.BindVertexArray(vaoHandle);


            // Create VBO
            vboHandle = Gl.GenBuffer();
            Gl.BindBuffer(BufferTarget.ArrayBuffer, vboHandle);


            // Set vertex attribute info
            VERTEX_SIZE = 4;

            Gl.EnableVertexAttribArray(0);
            Gl.VertexAttribPointer(0, 4, VertexAttribType.Byte, false, VERTEX_SIZE, IntPtr.Zero);


            // Must set the divisor so the instance buffer can be attached
            Gl.VertexAttribDivisor(0, 0);


            // Clean up
            UnbindVAO();
            UnbindVBO();
        }


        public ParticleVertex[] data;
        int VERTEX_SIZE;
        int used;
        int vboSize;
        bool dirty;


        // We expose the VAO handle so we can attach an InstanceBuffer to it
        public uint vaoHandle;
        uint vboHandle;


        // Data functions
        public void Append(ParticleVertex v)
        {
            if (used >= data.Length)
                throw new Exception("The particle buffer is full");

            data[used++] = v;
            dirty = true;
        }

        public unsafe void BufferData()
        {
            if (!dirty)
                return;


            BindVBO();


            // We must use 'fixed' to get a pointer to memory managed by C#
            fixed (ParticleVertex* p = data)
            {
                if (vboSize < used)
                {
                    // Buffer with StaticDraw since we're only setting this once
                    Gl.BufferData(BufferTarget.ArrayBuffer, BytesUsed, (IntPtr)p, BufferUsage.StaticDraw);


                    // Remember how much data we've allocated on the GPU for this buffer
                    vboSize = used;
                }
                else
                {
                    Gl.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, BytesUsed, (IntPtr)p);
                }
            }


            UnbindVBO();


            // Clear after successfully buffering the data
            dirty = false;
            data = null;
        }



        // Shortcut functions
        protected uint BytesUsed => (uint)(used * VERTEX_SIZE);
        protected void BindVAO() => Gl.BindVertexArray(vaoHandle);
        protected void UnbindVAO() => Gl.BindVertexArray(0);
        protected void BindVBO() => Gl.BindBuffer(BufferTarget.ArrayBuffer, vboHandle);
        protected void UnbindVBO() => Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);



        // Disposal functions
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
        ~ParticleVertexBuffer()
        {
            if (vboHandle != 0)
                throw new InvalidOperationException($"VBO not disposed: {vboHandle} | {vaoHandle}");

            if (vaoHandle != 0)
                throw new InvalidOperationException($"VAO not disposed: {vboHandle} | {vaoHandle}");
        }
    }
}
