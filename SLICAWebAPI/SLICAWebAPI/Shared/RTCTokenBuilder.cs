using Force.Crc32;
using System.Security.Cryptography;
using System.Text;
using static AdmissionPortalAPI.BAL.AccessToken;


namespace AdmissionPortalAPI.BAL
{
    public class RTCTokenBuilder
    {

        public enum Role
        {
            RoleRtmUser = 1,
        }
        public static string buildToken(string appId, string appCertificate, string userAccount, string uid = "")
        {
            AccessToken accessToken = new AccessToken(appId, appCertificate, userAccount, uid);
            accessToken.addPrivilege(Privileges.kRtmLogin, 1446455471);
            return accessToken.build();
        }
    }

    public class AccessToken
    {
        public enum Privileges
        {
            kJoinChannel = 1,
            kPublishAudioStream = 2,
            kPublishVideoStream = 3,
            kPublishDataStream = 4,
            kRtmLogin = 1000
        }

        private string _appId;
        private string _appCertificate;
        private string _channelName;
        private string _uid;
        private uint _ts;
        private uint _salt;
        private byte[] _signature;
        private uint _crcChannelName;
        private uint _crcUid;
        private byte[] _messageRawContent;
        public PrivilegeMessage message = new PrivilegeMessage();

        public AccessToken(string appId, string appCertificate, string channelName, string uid)
        {
            _appId = appId;
            _appCertificate = appCertificate;
            _channelName = channelName;
            _uid = uid;
        }

        public AccessToken(string appId, string appCertificate, string channelName, string uid, uint ts, uint salt)
        {
            this._appId = appId;
            this._appCertificate = appCertificate;
            this._channelName = channelName;
            this._uid = uid;
            this._ts = ts;
            this._salt = salt;
        }

        public void addPrivilege(Privileges kJoinChannel, uint expiredTs)
        {
            this.message.messages.Add((ushort)kJoinChannel, expiredTs);
        }

        public string build()
        {

            this._messageRawContent = Utils.pack(this.message);
            this._signature = generateSignature(_appCertificate
                    , _appId
                    , _channelName
                    , _uid
                    , _messageRawContent);

            this._crcChannelName = Crc32Algorithm.Compute(this._channelName.GetByteArray());
            this._crcUid = Crc32Algorithm.Compute(this._uid.GetByteArray());

            PackContent packContent = new PackContent(_signature, _crcChannelName, _crcUid, this._messageRawContent);
            byte[] content = Utils.pack(packContent);
            return getVersion() + this._appId + Utils.base64Encode(content);
        }
        public static String getVersion()
        {
            return "006";
        }

