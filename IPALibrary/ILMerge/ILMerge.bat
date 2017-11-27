set binaryPath=%CD%\..\bin\Release
set outputPath=%CD%\..\bin\ILMerge
IF NOT EXIST "%outputPath%" MKDIR "%outputPath%"
IF ["%programfiles(x86)%"]==[""] SET ilmergePath="%programfiles%\Microsoft\ILMerge"
IF NOT ["%programfiles(x86)%"]==[""] SET ilmergePath="%programfiles(x86)%\Microsoft\ILMerge"
%ilmergePath%\ilmerge /ndebug /target:library /out:"%outputPath%\IPALibrary.dll" /internalize "%binaryPath%\IPALibrary.dll" "%binaryPath%\ICSharpCode.SharpZipLib.dll" "%binaryPath%\PListNet.dll" "%binaryPath%\Utilities.dll"