Param(
	[switch]
	$rebuild = $false)

if ($rebuild)
{
	msbuild build/Build.proj /p:Configuration=Release,ShouldRebuild=true /t:Package
	https://ci.appveyor.com/project/Daniel15/routejs/build/70} else
{
	msbuild build/Build.proj /p:Configuration=Release /t:Package
}