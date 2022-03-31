using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Aspose.Words;

namespace LIMS_API.Blls.CommonBlls
{
    /// <summary>
    /// 化学式、单位替换
    /// </summary>
    public class FormulaHelper
    {
        private readonly Document _doc;
        private readonly Regex _paragraphRegex = new Regex(@"(?<=\@).*?(?=\#)");

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="document"></param>
        public FormulaHelper(Document document)
        {
            this._doc = document;
        }
        
        /// <summary>
        /// 替换文档中出现的公式
        /// </summary>
        public void ReplaceTheFormula()
        { 
            foreach (Paragraph paragraph in _doc.GetChildNodes(NodeType.Paragraph, true))
            {

                string pText = paragraph.GetText();
                //匹配格式
                MatchCollection pm = _paragraphRegex.Matches(pText);
                if (pm.Count > 0)
                {
                    if (pText.EndsWith('\a'))
                    {
                        pText = pText.TrimEnd('\a');
                    }
                    DealwithParagraph(paragraph, pText);
                }
            }
        }


        /// <summary>
        /// 处理Paragraph
        /// </summary>
        /// <param name="p"></param>
        /// <param name="str"></param>
        private void DealwithParagraph(Paragraph p,string str)
        {
            //清空段落
            RunCollection runs = p.Runs;
            bool isBold = false;
            if (p.Runs.Count>0)
            {
                isBold = p.Runs[0].Font.Bold;
            }
            foreach (var run in runs)
            {
                p.RemoveChild(run);
            }
            //ex: "我的物质1@CaCO_{3}#BqwrAa@SO_{4}^{2-}#w123w@NH_{4}#@CH_{4}#B"
            //从段落中判断字符串的位置信息
            Regex regWithChar = new Regex(@"\@.*?\#");
            MatchCollection matchCollectionWithChar = regWithChar.Matches(str);
            //需要替换位置字符的位置和长度
            Dictionary<int, ChemicalFormulaRange> dic = new Dictionary<int, ChemicalFormulaRange>();
            foreach (Match match in matchCollectionWithChar)
            {
                ChemicalFormulaRange cRange = new ChemicalFormulaRange();
                Regex reg = new Regex(@"(?<=\@).*?(?=\#)");
                Match chMatch = reg.Match(match.Value);
                cRange.chemicalFormula = chMatch.Value;
                cRange.length = match.Length;
                dic.Add(match.Index, cRange);
            }

            //遍历段落
            for (int i = 0; i < str.Length; i++)
            {
                if (dic.ContainsKey(i))
                {
                    var cRange = new ChemicalFormulaRange();
                    dic.TryGetValue(i, out cRange);
                    //处理公式
                    if (cRange!=null)
                    {
                        CreateFormula(p, cRange.chemicalFormula, isBold);
                        //跳转到公式之后的位置
                        int length = cRange.length;
                        i = i + length - 1;
                    }
                }
                else
                {
                    Run run = CreateRun(str[i].ToString(), isBold);
                    p.AppendChild(run);
                }
            }
        }

        /// <summary>
        /// 创建公式
        /// </summary>
        /// <param name="paragraph"></param>
        /// <param name="chemicalFormula"></param>
        /// <param name="isBold">字体是否加粗</param>
        private void CreateFormula(Paragraph paragraph, string chemicalFormula, bool isBold)
        {
            Dictionary<int, ChemicalFormulaRange> dic = new Dictionary<int, ChemicalFormulaRange>();
            Regex superRegex = new Regex(@"\^{.*?}");
            Regex superScriptRegex = new Regex(@"(?<=\^{).*?(?=})");
            MapperIndex("super", chemicalFormula, superRegex, superScriptRegex, ref dic);
            Regex subRegex = new Regex(@"_{.*?}");
            Regex subRegexScriptRegex = new Regex(@"(?<=_{).*?(?=})");
            MapperIndex("sub", chemicalFormula, subRegex, subRegexScriptRegex, ref dic);

            for (int i = 0; i < chemicalFormula.Length; i++)
            {
                if (dic.ContainsKey(i))
                {
                    ChemicalFormulaRange cRange = new ChemicalFormulaRange();
                    dic.TryGetValue(i, out cRange);
                    if (cRange!=null)
                    {
                        Run run = CreateRun(cRange.chemicalFormula, isBold, cRange.type);
                        paragraph.AppendChild(run);
                        i = i + cRange.length - 1;
                    }
                }
                else
                {
                    Run run = CreateRun(chemicalFormula[i].ToString(), isBold);
                    paragraph.AppendChild(run);
                }
            }
        }

        /// <summary>
        /// 创建上下标信息组字典
        /// </summary>
        /// <param name="scriptType"></param>
        /// <param name="chemicalFormula"></param>
        /// <param name="formulaRegex"></param>
        /// <param name="scriptRegex"></param>
        /// <param name="dic"></param>
        private void MapperIndex(string scriptType, string chemicalFormula, Regex formulaRegex, Regex scriptRegex, ref Dictionary<int, ChemicalFormulaRange> dic)
        {
            MatchCollection matchCollection = formulaRegex.Matches(chemicalFormula);
            foreach (Match match in matchCollection)
            {
                ChemicalFormulaRange cfRange = new ChemicalFormulaRange
                {
                    length = match.Length,
                    type = scriptType
                };
                Match chMatch = scriptRegex.Match(match.Value);
                cfRange.chemicalFormula = chMatch.Value;
                dic.Add(match.Index, cfRange);
            }
        }

        /// <summary>
        /// 创建Run
        /// </summary>
        /// <param name="str"></param>
        /// <param name="isBold"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private Run CreateRun(string str, bool isBold, string type = null)
        {
            Run run = new Run(_doc)
            {
                Text = str
            };
            if (type == "sub")
            {
                run.Font.Subscript = true;
            }
            else if (type == "super")
            {
                run.Font.Superscript = true;
            }
            run.Font.Bold = isBold;
            return run;
        }

        /// <summary>
        /// 替换位置信息组
        /// </summary>
        private class ChemicalFormulaRange
        {
            public int length { get; set; }
            public string chemicalFormula { get; set; }
            public string type { get; set; }
        }
    }
}
