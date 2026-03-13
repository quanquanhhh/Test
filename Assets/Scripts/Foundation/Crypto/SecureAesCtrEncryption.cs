using System.IO;
using YooAsset;
using System;
using UnityEngine;

public class AesCtrDecryptStream : Stream
{
    private readonly FileStream _fileStream;
    private readonly byte[] _key;
    private readonly byte[] _nonce;
    private readonly long _dataStartOffset;
    private readonly long _plainLength;
    private long _position;

    public AesCtrDecryptStream(string filePath, string bundleName)
    {
        _fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        var header = SecureBundleCrypto.ReadHeader(_fileStream);
        _key = SecureBundleCrypto.DeriveAesKey(bundleName);
        _nonce = header.Nonce;
        _dataStartOffset = SecureBundleCrypto.HeaderSize;
        _plainLength = header.PlainLength;
        _position = 0;
    }

    public override bool CanRead => true;
    public override bool CanSeek => true;
    public override bool CanWrite => false;
    public override long Length => _plainLength;

    public override long Position
    {
        get => _position;
        set => Seek(value, SeekOrigin.Begin);
    }

    public override void Flush() { }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (buffer == null) throw new ArgumentNullException(nameof(buffer));
        if (offset < 0 || count < 0 || offset + count > buffer.Length)
            throw new ArgumentOutOfRangeException();

        if (_position >= _plainLength)
            return 0;

        int readableCount = (int)Math.Min(count, _plainLength - _position);

        _fileStream.Position = _dataStartOffset + _position;
        int actualRead = _fileStream.Read(buffer, offset, readableCount);

        if (actualRead <= 0)
            return 0;

        byte[] temp = new byte[actualRead];
        Buffer.BlockCopy(buffer, offset, temp, 0, actualRead);

        SecureBundleCrypto.TransformAesCtrInPlace(temp, _key, _nonce, _position);

        Buffer.BlockCopy(temp, 0, buffer, offset, actualRead);
        _position += actualRead;
        return actualRead;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        long newPos;
        switch (origin)
        {
            case SeekOrigin.Begin:
                newPos = offset;
                break;
            case SeekOrigin.Current:
                newPos = _position + offset;
                break;
            case SeekOrigin.End:
                newPos = _plainLength + offset;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(origin), origin, null);
        }

        if (newPos < 0)
            throw new IOException("Attempted to seek before beginning of stream.");

        _position = newPos;
        return _position;
    }

    public override void SetLength(long value) => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _fileStream?.Dispose();
        }
        base.Dispose(disposing);
    }
}

public class SecureAesCtrEncryption : IEncryptionServices
{
    public EncryptResult Encrypt(EncryptFileInfo fileInfo)
    {
        EncryptResult result = new EncryptResult();

        if (!SecureBundleCrypto.ShouldEncrypt(fileInfo.BundleName))
        {
            result.Encrypted = false;
            return result;
        }

        byte[] plainData = File.ReadAllBytes(fileInfo.FileLoadPath);
        byte[] key = SecureBundleCrypto.DeriveAesKey(fileInfo.BundleName);
        byte[] nonce = SecureBundleCrypto.CreateRandomNonce16();

        // AES-CTR 加密（原地）
        SecureBundleCrypto.TransformAesCtrInPlace(plainData, key, nonce, 0);

        byte[] header = SecureBundleCrypto.BuildHeader(nonce, plainData.Length);
        byte[] finalData = new byte[header.Length + plainData.Length];

        Buffer.BlockCopy(header, 0, finalData, 0, header.Length);
        Buffer.BlockCopy(plainData, 0, finalData, header.Length, plainData.Length);

        result.Encrypted = true;
        result.EncryptedData = finalData;
        return result;
    }
}


public class SecureAesCtrDecryption : IDecryptionServices
{
    private const uint ManagedReadBufferSize = 1024 * 64; // 64KB

    public DecryptResult LoadAssetBundle(DecryptFileInfo fileInfo)
    {
        var stream = new AesCtrDecryptStream(fileInfo.FileLoadPath, fileInfo.BundleName);

        DecryptResult result = new DecryptResult();
        result.ManagedStream = stream;
        result.Result = AssetBundle.LoadFromStream(stream, fileInfo.FileLoadCRC, ManagedReadBufferSize);
        return result;
    }

    public DecryptResult LoadAssetBundleAsync(DecryptFileInfo fileInfo)
    {
        var stream = new AesCtrDecryptStream(fileInfo.FileLoadPath, fileInfo.BundleName);

        DecryptResult result = new DecryptResult();
        result.ManagedStream = stream;
        result.CreateRequest = AssetBundle.LoadFromStreamAsync(stream, fileInfo.FileLoadCRC, ManagedReadBufferSize);
        return result;
    }

    public DecryptResult LoadAssetBundleFallback(DecryptFileInfo fileInfo)
    {
        byte[] plainData = SecureBundleCrypto.DecryptWholeFile(fileInfo.FileLoadPath, fileInfo.BundleName);

        DecryptResult result = new DecryptResult();
        result.Result = AssetBundle.LoadFromMemory(plainData, fileInfo.FileLoadCRC);
        return result;
    }

    public byte[] ReadFileData(DecryptFileInfo fileInfo)
    {
        return SecureBundleCrypto.DecryptWholeFile(fileInfo.FileLoadPath, fileInfo.BundleName);
    }

    public string ReadFileText(DecryptFileInfo fileInfo)
    {
        byte[] bytes = SecureBundleCrypto.DecryptWholeFile(fileInfo.FileLoadPath, fileInfo.BundleName);
        return System.Text.Encoding.UTF8.GetString(bytes);
    }
}