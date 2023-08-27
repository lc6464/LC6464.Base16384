namespace LC6464.Base16384;

/// <summary>
/// Base16384 编解码器。
/// </summary>
public static partial class Base16384 {
	/// <summary>
	/// 编码二进制数据流中的数据到 Base16384 UTF-16 BE 编码文件。
	/// </summary>
	/// <param name="stream">二进制数据流</param>
	/// <param name="fileInfo">创建的 Base16384 UTF-16 BE 编码文件信息</param>
	/// <returns>已写入的数据长度</returns>
	public static long EncodeToNewFile(Stream stream, FileInfo fileInfo) =>
		EncodeToNewFile(stream, fileInfo, new byte[stream.Length > Buffer0Length ? Buffer0Length : stream.Length - stream.Position]);

	/// <summary>
	/// 解码 Base16384 UTF-16 BE 编码数据流中的数据到二进制文件。
	/// </summary>
	/// <param name="stream">Base16384 UTF-16 BE 编码数据流</param>
	/// <param name="fileInfo">创建的二进制文件信息</param>
	/// <returns>已写入的数据长度</returns>
	public static long DecodeToNewFile(Stream stream, FileInfo fileInfo) =>
		DecodeToNewFile(stream, fileInfo, new byte[stream.Length > Buffer1Length ? Buffer1Length + 2 : stream.Length - stream.Position]);


	/// <summary>
	/// 编码二进制数据到 Base16384 UTF-16 BE 编码文件。
	/// </summary>
	/// <param name="data">二进制数据</param>
	/// <param name="fileInfo">创建的 Base16384 UTF-16 BE 编码文件信息</param>
	/// <returns>已写入的数据长度</returns>
	public static long EncodeToNewFile(ReadOnlySpan<byte> data, FileInfo fileInfo) =>
		EncodeToNewFile(data, fileInfo, data.Length > Buffer0Length ? new byte[Buffer0Length] : Span<byte>.Empty);

	/// <summary>
	/// 解码 Base16384 UTF-16 BE 编码数据到二进制文件。
	/// </summary>
	/// <param name="data">Base16384 UTF-16 BE 编码数据</param>
	/// <param name="fileInfo">创建的二进制文件信息</param>
	/// <returns>已写入的数据长度</returns>
	public static long DecodeToNewFile(ReadOnlySpan<byte> data, FileInfo fileInfo) =>
		DecodeToNewFile(data, fileInfo, data.Length > Buffer1Length ? new byte[Buffer1Length + 2] : Span<byte>.Empty);


	/// <summary>
	/// 编码二进制文件中的数据至 Base16384 UTF-16 BE 编码数据，追加到输出数据流。
	/// </summary>
	/// <param name="fileInfo">二进制文件信息</param>
	/// <param name="output">输出数据流</param>
	/// <returns>已写入的数据长度</returns>
	public static long EncodeFromFileToStream(FileInfo fileInfo, Stream output) =>
		EncodeFromFileToStream(fileInfo, output, new byte[fileInfo.Length > Buffer0Length ? Buffer0Length : fileInfo.Length]);

	/// <summary>
	/// 解码 Base16384 UTF-16 BE 编码文件中的数据至二进制数据，追加到输出数据流。
	/// </summary>
	/// <param name="fileInfo">Base16384 UTF-16 BE 编码文件信息</param>
	/// <param name="output">输出数据流</param>
	/// <returns>已写入的数据长度</returns>
	public static long DecodeFromFileToStream(FileInfo fileInfo, Stream output) =>
		DecodeFromFileToStream(fileInfo, output, new byte[fileInfo.Length > Buffer1Length ? Buffer1Length + 2 : fileInfo.Length]);


	/// <summary>
	/// 编码二进制文件中的数据至 Base16384 UTF-16 BE 编码文件。
	/// </summary>
	/// <param name="fileInfo">二进制文件信息</param>
	/// <param name="outputFileInfo">Base16384 UTF-16 BE 编码文件信息</param>
	/// <returns>已写入的数据长度</returns>
	public static long EncodeFromFileToNewFile(FileInfo fileInfo, FileInfo outputFileInfo) =>
		EncodeFromFileToNewFile(fileInfo, outputFileInfo, new byte[fileInfo.Length > Buffer0Length ? Buffer0Length : fileInfo.Length]);

	/// <summary>
	/// 解码 Base16384 UTF-16 BE 编码文件中的数据至二进制文件。
	/// </summary>
	/// <param name="fileInfo">Base16384 UTF-16 BE 编码文件信息</param>
	/// <param name="outputFileInfo">二进制文件信息</param>
	/// <returns>已写入的数据长度</returns>
	public static long DecodeFromFileToNewFile(FileInfo fileInfo, FileInfo outputFileInfo) =>
		DecodeFromFileToNewFile(fileInfo, outputFileInfo, new byte[fileInfo.Length > Buffer1Length ? Buffer1Length + 2 : fileInfo.Length]);
}