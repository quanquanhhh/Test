using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class SecureBundleCrypto
{
    // 你自己改掉，不要直接用我这个示例值
    // 建议 32 字节
    private static readonly byte[] MasterKey = new byte[]
    {
        0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF1,
        0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF, 0x10,
        0x87, 0x65, 0x43, 0x21, 0x0F, 0xED, 0xCB, 0xA9,
        0x98, 0x76, 0x54, 0x32, 0x11, 0x22, 0x33, 0x44
    };

    public const uint Magic = 0x59414352; // "YCAR" 随便定义
    public const int Version = 1;
    public const int HeaderSize = 4 + 4 + 16 + 8; // magic + version + nonce + plainLength

    public struct Header
    {
        public uint MagicValue;
        public int VersionValue;
        public byte[] Nonce;       // 16 bytes
        public long PlainLength;
    }

    public static bool ShouldEncrypt(string bundleName)
    {
        // 你自己改规则
        // 建议：只加密远端 bundle / 指定目录 / 指定 tags 相关资源
        return true;
    }

    public static byte[] DeriveAesKey(string bundleName)
    {
        // 每个 bundle 派生不同 key
        using (var hmac = new HMACSHA256(MasterKey))
        {
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(bundleName));
            byte[] key = new byte[32];
            Buffer.BlockCopy(hash, 0, key, 0, 32);
            return key;
        }
    }

    public static byte[] CreateRandomNonce16()
    {
        byte[] nonce = new byte[16];
        RandomNumberGenerator.Fill(nonce);
        return nonce;
    }

    public static byte[] BuildHeader(byte[] nonce16, long plainLength)
    {
        byte[] header = new byte[HeaderSize];
        using (var ms = new MemoryStream(header))
        using (var bw = new BinaryWriter(ms))
        {
            bw.Write(Magic);
            bw.Write(Version);
            bw.Write(nonce16);
            bw.Write(plainLength);
        }
        return header;
    }

    public static Header ReadHeader(Stream stream)
    {
        using (var br = new BinaryReader(stream, Encoding.UTF8, true))
        {
            Header header = new Header
            {
                MagicValue = br.ReadUInt32(),
                VersionValue = br.ReadInt32(),
                Nonce = br.ReadBytes(16),
                PlainLength = br.ReadInt64()
            };

            if (header.MagicValue != Magic)
                throw new Exception("Invalid encrypted bundle header magic.");

            if (header.VersionValue != Version)
                throw new Exception($"Unsupported encrypted bundle version : {header.VersionValue}");

            if (header.Nonce == null || header.Nonce.Length != 16)
                throw new Exception("Invalid encrypted bundle nonce.");

            return header;
        }
    }

    /// <summary>
    /// 直接对整个 byte[] 做 AES-CTR 变换（加密解密同一个方法）
    /// </summary>
    public static void TransformAesCtrInPlace(byte[] data, byte[] key32, byte[] nonce16, long absoluteOffset = 0)
    {
        if (data == null || data.Length == 0)
            return;

        using (var aes = Aes.Create())
        {
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.None;
            aes.Key = key32;
            aes.IV = new byte[16];

            using (var encryptor = aes.CreateEncryptor())
            {
                byte[] counterBlock = new byte[16];
                Buffer.BlockCopy(nonce16, 0, counterBlock, 0, 16);

                long blockIndex = absoluteOffset / 16;
                int blockOffset = (int)(absoluteOffset % 16);

                AddCounter(counterBlock, blockIndex);

                byte[] keystream = new byte[16];
                int dataIndex = 0;

                while (dataIndex < data.Length)
                {
                    encryptor.TransformBlock(counterBlock, 0, 16, keystream, 0);

                    for (int i = blockOffset; i < 16 && dataIndex < data.Length; i++)
                    {
                        data[dataIndex] ^= keystream[i];
                        dataIndex++;
                    }

                    blockOffset = 0;
                    IncrementCounter(counterBlock);
                }
            }
        }
    }

    public static byte[] DecryptWholeFile(string filePath, string bundleName)
    {
        using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            Header header = ReadHeader(fs);
            byte[] encrypted = new byte[fs.Length - HeaderSize];
            int read = fs.Read(encrypted, 0, encrypted.Length);
            if (read != encrypted.Length)
                throw new Exception("Read encrypted bundle failed.");

            byte[] key = DeriveAesKey(bundleName);
            TransformAesCtrInPlace(encrypted, key, header.Nonce, 0);
            return encrypted;
        }
    }

    private static void IncrementCounter(byte[] counter)
    {
        for (int i = 15; i >= 0; i--)
        {
            counter[i]++;
            if (counter[i] != 0)
                break;
        }
    }

    private static void AddCounter(byte[] counter, long value)
    {
        // 把 value 加到末尾 8 字节，够用了
        ulong add = (ulong)value;
        for (int i = 15; i >= 8; i--)
        {
            ulong sum = (ulong)counter[i] + (add & 0xFF);
            counter[i] = (byte)(sum & 0xFF);
            add >>= 8;

            if (sum > 0xFF)
            {
                int j = i - 1;
                while (j >= 0)
                {
                    counter[j]++;
                    if (counter[j] != 0)
                        break;
                    j--;
                }
            }
        }
    }
}