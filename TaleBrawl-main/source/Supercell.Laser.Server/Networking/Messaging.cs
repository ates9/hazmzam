﻿using Supercell.Laser.Server.Networking.Security;
using Supercell.Laser.Titan.Library.Blake;

namespace Supercell.Laser.Server.Networking
{
    using Supercell.Laser.Logic.Message.Account;
    using Supercell.Laser.Logic.Message;
    using Supercell.Laser.Server.Message;
    using Supercell.Laser.Titan.Cryptography;
    using Supercell.Laser.Titan.Library;
    using Supercell.Laser.Titan.Math;
    using System.Linq;
    using Supercell.Laser.Logic.Util;
    using System.Net;
    using Org.BouncyCastle.Utilities.Net;
    using Discord;

    public class Messaging
    {
        public byte[] SessionToken { get; }

        private Connection Connection;
        private int PepperState;

        private StreamEncrypter Encrypter;
        private StreamEncrypter Decrypter;

        private MessageFactory MessageFactory;

        private byte[] RNonce;
        private byte[] SNonce;
        private byte[] SecretKey;
        private int mult;
        public byte serversc;
        private long cxmult;

        public int Seed { get; set; }

        private Dictionary<string, List<DateTime>> ipMessageLogs = new Dictionary<string, List<DateTime>>();

        private Dictionary<string, List<DateTime>> ipMessageLogs2 = new Dictionary<string, List<DateTime>>();

        private static readonly string BlockedIpsFilePath = "blocked_ips.txt";

        public Messaging(Connection connection)
        {
            Connection = connection;
            MessageFactory = MessageFactory.Instance;

            PepperState = 0;
            mult = 1;

            SessionToken = new byte[24];
            SNonce = new byte[24];
            SecretKey = new byte[32];
            serversc = 1;

            TweetNaCl.RandomBytes(SessionToken);
            TweetNaCl.RandomBytes(SNonce);
            TweetNaCl.RandomBytes(SecretKey);
        }

        public void Send(GameMessage message)
        {
            if (message.GetMessageType() == 20100 || message.GetMessageType() == 24115)
            {
                EncryptAndWrite(message);
            }
            else
            {
                Processor.Send(Connection, message);
            }
        }

        public void EncryptAndWrite(GameMessage message)
        {
            try
            {
                if (message.GetEncodingLength() == 0) message.Encode();

                byte[] payload = new byte[message.GetEncodingLength()];
                Buffer.BlockCopy(message.GetMessageBytes(), 0, payload, 0, payload.Length);

                int messageType = message.GetMessageType();
                int version = message.GetVersion();

                byte[] stream = new byte[payload.Length + 7];

                int length = payload.Length;

                stream[0] = (byte)(messageType >> 8);
                stream[1] = (byte)(messageType);
                stream[2] = (byte)(length >> 16);
                stream[3] = (byte)(length >> 8);
                stream[4] = (byte)(length);
                stream[5] = (byte)(version >> 8);
                stream[6] = (byte)(version);

                Buffer.BlockCopy(payload, 0, stream, 7, payload.Length);
                Connection.Write(stream);
                byte[] tss = { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, };
                Connection.Write(tss);
            }
            catch
            {

            }
        }
        public long GetChecksum(int type, int length, int version)
        {
            long ck = (serversc + type * ((version + 1) + length) + (version + 1) + 9 + cxmult) * serversc * mult + cxmult;
            return ck % 65535;
        }
        public bool VerifyChecksum(int a1, int type, int length, int version)
        {
            if (type >= 19900 && type <= 20000 && PepperState == 0)
            {
                PepperState = 1;
                return true;
            }
            if (type >= 11000 && type <= 11010 && PepperState == 0)
            {
                PepperState = 1;
                return true;
            }
            if (type == 10100 && (a1 == 2901 || a1 == 2802 || a1 == 1337) && PepperState == 0)
            {
                PepperState = 1;
                return true;
            }
            else if (PepperState == 1)
            {
                long v1 = GetChecksum(type, length, version);
                int v2 = a1;
                if (v1 < 0)
                {
                    return false;
                }


                mult++;
                cxmult++;
                if (mult > 15)
                {
                    mult = 1;
                    cxmult = 0;
                }
                if (cxmult > 255)
                {
                    mult++;
                    cxmult = 0;
                }
                if (v2 == v1)
                {
                    return true;
                }
                else if ((v2 - 3) < v1 && v1 < (v2 + 3))
                {
                    cxmult += v2 - v1;
                    return true;
                }
                return false;
            }
            return false;
        }
        public int OnReceive()
        {
            long position = Connection.Memory.Position;
            Connection.Memory.Position = 0;

            byte[] headerBuffer = new byte[7];
            Connection.Memory.Read(headerBuffer, 0, 7);

            // Messaging::readHeader inling? yes.
            int type = headerBuffer[0] << 8 | headerBuffer[1];
            int length = headerBuffer[2] << 16 | headerBuffer[3] << 8 | headerBuffer[4];
            int version = headerBuffer[5] << 8 | headerBuffer[6];


            byte[] payload = new byte[length];
            if (Connection.Memory.Read(payload, 0, length) < length)
            { // Packet still not received
                Connection.Memory.Position = position;
                return 0;
            }
            string ipAddress = Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0];
            if (this.ReadNewMessage(type, length, version, payload) != 0)
            {
                return -1;
            }

            byte[] all = Connection.Memory.ToArray();
            byte[] buffer = all.Skip(length + 7).ToArray();

