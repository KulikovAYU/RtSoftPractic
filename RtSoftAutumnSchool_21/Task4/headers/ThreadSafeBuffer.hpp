#include <iostream>
#include <memory>
#include <mutex>

namespace rt_soft_autumn_school {


	template<typename T, size_t MaxSize>
	class CircularThreadSafeBuffer final
	{
		using SizeType = size_t;
		using DataType = T;
		using DataTypePtr = T*;

		static_assert(!std::is_same_v<T, void>, "Type of the buffer can not be void");
		static_assert(MaxSize != 0, "MaxSize of the buffer can not be 0");

	public:
		CircularThreadSafeBuffer() : m_pBuffer{ reinterpret_cast<DataTypePtr>(new(std::nothrow) char[GetMaxSize()]) },
			m_pWritePtr{ reinterpret_cast<DataTypePtr>(m_pBuffer) },
			m_pReadPtr{ reinterpret_cast<DataTypePtr>(m_pBuffer) }
		{}

		~CircularThreadSafeBuffer() {
			delete[] reinterpret_cast<char*>(m_pBuffer);
		}

		CircularThreadSafeBuffer(const CircularThreadSafeBuffer& rhs) noexcept {
			
			this->CopyConstructFrom(rhs);
		}

		CircularThreadSafeBuffer(CircularThreadSafeBuffer&& rhs) noexcept {
		
			this->MoveConstructFrom(std::move(rhs));
		}

		CircularThreadSafeBuffer& operator =(const CircularThreadSafeBuffer& rhs) noexcept {

			this->CopyConstructFrom(rhs);
			return *this;
		}


		CircularThreadSafeBuffer& operator =(CircularThreadSafeBuffer&& rhs) noexcept {

			this->MoveConstructFrom(std::move(rhs));
			return *this;
		}

		constexpr SizeType GetMaxSize() const noexcept { return sizeof(T) * MaxSize; }


		//Construct in place in this circular buffer.
		template<typename... Args>
		void Emplace(Args&&... args) {
			std::unique_lock<std::mutex> lock(m_isBusy);

			//if buffer is full we erase last item
			if (IsFull()) {
				m_pWritePtr->~T();
				m_pWritePtr = MoveLeft(m_pWritePtr);
			}

			//invoke placement new (constructor) and forward args
			new(m_pWritePtr)T{ std::forward<Args>(args)... };

			m_pWritePtr = MoveRight(m_pWritePtr);

			m_notEmpty.notify_all();
		}


		// Add an item to this circular buffer.
		void Enqueue(T item) {
			std::unique_lock<std::mutex> lock(m_isBusy);

			//if buffer is full we erase last item
			if (IsFull()) {
				m_pWritePtr->~T();
				m_pWritePtr = MoveLeft(m_pWritePtr);
			}
				

			//invoke placement new and try to move entt
			new(m_pWritePtr)T{ std::move(item) };

			m_pWritePtr = MoveRight(m_pWritePtr);

			m_notEmpty.notify_all();
		}


		// Remove an item from this circular buffer
		void Dequeue() {

			std::unique_lock<std::mutex> lock(m_isBusy);

			if (IsEmpty())
			{
				m_pWritePtr->~T();
				m_notEmpty.notify_all();
				return;
			}

			//Get previous pointer of m_pWritePtr and invoke ~T(), and
			//rewrite m_pReadPtr
			DataTypePtr previousItm = MoveLeft(m_pWritePtr);
			previousItm->~T();
			m_pWritePtr = previousItm;
			m_notEmpty.notify_all();
		}


		T& Get(){

			std::unique_lock<std::mutex> lock(m_isBusy);

			//while construction explanation:
			//If anyone puts task
			//1-st thread or 2-nd thread can take that new item by 2 ways:
			//1-st thread is waking up by condition_variable notifying
			//2-nd thread is also trying to get item
			//and 2-nd thread takes that item before 1-st thread takes it.
			//If its case happens 1-st thread takes item from same data and can change it
			while (IsEmpty())
				m_notEmpty.wait(lock); //sleep

			return TakeLocked();
		}


