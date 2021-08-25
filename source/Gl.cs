using System;

namespace Particles
{
    public static class Gl
    {
        public const int MAP_WRITE_BIT = 2;

        public static uint GenBuffer() { return 1; }
        public static uint GenVertexArray() { return 2; }
        public static void DeleteBuffers(params uint[] buffers) { }
        public static void DeleteVertexArrays(params uint[] VertexArrays) { }
        public static void BindBuffer(BufferTarget index, uint buffer) { }
        public static void BindVertexArray(uint vertexArray) { }
        public static void DrawArrays(PrimitiveType mode, int first, int count) { }
        public static void DrawArraysInstanced(PrimitiveType mode, int first, int count, int instancecount) { }
        public static bool UnmapBuffer(BufferTarget target) { return true; }
        public static IntPtr MapBufferRange(BufferTarget target, IntPtr offset, uint length, uint access) { return IntPtr.Zero; }
        public static void BufferData(BufferTarget target, uint size, IntPtr data, BufferUsage usage) { }
        public static void BufferSubData(BufferTarget target, IntPtr offset, uint size, IntPtr data) { }
        public static void EnableVertexAttribArray(uint index) { }
        public static void VertexAttribDivisor(uint indx, uint divisor) { }
        public static void VertexAttribIPointer(uint indx, int size, VertexAttribType type, int stride, IntPtr ptr) { }
        public static void VertexAttribPointer(uint indx, int size, VertexAttribType type, bool normalized, Type structType, string memberName) { }
        public static void VertexAttribPointer(uint indx, int size, VertexAttribType type, bool normalized, int stride, IntPtr ptr) { }
    }

    public enum PrimitiveType
    {
        Points = 0,
        Lines = 1,
        LineLoop = 2,
        LineStrip = 3,
        Triangles = 4,
        TriangleStrip = 5,
        TriangleFan = 6,
        Quads = 7,
        QuadStrip = 8,
        Polygon = 9,
        LinesAdjacency = 10,
        LineStripAdjacency = 11,
        TrianglesAdjacency = 12,
        TriangleStripAdjacency = 13,
        Patches = 14
    }

    public enum BufferTarget
    {
        ArrayBuffer = 34962,
        ElementArrayBuffer = 34963,
        PixelPackBuffer = 35051,
        PixelUnpackBuffer = 35052,
        UniformBuffer = 35345,
        TextureBuffer = 35882,
        TransformFeedbackBuffer = 35982,
        CopyReadBuffer = 36662,
        CopyWriteBuffer = 36663,
        DrawIndirectBuffer = 36671,
        ShaderStorageBuffer = 37074,
        DispatchIndirectBuffer = 37102,
        QueryBuffer = 37266,
        AtomicCounterBuffer = 37568
    }

    public enum BufferUsage
    {
        StreamDraw = 35040,
        StreamRead = 35041,
        StreamCopy = 35042,
        StaticDraw = 35044,
        StaticRead = 35045,
        StaticCopy = 35046,
        DynamicDraw = 35048,
        DynamicRead = 35049,
        DynamicCopy = 35050
    }
    public enum VertexAttribType
    {
        Byte = 5120,
        UnsignedByte = 5121,
        Short = 5122,
        UnsignedShort = 5123,
        Int = 5124,
        UnsignedInt = 5125,
        Float = 5126,
        Double = 5130,
        HalfFloat = 5131,
        Fixed = 5132,
        UnsignedInt2101010Rev = 33640,
        UnsignedInt10f11f11fRev = 35899,
        Int2101010Rev = 36255
    }
}
