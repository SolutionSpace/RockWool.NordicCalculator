using MigraDoc.DocumentObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace REDAirCalculator.Utilities
{
    static class HtmlPdfHelper
    {
        public static void HtmlToParagraph(Paragraph paragraph, string html)
        {
            string pattern = @"<p>.*<\/p>";
            Regex rg = new Regex(pattern);


            MatchCollection matchedAuthors = rg.Matches("<p>" + html + "</p>");


            string pattern2 = @"(<.{2,6}?><.{2,6}?><.{2,6}?>.*?<\/.{2,6}?><\/.{2,6}?><\/.{2,6}?>)|(<.{2,6}?><.{2,6}?>.*?<\/.{2,6}?><\/.{2,6}?>)|(.*?<*?>)";
            Regex rg2 = new Regex(pattern2);


            List<Tuple<string, TextFormat>> types = new List<Tuple<string, TextFormat>>() {
                new Tuple<string, TextFormat>("strong", TextFormat.Bold),
                new Tuple<string, TextFormat>("em", TextFormat.Italic),
                new Tuple<string, TextFormat>("ins", TextFormat.Underline)
            };

            foreach (var matched in matchedAuthors.Cast<Match>().ToList())
            {
                var value = matched.Value;

                FormattedText formattedText = new FormattedText();

                MatchCollection matchedAuthors2 = rg2.Matches(value);

                foreach (var m in matchedAuthors2.Cast<Match>().ToList())
                {
                
                    int k = 0;
                    bool isStyled = false;
                    foreach (var t in types)
                    {
                        k++;
                        if (m.Value.Contains($"</{t.Item1}>"))
                        {
                            var row = Regex.Replace(m.Value, "<.*?>", string.Empty);
                            row = Regex.Replace(row, "&lt;", "<");
                            row = Regex.Replace(row, "&gt;", ">");
                            if (!isStyled)
                            {
                                formattedText = paragraph.AddFormattedText(Regex.Replace(row, "&nbsp;", " "));
                            }

                            if (t.Item1 == "em")
                                formattedText.Italic = true;
                            else if (t.Item1 == "strong")
                                formattedText.Bold = true;
                            else if (t.Item1 == "ins")
                                formattedText.Underline = Underline.Single;


                            isStyled = true;
                        }
                        else if (k == types.Count() && !isStyled)
                        {
                            var row = Regex.Replace(m.Value, "<.*?>", string.Empty);
                            paragraph.AddFormattedText(Regex.Replace(row, "&nbsp;", " "));
                        
                        }
                    }

                }

                paragraph.AddFormattedText("\n");
            }

        }

    }
}