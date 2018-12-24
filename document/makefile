LATEX := uplatex
MAIN := document

main.pdf: $(MAIN).tex preamble.tex definitions.tex
	$(LATEX) $(MAIN).tex
	$(LATEX) $(MAIN).tex
	$(LATEX) $(MAIN).tex

.PHONY: clean
clean:
	- rm *.aux *.log *.pdf *.dvi
