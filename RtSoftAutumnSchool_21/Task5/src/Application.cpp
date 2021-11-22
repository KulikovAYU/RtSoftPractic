#include "Application.hpp"
#include "BasicLogic.hpp"

namespace rt_soft_autumn_school {

	Application::Application(const std::string& name /*= "Gistogramm App"*/, ApplicationCommandLineArgs args /*= ApplicationCommandLineArgs()*/)
	{}

	void Application::Run()
	{
		//create basic logic of app
		BasicLogic logic;
		logic.Run();
	}


	std::unique_ptr<Application> CreateApplication(ApplicationCommandLineArgs args)
	{
		return std::make_unique<Application>("Gistogramm App", args);
	}
}