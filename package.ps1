Param(
	[switch]
	$rebuild = $false)

if ($rebuild)
{
	msbuild build/Build.proj /p:Configuration=Release,ShouldRebuild=true /t:Package
} else
{
	msbuild build/Build.proj /p:Configuration=Release /t:Package
}