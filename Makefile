M="Default message"

.PHONY: co ci

co:
	git checkout master
	git pull
	git checkout local
	git merge master

ci:
	git checkout local
	git commit -a -m "$(M)"	|| exit 0
	git checkout master
	git merge local
	git push

