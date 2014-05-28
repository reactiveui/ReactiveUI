MDTOOL ?= /Applications/Xamarin\ Studio.app/Contents/MacOS/mdtool

.PHONY: all clean

all: ReactiveUI.dll

ReactiveUI.dll:
	$(MDTOOL) build -c:Release ./ReactiveUI_XSAll.sln

clean:
	$(MDTOOL) build -t:Clean ./ReactiveUI_XSAll.sln
