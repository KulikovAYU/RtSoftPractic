#include <cpprest/http_client.h>
#include <cpprest/json.h>
//#pragma comment(lib, "cpprest_2_10")


#include <iostream>
#include <vector>

using namespace web;
using namespace web::http;
using namespace web::http::client;


void display_json(json::value const& jValue, utility::string_t const& prefix)
{
	std::wcout << prefix << jValue.serialize() << std::endl;
}

pplx::task<http_response> make_task_request(http_client& client, method mtd, json::value const& jValue) 
{
	if (mtd == methods::GET || mtd == methods::HEAD)
	{
		return client.request(mtd, L"/restdemo");
	}
	else if (mtd == methods::PUT)
	{
		web::uri_builder builder;
		return client.request(mtd, builder.append_path(L"/restdemo").to_string(), jValue);
	}

	return client.request(mtd, L"/restdemo", jValue);
}


void make_request(http_client& client, method mtd, json::value const& jValue) 
{
	
	make_task_request(client, mtd, jValue).then([](http_response response) 
		{
		if (response.status_code() == status_codes::OK)
		{
			return response.extract_json();
		}
		return pplx::task_from_result(json::value());
		}).then([](pplx::task<json::value> previousTask) 
			{
				try
				{
					display_json(previousTask.get(), L"R: ");
				}
				catch (http_exception const& e)
				{
					std::wcout << e.what() << std::endl;
				}
			}).wait();

}


int main(int argc, char* argv[])
{
	http_client client(L"http://localhost:12345/");

	auto postValue = json::value::object();
	postValue[U("creationFlag")] = json::value::boolean(true);
	make_request(client, methods::POST, postValue);

	auto getValue = json::value::object();
	web::uri_builder builder;
	client.request(methods::GET, builder.append_path(L"/restdemo").append_query(L"counter_value", L"1").to_string()).wait();

	auto putValue = json::value::object();
	putValue[L"puttingIndex"] = 1;
	putValue[L"puttingValue"] = 10;
	display_json(putValue, L"S: ");
	make_request(client, methods::PUT, putValue);

	return 0;
}