namespace LC6464.Base16384;

/// <summary>
/// Base16384 编解码器。
/// </summary>
public static partial class Base16384 {
	/// <summary>
	/// 计算编码指针需要的长度。
	/// </summary>
	/// <param name="dataLength">数据长度</param>
	/// <returns>编码指针需要的长度</returns>
	public static int EncodeLength(int dataLength) {
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
		}
		return outLength + 8 + 16;  // 冗余的8B用于可能的结尾的覆盖，再加上16B备用
	}

	/// <summary>
	/// 计算解码指针需要的长度。
	/// </summary>
	/// <param name="dataLength">数据长度</param>
	/// <param name="offset">偏移量（默认为0，作用未知，可参见 GitHub: fumiama/base16384。）</param>
	/// <returns>解码指针需要的长度</returns>
	public static int DecodeLength(int dataLength, int offset = 0) {
		var outLength = dataLength;
		switch (offset) {   // 算上偏移标志字符占用的2字节
			case 0: break;
			case 1: outLength -= 4; break;
			case 2:
			case 3: outLength -= 6; break;
			case 4:
			case 5: outLength -= 8; break;
			case 6: outLength -= 10; break;
		}
		return outLength / 8 * 7 + offset + 1 + 16; // 多出1字节用于循环覆盖，再加上16B备用
	}


	/// <summary>
	/// 编码二进制数据到 Base16384 UTF-16 BE 编码数据。
	/// </summary>
	/// <param name="data">二进制数据</param>
	/// <param name="bufferPtr">输出缓冲区指针</param>
	/// <param name="bufferLength">输出缓冲区长度</param>
	/// <returns>已写入输出缓冲区的内容的长度</returns>
	public static unsafe int Encode(byte[] data, byte* bufferPtr, int bufferLength) {
		var dataLength = data.Length;
		var dataPtr = (byte*)Marshal.AllocHGlobal(dataLength);
		Marshal.Copy(data, 0, (IntPtr)dataPtr, dataLength);

		var result = Encode(dataPtr, dataLength, bufferPtr, bufferLength);

		Marshal.FreeHGlobal((IntPtr)dataPtr);
		return result;
	}

	/// <summary>
	/// 解码 Base16384 UTF-16 BE 编码数据到二进制数据。
	/// </summary>
	/// <param name="data">Base16384 UTF-16 BE 编码数据</param>
	/// <param name="bufferPtr">输出缓冲区指针</param>
	/// <param name="bufferLength">输出缓冲区长度</param>
	/// <returns>已写入输出缓冲区的内容的长度</returns>
	public static unsafe int Decode(byte[] data, byte* bufferPtr, int bufferLength) {
		var dataLength = data.Length;
		var dataPtr = (byte*)Marshal.AllocHGlobal(dataLength);
		Marshal.Copy(data, 0, (IntPtr)dataPtr, dataLength);

		var result = Decode(dataPtr, dataLength, bufferPtr, bufferLength);

		Marshal.FreeHGlobal((IntPtr)dataPtr);
		return result;
	}


	/// <summary>
	/// 编码二进制数据到 Base16384 UTF-16 BE 编码数据。
	/// </summary>
	/// <param name="dataPtr">二进制数据指针</param>
	/// <param name="dataLength">二进制数据长度</param>
	/// <returns>编码结果</returns>
	public static unsafe byte[] Encode(byte* dataPtr, int dataLength) {
		var bufferLength = EncodeLength(dataLength);
		var bufferPtr = (byte*)Marshal.AllocHGlobal(bufferLength);

		var result = new byte[Encode(dataPtr, dataLength, bufferPtr, bufferLength)];

		Marshal.Copy((IntPtr)bufferPtr, result, 0, result.Length);
		Marshal.FreeHGlobal((IntPtr)bufferPtr);

		return result;
	}

	/// <summary>
	/// 解码 Base16384 UTF-16 BE 编码数据到二进制数据。
	/// </summary>
	/// <param name="dataPtr">Base16384 UTF-16 BE 编码数据指针</param>
	/// <param name="dataLength">Base16384 UTF-16 BE 编码数据长度</param>
	/// <returns>解码结果</returns>
	public static unsafe byte[] Decode(byte* dataPtr, int dataLength) {
		var bufferLength = DecodeLength(dataLength);
		var bufferPtr = (byte*)Marshal.AllocHGlobal(bufferLength);

		var result = new byte[Decode(dataPtr, dataLength, bufferPtr, bufferLength)];

		Marshal.Copy((IntPtr)bufferPtr, result, 0, result.Length);
		Marshal.FreeHGlobal((IntPtr)bufferPtr);

		return result;
	}


	/// <summary>
	/// 编码二进制数据到 Base16384 UTF-16 BE 编码数据。
	/// </summary>
	/// <param name="data">二进制数据</param>
	/// <returns>编码结果</returns>
	public static unsafe byte[] Encode(byte[] data) {
		var dataLength = data.Length;
		var dataPtr = (byte*)Marshal.AllocHGlobal(dataLength);
		Marshal.Copy(data, 0, (IntPtr)dataPtr, dataLength);

		var bufferLength = EncodeLength(dataLength);
		var bufferPtr = (byte*)Marshal.AllocHGlobal(bufferLength);

		var result = new byte[Encode(dataPtr, dataLength, bufferPtr, bufferLength)];

		Marshal.FreeHGlobal((IntPtr)dataPtr);

		Marshal.Copy((IntPtr)bufferPtr, result, 0, result.Length);
		Marshal.FreeHGlobal((IntPtr)bufferPtr);

		return result;
	}

	/// <summary>
	/// 解码 Base16384 UTF-16 BE 编码数据到二进制数据。
	/// </summary>
	/// <param name="data">Base16384 UTF-16 BE 编码数据</param>
	/// <returns>解码结果</returns>
	public static unsafe byte[] Decode(byte[] data) {
		var dataLength = data.Length;
		var dataPtr = (byte*)Marshal.AllocHGlobal(dataLength);
		Marshal.Copy(data, 0, (IntPtr)dataPtr, dataLength);

		var bufferLength = DecodeLength(dataLength);
		var bufferPtr = (byte*)Marshal.AllocHGlobal(bufferLength);

		var result = new byte[Decode(dataPtr, dataLength, bufferPtr, bufferLength)];

		Marshal.FreeHGlobal((IntPtr)dataPtr);

		Marshal.Copy((IntPtr)bufferPtr, result, 0, result.Length);
		Marshal.FreeHGlobal((IntPtr)bufferPtr);

		return result;
	}
}