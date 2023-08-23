namespace LC6464.Base16384;

/// <summary>
/// Base16384 编解码器。
/// </summary>
public static partial class Base16384 {
	/// <summary>
	/// UTF-16 BE 编码的 BOM，应在文件头部出现。
	/// </summary>
	public static ReadOnlySpan<byte> Utf16BEPreamble => new byte[] { 0xFE, 0xFF };


	/// <summary>
	/// 编码二进制数据流中的数据到 Base16384 UTF-16 BE 编码文件。
	/// </summary>
	/// <param name="stream">二进制数据流</param>
	/// <param name="fileInfo">创建的 Base16384 UTF-16 BE 编码文件信息</param>
	/// <returns>已写入的数据长度</returns>
	public static long EncodeToNewFile(Stream stream, FileInfo fileInfo) {
		using var file = fileInfo.Create();
		file.Write(Utf16BEPreamble);
		return EncodeToStream(stream, file);
	}

	/// <summary>
	/// 解码 Base16384 UTF-16 BE 编码数据流中的数据到二进制文件。
	/// </summary>
	/// <param name="stream">Base16384 UTF-16 BE 编码数据流</param>
	/// <param name="fileInfo">创建的二进制文件信息</param>
	/// <returns>已写入的数据长度</returns>
	public static long DecodeToNewFile(Stream stream, FileInfo fileInfo) {
		using var file = fileInfo.Create();
		return DecodeToStream(stream, file);
	}


	/// <summary>
	/// 编码二进制数据到 Base16384 UTF-16 BE 编码文件。
	/// </summary>
	/// <param name="data">二进制数据</param>
	/// <param name="fileInfo">创建的 Base16384 UTF-16 BE 编码文件信息</param>
	/// <returns>已写入的数据长度</returns>
	public static long EncodeToNewFile(byte[] data, FileInfo fileInfo) => EncodeToNewFile(new MemoryStream(data), fileInfo);

	/// <summary>
	/// 解码 Base16384 UTF-16 BE 编码数据到二进制文件。
	/// </summary>
	/// <param name="data">Base16384 UTF-16 BE 编码数据</param>
	/// <param name="fileInfo">创建的二进制文件信息</param>
	/// <returns>已写入的数据长度</returns>
	public static long DecodeToNewFile(byte[] data, FileInfo fileInfo) => DecodeToNewFile(new MemoryStream(data), fileInfo);


	/// <summary>
	/// 编码二进制文件中的数据至 Base16384 UTF-16 BE 编码数据，追加到输出数据流。
	/// </summary>
	/// <param name="fileInfo">二进制文件信息</param>
	/// <param name="output">输出数据流</param>
	/// <returns>已写入的数据长度</returns>
	public static long EncodeFromFileToStream(FileInfo fileInfo, Stream output) {
		using var file = fileInfo.OpenRead();
		return EncodeToStream(file, output);
	}

	/// <summary>
	/// 解码 Base16384 UTF-16 BE 编码文件中的数据至二进制数据，追加到输出数据流。
	/// </summary>
	/// <param name="fileInfo">Base16384 UTF-16 BE 编码文件信息</param>
	/// <param name="output">输出数据流</param>
	/// <returns>已写入的数据长度</returns>
	public static long DecodeFromFileToStream(FileInfo fileInfo, Stream output) {
		using var file = fileInfo.OpenRead();
		if (!(file.ReadByte() == 0xFE && file.ReadByte() == 0xFF)) {
			file.Position = 0; // 如果是无 BOM 的非标准文件则回退
		}
		return DecodeToStream(file, output);
	}


	/// <summary>
	/// 编码二进制文件中的数据至 Base16384 UTF-16 BE 编码文件。
	/// </summary>
	/// <param name="fileInfo">二进制文件信息</param>
	/// <param name="outputFileInfo">Base16384 UTF-16 BE 编码文件信息</param>
	/// <returns>已写入的数据长度</returns>
	public static long EncodeFromFileToNewFile(FileInfo fileInfo, FileInfo outputFileInfo) {
		using var output = outputFileInfo.Create();
		output.Write(Utf16BEPreamble);
		return EncodeFromFileToStream(fileInfo, output);
	}

	/// <summary>
	/// 解码 Base16384 UTF-16 BE 编码文件中的数据至二进制文件。
	/// </summary>
	/// <param name="fileInfo">Base16384 UTF-16 BE 编码文件信息</param>
	/// <param name="outputFileInfo">二进制文件信息</param>
	/// <returns>已写入的数据长度</returns>
	public static long DecodeFromFileToNewFile(FileInfo fileInfo, FileInfo outputFileInfo) {
		using var output = outputFileInfo.Create();
		return DecodeFromFileToStream(fileInfo, output);
	}
}