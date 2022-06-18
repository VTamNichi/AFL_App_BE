using AmateurFootballLeague.AgoraIO.Common;
using AmateurFootballLeague.AgoraIO.Extensions;
using AmateurFootballLeague.AgoraIO.Utils;
using Force.Crc32;

namespace AmateurFootballLeague.AgoraIO.Media
{
    public class AccessToken
    {
        private readonly string _appId;
        private readonly string _appCertificate;
        private readonly string _channelName;
        private readonly string _uid;
        private readonly uint _ts;
        private readonly uint _salt;

        private byte[]? _signature;
        private uint _crcChannelName;
        private uint _crcUid;
        private byte[]? _messageRawContent;
        public PrivilegeMessage message = new();

        public AccessToken(string appId, string appCertificate, string channelName, string uid)
        {
            _appId = appId;
            _appCertificate = appCertificate;
            _channelName = channelName;
            _uid = uid;
        }

        public AccessToken(string appId, string appCertificate, string channelName, string uid, uint ts, uint salt)
        {
            _appId = appId;
            _appCertificate = appCertificate;
            _channelName = channelName;
            _uid = uid;
            _ts = ts;
            _salt = salt;
        }

        public void AddPrivilege(Privileges kJoinChannel, uint expiredTs)
        {
            this.message.messages.Add((ushort)kJoinChannel, expiredTs);
        }

        public string Build()
        {
            //if (!Utils.isUUID(this.appId))
            //{
            //    return "";
            //}

            //if (!Utils.isUUID(this.appCertificate))
            //{
            //    return "";
            //}

            this._messageRawContent = Utils.Utils.pack(this.message);
            this._signature = GenerateSignature(_appCertificate
                    , _appId
                    , _channelName
                    , _uid
                    , _messageRawContent);

            this._crcChannelName = Crc32Algorithm.Compute(this._channelName.GetByteArray());
            this._crcUid = Crc32Algorithm.Compute(this._uid.GetByteArray());

            PackContent packContent = new(_signature, _crcChannelName, _crcUid, this._messageRawContent);
            byte[] content = Utils.Utils.pack(packContent);
            return GetVersion() + this._appId + Utils.Utils.base64Encode(content);
        }
        public static String GetVersion()
        {
            return "006";
        }

        public static byte[] GenerateSignature(String appCertificate
                , String appID
                , String channelName
                , String uid
                , byte[] message)
        {

            using var ms = new MemoryStream();
            using BinaryWriter baos = new(ms);
            baos.Write(appID.GetByteArray());
            baos.Write(channelName.GetByteArray());
            baos.Write(uid.GetByteArray());
            baos.Write(message);
            baos.Flush();

            byte[] sign = DynamicKeyUtil.encodeHMAC(appCertificate, ms.ToArray(), "SHA256");
            return sign;
        }
    }
}
