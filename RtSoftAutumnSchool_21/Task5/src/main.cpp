#include "Application.hpp"

int main(int argc, char** argv){

	using namespace rt_soft_autumn_school;

	auto app = rt_soft_autumn_school::CreateApplication({ argc, argv });
	app->Run();

	return 0;
}