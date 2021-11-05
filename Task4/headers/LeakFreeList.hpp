namespace rt_soft_autumn_school
{

	//1. Реализовать структуру данных :
	//-обычный односвязный список(с указателем на голову)
	//	- чтобы была leek free(со smart ptr) и шаблонной
	//	- у каждого элемента есть ссылка еще дополнительно на произвольный элемент в списке

	//2. Реализовать стратегию для копирования этой структуры данных(односвязный список)

	template<typename Val>
    struct Node;

    template<typename Val>
    using HardPointer = std::unique_ptr<Node<Val>>;

	template<typename Val>
	using SoftPointer = Node<Val>*;

    class LinkedListBuilder;

    template<typename Val>
    struct Node
    {
        explicit Node(Val data) : m_data_{std::move(data)} {}

        Val m_data_;
        HardPointer<Val> m_pNext_ = {nullptr};
        SoftPointer<Val> m_pRand_node_ = { nullptr };
    };

    
	template<typename Val>
    struct DeepCloneStrat;

    template<typename Val, typename CopyStrat = DeepCloneStrat<Val>>
    class LeakFreeList
    {
     
    public:

        std::unique_ptr<LeakFreeList<Val>> clone() {
            return CopyStrat().clone(this);
        }


		template <typename T>
		void AddNode(T&& val)
		{
			HardPointer<Val> new_node = std::make_unique<Node<Val>>(std::forward<T>(val));

			if (!this->m_pHead_)
			{
				this->m_pHead_ = std::move(new_node);
                m_pTail_ = m_pHead_.get();
				return;
			}

            m_pTail_->m_pNext_ = std::move(new_node);
            m_pTail_ = m_pTail_->m_pNext_.get();
		}

        SoftPointer<Val> GetHead() const {
            return m_pHead_.get();
		}

    private:
        friend class LinkedListBuilder;
        friend struct DeepCloneStrat<Val>;

        HardPointer<Val> m_pHead_ = { nullptr };
        SoftPointer<Val> m_pTail_ = { nullptr };
    };



    template<typename Val>
    struct DeepCloneStrat
    {
        std::unique_ptr<LeakFreeList<Val>> clone(LeakFreeList<Val>* pList)
        {
			//make clone all nodes and make hard links
		    //create a vector of pointers to the source tree nodes
		    //create a vector of pointers to the copy tree nodes
		    //because source and dest list full identity we can find index by pointer "m_pRand_node_"
            //in source vector and take pointer by index in dest vector.
		    //and then iterate throw cloned list make chain

            std::unique_ptr<LeakFreeList<Val>> p_new_list = std::make_unique<LeakFreeList<Val>>();
            CloneNodesWithHardPointers(p_new_list, pList);
            ConnectSoftPointers(p_new_list, pList);
           
            
            return p_new_list;
        }

    private:
        void CloneNodesWithHardPointers(std::unique_ptr<LeakFreeList<Val>>& destList, LeakFreeList<Val>* pSrcList)
        {
            //clones only hard links and fill vectors
            auto p_src_curr = pSrcList->GetHead();
        
			while (p_src_curr)
			{
                m_src_list_.emplace_back(p_src_curr);
                destList->AddNode(p_src_curr->m_data_);
                m_dest_list_.emplace_back(destList->m_pTail_);

                p_src_curr = p_src_curr->m_pNext_.get();
			}
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
            auto& pHead = pList->m_pHead_;
            
            //0
            pHead->m_pRand_node_ = pHead->m_pNext_.get();

            //1
            pHead->m_pNext_->m_pRand_node_ = pHead.get();

            //2
            pHead->m_pNext_->m_pNext_->m_pRand_node_ = pHead.get();

            //3
            pHead->m_pNext_->m_pNext_->m_pNext_->m_pRand_node_ = pHead->m_pNext_->m_pNext_.get();

            //4
            pHead->m_pNext_->m_pNext_->m_pNext_->m_pNext_->m_pRand_node_ = pHead.get();
        }
    };

    template<typename Val>
    void PrintList(LeakFreeList<Val>* pSrcList)
    {
		auto pCurr = pSrcList->GetHead();

		while (pCurr)
		{
			std::cout <<"Hard node = "<<pCurr->m_data_ << std::endl;
            std::cout <<"Soft node = "<<pCurr->m_pRand_node_->m_data_ << std::endl;

			pCurr = pCurr->m_pNext_.get();
		}
    }
}