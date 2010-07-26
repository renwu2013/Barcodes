﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Barcode_Writer
{
    /// <summary>
    /// Codabar generator
    /// </summary>
    public class Codabar : BarcodeBase
    {
        public const string LIMITVALUEA = "A";
        public const string LIMITVALUEB = "B";
        public const string LIMITVALUEC = "C";
        public const string LIMITVALUED = "D";

        private const string _Extras = ":/.+abcdent*";
        
        protected override void Init()
        {
            PatternSet = new Dictionary<int, Pattern>();

            PatternSet.Add('0', Pattern.Parse("nb nw nb nw nb ww wb"));
            PatternSet.Add('1', Pattern.Parse("nb nw nb nw wb ww nb"));
            PatternSet.Add('2', Pattern.Parse("nb nw nb ww nb nw wb"));
            PatternSet.Add('3', Pattern.Parse("wb ww nb nw nb nw nb"));
            PatternSet.Add('4', Pattern.Parse("nb nw wb nw nb ww nb"));
            PatternSet.Add('5', Pattern.Parse("wb nw nb nw nb ww nb"));
            PatternSet.Add('6', Pattern.Parse("nb ww nb nw nb nw wb"));
            PatternSet.Add('7', Pattern.Parse("nb ww nb nw wb nw nb"));
            PatternSet.Add('8', Pattern.Parse("nb ww wb nw nb nw nb"));
            PatternSet.Add('9', Pattern.Parse("wb nw nb ww nb nw nb"));

            PatternSet.Add('-', Pattern.Parse("nb nw nb ww wb nw nb"));
            PatternSet.Add('$', Pattern.Parse("nb nw wb ww nb nw nb"));
            PatternSet.Add(':', Pattern.Parse("wb nw nb nw wb nw wb"));
            PatternSet.Add('/', Pattern.Parse("wb nw wb nw nb nw wb"));
            PatternSet.Add('.', Pattern.Parse("wb nw wb nw wb nw nb"));
            PatternSet.Add('+', Pattern.Parse("nb nw wb nw wb nw wb"));

            PatternSet.Add('a', Pattern.Parse("nb nw wb ww nb ww nb"));
            PatternSet.Add('b', Pattern.Parse("nb ww nb ww nb nw wb"));
            PatternSet.Add('c', Pattern.Parse("nb nw nb ww nb ww wb"));
            PatternSet.Add('d', Pattern.Parse("nb nw nb ww wb ww nb"));
            PatternSet.Add('t', Pattern.Parse("nb nw wb ww nb ww nb"));
            PatternSet.Add('n', Pattern.Parse("nb ww nb ww nb nw wb"));
            PatternSet.Add('*', Pattern.Parse("nb nw nb ww nb ww wb"));
            PatternSet.Add('e', Pattern.Parse("nb nw nb ww wb ww nb"));

            AllowedCharsPattern = new System.Text.RegularExpressions.Regex(@"^[atbnc\*de][\d-$:/\.\+]+[atbnc\*de]$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        protected override string ParseText(string value, CodedValueCollection codes)
        {
            value = value.ToLower();
            string tmp = value.Replace(" ", "");

            tmp = base.ParseText(tmp, codes);

            foreach (char item in tmp.ToCharArray())
            {
                codes.Add(item);
            }

            return value.Substring(1, value.Length - 2);
        }

        protected override int GetModuleWidth(BarcodeSettings settings)
        {
            return (2 * settings.WideWidth) + (5 * settings.NarrowWidth);
        }

        protected override int OnCalculateWidth(int width, BarcodeSettings settings, CodedValueCollection codes)
        {
            width += (settings.ModulePadding * (codes.Count - 1));

            int count = (from t in codes where _Extras.IndexOf((char)t) > -1 select t).Count();
            return width + (count * settings.WideWidth) - (count * settings.NarrowWidth);
        }

        protected override void OnAfterDrawModule(State state, int index)
        {
            if (_Extras.Contains(state.ModuleValue))
                state.Left += GetModuleWidth(state.Settings) + state.Settings.WideWidth - state.Settings.NarrowWidth;
            else
                base.OnAfterDrawModule(state, index);

            state.Left += state.Settings.ModulePadding;
        }

        public static void AddChecksumEventHandler(object sender, AddChecksumEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(e.Text, "^\\d+$"))
                throw new ArgumentException("Only numeric values can have a check digit");

            int total = 0;

            for (int i = 0; i < e.Text.Length; i++)
            {
                if (i % 2 == 0)
                    total += int.Parse(e.Text.Substring(i, 1));
                else
                {
                    int tmp = int.Parse(e.Text.Substring(i, 1)) * 2;
                    total += (tmp % 9);
                }
            }

            total = total % 10;

            e.Text += total.ToString();
            e.Codes.Add(total.ToString()[0]);
        }
    }
}
