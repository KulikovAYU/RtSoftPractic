#include "GithubParser.h"


int main(int argc, char* argv[])
{
	setlocale(LC_ALL, "Ru");

	const std::set<std::wstring>& reposDefs = RepoFactory::CreateReposDefs();
	const std::set<std::wstring>& patternsDefs = PatternsFactory::CreatePatternsDefs();

	GithubPatternsParser::ScanRepositories(reposDefs, patternsDefs);

	return 0;
}