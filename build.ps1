Param(
	[switch]
	$rebuild = $false)

if ($rebuild)
{
	msbuild build/Build.proj /p:Configuration=Release,ShouldRebuild=true
} else
{
	msbuild build/Build.proj /p:Configuration=Release
}