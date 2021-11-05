#include <gtest/gtest.h>
#include <LeakFreeList.hpp>


using namespace rt_soft_autumn_school;


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