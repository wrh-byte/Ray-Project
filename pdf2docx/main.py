
from pdf2docx import Converter


def main(pdf_file, docx_file):
    cv = Converter(pdf_file)
    cv.convert(docx_file)
    cv.close()


if __name__ == "__main__":
    source_path = 'source\\MS-900_198Q.pdf'
    target_path = 'target\\MS-900.docx'
    main(source_path, target_path)
