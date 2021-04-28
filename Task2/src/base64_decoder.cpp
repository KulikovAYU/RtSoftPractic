#include "base64_decoder.h"
#include <codecvt>

static const std::wstring base64_chars =
L"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

std::string base64_decode(std::wstring const& encoded_string){
size_t in_len = encoded_string.size();
size_t i = 0;
size_t j = 0;
size_t in_ = 0;
unsigned char char_array_4[4], char_array_3[3];
std::string ret;

while (in_len-- && (encoded_string[in_] != '=') /*&& is_base64(encoded_string[in_])*/)
{
	if (encoded_string[in_] == '\r' || encoded_string[in_] == '\n')
	{
		in_++;
	}
	else
	{
		char_array_4[i++] = static_cast<unsigned char>(encoded_string[in_]); in_++;
		if (i == 4) {
			for (i = 0; i < 4; i++)
				char_array_4[i] = static_cast<unsigned char>(base64_chars.find(char_array_4[i]));

			char_array_3[0] = (char_array_4[0] << 2) + ((char_array_4[1] & 0x30) >> 4);
			char_array_3[1] = ((char_array_4[1] & 0xf) << 4) + ((char_array_4[2] & 0x3c) >> 2);
			char_array_3[2] = ((char_array_4[2] & 0x3) << 6) + char_array_4[3];

			for (i = 0; (i < 3); i++)
				ret += char_array_3[i];
			i = 0;
		}
	}
}

if (i) {
	for (j = i; j < 4; j++)
		char_array_4[j] = 0;

	for (j = 0; j < 4; j++)
		char_array_4[j] = static_cast<unsigned char>(base64_chars.find(char_array_4[j]));

	char_array_3[0] = (char_array_4[0] << 2) + ((char_array_4[1] & 0x30) >> 4);
	char_array_3[1] = ((char_array_4[1] & 0xf) << 4) + ((char_array_4[2] & 0x3c) >> 2);
	char_array_3[2] = ((char_array_4[2] & 0x3) << 6) + char_array_4[3];

	for (j = 0; (j < i - 1); j++) ret += char_array_3[j];
}

return ret;
}

std::wstring Base64::get_string_from_base64_format(const std::wstring& base64byteString)
{
	const std::string& base64codeString = base64_decode(base64byteString);

	//get normal string from base64 decoding striong
	std::wstring_convert<std::codecvt_utf8<wchar_t>> myconv;
	return myconv.from_bytes(std::string(base64codeString.begin(), base64codeString.end()));
}
