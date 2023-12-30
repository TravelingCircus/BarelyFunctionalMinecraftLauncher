namespace Common;

public sealed class Skin
{
    public IReadOnlyCollection<byte> SkinBytes => _skinBytes;
    private byte[] _skinBytes;

    public static Skin FromBytes(byte[] bytes)
    {
        if (bytes is null || bytes.Length < 1) throw new InvalidDataException("Empty byte array!");

        Skin skin = new Skin
        {
            _skinBytes = new byte[bytes.Length]
        };

        Array.Copy(bytes, skin._skinBytes, bytes.Length);
        return skin;
    }

    public static Skin Default() => throw new NotImplementedException();
}