#include <gtest/gtest.h>
#include <LeakFreeList.hpp>


using namespace rt_soft_autumn_school;

class LinkedListBuilder
{
public:

	static std::unique_ptr<LeakFreeList<size_t>> MakeList()
	{
		std::unique_ptr<LeakFreeList<size_t>> ptrList = std::make_unique<LeakFreeList<size_t>>();

		ptrList->AddNode(0);
		ptrList->AddNode(1);
		ptrList->AddNode(2);
		ptrList->AddNode(3);
		ptrList->AddNode(4);

		MixList(ptrList);
		return ptrList;
	}

private:
	static void MixList(std::unique_ptr<LeakFreeList<size_t>>& pList)
	{
		auto pHead = pList->GetHead();

		//0
		pHead->m_pRand_node_ = pHead->m_pNext_.get();

		//1
		pHead->m_pNext_->m_pRand_node_ = pHead;

		//2
		pHead->m_pNext_->m_pNext_->m_pRand_node_ = pHead;

		//3
		pHead->m_pNext_->m_pNext_->m_pNext_->m_pRand_node_ = pHead->m_pNext_->m_pNext_.get();

		//4
		pHead->m_pNext_->m_pNext_->m_pNext_->m_pNext_->m_pRand_node_ = pHead;
	}
};

TEST(SimpleSharedPtrTest, CheckListsLenght)
{
	auto pSrcList = LinkedListBuilder::MakeList();

	auto pClonedList = pSrcList->clone(DeepCloneStrat<size_t>());


	size_t nSrcListLenght = GetLenght(pSrcList.get());
	size_t nClonedListLenght = GetLenght(pClonedList.get());

	EXPECT_TRUE(nSrcListLenght == nClonedListLenght);
}

TEST(SimpleSharedPtrTest, TailDataIs4)
{
	auto pSrcList = LinkedListBuilder::MakeList();

	auto pClonedList = pSrcList->clone(DeepCloneStrat<size_t>());


	GoToTheEndList(pSrcList.get());
	GoToTheEndList(pClonedList.get());


	EXPECT_TRUE(pSrcList->GetTail()->m_data_ == 4);
	EXPECT_TRUE(pSrcList->GetTail()->m_data_ == 4);
}

TEST(SimpleSharedPtrTest, NextOfLastNodesBoothNullptr)
{
	auto pSrcList = LinkedListBuilder::MakeList();

	auto pClonedList = pSrcList->clone(DeepCloneStrat<size_t>());


	GoToTheEndList(pSrcList.get());
	GoToTheEndList(pClonedList.get());


	EXPECT_TRUE(pSrcList->GetTail()->m_pNext_ == nullptr);
	EXPECT_TRUE(pClonedList->GetTail()->m_pNext_ == nullptr);
}


TEST(SimpleSharedPtrTest, CheckFullEqualsList)
{
	auto pSrcList = LinkedListBuilder::MakeList();
	
	auto pClonedList = pSrcList->clone(DeepCloneStrat<size_t>());

	auto pCurrSrcList = pSrcList->GetHead();
	auto pCurrClonedList = pClonedList->GetHead();

	while (pCurrSrcList && pCurrClonedList)
	{
		EXPECT_TRUE(pCurrSrcList->m_data_ == pCurrClonedList->m_data_);
		EXPECT_TRUE(pCurrSrcList->m_pRand_node_->m_data_ == pCurrClonedList->m_pRand_node_->m_data_);

		pCurrSrcList = pCurrSrcList->m_pNext_.get();
		pCurrClonedList = pCurrClonedList->m_pNext_.get();
	}
}