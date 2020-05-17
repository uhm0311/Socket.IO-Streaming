using System;
using System.Windows.Forms;

namespace ClientLibrary.Utils.Forms
{
    public static partial class Dialog
    {
        private enum CheckType { KeyEvent, Assignment };

        private static bool typeCheck(ref object value, TextBox textBox, CheckType type)
        {
            if (value != null)
            {
                KeyPressEventHandler handler = null;
                object temp = null;

                if (value.GetType().Equals(typeof(string)))
                {
                    temp = textBox.Text;
                }
                else if (value.GetType().Equals(typeof(char)))
                {
                    char result = '\0';
                    char.TryParse(textBox.Text, out result);
                    temp = result;

                    handler = (sender, e) => { (sender as TextBox).MaxLength = 1; };
                }
                else
                {
                    if (value.GetType().Equals(typeof(bool)))
                    {
                        bool result = false;
                        bool.TryParse(textBox.Text, out result);
                        temp = result;

                        handler = (sender, e) => allowRange(sender as TextBox, ref e, 0, 1);
                    }

                    else if (value.GetType().Equals(typeof(sbyte)))
                    {
                        sbyte result = 0;
                        sbyte.TryParse(textBox.Text, out result);
                        temp = result;

                        handler = (sender, e) => allowRange(sender as TextBox, ref e, sbyte.MinValue, sbyte.MaxValue, true);
                    }
                    else if (value.GetType().Equals(typeof(byte)))
                    {
                        byte result = 0;
                        byte.TryParse(textBox.Text, out result);
                        temp = result;

                        handler = (sender, e) => allowRange(sender as TextBox, ref e, byte.MinValue, byte.MaxValue, true);
                    }

                    else if (value.GetType().Equals(typeof(short)))
                    {
                        short result = 0;
                        short.TryParse(textBox.Text, out result);
                        temp = result;

                        handler = (sender, e) => allowRange(sender as TextBox, ref e, short.MinValue, short.MaxValue, true);
                    }
                    else if (value.GetType().Equals(typeof(ushort)))
                    {
                        ushort result = 0;
                        ushort.TryParse(textBox.Text, out result);
                        temp = result;

                        handler = (sender, e) => allowRange(sender as TextBox, ref e, ushort.MinValue, ushort.MaxValue, true);
                    }

                    else if (value.GetType().Equals(typeof(int)))
                    {
                        int result = 0;
                        int.TryParse(textBox.Text, out result);
                        temp = result;

                        handler = (sender, e) => allowRange(sender as TextBox, ref e, int.MinValue, int.MaxValue, true);
                    }
                    else if (value.GetType().Equals(typeof(uint)))
                    {
                        uint result = 0;
                        uint.TryParse(textBox.Text, out result);
                        temp = result;

                        handler = (sender, e) => allowRange(sender as TextBox, ref e, uint.MinValue, uint.MaxValue, true);
                    }

                    else if (value.GetType().Equals(typeof(long)))
                    {
                        long result = 0;
                        long.TryParse(textBox.Text, out result);
                        temp = result;

                        handler = (sender, e) => allowRange(sender as TextBox, ref e, long.MinValue, long.MaxValue, true);
                    }
                    else if (value.GetType().Equals(typeof(ulong)))
                    {
                        ulong result = 0;
                        ulong.TryParse(textBox.Text, out result);
                        temp = result;

                        handler = (sender, e) => allowRange(sender as TextBox, ref e, ulong.MinValue, ulong.MaxValue, true);
                    }

                    else if (value.GetType().Equals(typeof(float)))
                    {
                        float result = 0;
                        float.TryParse(textBox.Text, out result);
                        temp = result;

                        handler = (sender, e) => allowRange(sender as TextBox, ref e, float.MinValue, float.MaxValue, true, true);
                    }
                    else if (value.GetType().Equals(typeof(double)))
                    {
                        double result = 0;
                        double.TryParse(textBox.Text, out result);
                        temp = result;

                        handler = (sender, e) => allowRange(sender as TextBox, ref e, double.MinValue, double.MaxValue, true, true);
                    }
                    else return false;
                }

                if (type == CheckType.Assignment)
                    value = temp;
                
                else if (type == CheckType.KeyEvent)
                    textBox.KeyPress += handler;
                
            }

            return true;
        }

        private static void allowedNumericAndAdditionalCharacters(TextBox textBox, ref KeyPressEventArgs e, bool allowNegative, bool allowDecimalPoint)
        {
            char negative = '-';
            char decimalPoint = '.';

            bool allowed = char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar);

            if (allowNegative)
                allowed = allowed || ((e.KeyChar == negative) && (textBox.Text.IndexOf(negative) < 0) && (textBox.SelectionStart == 0));

            if (allowDecimalPoint)
                allowed = allowed || ((e.KeyChar == decimalPoint) && (textBox.Text.IndexOf(decimalPoint) < 0));

            if (!allowed)
                e.Handled = true;
        }

        private static void allowRange(TextBox textBox, ref KeyPressEventArgs e, double minValue, double maxValue, bool allowNegative = false, bool allowDecimalPoint = false)
        {
            textBox.MaxLength = Math.Max(minValue.ToString().Length, maxValue.ToString().Length);
            allowedNumericAndAdditionalCharacters(textBox, ref e, allowNegative, allowDecimalPoint);

            double temp = parseNumber(textBox, e);

            if (temp < minValue || temp > maxValue)
                e.Handled = true;
        }

        private static double parseNumber(TextBox textBox, KeyPressEventArgs e)
        {
            double number = double.NaN;

            if (!char.IsControl(e.KeyChar) && !e.Handled)
            {
                string text = string.Empty;
                bool isBlocked = textBox.SelectionLength > 0;

                text = textBox.Text.Substring(0, textBox.SelectionStart) + textBox.Text.Substring(textBox.SelectionStart + textBox.SelectionLength);
                text = text.Substring(0, textBox.SelectionStart) + e.KeyChar + text.Substring(textBox.SelectionStart);

                if (text.Length <= textBox.MaxLength)
                {
                    double.TryParse(text, out number);
                    return number;
                }
            }
            
            double.TryParse(textBox.Text, out number);
            return number;
        }
    }
}
