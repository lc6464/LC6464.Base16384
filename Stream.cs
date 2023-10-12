namespace LC6464.Base16384;

/// <summary>
/// Base16384 编解码器。
/// </summary>
public static partial class Base16384 {
	/// <summary>
	/// 强制使用长流模式（分段编码）编码二进制数据流中的数据至 Base16384 UTF-16 BE 编码数据，追加到输出数据流。<br/>
	/// 特别提醒：必须保证外部提供的缓存空间长度足够大，否则将会引发异常。
	/// </summary>
	/// <param name="stream">二进制数据流</param>
	/// <param name="output">输出数据流</param>
	/// <param name="buffer">外部提供的缓存空间（长度必须大于等于 <see cref="Buffer0Length"/>）</param>
	/// <param name="encodingBuffer">外部提供的用于编码的缓存空间（长度必须大于等于 <see cref="EncodeLength"/>(<see cref="Buffer0Length"/>)）</param>
	/// <exception cref="ArgumentException">外部提供的缓存空间不足</exception>
	/// <returns>已写入的数据长度</returns>
	public static unsafe long EncodeFromLongStreamToStream(Stream stream, Stream output, Span<byte> buffer, Span<byte> encodingBuffer) {
		if (buffer.Length < Buffer0Length) {
			throw new ArgumentException("外部提供的缓存空间不足，无法完成编码。", nameof(buffer));
		}
		var encodeLength = (int)EncodeLength(Buffer0Length);
		if (encodingBuffer.Length < encodeLength) {
			throw new ArgumentException("外部提供的缓存空间不足，无法完成编码。", nameof(encodingBuffer));
		}

		var readingBuffer = buffer[..Buffer0Length]; // 防止一次读入过多数据，此处使用切片
		int readCount = stream.Read(readingBuffer),
			writeCount = 0;
		do {
			var encodedLength = Encode(buffer[..readCount], encodingBuffer); // 编码结果写入缓冲区
			output.Write(encodingBuffer[..encodedLength]); // 将缓冲区中指定长度的数据写入输出流
			writeCount += encodedLength;
		} while ((readCount = stream.Read(readingBuffer)) > 0);
		output.Flush();

		return writeCount;
	}

	/// <summary>
	/// 编码二进制数据流中的数据至 Base16384 UTF-16 BE 编码数据，追加到输出数据流。<br/>
	/// 特别提醒：必须保证外部提供的缓存空间长度足够大，否则将会引发异常。
	/// </summary>
	/// <param name="stream">二进制数据流</param>
	/// <param name="output">输出数据流</param>
	/// <param name="buffer">外部提供的缓存空间（若 <paramref name="stream"/>.Length > <see cref="Buffer0Length"/>，则长度必须大于等于 <see cref="Buffer0Length"/>；否则长度必须大于等于 <paramref name="stream"/>.Length - <paramref name="stream"/>.Position）</param>
	/// <param name="encodingBuffer">外部提供的用于编码的缓存空间（若 <paramref name="stream"/>.Length > <see cref="Buffer0Length"/>，则长度必须大于等于 <see cref="EncodeLength"/>(<see cref="Buffer0Length"/>)；否则长度必须大于等于 <see cref="EncodeLength"/>(<paramref name="stream"/>.Length - <paramref name="stream"/>.Position)）</param>
	/// <exception cref="ArgumentException">外部提供的缓存空间不足</exception>
	/// <returns>已写入的数据长度</returns>
	public static unsafe long EncodeToStream(Stream stream, Stream output, Span<byte> buffer, Span<byte> encodingBuffer) {
		if (!stream.CanSeek || stream.Length > Buffer0Length) {
			return EncodeFromLongStreamToStream(stream, output, buffer, encodingBuffer);
		}

		var remainingLength = stream.Length - stream.Position;
		if (buffer.Length < remainingLength) {
			throw new ArgumentException("外部提供的缓存空间不足，无法完成编码。", nameof(buffer));
		}
		var encodeLength = (int)EncodeLength(remainingLength);
		if (encodingBuffer.Length < encodeLength) {
			throw new ArgumentException("外部提供的缓存空间不足，无法完成编码。", nameof(encodingBuffer));
		}

		var encodedLength = Encode(buffer[..stream.Read(buffer)], encodingBuffer);
		output.Write(encodingBuffer[..encodedLength]); // 将缓冲区中指定长度的数据写入输出流
		output.Flush();
		return encodedLength;
	}