	private:

		bool IsFull() const noexcept {
			return PtrInTheEnd(m_pWritePtr);
		}

		bool IsEmpty() const noexcept {
			return m_pBuffer == m_pWritePtr;
		}

		bool PtrInTheEnd(DataTypePtr ptr) const noexcept {
			return (m_pBuffer + GetMaxSize()) == ptr;
		}

		DataTypePtr MoveLeft(DataTypePtr pSrc, SizeType memCellOffset = 1) const {

			while (pSrc != m_pBuffer &&
				memCellOffset > 0){
				--pSrc;
				--memCellOffset;
			}

			return pSrc;
		}

		DataTypePtr MoveRight(DataTypePtr pSrc, SizeType memCellOffset = 1) const {

			while (pSrc != (m_pBuffer + GetMaxSize()) &&
				memCellOffset > 0){
				++pSrc;
				--memCellOffset;
			}

			return pSrc;
		}

		T& TakeLocked(){

			DataTypePtr pBuff = m_pReadPtr;

			if (PtrInTheEnd(++m_pReadPtr))
				m_pReadPtr = reinterpret_cast<DataTypePtr>(m_pBuffer);

			return *pBuff;
		}


		template<typename T2, size_t MaxSize2>
		void CopyConstructFrom(const CircularThreadSafeBuffer<T2, MaxSize2>& rhs) noexcept {

			static_assert(std::is_same_v<T, T2>, "Type of the buffer can not be difference");

			std::scoped_lock guard_(m_isBusy, rhs.m_isBusy);

			//cut count to copy some objects if 
			//size of rhs is bigger than size this
			size_t countOfCopyObjs = MaxSize2;
			if (MaxSize < MaxSize2)
				countOfCopyObjs = MaxSize;

			//allocate new chunk of memory
			DataTypePtr pNewBuffer = reinterpret_cast<DataTypePtr>(new(std::nothrow) char[countOfCopyObjs * sizeof(T)]);
			ptrdiff_t distance = 0;

			if (!rhs.IsEmpty())
			{
				//set pointer on begin other block
				DataTypePtr pStartRhsBuffer = reinterpret_cast<DataTypePtr>(rhs.m_pBuffer);

				//let's calculate distance between blocks
				distance = rhs.m_pWritePtr - pStartRhsBuffer;

				int bufDistance = static_cast<int>(distance);
				for (SizeType i = 0; i < countOfCopyObjs && bufDistance > 0; ++i) {
					//invoke placement new and copy constructor
					new(pNewBuffer + i)T(*pStartRhsBuffer);
					pStartRhsBuffer++;
					--bufDistance;
				}

				SizeType elements = static_cast<SizeType>(distance);
				for (SizeType i = 0; i < elements; ++i)
					(m_pBuffer + i)->~T();
			}

			delete[] reinterpret_cast<char*>(m_pBuffer);
			m_pBuffer = pNewBuffer;
			m_pReadPtr = m_pBuffer;
			m_pWritePtr = m_pBuffer + distance;
		}

		template<typename T2, size_t MaxSize2>
		void MoveConstructFrom(CircularThreadSafeBuffer<T2, MaxSize2>&& rhs) noexcept {

			static_assert(std::is_same_v<T, T2>, "Type of the buffer can not be difference");
			static_assert(MaxSize == MaxSize2, "Size of buffers are not equals");

			this->Swap(rhs);
		}


		void Swap(CircularThreadSafeBuffer& rhs) noexcept {

			std::scoped_lock guard_(m_isBusy, rhs.m_isBusy);

			using std::swap;

			swap(m_pBuffer, rhs.m_pBuffer);
			swap(m_pReadPtr, rhs.m_pReadPtr);
			swap(m_pWritePtr, rhs.m_pWritePtr);
		}

		DataTypePtr m_pBuffer = { nullptr };

		DataTypePtr m_pReadPtr = { nullptr };
		DataTypePtr m_pWritePtr = { nullptr };

		std::condition_variable m_notEmpty; //guarded by mutex
		mutable std::mutex m_isBusy;
	};
}