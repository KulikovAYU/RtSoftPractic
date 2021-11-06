#pragma once
#include <cpprest\http_client.h>
#include <cpprest\base_uri.h>
#include <set>
#include <string>


class RepoFactory
{
public:
	static std::set<std::wstring> CreateReposDefs();
};


class PatternsFactory
{
public:
	static std::set<std::wstring> CreatePatternsDefs();
};


class GithubRepoQuery
{
public:
	explicit GithubRepoQuery(const std::wstring& strRepoFullPath) : m_strRepoFullPath(strRepoFullPath)
	{}


	bool createUriToReadmeContentRequest(web::http::http_request& request) const;

private:
	[[nodiscard]] web::uri createUriToReadmeContentRequest() const;
	[[nodiscard]] std::wstring createToReadmeContentRequest() const;
	std::wstring getRepoName() const;


	std::wstring m_strRepoFullPath;
};

class GithubPatternsParser
{
public:
	static void ScanRepositories( const std::set<std::wstring>& githubReposPaths, const std::set<std::wstring>& strReqPatterns);

	static void ScanRepository( const std::wstring& githubRepoPath, const std::set<std::wstring>& strReqPatterns);

private:
	static std::set<std::wstring> CheckReqPatternsInProjectsReadme(const std::wstring& githubRepoPath, const std::set<std::wstring>& strReqPatterns);

	static void PrintPatternsName(const std::set<std::wstring>& patterns);
};