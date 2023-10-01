using System.Text;

namespace LC6464.Base16384;

/// <summary>
/// Base16384 编解码器。
/// </summary>
public static partial class Base16384 {
	/// <summary>
	/// 将 UTF-16 BE 编码的数据转换为 UTF-8 编码的数据。
	/// </summary>
	/// <param name="data">UTF-16 BE 编码的数据</param>
	/// <returns>UTF-8 编码的数据</returns>
	public static ReadOnlySpan<byte> ConvertFromUtf16BEBytesToUtf8Bytes(this ReadOnlySpan<byte> data) =>
		Encoding.Convert(Encoding.BigEndianUnicode, Encoding.UTF8, data.ToArray());

	/// <summary>
	/// 将 UTF-16 BE 编码的数据转换为 UTF-8 with BOM 编码的数据。
	/// </summary>
	/// <param name="data">UTF-16 BE 编码的数据</param>
	/// <returns>UTF-8 with BOM 编码的数据</returns>
	public static ReadOnlySpan<byte> ConvertFromUtf16BEBytesToUtf8BOMBytes(this ReadOnlySpan<byte> data) {
		var utf8Bytes = data.ConvertFromUtf16BEBytesToUtf8Bytes();
		Span<byte> buffer = new byte[utf8Bytes.Length + 3];
		Utf8Preamble.CopyTo(buffer);
		utf8Bytes.CopyTo(buffer[3..]);
		return buffer;
	}


	/// <summary>
	/// 将 UTF-16 BE 编码的数据转换为字符串。
	/// </summary>
	/// <param name="data">UTF-16 BE 编码的数据</param>
	/// <returns>字符串</returns>
	public static ReadOnlySpan<byte> ConvertFromUtf16BEBytesToUtf16LEBytes(this ReadOnlySpan<byte> data) {
		Span<byte> buffer = new byte[data.Length];
		var count = data.Length / 2;
		for (var i = 0; i < count; i++) {
			buffer[i * 2] = data[i * 2 + 1];
			buffer[i * 2 + 1] = data[i * 2];
		}
		return buffer;
	}

	/// <summary>
	/// 将 UTF-16 BE 编码的数据转换为字符串。
	/// </summary>
	/// <param name="data">UTF-16 BE 编码的数据</param>
	/// <returns>字符串</returns>
	public static ReadOnlySpan<char> ConvertFromUtf16BEBytesToString(this ReadOnlySpan<byte> data) {
		var count = data.Length / 2;
		Span<char> buffer = new char[count];
		for (var i = 0; i < count; i++) {
			buffer[i] = (char)(data[i * 2] << 8 | data[i * 2 + 1]);
		}
		return buffer;
	}
}