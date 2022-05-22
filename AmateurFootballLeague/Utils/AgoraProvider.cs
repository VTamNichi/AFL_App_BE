using AmateurFootballLeague.AgoraIO.Media;

namespace AmateurFootballLeague.Utils
{
    public interface IAgoraProvider
    {
        string GenerateToken(string channel, string uId, uint expiredTime);
    }
    public class AgoraProvider : IAgoraProvider
    {
        public string GenerateToken(string channel, string uId, uint expiredTime)
        {
            try
            {
                var tokenBuilder = new AccessToken("629c856215b345779a8fb2a691f51976", "9414440564fc43bd83f859d09049f0b6", channel, uId);

                tokenBuilder.addPrivilege(Privileges.kJoinChannel, expiredTime);

                tokenBuilder.addPrivilege(Privileges.kPublishAudioStream, expiredTime);

                tokenBuilder.addPrivilege(Privileges.kPublishVideoStream, expiredTime);

                tokenBuilder.addPrivilege(Privileges.kPublishDataStream, expiredTime);

                tokenBuilder.addPrivilege(Privileges.kRtmLogin, expiredTime);

                return tokenBuilder.build();
            } catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                return "";
            }
        }
    }
}
