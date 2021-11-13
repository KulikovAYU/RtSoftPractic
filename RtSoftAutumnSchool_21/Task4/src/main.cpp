#include "ThreadSafeBuffer.hpp"
#include <iostream>


struct Test {

	int x = 1;

	~Test() { std::cout << "~Test()" << std::endl; }
};

int main() {
	using namespace rt_soft_autumn_school;
	CircularThreadSafeBuffer<Test, 1> buffer;
	buffer.Enqueue(Test());

	CircularThreadSafeBuffer<Test, 1> buffer1(buffer);

	CircularThreadSafeBuffer<Test, 1> buffer3(std::move(buffer1));

	
	buffer.Emplace(10);

	buffer.Dequeue();
	buffer.Dequeue();
	buffer.Get();
}