            Connection.Memory = new MemoryStream();
            Connection.Memory.Write(buffer, 0, buffer.Length);

            if (buffer.Length >= 7) OnReceive();
            return 0;
        }

        private int ReadNewMessage(int type, int length, int version, byte[] payload)
        {
            string ipAddress = Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0];
            GameMessage message = MessageFactory.CreateMessageByType(type);
            if (message != null)
            {
                message.GetByteStream().SetByteArray(payload, payload.Length);
                message.Decode();
                if (message.GetMessageType() == 10100)
                {
                    Connection.MessageManager.ReceiveMessage(message);
                }
                else
                {
                    Processor.Receive(Connection, message);
                }
                //if (Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0].StartsWith("188")) Console.WriteLine("SJS MK BRUH ::: " + type);

                /*if (type == 10101)
                {
                    DateTime now = DateTime.UtcNow;

                    if (!ipMessageLogs2.ContainsKey(ipAddress))
                    {
                        ipMessageLogs2[ipAddress] = new List<DateTime>();
                    }

                    ipMessageLogs2[ipAddress].Add(now);

                    ipMessageLogs2[ipAddress] = ipMessageLogs2[ipAddress]
                        .Where(t => (now - t).TotalSeconds <= 6)
                        .ToList();

                    if (ipMessageLogs2[ipAddress].Count > 3)
                    {
                        File.AppendAllLines(BlockedIpsFilePath, new[] { Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0] });
                        Logger.Print($"fast login porno");
                        return 0;
                    }
                }*/

                if (type == 14110)
                {
                    DateTime now = DateTime.UtcNow;

                    if (!ipMessageLogs.ContainsKey(ipAddress))
                    {
                        ipMessageLogs[ipAddress] = new List<DateTime>();
                    }

                    ipMessageLogs[ipAddress].Add(now);

                    ipMessageLogs[ipAddress] = ipMessageLogs[ipAddress]
                        .Where(t => (now - t).TotalSeconds <= 10)
                        .ToList();

                    if (ipMessageLogs[ipAddress].Count > 1)
                    {
                        File.AppendAllLines(BlockedIpsFilePath, new[] { Connection.Socket.RemoteEndPoint.ToString().Split(" ")[0] });
                        Logger.Print($"battleend hack porno");
                        return 0;
                    }
                }
            }
            else
            {
                Logger.Print("Ignoring message of unknown type " + type);
            }

            return 0;
        }

        private byte[] client_pk;
        private byte[] client_sk;

        private byte[] HandlePepperLogin(byte[] payload)
        {
            try
            {
                client_pk = payload.Take(32).ToArray();

                client_sk = PepperKey.CLIENT_SK;
                LogicRandom rand = new LogicRandom(Seed);

                int c = 0;
                for (int i = 0; i < 12; i++)
                {
                    c = rand.Rand(256);
                }

                for (int i = 0; i < 32; i++)
                {
                    //client_sk[i] = (byte)(rand.Rand(256) ^ c);
                }

                if (!PepperKey.CLIENT_PK.SequenceEqual(client_pk))
                {
                    byte[] b = PepperKey.CLIENT_PK;
                    foreach (byte b2 in b)
                    {
                        byte[] b3 = { b2, };
                        Console.Write("0x" + BitConverter.ToString(b3) + ", ");
                    }
                    Console.Write('\n');
                    byte[] b4 = client_pk;
                    foreach (byte b2 in b4)
                    {
                        byte[] b3 = { b2, };
                        Console.Write("0x" + BitConverter.ToString(b3) + ", ");
                    }

                    Logger.Print("Cryptography error. 10101 public key is invalid!");
                    return null;
                }

                Blake2BHasher hasher = new Blake2BHasher();
                hasher.Update(client_pk);
                hasher.Update(PepperKey.SERVER_PK);
                byte[] nonce = hasher.Finish();

                byte[] decrypted = TweetNaCl.CryptoBoxOpen(payload.Skip(32).ToArray(), nonce, PepperKey.SERVER_PK, client_sk);

                byte[] session_token = decrypted.Take(24).ToArray();
                if (!session_token.SequenceEqual(SessionToken))
                {
                    Logger.Print("Cryptography error. Session token is invalid!");
                    return null;
                }

                PepperState = 4;

                RNonce = decrypted.Skip(24).Take(24).ToArray();

                return decrypted.Skip(48).ToArray();
            }
            catch (TweetNaCl.InvalidCipherTextException)
            {
                Console.WriteLine("Failed to decrypt 10101");
                return null;
            }
        }

        private byte[] SendPepperLoginResponse(byte[] payload)
        {
            byte[] packet = new byte[payload.Length + 32 + 24];

            Buffer.BlockCopy(SNonce, 0, packet, 0, 24);
            Buffer.BlockCopy(SecretKey, 0, packet, 24, 32);
            Buffer.BlockCopy(payload, 0, packet, 24 + 32, payload.Length);

            Blake2BHasher hasher = new Blake2BHasher();
            hasher.Update(RNonce);
            hasher.Update(client_pk);
            hasher.Update(PepperKey.SERVER_PK);
            byte[] nonce = hasher.Finish();

            byte[] encrypted = TweetNaCl.CryptoBox(packet, nonce, PepperKey.SERVER_PK, client_sk);

            PepperState = 5;

            Decrypter = new PepperEncrypter(SecretKey, RNonce);
            Encrypter = new PepperEncrypter(SecretKey, SNonce);

            return encrypted;
        }
    }
}
