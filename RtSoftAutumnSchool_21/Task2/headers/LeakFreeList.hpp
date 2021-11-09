#include <iostream>
#include <memory>
#include <vector>

namespace rt_soft_autumn_school
{
	template<typename Val>
	struct Node;

	template<typename Val>
	using HardPointer = std::unique_ptr<Node<Val>>;

	template<typename Val>
	using SoftPointer = Node<Val>*;

	template<typename Val>
	class DeepCloneStrat;

	template<typename Val>
	class LeakFreeList;

	template<typename Val, typename Callback>
	void TraverseList(LeakFreeList<Val>* pSrcList, const Callback& pCb);

	template<typename Val>
	struct Node
	{
		explicit Node(Val data) : m_data_{ std::move(data) } {}

		Val m_data_;
		HardPointer<Val> m_pNext_ = { nullptr };
		SoftPointer<Val> m_pRand_node_ = { nullptr };
	};

	template<typename Val>
	class LeakFreeList
	{

	public:

		//static polyrphism is better than dynamic
		//so i used that pattern
		template<typename CopyStrat>
		std::unique_ptr<LeakFreeList<Val>> clone(CopyStrat copier) {
			return copier.clone(this);
		}

		template <typename T>
		void AddNode(T&& val) {
			HardPointer<Val> new_node = std::make_unique<Node<Val>>(std::forward<T>(val));

			if (!this->m_pHead_) {
				this->m_pHead_ = std::move(new_node);
				m_pTail_ = m_pHead_.get();
				return;
			}

			m_pTail_->m_pNext_ = std::move(new_node);
			m_pTail_ = m_pTail_->m_pNext_.get();
		}

		SoftPointer<Val> GetHead() const noexcept {
			return m_pHead_.get();
		}


		SoftPointer<Val> GetTail() const noexcept {
			return m_pTail_;
		}

	private:
		HardPointer<Val> m_pHead_ = { nullptr };
		SoftPointer<Val> m_pTail_ = { nullptr };
	};



	template<typename Val>
	class DeepCloneStrat
	{
	public:
		std::unique_ptr<LeakFreeList<Val>> clone(LeakFreeList<Val>* pList)
		{
			//make clone all nodes and make hard links
			//create a vector of pointers to the source tree nodes
			//create a vector of pointers to the copy tree nodes
			//because source and dest list full identity we can find index by pointer "m_pRand_node_"
			//in source vector and take pointer by index in dest vector.
			//and then iterate throw cloned list and make chain

			std::unique_ptr<LeakFreeList<Val>> p_new_list = std::make_unique<LeakFreeList<Val>>();
			CloneNodesWithHardPointers(p_new_list, pList);
			ConnectSoftPointers(p_new_list, pList);

			return p_new_list;
		}

	private:
		void CloneNodesWithHardPointers(std::unique_ptr<LeakFreeList<Val>>& destList, LeakFreeList<Val>* pSrcList)
		{
			//clones only hard links and fill vectors
			TraverseList(pSrcList, [this, &destList](SoftPointer<Val> pCurr) {
				m_src_list_.emplace_back(pCurr);
				destList->AddNode(pCurr->m_data_);
				m_dest_list_.emplace_back(destList->GetTail()); });

		}

		void ConnectSoftPointers(std::unique_ptr<LeakFreeList<Val>>& destList, LeakFreeList<Val>* pSrcList)
		{
			auto p_src_curr = pSrcList->GetHead();
			auto p_dest_curr = destList->GetHead();

			while (p_src_curr && p_dest_curr)
			{
				size_t src_index = 0;
				if (GetIndex(m_src_list_, p_src_curr->m_pRand_node_, src_index))
				{
					//ok. we found index so let's take in m_dest_list_
					p_dest_curr->m_pRand_node_ = m_dest_list_[src_index];
				}

				p_src_curr = p_src_curr->m_pNext_.get();
				p_dest_curr = p_dest_curr->m_pNext_.get();
			}

			m_src_list_.clear();
			m_dest_list_.clear();
		}


		bool GetIndex(const std::vector<SoftPointer<Val>>& src_list, const SoftPointer<Val>& req_pointer, size_t& out_index)
		{
			auto it = std::find(src_list.cbegin(), src_list.cend(), req_pointer);
			if (it != src_list.cend())
			{
				out_index = it - src_list.cbegin();
				return true;
			}

			return false;
		}


		std::vector<SoftPointer<Val>> m_src_list_;
		std::vector<SoftPointer<Val>> m_dest_list_;
	};

	template<typename Val, typename Callback>
	void TraverseList(LeakFreeList<Val>* pSrcList, const Callback& pCb)
	{
		auto pCurr = pSrcList->GetHead();

		while (pCurr)
		{
			pCb(pCurr);
			pCurr = pCurr->m_pNext_.get();
		}
	}

	template<typename Val>
	void GoToTheEndList(LeakFreeList<Val>* pSrcList)
	{
		TraverseList(pSrcList, [](const SoftPointer<Val>& pCurr) {});
	}

	template<typename Val>
	void PrintList(LeakFreeList<Val>* pSrcList)
	{
		auto printFunc = [](const SoftPointer<Val>& pCurr) {
			std::cout << "Hard node = " << pCurr->m_data_ << std::endl;
			std::cout << "Soft node = " << pCurr->m_pRand_node_->m_data_ << std::endl; };

		TraverseList(pSrcList, printFunc);
	}

	template<typename Val>
	size_t GetLenght(LeakFreeList<Val>* pSrcList)
	{
		size_t counter = 0;
		TraverseList(pSrcList, [&counter](const SoftPointer<Val>& pCurr) { ++counter; });

		return counter;
	}
}