namespace LC6464.Base16384;

/// <summary>
/// Base16384 编解码器。
/// </summary>
public static partial class Base16384 {
	/// <summary>
	/// UTF-16 BE 编码的 BOM，应在文件头部出现。
	/// </summary>
	public static ReadOnlySpan<byte> Utf16BEPreamble => new byte[] { 0xfe, 0xff };


	/// <summary>
	/// 编码二进制数据到 Base16384 UTF-16 BE 编码文件。
	/// </summary>
	/// <param name="stream">二进制数据流</param>
	/// <param name="filePath">创建的 Base16384 UTF-16 BE 编码文件路径</param>
	/// <returns>已写入的数据长度</returns>
	public static long EncodeToNewFile(Stream stream, string filePath) {
		using var file = File.Create(filePath);
		file.Write(Utf16BEPreamble);
		return EncodeToStream(stream, file);
	}

	/// <summary>
	/// 解码 Base16384 UTF-16 BE 编码数据到二进制文件。
	/// </summary>
	/// <param name="stream">Base16384 UTF-16 BE 编码数据流</param>
	/// <param name="filePath">创建的二进制文件路径</param>
	/// <returns>已写入的数据长度</returns>
	public static long DecodeToNewFile(Stream stream, string filePath) {
		using var file = File.Create(filePath);
		return DecodeToStream(stream, file);
	}


	/// <summary>
	/// 编码二进制文件中的数据至 Base16384 UTF-16 BE 编码数据，追加到输出数据流。
	/// </summary>
	/// <param name="filePath">二进制文件路径</param>
	/// <param name="output">输出数据流</param>
	/// <returns>已写入的数据长度</returns>
	public static long EncodeFromFileToStream(string filePath, Stream output) {
		using var file = File.OpenRead(filePath);
		return EncodeToStream(file, output);
	}

	/// <summary>
	/// 解码 Base16384 UTF-16 BE 编码文件中的数据至二进制数据，追加到输出数据流。
	/// </summary>
	/// <param name="filePath">Base16384 UTF-16 BE 编码文件路径</param>
	/// <param name="output">输出数据流</param>
	/// <returns>已写入的数据长度</returns>
	public static long DecodeFromFileToStream(string filePath, Stream output) {
		using var file = File.OpenRead(filePath);
		file.Seek(Utf16BEPreamble.Length, SeekOrigin.Begin);
		return DecodeToStream(file, output);
	}


	/// <summary>
	/// 编码二进制文件中的数据至 Base16384 UTF-16 BE 编码文件。
	/// </summary>
	/// <param name="filePath">二进制文件路径</param>
	/// <param name="outputFilePath">Base16384 UTF-16 BE 编码文件路径</param>
	/// <returns>已写入的数据长度</returns>
	public static long EncodeFromFileToFile(string filePath, string outputFilePath) {
		using var file = File.OpenRead(filePath);
		using var output = File.Create(outputFilePath);
		output.Write(Utf16BEPreamble);
		return EncodeToStream(file, output);
	}

	/// <summary>
	/// 解码 Base16384 UTF-16 BE 编码文件中的数据至二进制文件。
	/// </summary>
	/// <param name="filePath">Base16384 UTF-16 BE 编码文件路径</param>
	/// <param name="outputFilePath">二进制文件路径</param>
	/// <returns>已写入的数据长度</returns>
	public static long DecodeFromFileToFile(string filePath, string outputFilePath) {
		using var file = File.OpenRead(filePath);
		using var output = File.Create(outputFilePath);
		file.Seek(Utf16BEPreamble.Length, SeekOrigin.Begin);
		return DecodeToStream(file, output);
	}
}