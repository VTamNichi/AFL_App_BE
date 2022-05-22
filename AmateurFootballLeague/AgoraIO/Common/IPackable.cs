namespace AmateurFootballLeague.AgoraIO.Common
{
    public interface IPackable
    {
        ByteBuf marshal(ByteBuf outBuf);
    }
}
