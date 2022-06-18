namespace AmateurFootballLeague.AgoraIO.Common
{
    public class PackContent : IPackable
    {
        public byte[] signature;
        public uint crcChannelName;
        public uint crcUid;
        public byte[] rawMessage;

        public PackContent(byte[] signature, uint crcChannelName, uint crcUid, byte[] rawMessage)
        {
            this.signature = signature;
            this.crcChannelName = crcChannelName;
            this.crcUid = crcUid;
            this.rawMessage = rawMessage;
        }


        public ByteBuf Marshal(ByteBuf outBuf)
        {
            return outBuf.put(signature).put(crcChannelName).put(crcUid).put(rawMessage);
        }


        public void Unmarshal(ByteBuf inBuf)
        {
            this.signature = inBuf.readBytes();
            this.crcChannelName = inBuf.readInt();
            this.crcUid = inBuf.readInt();
            this.rawMessage = inBuf.readBytes();
        }
    }
}
