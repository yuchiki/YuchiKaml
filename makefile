.PHONY: clean

YuchikiML:
	$(eval RID := $(shell ./GetRid.sh))
	dotnet publish -r $(RID)
	cp -r ./Out/netcoreapp2.1/$(RID) ./YuchikiML_build

clean:
	- dotnet clean
	- rm -rf Out YuchikiML_build obj bin