	/// <summary>
	/// 强制使用长流模式（分段编码）解码 Base16384 UTF-16 BE 编码数据流中的数据至二进制数据，追加到输出数据流。<br/>
	/// 特别提醒：若使用外部提供的缓存空间，必须保证其长度足够大，否则将会引发异常。
	/// </summary>
	/// <param name="stream">Base16384 UTF-16 BE 编码数据流</param>
	/// <param name="output">输出数据流</param>
	/// <param name="buffer">外部提供的缓存空间（长度必须大于等于 <see cref="Buffer1Length"/> + 2）</param>
	/// <param name="decodingBuffer">外部提供的用于编码的缓存空间（长度必须大于等于 <see cref="DecodeLength"/>(<see cref="Buffer1Length"/>)）</param>
	/// <exception cref="ArgumentException">外部提供的缓存空间不足</exception>
	/// <returns>已写入的数据长度</returns>
	public static unsafe long DecodeFromLongStreamToStream(Stream stream, Stream output, Span<byte> buffer, Span<byte> decodingBuffer) {
		if (buffer.Length < Buffer1Length + 2) {
			throw new ArgumentException("外部提供的缓存空间不足，无法完成解码。", nameof(buffer));
		}
		var decodeLength = (int)DecodeLength(Buffer1Length);
		if (decodingBuffer.Length < decodeLength) {
			throw new ArgumentException("外部提供的缓存空间不足，无法完成解码。", nameof(decodingBuffer));
		}

		var readingBuffer = buffer[..Buffer1Length]; // 防止一次读入过多数据，此处使用切片
		byte end; // skipcq: CS-W1022 对 end 赋值的确是不必要的
		int readCount = stream.Read(readingBuffer),
			writeCount = 0;
		do {
			if (Convert.ToBoolean(end = IsNextEnd(stream))) {
				buffer[readCount++] = 61; // (byte)'=' // skipcq: CS-W1082 readCount 值已递增，61 不会被后续语句覆盖
				buffer[readCount++] = end;
			}
			var decodedLength = Decode(buffer[..readCount], decodingBuffer); // 解码结果写入缓冲区
			output.Write(decodingBuffer[..decodedLength]); // 将缓冲区中指定长度的数据写入输出流
			writeCount += decodedLength;
		} while ((readCount = stream.Read(readingBuffer)) > 0);
		output.Flush();

		return writeCount;
	}

	/// <summary>
	/// 解码 Base16384 UTF-16 BE 编码数据流中的数据至二进制数据，追加到输出数据流。<br/>
	/// 特别提醒：若使用外部提供的缓存空间，必须保证其长度足够大，否则将会引发异常。
	/// </summary>
	/// <param name="stream">Base16384 UTF-16 BE 编码数据流</param>
	/// <param name="output">输出数据流</param>
	/// <param name="buffer">外部提供的缓存空间（若 <paramref name="stream"/>.Length > <see cref="Buffer1Length"/>，则长度必须大于等于 <see cref="Buffer1Length"/> + 2；否则长度必须大于等于 <paramref name="stream"/>.Length - <paramref name="stream"/>.Position）</param>
	/// <param name="decodingBuffer">外部提供的用于编码的缓存空间（若 <paramref name="stream"/>.Length > <see cref="Buffer1Length"/>，则长度必须大于等于 <see cref="DecodeLength"/>(<see cref="Buffer1Length"/>)；否则长度必须大于等于 <see cref="DecodeLength"/>(<paramref name="stream"/>.Length - <paramref name="stream"/>.Position)）</param>
	/// <exception cref="ArgumentException">外部提供的缓存空间不足</exception>
	/// <returns>已写入的数据长度</returns>
	public static unsafe long DecodeToStream(Stream stream, Stream output, Span<byte> buffer, Span<byte> decodingBuffer) {
		if (!stream.CanSeek || stream.Length > Buffer1Length) {
			return DecodeFromLongStreamToStream(stream, output, buffer, decodingBuffer);
		}

		var remainingLength = stream.Length - stream.Position;
		if (buffer.Length < remainingLength) {
			throw new ArgumentException("外部提供的缓存空间不足，无法完成解码。", nameof(buffer));
		}
		var decodeLength = (int)DecodeLength(remainingLength);
		if (decodingBuffer.Length < decodeLength) {
			throw new ArgumentException("外部提供的缓存空间不足，无法完成解码。", nameof(decodingBuffer));
		}

		var decodedLength = Decode(buffer[..stream.Read(buffer)], decodingBuffer);
		output.Write(decodingBuffer[..decodedLength]);
		output.Flush();
		return decodedLength;
	}


