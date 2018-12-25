LATEX := uplatex
DVIPDF := dvipdfmx
MAIN := document

$(MAIN).pdf: $(MAIN).dvi
	$(DVIPDF) $(MAIN).dvi

$(MAIN).dvi: $(MAIN).tex preamble.tex definitions.tex
	$(LATEX) $(MAIN).tex
	$(LATEX) $(MAIN).tex
	$(LATEX) $(MAIN).tex

.PHONY: clean
clean:
	- rm *.aux *.log *.pdf *.dvi *.toc
