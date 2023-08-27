namespace LC6464.Base16384;

/// <summary>
/// Base16384 编解码器。
/// </summary>
public static partial class Base16384 {
	/// <summary>
	/// 编码二进制数据流中的数据至 Base16384 UTF-16 BE 编码数据，追加到输出数据流。
	/// </summary>
	/// <param name="stream">二进制数据流</param>
	/// <param name="output">输出数据流</param>
	/// <returns>已写入的数据长度</returns>
	public static long EncodeToStream(Stream stream, Stream output) =>
		EncodeToStream(stream, output, new byte[stream.Length > Buffer0Length ? Buffer0Length : stream.Length - stream.Position]);

	/// <summary>
	/// 解码 Base16384 UTF-16 BE 编码数据流中的数据至二进制数据，追加到输出数据流。
	/// </summary>
	/// <param name="stream">Base16384 UTF-16 BE 编码数据流</param>
	/// <param name="output">输出数据流</param>
	/// <returns>已写入的数据长度</returns>
	public static long DecodeToStream(Stream stream, Stream output) =>
		DecodeToStream(stream, output, new byte[stream.Length > Buffer1Length ? Buffer1Length + 2 : stream.Length - stream.Position]);


	/// <summary>
	/// 编码二进制数据至 Base16384 UTF-16 BE 编码数据，追加到输出数据流。
	/// </summary>
	/// <param name="data">二进制数据</param>
	/// <param name="output">输出数据流</param>
	/// <returns>已写入的数据长度</returns>
	public static long EncodeToStream(ReadOnlySpan<byte> data, Stream output) =>
		EncodeToStream(data, output, data.Length > Buffer0Length ? new byte[Buffer0Length] : Span<byte>.Empty);

	/// <summary>
	/// 解码 Base16384 UTF-16 BE 编码数据至二进制数据，追加到输出数据流。
	/// </summary>
	/// <param name="data">Base16384 UTF-16 BE 编码数据</param>
	/// <param name="output">输出数据流</param>
	/// <returns>已写入的数据长度</returns>
	public static long DecodeToStream(ReadOnlySpan<byte> data, Stream output) =>
		DecodeToStream(data, output, data.Length > Buffer1Length ? new byte[Buffer1Length + 2] : Span<byte>.Empty);


	/// <summary>
	/// 编码二进制数据流中的数据到新的 Base16384 UTF-16 BE 编码数据流。
	/// </summary>
	/// <param name="stream">二进制数据流</param>
	/// <returns>Base16384 UTF-16 BE 编码数据流</returns>
	public static MemoryStream EncodeToNewMemoryStream(Stream stream) {
		var output = new MemoryStream();
		_ = EncodeToStream(stream, output);
		return output;
	}

	/// <summary>
	/// 解码 Base16384 UTF-16 BE 编码数据流中的数据到新的二进制数据流。
	/// </summary>
	/// <param name="stream">Base16384 UTF-16 BE 编码数据流</param>
	/// <returns>二进制数据流</returns>
	public static MemoryStream DecodeToNewMemorySteam(Stream stream) {
		var output = new MemoryStream();
		_ = DecodeToStream(stream, output);
		return output;
	}


	/// <summary>
	/// 编码二进制数据到新的 Base16384 UTF-16 BE 编码数据流。
	/// </summary>
	/// <param name="data">二进制数据</param>
	/// <returns>Base16384 UTF-16 BE 编码数据流</returns>
	public static MemoryStream EncodeToNewMemoryStream(ReadOnlySpan<byte> data) {
		var output = new MemoryStream();
		_ = EncodeToStream(data, output);
		return output;
	}

	/// <summary>
	/// 解码 Base16384 UTF-16 BE 编码数据到新的二进制数据流。
	/// </summary>
	/// <param name="data">Base16384 UTF-16 BE 编码数据</param>
	/// <returns>二进制数据流</returns>
	public static MemoryStream DecodeToNewMemorySteam(ReadOnlySpan<byte> data) {
		var output = new MemoryStream();
		_ = DecodeToStream(data, output);
		return output;
	}
}