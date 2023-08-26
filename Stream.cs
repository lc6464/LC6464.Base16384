using System.IO;

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

	/// <summary>
	/// 原始数据缓冲区长度。
	/// </summary>
	public const int Buffer0Length = 8192 * 1024 / 7 * 7;

	/// <summary>
	/// 编码后数据缓冲区长度。
	/// </summary>
	public const int Buffer1Length = 8192 * 1024 / 8 * 8;


	/// <summary>
	/// 计算编码指针需要的长度。
	/// </summary>
	/// <param name="dataLength">数据长度</param>
	/// <returns>编码指针需要的长度</returns>
	public static long EncodeLength(long dataLength) {
		var outLength = dataLength / 7 * 8;
		var offset = dataLength % 7;
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
	/// 编码二进制数据流中的数据至 Base16384 UTF-16 BE 编码数据，追加到输出数据流。<br/>
	/// 特别提醒：必须保证外部提供的缓存空间长度足够大，否则将会引发异常。
	/// </summary>
	/// <param name="stream">二进制数据流</param>
	/// <param name="output">输出数据流</param>
	/// <param name="buffer">外部提供的缓存空间（若 <paramref name="stream"/>.Length > <see cref="Buffer1Length"/>，则长度必须大于等于 <see cref="Buffer0Length"/>；否则长度必须大于等于 <paramref name="stream"/>.Length - <paramref name="stream"/>.Position）</param>
	/// <exception cref="ArgumentException">外部提供的缓存空间不足</exception>
	/// <returns>已写入的数据长度</returns>
	public static long EncodeToStream(Stream stream, Stream output, Span<byte> buffer) {
		if (stream.Length > Buffer0Length) {
			if (buffer.Length < Buffer0Length) {
				throw new ArgumentException("外部提供的缓存空间不足，无法完成编码。", nameof(buffer));
			}

			int readCount = stream.Read(buffer),
				writeCount = 0;
			do {
				var encodedData = Encode(buffer, readCount);
				output.Write(encodedData);
				writeCount += encodedData.Length;
			} while ((readCount = stream.Read(buffer)) > 0);
			output.Flush();

			return writeCount;
		}
		{
			if (buffer.Length < stream.Length - stream.Position) {
				throw new ArgumentException("外部提供的缓存空间不足，无法完成编码。", nameof(buffer));
			}

			var encodedData = Encode(buffer, stream.Read(buffer));
			output.Write(encodedData);
			output.Flush();
			return encodedData.Length;
		}
	}

	/// <summary>
	/// 解码 Base16384 UTF-16 BE 编码数据流中的数据至二进制数据，追加到输出数据流。<br/>
	/// 特别提醒：若使用外部提供的缓存空间，必须保证其长度足够大，否则将会引发异常。
	/// </summary>
	/// <param name="stream">Base16384 UTF-16 BE 编码数据流</param>
	/// <param name="output">输出数据流</param>
	/// <param name="buffer">外部提供的缓存空间（若 <paramref name="stream"/>.Length > <see cref="Buffer1Length"/>，则长度必须大于等于 <see cref="Buffer1Length"/>；否则长度必须大于等于 <paramref name="stream"/>.Length - <paramref name="stream"/>.Position）</param>
	/// <exception cref="ArgumentException">外部提供的缓存空间不足</exception>
	/// <returns>已写入的数据长度</returns>
	public static long DecodeToStream(Stream stream, Stream output, Span<byte> buffer) {
		if (stream.Length > Buffer1Length) {
			if (buffer.Length < Buffer1Length) {
				throw new ArgumentException("外部提供的缓存空间不足，无法完成解码。", nameof(buffer));
			}

			byte end; // skipcq: CS-W1022 对 end 赋值的确是不必要的
			int readCount = stream.Read(buffer),
				writeCount = 0;
			do {
				if (Convert.ToBoolean(end = IsNextEnd(stream))) {
					buffer[readCount++] = 61; // (byte)'=' // skipcq: CS-W1082 readCount 值已递增，61 不会被后续语句覆盖
					buffer[readCount++] = end;
				}
				var decodedData = Decode(buffer, readCount);
				output.Write(decodedData);
				writeCount += decodedData.Length;
			} while ((readCount = stream.Read(buffer)) > 0);
			output.Flush();

			return writeCount;
		}
		{
			if (buffer.Length < stream.Length - stream.Position) {
				throw new ArgumentException("外部提供的缓存空间不足，无法完成解码。", nameof(buffer));
			}

			var decodedData = Decode(buffer, stream.Read(buffer));
			output.Write(decodedData);
			output.Flush();
			return decodedData.Length;
		}
	}


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
		DecodeToStream(stream, output, new byte[stream.Length > Buffer1Length ? Buffer1Length : stream.Length - stream.Position]);


	/// <summary>
	/// 编码二进制数据至 Base16384 UTF-16 BE 编码数据，追加到输出数据流。<br/>
	/// 特别提醒：若使用外部提供的缓存空间，必须保证其长度足够大，否则将会引发异常。
	/// </summary>
	/// <param name="data">二进制数据</param>
	/// <param name="output">输出数据流</param>
	/// <param name="buffer">外部提供的缓存空间（若 <paramref name="data"/>.Length > <see cref="Buffer0Length"/>，则长度必须大于等于 <see cref="Buffer0Length"/>）</param>
	/// <returns>已写入的数据长度</returns>
	public static long EncodeToStream(ReadOnlySpan<byte> data, Stream output, Span<byte> buffer) {
		if (data.Length > Buffer0Length) {
			if (buffer.Length < Buffer0Length) {
				throw new ArgumentException("外部提供的缓存空间不足，无法完成编码。", nameof(buffer));
			}

			int remainingCount = data.Length,
				encodedCount = 0,
				writeCount = 0;
			do {
				var readCount = Math.Min(Buffer0Length, remainingCount);
				remainingCount -= readCount;
				data.Slice(encodedCount, readCount).CopyTo(buffer);
				encodedCount += readCount;
				var encodedData = Encode(buffer, readCount);
				output.Write(encodedData);
				writeCount += encodedData.Length;
			} while (remainingCount > 0);
			output.Flush();

			return writeCount;
		}
		{
			var encodedData = Encode(data);
			output.Write(encodedData);
			output.Flush();
			return encodedData.Length;
		}
	}

	/// <summary>
	/// 解码 Base16384 UTF-16 BE 编码数据至二进制数据，追加到输出数据流。<br/>
	/// 特别提醒：若使用外部提供的缓存空间，必须保证其长度足够大，否则将会引发异常。
	/// </summary>
	/// <param name="data">Base16384 UTF-16 BE 编码数据</param>
	/// <param name="output">输出数据流</param>
	/// <param name="buffer">外部提供的缓存空间（若 <paramref name="data"/>.Length > <see cref="Buffer1Length"/>，则长度必须大于等于 <see cref="Buffer1Length"/> + 2）</param>
	/// <returns>已写入的数据长度</returns>
	public static long DecodeToStream(ReadOnlySpan<byte> data, Stream output, Span<byte> buffer) {
		if (data.Length > Buffer1Length) {
			if (buffer.Length < Buffer1Length + 2) {
				throw new ArgumentException("外部提供的缓存空间不足，无法完成解码。", nameof(buffer));
			}

			int remainingCount = data.Length,
				decodedCount = 0,
				writeCount = 0;
			do {
				var readCount = Math.Min(Buffer1Length, remainingCount);
				remainingCount -= readCount;
				data.Slice(decodedCount, readCount).CopyTo(buffer);
				decodedCount += readCount;
				if (remainingCount == 2) { // 剩下两字节，可能是 '=' + (1~6)，一起处理了
					buffer[readCount++] = data[^2];
					buffer[readCount++] = data[^1];
					remainingCount = 0;
				} else if (remainingCount == 1) { // 剩下一字节，一起处理了
					buffer[readCount++] = data[^1];
					remainingCount = 0;
				}
				var decodedData = Decode(buffer, readCount);
				output.Write(decodedData);
				writeCount += decodedData.Length;
			} while (remainingCount > 0);
			output.Flush();

			return writeCount;
		}
		{
			var decodedData = Decode(data);
			output.Write(decodedData);
			output.Flush();
			return decodedData.Length;
		}
	}


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
	public static MemoryStream EncodeToNewMemoryStream(byte[] data) => EncodeToNewMemoryStream(new MemoryStream(data));

	/// <summary>
	/// 解码 Base16384 UTF-16 BE 编码数据到新的二进制数据流。
	/// </summary>
	/// <param name="data">Base16384 UTF-16 BE 编码数据</param>
	/// <returns>二进制数据流</returns>
	public static MemoryStream DecodeToNewMemorySteam(byte[] data) => DecodeToNewMemorySteam(new MemoryStream(data));
}