	/// <summary>
	/// 编码二进制数据至 Base16384 UTF-16 BE 编码数据，追加到输出数据流。<br/>
	/// 特别提醒：若使用外部提供的缓存空间，必须保证其长度足够大，否则将会引发异常。
	/// </summary>
	/// <param name="data">二进制数据</param>
	/// <param name="output">输出数据流</param>
	/// <param name="encodingBuffer">外部提供的用于编码的缓存空间（若 <paramref name="data"/>.Length > <see cref="Buffer0Length"/>，则长度必须大于等于 <see cref="EncodeLength"/>(<see cref="Buffer0Length"/>)；否则长度必须大于等于 <see cref="EncodeLength"/>(<paramref name="data"/>.Length)）</param>
	/// <exception cref="ArgumentException">外部提供的缓存空间不足</exception>
	/// <returns>已写入的数据长度</returns>
	public static long EncodeToStream(ReadOnlySpan<byte> data, Stream output, Span<byte> encodingBuffer) {
		if (data.Length > Buffer0Length) {
			var encodeLength = (int)EncodeLength(Buffer0Length);
			if (encodingBuffer.Length < encodeLength) {
				throw new ArgumentException("外部提供的缓存空间不足，无法完成编码。", nameof(encodingBuffer));
			}

			int remainingCount = data.Length,
				encodedCount = 0,
				writeCount = 0;
			do {
				var readCount = Math.Min(Buffer0Length, remainingCount); // 读取数据长度
				remainingCount -= readCount;
				var encodedLength = Encode(data.Slice(encodedCount, readCount), encodingBuffer); // 编码结果写入缓冲区
				encodedCount += readCount;
				output.Write(encodingBuffer[..encodedLength]); // 将缓冲区中指定长度的数据写入输出流
				writeCount += encodedLength;
			} while (remainingCount > 0);
			output.Flush();

			return writeCount;
		}
		{
			var encodeLength = (int)EncodeLength(data.Length);
			if (encodingBuffer.Length < encodeLength) {
				throw new ArgumentException("外部提供的缓存空间不足，无法完成编码。", nameof(encodingBuffer));
			}

			var encodedLength = Encode(data, encodingBuffer);
			output.Write(encodingBuffer[..encodedLength]);
			output.Flush();
			return encodedLength;
		}
	}

	/// <summary>
	/// 解码 Base16384 UTF-16 BE 编码数据至二进制数据，追加到输出数据流。<br/>
	/// 特别提醒：若使用外部提供的缓存空间，必须保证其长度足够大，否则将会引发异常。
	/// </summary>
	/// <param name="data">Base16384 UTF-16 BE 编码数据</param>
	/// <param name="output">输出数据流</param>
	/// <param name="decodingBuffer">外部提供的用于编码的缓存空间（若 <paramref name="data"/>.Length > <see cref="Buffer1Length"/>，则长度必须大于等于 <see cref="DecodeLength"/>(<see cref="Buffer1Length"/>)；否则长度必须大于等于 <see cref="DecodeLength"/>(<paramref name="data"/>.Length)）</param>
	/// <exception cref="ArgumentException">外部提供的缓存空间不足</exception>
	/// <returns>已写入的数据长度</returns>
	public static unsafe long DecodeToStream(ReadOnlySpan<byte> data, Stream output, Span<byte> decodingBuffer) {
		if (data.Length > Buffer1Length) {
			var decodeLength = (int)DecodeLength(Buffer1Length);
			if (decodingBuffer.Length < decodeLength) {
				throw new ArgumentException("外部提供的缓存空间不足，无法完成解码。", nameof(decodingBuffer));
			}

			var memory = Marshal.AllocHGlobal(Buffer1Length + 2); // 临时储存待解码数据的非托管内存
			var buffer = new Span<byte>((byte*)memory, Buffer1Length + 2); // 指向非托管内存的 Span

			int remainingCount = data.Length,
				decodedCount = 0,
				writeCount = 0,
				readCount = Math.Min(Buffer1Length, remainingCount);
			do {
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
				var decodedLength = Decode(buffer[..readCount], decodingBuffer);
				output.Write(decodingBuffer[..decodedLength]);
				writeCount += decodedLength;

				readCount = Math.Min(Buffer1Length, remainingCount);
			} while (remainingCount > 0);
			output.Flush();

			Marshal.FreeHGlobal(memory); // 释放非托管内存

			return writeCount;
		}
		{
			var decodeLength = (int)DecodeLength(data.Length);
			if (decodingBuffer.Length < decodeLength) {
				throw new ArgumentException("外部提供的缓存空间不足，无法完成编码。", nameof(decodingBuffer));
			}

			var decodedData = Decode(data);
			output.Write(decodedData);
			output.Flush();
			return decodedData.Length;
		}
	}


