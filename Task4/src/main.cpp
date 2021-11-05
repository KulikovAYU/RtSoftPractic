int main()
{
   auto pList = rt_soft_autumn_school::LinkedListBuilder::MakeList();
   
   std::cout << "Src list"<< std::endl;
   PrintList(pList.get());

   auto pNewList = pList->clone();

   std::cout << "Dest list" << std::endl;
   PrintList(pNewList.get());

  /* auto pCurr = pList->GetHead();

   while (pCurr)
   {
       std::cout << pCurr->m_data_ << std::endl;

       pCurr =  pCurr->m_pNext_.get();
   }*/

    system("pause");
    return 0;
}