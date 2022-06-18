namespace AmateurFootballLeague.AgoraIO.Common
{
    public interface IPackable
    {
        ByteBuf Marshal(ByteBuf outBuf);
    }
}
