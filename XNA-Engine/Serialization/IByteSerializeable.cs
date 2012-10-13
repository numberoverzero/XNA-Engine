using Engine.DataStructures;

namespace Engine.Serialization
{
    public interface IByteSerializeable
    {
        byte[] AsByteArray();

        void BuildAsByteArray(ByteArrayBuilder builder);

        /// <summary>
        ///   <para> Returns the position of the last character of the object in the byte array. </para>
        ///   <para> Returns a number less than startIndex if the object does not start at the given index. </para>
        /// </summary>
        /// <param name="bytes"> </param>
        /// <param name="startIndex"> </param>
        /// <returns> </returns>
        int FromByteArray(byte[] bytes, int startIndex);
    }
}