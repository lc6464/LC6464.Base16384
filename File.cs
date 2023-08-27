namespace LC6464.Base16384;

/// <summary>
/// Base16384 编解码器。
/// </summary>
public static partial class Base16384 {
	/// <summary>
	/// 编码二进制数据流中的数据到 Base16384 UTF-16 BE 编码文件。<br/>
	/// 特别提醒：必须保证外部提供的缓存空间长度足够大，否则将会引发异常。
	/// </summary>
	/// <param name="stream">二进制数据流</param>
	/// <param name="fileInfo">创建的 Base16384 UTF-16 BE 编码文件信息</param>
	/// <param name="buffer">外部提供的缓存空间（若 <paramref name="stream"/>.Length > <see cref="Buffer0Length"/>，则长度必须大于等于 <see cref="Buffer0Length"/>；否则长度必须大于等于 <paramref name="stream"/>.Length - <paramref name="stream"/>.Position）</param>
	/// <exception cref="ArgumentException">外部提供的缓存空间不足</exception>
	/// <returns>已写入的数据长度</returns>
	public static long EncodeToNewFile(Stream stream, FileInfo fileInfo, Span<byte> buffer) {
		using var file = fileInfo.Create();
		file.Write(Utf16BEPreamble);
		return EncodeToStream(stream, file, buffer);
	}

	/// <summary>
	/// 解码 Base16384 UTF-16 BE 编码数据流中的数据到二进制文件。<br/>
	/// 特别提醒：必须保证外部提供的缓存空间长度足够大，否则将会引发异常。
	/// </summary>
	/// <param name="stream">Base16384 UTF-16 BE 编码数据流</param>
	/// <param name="fileInfo">创建的二进制文件信息</param><param name="buffer">外部提供的缓存空间（若 <paramref name="stream"/>.Length > <see cref="Buffer1Length"/>，则长度必须大于等于 <see cref="Buffer1Length"/> + 2；否则长度必须大于等于 <paramref name="stream"/>.Length - <paramref name="stream"/>.Position）</param>
	/// <exception cref="ArgumentException">外部提供的缓存空间不足</exception>
	/// <returns>已写入的数据长度</returns>
	public static long DecodeToNewFile(Stream stream, FileInfo fileInfo, Span<byte> buffer) {
		using var file = fileInfo.Create();
		return DecodeToStream(stream, file, buffer);
	}


	/// <summary>
	/// 编码二进制数据到 Base16384 UTF-16 BE 编码文件。<br/>
	/// 特别提醒：必须保证外部提供的缓存空间长度足够大，否则将会引发异常。
	/// </summary>
	/// <param name="data">二进制数据</param>
	/// <param name="fileInfo">创建的 Base16384 UTF-16 BE 编码文件信息</param>
	/// <param name="buffer">外部提供的缓存空间（若 <paramref name="data"/>.Length > <see cref="Buffer0Length"/>，则长度必须大于等于 <see cref="Buffer0Length"/>）</param>
	/// <exception cref="ArgumentException">外部提供的缓存空间不足</exception>
	/// <returns>已写入的数据长度</returns>
	public static long EncodeToNewFile(ReadOnlySpan<byte> data, FileInfo fileInfo, Span<byte> buffer) {
		using var file = fileInfo.Create();
		file.Write(Utf16BEPreamble);
		return EncodeToStream(data, file, buffer);
	}

	/// <summary>
	/// 解码 Base16384 UTF-16 BE 编码数据到二进制文件。<br/>
	/// 特别提醒：必须保证外部提供的缓存空间长度足够大，否则将会引发异常。
	/// </summary>
	/// <param name="data">Base16384 UTF-16 BE 编码数据</param>
	/// <param name="fileInfo">创建的二进制文件信息</param>
	/// <param name="buffer">外部提供的缓存空间（若 <paramref name="data"/>.Length > <see cref="Buffer1Length"/>，则长度必须大于等于 <see cref="Buffer1Length"/> + 2）</param>
	/// <exception cref="ArgumentException">外部提供的缓存空间不足</exception>
	/// <returns>已写入的数据长度</returns>
	public static long DecodeToNewFile(ReadOnlySpan<byte> data, FileInfo fileInfo, Span<byte> buffer) {
		using var file = fileInfo.Create();
		return DecodeToStream(data, file, buffer);
	}


