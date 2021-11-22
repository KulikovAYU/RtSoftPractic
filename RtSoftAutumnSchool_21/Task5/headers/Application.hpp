#pragma once
#include <cassert>
#include <memory>
#include <string>

int main(int argc, char** argv);

namespace rt_soft_autumn_school {


	struct ApplicationCommandLineArgs
	{
		int Count = 0;
		char** Args = nullptr;

		const char* operator[](int index) const
		{
			assert(index < Count);
			return Args[index];
		}
	};


	class Application 
	{
	public:
		Application(const std::string& name = "BarChart App", ApplicationCommandLineArgs args = ApplicationCommandLineArgs());

		void Run();

	private:
		friend int ::main(int argc, char** argv);
	};


	std::unique_ptr<Application> CreateApplication(ApplicationCommandLineArgs args);

}