	/// <summary>
	/// 强制使用长流模式（分段编码）编码二进制数据流中的数据到新的 Base16384 UTF-16 BE 编码数据流。<br/>
	/// 特别提醒：必须保证外部提供的缓存空间长度足够大，否则将会引发异常。
	/// </summary>
	/// <param name="stream">二进制数据流</param>
	/// <param name="buffer">外部提供的缓存空间（长度必须大于等于 <see cref="Buffer0Length"/>）</param>
	/// <param name="encodingBuffer">外部提供的用于编码的缓存空间（长度必须大于等于 <see cref="EncodeLength"/>(<see cref="Buffer0Length"/>)）</param>
	/// <exception cref="ArgumentException">外部提供的缓存空间不足</exception>
	/// <returns>Base16384 UTF-16 BE 编码数据流</returns>
	public static MemoryStream EncodeFromLongStreamToNewMemoryStream(Stream stream, Span<byte> buffer, Span<byte> encodingBuffer) {
		var output = new MemoryStream();
		_ = EncodeFromLongStreamToStream(stream, output, buffer, encodingBuffer);
		return output;
	}

	/// <summary>
	/// 编码二进制数据流中的数据到新的 Base16384 UTF-16 BE 编码数据流。<br/>
	/// 特别提醒：必须保证外部提供的缓存空间长度足够大，否则将会引发异常。
	/// </summary>
	/// <param name="stream">二进制数据流</param>
	/// <param name="buffer">外部提供的缓存空间（若 <paramref name="stream"/>.Length > <see cref="Buffer0Length"/>，则长度必须大于等于 <see cref="Buffer0Length"/>；否则长度必须大于等于 <paramref name="stream"/>.Length - <paramref name="stream"/>.Position）</param>
	/// <param name="encodingBuffer">外部提供的用于编码的缓存空间（若 <paramref name="stream"/>.Length > <see cref="Buffer0Length"/>，则长度必须大于等于 <see cref="EncodeLength"/>(<see cref="Buffer0Length"/>)；否则长度必须大于等于 <see cref="EncodeLength"/>(<paramref name="stream"/>.Length - <paramref name="stream"/>.Position)）</param>
	/// <exception cref="ArgumentException">外部提供的缓存空间不足</exception>
	/// <returns>Base16384 UTF-16 BE 编码数据流</returns>
	public static MemoryStream EncodeToNewMemoryStream(Stream stream, Span<byte> buffer, Span<byte> encodingBuffer) {
		var output = new MemoryStream();
		_ = EncodeToStream(stream, output, buffer, encodingBuffer);
		return output;
	}

