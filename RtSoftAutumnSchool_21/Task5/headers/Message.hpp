#pragma once
#include <Constants.hpp>


namespace rt_soft_autumn_school {

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

		uint8_t m_measurements[MAX_SIZE];
	};

}