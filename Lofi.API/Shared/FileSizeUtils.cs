namespace Lofi.API.Shared.FileSizeUtils
{
    public static class FileSizeUtils
    {
        public static int Bytes(int b) => b;
        public static int KiloBytes(int kb) => kb * 1024;
        public static int MegaBytes(int mb) => mb * 1024 * 1024;
    }
}