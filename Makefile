MDTOOL ?= /Applications/Xamarin\ Studio.app/Contents/MacOS/mdtool

.PHONY: all clean

all: ReactiveUI.dll

ReactiveUI.dll:
	/usr/bin/env mono ./.nuget/NuGet.exe restore ./ReactiveUI_XSAll.sln
	$(MDTOOL) build -c:Release ./ReactiveUI_XSAll.sln

clean:
	$(MDTOOL) build -t:Clean ./ReactiveUI_XSAll.sln
