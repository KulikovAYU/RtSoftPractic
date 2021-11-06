#include <cpprest/http_listener.h>
#include <cpprest/http_msg.h>
#include <cpprest/base_uri.h>
#include <map>

using namespace web::http::experimental::listener;
using namespace web::http;


const utility::string_t BAD_REQUEST = U("Bad_request");
const utility::string_t GOOD_RESPONSE = U("Good_response");
const utility::string_t CAN_T_FOUND_REQ_VALUE = U("cant_found_req_value");

std::map<size_t, int32_t> counters_map;



void print_message(bool bIsServerMsg, const utility::string_t& tag, const utility::string_t& val)
{
	const utility::string_t& prefix = bIsServerMsg ? U("Server: ") : U("Client: ");
	std::wcout << prefix << tag << " " << val << std::endl;
}


bool getValueFromRequestString(const http_request& req, const utility::string_t& inValue,utility::string_t& outValue)
{
	auto req_map = web::uri::split_query(req.request_uri().query());

	auto it = req_map.find(inValue);
	if (it != req_map.end())
	{
		outValue = it->second;
		return true;
	}

	return false;
}


void extract_json_from_body(const http_request& req, web::json::value& outJson)
{
	// extracts the request content into a json
	req.extract_json().
		then([&outJson](pplx::task<web::json::value> task)
			{
				outJson = task.get();
			}).wait();
}


template<typename T>
[[nodiscard]] utility::string_t convert_to_string(const T& valToConvert)
{
	utility::stringstream_t ss;
	ss << valToConvert;
	return ss.str();
}


[[nodiscard]] utility::string_t make_connetion_string(const utility::string_t& mainUri = U("http://localhost"),
													  const utility::string_t& port = U("12345"),
													  const utility::string_t& nextUri = U("restdemo"))
{
	return mainUri + utility::string_t(U(":")) + port + utility::string_t(U("/"))+ nextUri;
}


void handle_get(http_request req)
{
	auto answer = web::json::value::object();
	
	utility::string_t outValue;
	if (getValueFromRequestString(req, U("counter_value"), outValue))
	{
		int nReqIndex = _wtoi(outValue.c_str());

		auto it = counters_map.find(nReqIndex);
		if (it != counters_map.end())
		{
			utility::stringstream_t ss;
			ss << it->second;
			answer[U("result")] = web::json::value::string(utility::conversions::to_utf16string(ss.str()));

			print_message(true, GOOD_RESPONSE, answer.serialize());
			req.reply(status_codes::OK, answer);
		}
	}
	else
	{
		print_message(true, BAD_REQUEST, CAN_T_FOUND_REQ_VALUE);
		req.reply(status_codes::NotFound, answer);
	}
}

void handle_post(http_request req)
{
	web::json::value jsonBody;
	extract_json_from_body(req, jsonBody);

	if (jsonBody.at(U("creationFlag")).is_boolean())
	{
		bool creationFlag = jsonBody.at(U("creationFlag")).as_bool();
		size_t nMapSize = counters_map.size();

		counters_map.emplace(std::make_pair(++nMapSize, 0));
		const utility::string_t& strCounterIndx  = convert_to_string(std::prev(counters_map.end())->first);
		const utility::string_t& strCounterValue = convert_to_string(std::prev(counters_map.end())->second);
		
		web::json::value answer;
		answer[L"CounterIndx"]  = web::json::value::string(strCounterIndx);
		answer[L"CounterValue"] = web::json::value::string(strCounterValue);
		answer[L"Status"] = web::json::value::string(U("Successful"));

		print_message(true, GOOD_RESPONSE, answer.serialize());
		req.reply(status_codes::OK, answer);
	}
	else
	{
		print_message(true, BAD_REQUEST, CAN_T_FOUND_REQ_VALUE);
		req.reply(status_codes::BadRequest);
	}
}

void handle_put(const http_request& req) 
{
	web::json::value jsonBody;
	extract_json_from_body(req, jsonBody);

	if (jsonBody.at(U("puttingIndex")).is_integer() &&
		jsonBody.at(U("puttingValue")).is_integer())
	{
		size_t nPuttingIndex = jsonBody.at(U("puttingIndex")).as_integer();

		auto it = counters_map.find(nPuttingIndex);
		if (it == counters_map.end()) 
		{
			req.reply(status_codes::NotFound);
			return;
		}
		
		int32_t nPuttingValue = jsonBody.at(U("puttingValue")).as_integer();
		it->second = nPuttingValue;

		const utility::string_t& strCounterIndx = convert_to_string(std::prev(counters_map.end())->first);
		const utility::string_t& strCounterValue = convert_to_string(std::prev(counters_map.end())->second);

		auto answer = web::json::value::object();
		answer[L"CounterIndx"] = web::json::value::string(strCounterIndx);
		answer[L"CounterValue"] = web::json::value::string(strCounterValue);
		answer[L"Status"] = web::json::value::string(U("Successful"));

		print_message(true, GOOD_RESPONSE, answer.serialize());
		req.reply(status_codes::OK, answer);
	}
	else
	{
		print_message(true, BAD_REQUEST, CAN_T_FOUND_REQ_VALUE);
		req.reply(status_codes::BadRequest);
	}
}

void handle_del(const http_request& req) 
{
	web::json::value jsonBody;
	extract_json_from_body(req, jsonBody);

	if (jsonBody.at(U("removingIndex")).is_integer())
	{
		size_t nRemovingIndex = jsonBody.at(U("removingIndex")).as_integer();

		auto it = counters_map.find(nRemovingIndex);
		if (it == counters_map.end())
		{
			req.reply(status_codes::NotFound);
			return;
		}

		counters_map.erase(it);

		auto answer = web::json::value::object();
		answer[L"Status"] = web::json::value::string(U("Successful"));

		print_message(true, GOOD_RESPONSE, answer.serialize());
		req.reply(status_codes::OK, answer);
	}
	else
	{
		print_message(true, BAD_REQUEST, CAN_T_FOUND_REQ_VALUE);
		req.reply(status_codes::NotFound);
	}
}

int main(int argc, char* argv[])
{
	const utility::string_t& strConnection = make_connetion_string();
	http_listener listener(strConnection);
	listener.support(methods::GET, handle_get);
	listener.support(methods::POST, handle_post);
	listener.support(methods::PUT, handle_put);
	listener.support(methods::DEL, handle_del);

	try
	{
		listener.open().then([&listener, &strConnection]() {std::wcout << L"Starting listening: " << strConnection.c_str() << std::endl; }).wait();
		
		while (true);
	}
	catch (const std::exception& e )
	{
		std::wcout << e.what() << std::endl;
	}

	return 0;
}