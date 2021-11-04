#include <iostream>
#include <atomic>
#include <new>
#include <mutex>


namespace rt_soft_autumn_school
{
	#define  NODISCARD_DEF [[nodiscard]]

	template<typename T>
	class shared_ptr;


	//simle ref counter (control block)
	//with thread safe counter
	template<typename T>
	struct ref_count
	{
		using atomic_counter = std::atomic<int>;
		using data_pointer = T*;

	public:

		explicit ref_count(data_pointer p_data) : p_data_(p_data) {}

		void incref() noexcept { //increment shared pointer cnt
			++uses_;
		}

		void incwref() noexcept { //increment weak pointer cnt
			++weaks_;
		}

		void decref() noexcept { //decrement shared pointer cnt
			if (--uses_ == 0) {
				destroy_data_pointer();
				decwref(); //cause we can reproduce shared_ptr
			}
		}

		void decwref() noexcept { //decrement weak pointer cnt
			if (--weaks_ <= 0)
				self_destroy();
		}

		size_t use_count() const noexcept {
			return uses_;
		}

	private:
		void destroy_data_pointer() noexcept {
			delete p_data_;
		}

		void self_destroy() noexcept {
			delete this;
		}

	private:
		atomic_counter uses_ = { 0 }; //shared pointer cnt
		atomic_counter weaks_ = { 0 };//weak pointer cnt


		data_pointer p_data_; //data pointer to storage
	};


	template<typename T>
	class shared_ptr
	{
	public:
		using element_type = std::remove_extent_t<T>;

		constexpr shared_ptr() noexcept = delete;

		shared_ptr(const shared_ptr& rhs) noexcept {
			this->copy_construct_from(rhs);
		}

		constexpr shared_ptr(element_type* p) noexcept : ptr_contr_block_{ new(std::nothrow) ref_count<T>(p) }, p_data_{p} {
			this->incref();
		}

		shared_ptr(shared_ptr&& rhs) noexcept {
			this->move_construct_from(std::move(rhs));
		}

		shared_ptr& operator=(const shared_ptr& rhs) noexcept {
			if (this == &rhs)
				return *this;

			//copy and swap
			shared_ptr(rhs).swap(*this);

			return *this;
		}

		shared_ptr& operator=(shared_ptr&& rhs) noexcept { //take recourse from rhs
			if (this == &rhs)
				return *this;

			//let's make prvalue, because rhs - is lvalue
			shared_ptr(std::move(rhs)).swap(*this);

			return *this;
		}

		~shared_ptr() {
			this->decref();
		}

		NODISCARD_DEF size_t use_count() const noexcept {
			return ptr_contr_block_ ? ptr_contr_block_->use_count() : 0;
		}

		NODISCARD_DEF T& operator*() const noexcept {
			return *get();
		}

		NODISCARD_DEF T& operator->() const noexcept {
			return get();
		}

	
		NODISCARD_DEF element_type* get()  const noexcept {
			return p_data_;
		}

	private:
		template<typename T2>
		void move_construct_from(shared_ptr<T2>&& rhs) noexcept {

			std::scoped_lock guard_(sync_, rhs.sync_);

			this->p_data_ = rhs.p_data_;
			this->ptr_contr_block_ = rhs.ptr_contr_block_;

			rhs.p_data_ = nullptr;
			rhs.ptr_contr_block_ = nullptr;
		}


		template<typename T2>
		void copy_construct_from(const shared_ptr<T2>& rhs) noexcept {

			std::scoped_lock guard_(sync_, rhs.sync_);

			rhs.incref();

			this->p_data_ = rhs.p_data_;
			this->ptr_contr_block_ = rhs.ptr_contr_block_;
		}


		void swap(shared_ptr& rhs) noexcept
		{
			std::scoped_lock guard_(sync_, rhs.sync_);

			using std::swap;
			swap(this->p_data_, rhs.p_data_);
			swap(this->ptr_contr_block_, rhs.ptr_contr_block_);
		}

		void incref() const noexcept {
			if (ptr_contr_block_)
				ptr_contr_block_->incref();
		}

		void incwref() noexcept {
			if (ptr_contr_block_)
				ptr_contr_block_->incwref();
		}


		void decref() noexcept {
			if (ptr_contr_block_)
				ptr_contr_block_->decref();
		}

		void decwref() noexcept {
			if (ptr_contr_block_)
				ptr_contr_block_->decwref();
		}


		element_type* p_data_ = { nullptr };
		ref_count<T>* ptr_contr_block_ = { nullptr };

		mutable std::mutex sync_;
	};

}