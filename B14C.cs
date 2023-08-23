using System.Buffers.Binary;

namespace LC6464.Base16384;

/// <summary>
/// Base16384 编解码器。
/// </summary>
public static partial class Base16384 {
	/// <summary>
	/// 将主机字节序转换为大端字节序。
	/// </summary>
	/// <param name="value">主机字节序值</param>
	/// <returns>若主机字节序为大端字节序，则直接返回输入的值；否则返回反转后的结果。</returns>
	public static ulong HostToBigEndian(ulong value) =>
		BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value;

	/// <summary>
	/// 将主机字节序转换为大端字节序。
	/// </summary>
	/// <param name="value">主机字节序值</param>
	/// <returns>若主机字节序为大端字节序，则直接返回输入的值；否则返回反转后的结果。</returns>
	public static ushort HostToBigEndian(ushort value) =>
		BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value;

	/// <summary>
	/// 将大端字节序转换为主机字节序。
	/// </summary>
	/// <param name="value">大端字节序值</param>
	/// <returns>若主机字节序为大端字节序，则直接返回输入的值；否则返回反转后的结果。</returns>
	public static ulong BigEndianToHost(ulong value) =>
		BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value;


	/// <summary>
	/// 编码二进制数据到 Base16384 UTF-16 BE 编码数据。
	/// </summary>
	/// <param name="dataPtr">二进制数据指针</param>
	/// <param name="dataLength">二进制数据长度</param>
	/// <param name="bufferPtr">输出缓冲区指针</param>
	/// <param name="bufferLength">输出缓冲区长度</param>
	/// <returns>已写入输出缓冲区的内容的长度</returns>
	public static unsafe int Encode(byte* dataPtr, int dataLength, byte* bufferPtr, int bufferLength) {
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
		var values = (ulong*)bufferPtr;
		ulong n = 0;
		long i = 0;
		for (; i <= dataLength - 7; i += 7) {
			ulong sum = 0;
			var shift = HostToBigEndian(*(ulong*)(dataPtr + i)) >> 2; // 这里有读取越界
			sum |= shift & 0x3fff000000000000;
			shift >>= 2;
			sum |= shift & 0x00003fff00000000;
			shift >>= 2;
			sum |= shift & 0x000000003fff0000;
			shift >>= 2;
			sum |= shift & 0x0000000000003fff;
			sum += 0x4e004e004e004e00;
			values[n++] = BigEndianToHost(sum);
		}
		var o = offset;
		if (Convert.ToBoolean(o--)) {
			var sum = 0x000000000000003f & ((ulong)dataPtr[i] >> 2);
			sum |= ((ulong)dataPtr[i] << 14) & 0x000000000000c000;
			if (Convert.ToBoolean(o--)) {
				sum |= ((ulong)dataPtr[i + 1] << 6) & 0x0000000000003f00;
				sum |= ((ulong)dataPtr[i + 1] << 20) & 0x0000000000300000;
				if (Convert.ToBoolean(o--)) {
					sum |= ((ulong)dataPtr[i + 2] << 12) & 0x00000000000f0000;
					sum |= ((ulong)dataPtr[i + 2] << 28) & 0x00000000f0000000;
					if (Convert.ToBoolean(o--)) {
						sum |= ((ulong)dataPtr[i + 3] << 20) & 0x000000000f000000;
						sum |= ((ulong)dataPtr[i + 3] << 34) & 0x0000003c00000000;
						if (Convert.ToBoolean(o--)) {
							sum |= ((ulong)dataPtr[i + 4] << 26) & 0x0000000300000000;
							sum |= ((ulong)dataPtr[i + 4] << 42) & 0x0000fc0000000000;
							if (Convert.ToBoolean(o--)) {
								sum |= ((ulong)dataPtr[i + 5] << 34) & 0x0000030000000000;
								sum |= ((ulong)dataPtr[i + 5] << 48) & 0x003f000000000000;
							}
						}
					}
				}
			}
			sum += 0x004e004e004e004e;
			values[n] = BitConverter.IsLittleEndian ? sum : BinaryPrimitives.ReverseEndianness(sum);
			bufferPtr[outLength - 2] = (byte)'=';
			bufferPtr[outLength - 1] = (byte)offset;
		}
		return outLength;
	}

	/// <summary>
	/// 解码 Base16384 UTF-16 BE 编码数据到二进制数据。
	/// </summary>
	/// <param name="dataPtr">Base16384 UTF-16 BE 编码数据指针</param>
	/// <param name="dataLength">Base16384 UTF-16 BE 编码数据长度</param>
	/// <param name="bufferPtr">输出缓冲区指针</param>
	/// <param name="bufferLength">输出缓冲区长度</param>
	/// <returns>已写入输出缓冲区的内容的长度</returns>
	public static unsafe int Decode(byte* dataPtr, int dataLength, byte* bufferPtr, int bufferLength) {
		var outLength = dataLength;
		var offset = 0;
		if (dataPtr[dataLength - 2] == '=') {
			offset = dataPtr[dataLength - 1];
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
		}
		outLength = (outLength / 8 * 7) + offset;
		var values = (ulong*)dataPtr;
		ulong n = 0;
		long i = 0;
		for (; i <= outLength - 7; n++, i += 7) {
			ulong sum = 0;
			var shift = HostToBigEndian(values[n]) - 0x4e004e004e004e00;
			shift <<= 2;
			sum |= shift & 0xfffc000000000000;
			shift <<= 2;
			sum |= shift & 0x0003fff000000000;
			shift <<= 2;
			sum |= shift & 0x0000000fffc00000;
			shift <<= 2;
			sum |= shift & 0x00000000003fff00;
			*(ulong*)(bufferPtr + i) = BigEndianToHost(sum);
		}
		if (Convert.ToBoolean(offset--)) {
			// 这里有读取越界
			var sum = (BitConverter.IsLittleEndian ? values[n] : BinaryPrimitives.ReverseEndianness(values[n])) - 0x000000000000004e;
			bufferPtr[i++] = (byte)(((sum & 0x000000000000003f) << 2) | ((sum & 0x000000000000c000) >> 14));
			if (Convert.ToBoolean(offset--)) {
				sum -= 0x00000000004e0000;
				bufferPtr[i++] = (byte)(((sum & 0x0000000000003f00) >> 6) | ((sum & 0x0000000000300000) >> 20));
				if (Convert.ToBoolean(offset--)) {
					bufferPtr[i++] = (byte)(((sum & 0x00000000000f0000) >> 12) | ((sum & 0x00000000f0000000) >> 28));
					if (Convert.ToBoolean(offset--)) {
						sum -= 0x0000004e00000000;
						bufferPtr[i++] = (byte)(((sum & 0x000000000f000000) >> 20) | ((sum & 0x0000003c00000000) >> 34));
						if (Convert.ToBoolean(offset--)) {
							bufferPtr[i++] = (byte)(((sum & 0x0000000300000000) >> 26) | ((sum & 0x0000fc0000000000) >> 42));
							if (Convert.ToBoolean(offset--)) {
								sum -= 0x004e000000000000;
								bufferPtr[i] = (byte)(((sum & 0x0000030000000000) >> 34) | ((sum & 0x003f000000000000) >> 48));
							}
						}
					}
				}
			}
		}
		return outLength;
	}
}