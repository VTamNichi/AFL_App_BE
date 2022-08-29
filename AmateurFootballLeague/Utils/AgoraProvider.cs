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
                var tokenBuilder = new AccessToken("70217642f3314dc0803bb253e501cf2d", "2059d409372446b2afaba92a503bef26", channel, uId);

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