	/// <summary>
	/// 强制使用长流模式（分段编码）解码 Base16384 UTF-16 BE 编码数据流中的数据到新的二进制数据流。<br/>
	/// 特别提醒：若使用外部提供的缓存空间，必须保证其长度足够大，否则将会引发异常。
	/// </summary>
	/// <param name="stream">Base16384 UTF-16 BE 编码数据流</param>
	/// <param name="buffer">外部提供的缓存空间（长度必须大于等于 <see cref="Buffer1Length"/> + 2）</param>
	/// <param name="decodingBuffer">外部提供的用于编码的缓存空间（长度必须大于等于 <see cref="DecodeLength"/>(<see cref="Buffer1Length"/>)）</param>
	/// <exception cref="ArgumentException">外部提供的缓存空间不足</exception>
	/// <returns>二进制数据流</returns>
	public static MemoryStream DecodeFromLongStreamToNewMemorySteam(Stream stream, Span<byte> buffer, Span<byte> decodingBuffer) {
		var output = new MemoryStream();
		_ = DecodeFromLongStreamToStream(stream, output, buffer, decodingBuffer);
		return output;
	}

	/// <summary>
	/// 解码 Base16384 UTF-16 BE 编码数据流中的数据到新的二进制数据流。<br/>
	/// 特别提醒：若使用外部提供的缓存空间，必须保证其长度足够大，否则将会引发异常。
	/// </summary>
	/// <param name="stream">Base16384 UTF-16 BE 编码数据流</param>
	/// <param name="buffer">外部提供的缓存空间（若 <paramref name="stream"/>.Length > <see cref="Buffer1Length"/>，则长度必须大于等于 <see cref="Buffer1Length"/> + 2；否则长度必须大于等于 <paramref name="stream"/>.Length - <paramref name="stream"/>.Position）</param>
	/// <param name="decodingBuffer">外部提供的用于编码的缓存空间（若 <paramref name="stream"/>.Length > <see cref="Buffer1Length"/>，则长度必须大于等于 <see cref="DecodeLength"/>(<see cref="Buffer1Length"/>)；否则长度必须大于等于 <see cref="DecodeLength"/>(<paramref name="stream"/>.Length - <paramref name="stream"/>.Position)）</param>
	/// <exception cref="ArgumentException">外部提供的缓存空间不足</exception>
	/// <returns>二进制数据流</returns>
	public static MemoryStream DecodeToNewMemorySteam(Stream stream, Span<byte> buffer, Span<byte> decodingBuffer) {
		var output = new MemoryStream();
		_ = DecodeToStream(stream, output, buffer, decodingBuffer);
		return output;
	}


	/// <summary>
	/// 编码二进制数据到新的 Base16384 UTF-16 BE 编码数据流。<br/>
	/// 特别提醒：必须保证外部提供的缓存空间长度足够大，否则将会引发异常。
	/// </summary>
	/// <param name="data">二进制数据</param>
	/// <param name="encodingBuffer">外部提供的用于编码的缓存空间（若 <paramref name="data"/>.Length > <see cref="Buffer0Length"/>，则长度必须大于等于 <see cref="EncodeLength"/>(<see cref="Buffer0Length"/>)；否则长度必须大于等于 <see cref="EncodeLength"/>(<paramref name="data"/>.Length)）</param>
	/// <exception cref="ArgumentException">外部提供的缓存空间不足</exception>
	/// <returns>Base16384 UTF-16 BE 编码数据流</returns>
	public static MemoryStream EncodeToNewMemoryStream(ReadOnlySpan<byte> data, Span<byte> encodingBuffer) {
		var output = new MemoryStream();
		_ = EncodeToStream(data, output, encodingBuffer);
		return output;
	}

	/// <summary>
	/// 解码 Base16384 UTF-16 BE 编码数据到新的二进制数据流。<br/>
	/// 特别提醒：必须保证外部提供的缓存空间长度足够大，否则将会引发异常。
	/// </summary>
	/// <param name="data">Base16384 UTF-16 BE 编码数据</param>
	/// <param name="decodingBuffer">外部提供的用于编码的缓存空间（若 <paramref name="data"/>.Length > <see cref="Buffer1Length"/>，则长度必须大于等于 <see cref="DecodeLength"/>(<see cref="Buffer1Length"/>)；否则长度必须大于等于 <see cref="DecodeLength"/>(<paramref name="data"/>.Length)）</param>
	/// <exception cref="ArgumentException">外部提供的缓存空间不足</exception>
	/// <returns>二进制数据流</returns>
	public static MemoryStream DecodeToNewMemorySteam(ReadOnlySpan<byte> data, Span<byte> decodingBuffer) {
		var output = new MemoryStream();
		_ = DecodeToStream(data, output, decodingBuffer);
		return output;
	}
}