#!/bin/bash

dotnet tool restore

echo "================================="
echo "Cleanup"
echo "================================="
cleanup -y
rm ./Artifacts -R

echo "================================="
echo "Build Debug"
echo "================================="

dotnet build ./Xamarin.BetterNavigation.UnitTests/Xamarin.BetterNavigation.UnitTests.csproj -c Debug

echo "================================="
echo "Test"
echo "================================="
testProjectPath="./Xamarin.BetterNavigation.UnitTests/Xamarin.BetterNavigation.UnitTests.csproj"
dllPath="./Xamarin.BetterNavigation.UnitTests/bin/Debug/netcoreapp2.0/Xamarin.BetterNavigation.UnitTests.dll"

dotnet coverlet $dllPath --target "dotnet" --targetargs "test $testProjectPath --no-build" --format opencover --include "[Xamarin.BetterNavigation.Core]*" --include "[Xamarin.BetterNavigation.Forms]*" --output "./Artifacts/coverage.xml"
dotnet reportgenerator "-reports:./Artifacts/coverage.xml" "-targetdir:./Artifacts/CoverageReport"

echo "================================="
echo "> Mutation tests"
echo "================================="
cd Xamarin.BetterNavigation.UnitTests
dotnet stryker --project-file "Xamarin.BetterNavigation.Forms.csproj" --reporters "['html', 'Dots']"
cd ..

echo "================================="
echo "Build Release"
echo "================================="

dotnet build ./Xamarin.BetterNavigation.Forms/Xamarin.BetterNavigation.Forms.csproj -c Release

echo "================================="
echo "Nuget Pack"
echo "================================="
version=`dotnet-gitversion -showvariable NuGetVersion`

coreNuspec=./Xamarin.BetterNavigation.Core/Xamarin.BetterNavigation.Core.nuspec
coreBackupNuspec=./Xamarin.BetterNavigation.Core/Xamarin.BetterNavigation.Core.bak.nuspec
formsNuspec=./Xamarin.BetterNavigation.Forms/Xamarin.BetterNavigation.Forms.nuspec
formsBackupNuspec=./Xamarin.BetterNavigation.Forms/Xamarin.BetterNavigation.Forms.bak.nuspec

cp $formsNuspec $formsBackupNuspec
cp $coreNuspec $coreBackupNuspec

perl -p -i -e "s/\@version/$version/g" "$formsBackupNuspec"
perl -p -i -e "s/\@version/$version/g" "$coreBackupNuspec"
rm $formsBackupNuspec.bak
rm $coreBackupNuspec.bak

nuget pack $coreBackupNuspec -Version $version -OutputDirectory ./Artifacts -BasePath .
nuget pack $formsBackupNuspec -Version $version -OutputDirectory ./Artifacts -BasePath .

rm $formsBackupNuspec
rm $coreBackupNuspec

dotnet build-server shutdown
exit 0