	/// <summary>
	/// 编码二进制文件中的数据至 Base16384 UTF-16 BE 编码数据，追加到输出数据流。<br/>
	/// 特别提醒：必须保证外部提供的缓存空间长度足够大，否则将会引发异常。
	/// </summary>
	/// <param name="fileInfo">二进制文件信息</param>
	/// <param name="output">输出数据流</param>
	/// <param name="buffer">外部提供的缓存空间（若 <paramref name="fileInfo"/>.Length > <see cref="Buffer0Length"/>，则长度必须大于等于 <see cref="Buffer0Length"/>；否则长度必须大于等于 <paramref name="fileInfo"/>.Length）</param>
	/// <exception cref="ArgumentException">外部提供的缓存空间不足</exception>
	/// <returns>已写入的数据长度</returns>
	public static long EncodeFromFileToStream(FileInfo fileInfo, Stream output, Span<byte> buffer) {
		using var file = fileInfo.OpenRead();
		return EncodeToStream(file, output, buffer);
	}

	/// <summary>
	/// 解码 Base16384 UTF-16 BE 编码文件中的数据至二进制数据，追加到输出数据流。<br/>
	/// 特别提醒：必须保证外部提供的缓存空间长度足够大，否则将会引发异常。
	/// </summary>
	/// <param name="fileInfo">Base16384 UTF-16 BE 编码文件信息</param>
	/// <param name="output">输出数据流</param>
	/// <param name="buffer">外部提供的缓存空间（若 <paramref name="fileInfo"/>.Length > <see cref="Buffer1Length"/>，则长度必须大于等于 <see cref="Buffer1Length"/> + 2；否则长度必须大于等于 <paramref name="fileInfo"/>.Length）</param>
	/// <exception cref="ArgumentException">外部提供的缓存空间不足</exception>
	/// <returns>已写入的数据长度</returns>
	public static long DecodeFromFileToStream(FileInfo fileInfo, Stream output, Span<byte> buffer) {
		using var file = fileInfo.OpenRead();
		if (((file.ReadByte() << 8) | file.ReadByte()) != 0xFEFF) {
			file.Position = 0; // 如果是无 BOM 的非标准文件则回退
		}
		return DecodeToStream(file, output, buffer);
	}


	/// <summary>
	/// 编码二进制文件中的数据至 Base16384 UTF-16 BE 编码文件。<br/>
	/// 特别提醒：必须保证外部提供的缓存空间长度足够大，否则将会引发异常。
	/// </summary>
	/// <param name="fileInfo">二进制文件信息</param>
	/// <param name="outputFileInfo">Base16384 UTF-16 BE 编码文件信息</param>
	/// <param name="buffer">外部提供的缓存空间（若 <paramref name="fileInfo"/>.Length > <see cref="Buffer0Length"/>，则长度必须大于等于 <see cref="Buffer0Length"/>；否则长度必须大于等于 <paramref name="fileInfo"/>.Length）</param>
	/// <exception cref="ArgumentException">外部提供的缓存空间不足</exception>
	/// <returns>已写入的数据长度</returns>
	public static long EncodeFromFileToNewFile(FileInfo fileInfo, FileInfo outputFileInfo, Span<byte> buffer) {
		using var output = outputFileInfo.Create();
		output.Write(Utf16BEPreamble);
		return EncodeFromFileToStream(fileInfo, output, buffer);
	}

	/// <summary>
	/// 解码 Base16384 UTF-16 BE 编码文件中的数据至二进制文件。<br/>
	/// 特别提醒：必须保证外部提供的缓存空间长度足够大，否则将会引发异常。
	/// </summary>
	/// <param name="fileInfo">Base16384 UTF-16 BE 编码文件信息</param>
	/// <param name="outputFileInfo">二进制文件信息</param>
	/// <param name="buffer">外部提供的缓存空间（若 <paramref name="fileInfo"/>.Length > <see cref="Buffer1Length"/>，则长度必须大于等于 <see cref="Buffer1Length"/> + 2；否则长度必须大于等于 <paramref name="fileInfo"/>.Length）</param>
	/// <exception cref="ArgumentException">外部提供的缓存空间不足</exception>
	/// <returns>已写入的数据长度</returns>
	public static long DecodeFromFileToNewFile(FileInfo fileInfo, FileInfo outputFileInfo, Span<byte> buffer) {
		using var output = outputFileInfo.Create();
		return DecodeFromFileToStream(fileInfo, output, buffer);
	}
}