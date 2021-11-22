#include "Application.hpp"

int main(int argc, char** argv)
{
	auto app = rt_soft_autumn_school::CreateApplication({ argc, argv });
	app->Run();

	return 0;
}