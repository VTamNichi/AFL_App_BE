namespace AmateurFootballLeague.AgoraIO.Common
{
    public class PrivilegeMessage : IPackable
    {
        public uint salt;
        public uint ts;
        public Dictionary<ushort, uint> messages;
        public PrivilegeMessage()
        {
            this.salt = (uint)Utils.Utils.randomInt();
            this.ts = (uint)(Utils.Utils.getTimestamp() + 24 * 3600);
            this.messages = new Dictionary<ushort, uint>();
        }

        public ByteBuf Marshal(ByteBuf outBuf)
        {
            return outBuf.put(salt).put(ts).putIntMap(messages);
        }

        public void Unmarshal(ByteBuf inBuf)
        {
            this.salt = inBuf.readInt();
            this.ts = inBuf.readInt();
            this.messages = inBuf.readIntMap();
        }
    }
}
