#pragma once
#include <Constants.hpp>


namespace rt_soft_autumn_school {
	//two rows array
	//with black and white picture
	//which represents a pixel from width and height
	struct Picture {
		uint8_t m_picture[WIDTH][HEIGHT] = {0};

		uint8_t* operator[](size_t nRow){
			return m_picture[nRow];
		}

		const uint8_t* operator[](size_t nRow) const {
			return m_picture[nRow];
		}

		constexpr size_t GetW() const {
			return WIDTH;
		}

		constexpr size_t GetH() const {
			return HEIGHT;
		}
	};


	struct Message
	{
		const uint8_t& operator[](size_t n) const {
			return m_measurements[n];
		}

		uint8_t& operator[](size_t n) {
			return const_cast<uint8_t&>(static_cast<const Message&>(*this)[n]);
		}

		constexpr size_t GetSize() const noexcept {
			return sizeof(m_measurements) / sizeof(uint8_t);
		}

		uint8_t m_measurements[MAX_SIZE]; // массив измерений
	};

}