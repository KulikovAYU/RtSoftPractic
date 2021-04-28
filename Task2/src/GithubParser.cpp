#include "GithubParser.h"
#include <cpprest\json.h>
#include <cpprest\uri_builder.h>
#include <algorithm>
#include <string>
#include "base64_decoder.h"
#ifdef DEBUG
#include <cstdlib>
#endif

using namespace web;
using namespace web::http;

#define NOT_FOUND -1
#define JSON_PRINT_RESPONSE_ON 1

void display_json(json::value const& jValue, utility::string_t const& prefix)
{
	std::wcout << prefix << jValue.serialize() << std::endl;
}

std::wstring toUpper(std::wstring srcString)
{
	std::transform(srcString.begin(), srcString.end(), srcString.begin(), toupper);
	return srcString;
}

std::set<std::wstring> GithubPatternsParser::CheckReqPatternsInProjectsReadme(const std::wstring& githubRepoPath, const std::set<std::wstring>& strReqPatterns)
{
	std::set<std::wstring> outPatterns;
	
	if (!web::uri::validate(githubRepoPath))
		return outPatterns;
	
	GithubRepoQuery query(githubRepoPath);
	http_request req;
	if (!query.createUriToReadmeContentRequest(req))
		return outPatterns;

	web::http::client::http_client m_httpClient(L"https://api.github.com");
	m_httpClient.request(req).then([](http_response response)
		{
			auto sk = response.status_code();
			if (response.status_code() == status_codes::OK)
			{
				return response.extract_json();
			}
			return pplx::task_from_result(json::value());
		}).then([&outPatterns, &strReqPatterns](pplx::task<json::value> previousTask)
			{
				try
				{
					const json::value& jValue = previousTask.get();
					if (!jValue.has_field(U("content")))
						return;

					#ifndef JSON_PRINT_RESPONSE_ON
					display_json(previousTask.get(), L"R: ");
					//clear the console
					system("cls");
					#endif // DEBUG

				
					const auto& content = jValue.at(U("content")).as_string();
					const std::wstring& strComment = Base64::get_string_from_base64_format(content);
					
					const std::wstring& strUpperComment = toUpper(strComment);

					std::copy_if(strReqPatterns.begin(), strReqPatterns.end(), std::inserter(outPatterns, outPatterns.begin()),
						[&strUpperComment](const std::wstring& key_value) {
							return strUpperComment.find(toUpper(key_value)) != NOT_FOUND;
						});
				}
				catch (http_exception const& e)
				{
					std::wcout << e.what() << std::endl;
				}
			}). wait();


	return outPatterns;
}

void GithubPatternsParser::ScanRepositories(const std::set<std::wstring>& githubReposPaths, const std::set<std::wstring>& strReqPatterns)
{
	for (const std::wstring& githubRepoPath : githubReposPaths)
		ScanRepository(githubRepoPath, strReqPatterns);
}

void GithubPatternsParser::ScanRepository(const std::wstring& githubRepoPath, const std::set<std::wstring>& strReqPatterns)
{
	std::wcout << L"Repo path: " << githubRepoPath << std::endl << L"Founded patterns: " << std::endl;

	const std::set<std::wstring>& patterns = CheckReqPatternsInProjectsReadme(githubRepoPath, strReqPatterns);
	PrintPatternsName(patterns);
	std::wcout << std::endl << std::endl;
}

void GithubPatternsParser::PrintPatternsName(const std::set<std::wstring>& patterns)
{
	for (const std::wstring& pattern : patterns)
		std::wcout << pattern << std::endl;
}

std::set<std::wstring> PatternsFactory::CreatePatternsDefs()
{
	return {
	L"Composite",
	L"Template Method",
	L"Abstract factory",
	L"Singleton",
	L"Visitor",
	L"Builder",
	L"Proxy",
	L"Pimpl"
	};
}

web::uri GithubRepoQuery::createUriToReadmeContentRequest() const
{
	web::uri_builder builder;
	const std::wstring& request = createToReadmeContentRequest();
	builder.append_path(request);
	return builder.to_uri();
}

bool GithubRepoQuery::createUriToReadmeContentRequest(web::http::http_request& request) const
{
	request.set_method(methods::GET);
	request.headers().set_content_type(L"application/vnd.github.v3+json");

	const web::uri& uri = createUriToReadmeContentRequest();
	if (!web::uri::validate(uri.to_string()))
		return false;

	request.set_request_uri(uri);

	return true;
}

std::wstring GithubRepoQuery::createToReadmeContentRequest() const
{
	return std::wstring(L"repos") + getRepoName() + std::wstring(L"/readme");
}

std::wstring GithubRepoQuery::getRepoName() const
{
	std::wstring githubString(L"https://github.com");
	return	m_strRepoFullPath.substr(githubString.length());
}

std::set<std::wstring> RepoFactory::CreateReposDefs()
{
	return{ L"https://github.com/ehsangazar/design-patterns-cpp",
			L"https://github.com/Junzhuodu/design-patterns",
			L"https://github.com/stefanmiletic-pmf/design_patterns_1"
	};
}