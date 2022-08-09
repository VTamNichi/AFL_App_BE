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
                var tokenBuilder = new AccessToken("ab5ddade1a8c4bfaa6f7018e03f73463", "13722f2747f949bd91cd096c64ca9364", channel, uId);

                tokenBuilder.AddPrivilege(Privileges.kJoinChannel, expiredTime);

                tokenBuilder.AddPrivilege(Privileges.kPublishAudioStream, expiredTime);

                tokenBuilder.AddPrivilege(Privileges.kPublishVideoStream, expiredTime);

                tokenBuilder.AddPrivilege(Privileges.kPublishDataStream, expiredTime);

                tokenBuilder.AddPrivilege(Privileges.kRtmLogin, expiredTime);

                return tokenBuilder.Build();
            } catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                return "";
            }
        }
    }
}
