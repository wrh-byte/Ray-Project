
from pdf2docx import Converter


def main(pdf_file, docx_file):
    cv = Converter(pdf_file)
    cv.convert(docx_file)
    cv.close()


if __name__ == "__main__":
    source_path = 'source\\MB-210_237Q.pdf'
    target_path = 'target\\MB-210.docx'
    main(source_path, target_path)
