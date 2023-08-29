namespace LC6464.Base16384;

/// <summary>
/// Base16384 编解码器。
/// </summary>
public static partial class Base16384 {
	private static byte IsNextEnd(Stream stream) {
		var ch = stream.ReadByte();

		if (ch == -1) {
			return 0;
		}

		if (ch == '=') {
			return (byte)stream.ReadByte();
		}

		stream.Position--;
		return 0;
	}


	private static void EncodeLengthInternal(long dataLength, out long outLength, out long offset) {
		outLength = dataLength / 7 * 8;
		offset = dataLength % 7;
		switch (offset) {   // 算上偏移标志字符占用的2字节
			case 0: break;
			case 1: outLength += 4; break;
			case 2:
			case 3: outLength += 6; break;
			case 4:
			case 5: outLength += 8; break;
			case 6: outLength += 10; break;
			default: break; // outLength += 0;
		}
	}


	/// <summary>
	/// 原始数据缓冲区长度。
	/// </summary>
	public const int Buffer0Length = 8192 * 1024 / 7 * 7;

	/// <summary>
	/// 编码后数据缓冲区长度。
	/// </summary>
	public const int Buffer1Length = 8192 * 1024 / 8 * 8;


	/// <summary>
	/// UTF-16 BE 编码的 BOM，应在文件头部出现。
	/// </summary>
	public static ReadOnlySpan<byte> Utf16BEPreamble => new byte[] { 0xFE, 0xFF };

	/// <summary>
	/// UTF-16 LE 编码的 BOM，应在文件头部出现。
	/// </summary>
	public static ReadOnlySpan<byte> Utf16LEPreamble => new byte[] { 0xFF, 0xFE };


	/// <summary>
	/// 计算编码指针需要的长度。
	/// </summary>
	/// <param name="dataLength">数据长度</param>
	/// <returns>编码指针需要的长度</returns>
	public static long EncodeLength(long dataLength) {
		EncodeLengthInternal(dataLength, out var outLength, out _);
		return outLength + 8 + 16;  // 冗余的8B用于可能的结尾的覆盖，再加上16B备用
	}

	/// <summary>
	/// 计算解码指针需要的长度。
	/// </summary>
	/// <param name="dataLength">数据长度</param>
	/// <param name="offset">偏移量（默认为0，作用未知，可参见 GitHub: fumiama/base16384。）</param>
	/// <returns>解码指针需要的长度</returns>
	public static long DecodeLength(long dataLength, long offset = 0) {
		var outLength = dataLength;
		switch (offset) {   // 算上偏移标志字符占用的2字节
			case 0: break;
			case 1: outLength -= 4; break;
			case 2:
			case 3: outLength -= 6; break;
			case 4:
			case 5: outLength -= 8; break;
			case 6: outLength -= 10; break;
			default: break; // outLength += 0;
		}
		return (outLength / 8 * 7) + offset + 1 + 16; // 多出1字节用于循环覆盖，再加上16B备用
	}


	/// <summary>
	/// 将 <paramref name="data"/>（可能在非托管内存中）复制到托管内存中。
	/// </summary>
	/// <typeparam name="T"><paramref name="data"/> 的类型</typeparam>
	/// <param name="data">要复制的数据</param>
	/// <returns>复制结果</returns>
	public static ReadOnlySpan<T> CopyToManagedMemory<T>(this ReadOnlySpan<T> data) {
		Span<T> result = new(new T[data.Length]);
		data.CopyTo(result);
		return result;
	}

	/// <summary>
	/// 将非托管内存中的 <paramref name="data"/> 移动到托管内存中。
	/// </summary>
	/// <param name="data">要移动的数据</param>
	/// <returns>移动结果</returns>
	public static unsafe ReadOnlySpan<byte> MoveFromUnmanagedMemoryToManagedMemory(this ReadOnlySpan<byte> data) {
		var result = data.CopyToManagedMemory();
		fixed (byte* ptr = data) {
			Marshal.FreeHGlobal((nint)ptr);
		}
		return result;
	}
}