        public static byte[] generateSignature(String appCertificate
                , String appID
                , String channelName
                , String uid
                , byte[] message)
        {

            using (var ms = new MemoryStream())
            using (BinaryWriter baos = new BinaryWriter(ms))
            {
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
    public class PrivilegeMessage : IPackable
    {
        public uint salt;
        public uint ts;
        public Dictionary<ushort, uint> messages;
        public PrivilegeMessage()
        {
            this.salt = (uint)Utils.randomInt();
            this.ts = (uint)(Utils.getTimestamp() + 24 * 3600);
            this.messages = new Dictionary<ushort, uint>();
        }

        public ByteBuf marshal(ByteBuf outBuf)
        {
            return outBuf.put(salt).put(ts).putIntMap(messages);
        }

        public void unmarshal(ByteBuf inBuf)
        {
            this.salt = inBuf.readInt();
            this.ts = inBuf.readInt();
            this.messages = inBuf.readIntMap();
        }
    }
    public interface IPackable
    {
        ByteBuf marshal(ByteBuf outBuf);
    }
    public class ByteBuf
    {
        ByteBuffer buffer = new ByteBuffer();
        public ByteBuf()
        {

        }

        public ByteBuf(byte[] source)
        {
            buffer.PushByteArray(source);
        }

        public byte[] asBytes()
        {
            return buffer.ToByteArray();
        }

        public ByteBuf put(ushort v)
        {
            buffer.PushUInt16(v);
            return this;
        }

        public ByteBuf put(uint v)
        {
            buffer.PushLong(v);
            return this;
        }

        public ByteBuf put(byte[] v)
        {
            put((ushort)v.Length);
            buffer.PushByteArray(v);
            return this;
        }

        public ByteBuf putIntMap(Dictionary<ushort, uint> extra)
        {
            put((ushort)extra.Count);

            foreach (var item in extra)
            {
                put(item.Key);
                put(item.Value);
            }
            return this;
        }

        public ushort readShort()
        {
            return buffer.PopUInt16();
        }

        public uint readInt()
        {
            return buffer.PopUInt();
        }

        public byte[] readBytes()
        {
            ushort length = readShort();
            byte[] bytes = new byte[length];
            return buffer.PopByteArray(length);
        }

        public Dictionary<ushort, uint> readIntMap()
        {
            Dictionary<ushort, uint> map = new Dictionary<ushort, uint>();

            ushort length = readShort();

            for (short i = 0; i < length; ++i)
            {
                ushort k = readShort();
                uint v = readInt();
                map.Add(k, v);
            }

            return map;
        }
    }
    public class ByteBuffer
    {
        private const int MAX_LENGTH = 1024;

        private byte[] TEMP_BYTE_ARRAY = new byte[MAX_LENGTH];

        private int CURRENT_LENGTH = 0;

        private int CURRENT_POSITION = 0;

        private byte[] RETURN_ARRAY;

        public ByteBuffer()
        {
            this.Initialize();
        }

        public ByteBuffer(byte[] bytes)
        {
            this.Initialize();
            this.PushByteArray(bytes);
        }
        public int Length
        {
            get
            {
                return CURRENT_LENGTH;
            }
        }

        public int Position
        {
            get
            {
                return CURRENT_POSITION;
            }
            set
            {
                CURRENT_POSITION = value;
            }
        }
        public byte[] ToByteArray()
        {
            RETURN_ARRAY = new byte[CURRENT_LENGTH];
            Array.Copy(TEMP_BYTE_ARRAY, 0, RETURN_ARRAY, 0, CURRENT_LENGTH);
            return RETURN_ARRAY;
        }

        public void Initialize()
        {
            TEMP_BYTE_ARRAY.Initialize();
            CURRENT_LENGTH = 0;
            CURRENT_POSITION = 0;
        }

        public void PushByte(byte by)
        {
            TEMP_BYTE_ARRAY[CURRENT_LENGTH++] = by;
        }

        public void PushByteArray(byte[] ByteArray)
        {
            ByteArray.CopyTo(TEMP_BYTE_ARRAY, CURRENT_LENGTH);
            CURRENT_LENGTH += ByteArray.Length;
        }

        public void PushUInt16(UInt16 Num)
        {
            TEMP_BYTE_ARRAY[CURRENT_LENGTH++] = (byte)((Num & 0x00ff) & 0xff);
            TEMP_BYTE_ARRAY[CURRENT_LENGTH++] = (byte)(((Num & 0xff00) >> 8) & 0xff);
        }

        public void PushInt(UInt32 Num)
        {
            TEMP_BYTE_ARRAY[CURRENT_LENGTH++] = (byte)((Num & 0x000000ff) & 0xff);
            TEMP_BYTE_ARRAY[CURRENT_LENGTH++] = (byte)(((Num & 0x0000ff00) >> 8) & 0xff);
            TEMP_BYTE_ARRAY[CURRENT_LENGTH++] = (byte)(((Num & 0x00ff0000) >> 16) & 0xff);
            TEMP_BYTE_ARRAY[CURRENT_LENGTH++] = (byte)(((Num & 0xff000000) >> 24) & 0xff);
        }

        public void PushLong(long Num)
        {
            TEMP_BYTE_ARRAY[CURRENT_LENGTH++] = (byte)((Num & 0x000000ff) & 0xff);
            TEMP_BYTE_ARRAY[CURRENT_LENGTH++] = (byte)(((Num & 0x0000ff00) >> 8) & 0xff);
            TEMP_BYTE_ARRAY[CURRENT_LENGTH++] = (byte)(((Num & 0x00ff0000) >> 16) & 0xff);
            TEMP_BYTE_ARRAY[CURRENT_LENGTH++] = (byte)(((Num & 0xff000000) >> 24) & 0xff);
        }

        public byte PopByte()
        {
            byte ret = TEMP_BYTE_ARRAY[CURRENT_POSITION++];
            return ret;
        }

        public UInt16 PopUInt16()
        {

            if (CURRENT_POSITION + 1 >= CURRENT_LENGTH)
            {
                return 0;
            }
            UInt16 ret = (UInt16)(TEMP_BYTE_ARRAY[CURRENT_POSITION] | TEMP_BYTE_ARRAY[CURRENT_POSITION + 1] << 8);
            CURRENT_POSITION += 2;
            return ret;
        }
        public uint PopUInt()
        {
            if (CURRENT_POSITION + 3 >= CURRENT_LENGTH)
                return 0;
            uint ret = (uint)(TEMP_BYTE_ARRAY[CURRENT_POSITION] | TEMP_BYTE_ARRAY[CURRENT_POSITION + 1] << 8 | TEMP_BYTE_ARRAY[CURRENT_POSITION + 2] << 16 | TEMP_BYTE_ARRAY[CURRENT_POSITION + 3] << 24);
            CURRENT_POSITION += 4;
            return ret;
        }

        public long PopLong()
        {
            if (CURRENT_POSITION + 3 >= CURRENT_LENGTH)
                return 0;
            long ret = (long)(TEMP_BYTE_ARRAY[CURRENT_POSITION] << 24 | TEMP_BYTE_ARRAY[CURRENT_POSITION + 1] << 16 | TEMP_BYTE_ARRAY[CURRENT_POSITION + 2] << 8 | TEMP_BYTE_ARRAY[CURRENT_POSITION + 3]);
            CURRENT_POSITION += 4;
            return ret;
        }

        public byte[] PopByteArray(int Length)
        {
            if (CURRENT_POSITION + Length > CURRENT_LENGTH)
            {
                return new byte[0];
            }
            byte[] ret = new byte[Length];
            Array.Copy(TEMP_BYTE_ARRAY, CURRENT_POSITION, ret, 0, Length);
            CURRENT_POSITION += Length;
            return ret;
        }

        public byte[] PopByteArray2(int Length)
        {
            if (CURRENT_POSITION <= Length)
            {
                return new byte[0];
            }
            byte[] ret = new byte[Length];
            Array.Copy(TEMP_BYTE_ARRAY, CURRENT_POSITION - Length, ret, 0, Length);
            CURRENT_POSITION -= Length;
            return ret;
        }

    }
    public class Utils
    {
        public static int getTimestamp()
        {
            return (int)new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        }

        public static int randomInt()
        {
            return new Random().Next();
        }

        public static byte[] pack(PrivilegeMessage packableEx)
        {
            ByteBuf buffer = new ByteBuf();
            packableEx.marshal(buffer);
            return buffer.asBytes();
        }
        public static byte[] pack(IPackable packableEx)
        {
            ByteBuf buffer = new ByteBuf();
            packableEx.marshal(buffer);
            return buffer.asBytes();
        }

        public static string base64Encode(byte[] data)
        {
            return Convert.ToBase64String(data);
        }
    }
    public class PackContent : IPackable
    {
        public byte[] signature;
        public uint crcChannelName;
        public uint crcUid;
        public byte[] rawMessage;

        public PackContent()
        {
        }

        public PackContent(byte[] signature, uint crcChannelName, uint crcUid, byte[] rawMessage)
        {
            this.signature = signature;
            this.crcChannelName = crcChannelName;
            this.crcUid = crcUid;
            this.rawMessage = rawMessage;
        }


        public ByteBuf marshal(ByteBuf outBuf)
        {
            return outBuf.put(signature).put(crcChannelName).put(crcUid).put(rawMessage);
        }


        public void unmarshal(ByteBuf inBuf)
        {
            this.signature = inBuf.readBytes();
            this.crcChannelName = inBuf.readInt();
            this.crcUid = inBuf.readInt();
            this.rawMessage = inBuf.readBytes();
        }
    }
    public class DynamicKeyUtil
    {
        public static byte[] encodeHMAC(String key, byte[] message, string alg = "SHA1")
        {
            return encodeHMAC(Encoding.UTF8.GetBytes(key), message, alg);
        }

        public static byte[] encodeHMAC(byte[] keyBytes, byte[] textBytes, string alg = "SHA1")
        {

            KeyedHashAlgorithm hash;
            switch (alg)
            {
                case "MD5":
                    hash = new HMACMD5(keyBytes);
                    break;
                case "SHA256":
                    hash = new HMACSHA256(keyBytes);
                    break;
                case "SHA384":
                    hash = new HMACSHA384(keyBytes);
                    break;
                case "SHA512":
                    hash = new HMACSHA512(keyBytes);
                    break;
                case "SHA1":
                default:
                    hash = new HMACSHA1(keyBytes);
                    break;
            }


            Byte[] hashBytes = hash.ComputeHash(textBytes);

            return hashBytes;
        }

        public static String bytesToHex(byte[] inData)
        {
            StringBuilder builder = new StringBuilder();
            foreach (byte b in inData)
            {
                builder.Append(b.ToString("X2"));
            }
            return builder.ToString().ToLower();
        }
    }
    public static class MemoryStreamExtensions
    {
        public static void write(this MemoryStream obj, string data)
        {
            var array = Encoding.UTF8.GetBytes(data);
            obj.Write(array, (int)obj.Length, array.Length);
        }

        public static byte[] GetByteArray(this string obj)
        {
            return Encoding.UTF8.GetBytes(obj);
        }
        public static byte[] getBytes(this string obj)
        {
            return Encoding.UTF8.GetBytes(obj);
        }
    }
}
