using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ClientLibrary.Utils.Forms
{
    public static partial class Dialog
    {
        public static object InputBox(string title, string promptText, object defaultValue, bool disableInput)
        {
            DialogResult dialogResult;
            object resultValue = InputBox(title, promptText, defaultValue, disableInput, out dialogResult);

            if (dialogResult == DialogResult.OK)
                return resultValue;
            else return defaultValue;
        }

        public static object InputBox(string title, string promptText, object defaultValue, bool disableInput, out DialogResult dialogResult)
        {
            promptText = promptText != null ? promptText : "Input value : ";

            string[] promptTextList = new string[] { promptText };
            object[] defaultValueList = new object[] { defaultValue };
            bool[] disableInputList = new bool[] { disableInput };
            object[] resultValueList = InputBox(title, promptTextList, defaultValueList, disableInputList, out dialogResult);

            if (dialogResult == DialogResult.OK)
                return resultValueList[0];
            else return defaultValue;
        }

        public static object[] InputBox(string title, string[] promptText, object[] defaultValue, bool[] disableInput)
        {
            DialogResult dialogResult;
            object[] resultValue = InputBox(title, promptText, defaultValue, disableInput, out dialogResult);

            if (dialogResult == DialogResult.OK)
                return resultValue;
            else return defaultValue;
        }

        public static object[] InputBox(string title, string[] promptText, object[] defaultValue, bool[] disableInput, out DialogResult dialogResult)
        {
            int count = defaultValue != null ? defaultValue.Length : (defaultValue = new object[0]).Length;
            object[] resultValue = new List<object>(defaultValue).ToArray();

            promptText = zeroPadding<string>(promptText, null, count);
            disableInput = zeroPadding<bool>(disableInput, false, count);

            if (count > 0)
            {
                Form form = new Form() { Text = title };
                Button buttonOk = new Button();
                Button buttonCancel = new Button();

                List<Control> controls = new List<Control>();
                List<Label> label = new List<Label>();
                List<TextBox> textBox = new List<TextBox>();

                Rectangle bounds = new Rectangle(0, 0, 0, 0);

                List<string> tempPromptText = new List<string>(promptText);
                List<object> tempDefaultValue = new List<object>(defaultValue);
                List<bool> tempDisableInput = new List<bool>(disableInput);
                List<object> tempResultValue = new List<object>(resultValue);

                for (int i = 0; i < defaultValue.Length; i++)
                {
                    Label tempLabel = new Label();
                    TextBox tempTextBox = new TextBox();

                    promptText[i] = promptText[i] != null ? promptText[i] : "Input " + (i + 1) + "th " + defaultValue[i].GetType().ToString().Substring(defaultValue[i].GetType().ToString().LastIndexOf('.') + 1) + " value : ";

                    if (typeCheck(ref defaultValue[i], tempTextBox, CheckType.KeyEvent))
                    {
                        
                    }
                    else
                    {
                        tempPromptText.RemoveAt(i);
                        promptText = tempPromptText.ToArray();

                        tempDefaultValue.RemoveAt(i);
                        defaultValue = tempDefaultValue.ToArray();

                        tempDisableInput.RemoveAt(i);
                        disableInput = tempDisableInput.ToArray();

                        tempResultValue.RemoveAt(i--);
                        resultValue = tempResultValue.ToArray();
                        
                        continue;
                    }

                    if (i > 0)
                        bounds = textBox[i - 1].Bounds;

                    label.Add(tempLabel);
                    textBox.Add(tempTextBox);

                    label[i].Text = promptText[i];
                    textBox[i].Text = defaultValue[i].GetType().Equals(typeof(bool)) ? ((bool)defaultValue[i] ? "1" : "0") : defaultValue[i].ToString();

                    label[i].SetBounds(9, 20 + bounds.Bottom, 372, 13);
                    textBox[i].SetBounds(12, label[i].Bounds.Y + 16, 372, 20);

                    label[i].AutoSize = true;
                    textBox[i].Anchor = textBox[i].Anchor | AnchorStyles.Right;

                    textBox[i].Enabled = !tempDisableInput[i];
                }

                if (defaultValue.Length > 0)
                {
                    controls.AddRange(label);
                    controls.AddRange(textBox);
                    controls.Add(buttonOk);
                    controls.Add(buttonCancel);

                    buttonOk.Text = "OK";
                    buttonCancel.Text = "Cancel";
                    buttonOk.DialogResult = DialogResult.OK;
                    buttonCancel.DialogResult = DialogResult.Cancel;

                    bounds = textBox[textBox.Count - 1].Bounds;
                    buttonOk.SetBounds(228, bounds.Bottom + 16, 75, 23);
                    buttonCancel.SetBounds(309, bounds.Bottom + 16, 75, 23);

                    buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                    buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

                    form.ClientSize = new Size(396, buttonOk.Bounds.Bottom + 12);
                    form.Controls.AddRange(controls.ToArray());
                    form.ClientSize = new Size(Math.Max(300, label[0].Right + 10), form.ClientSize.Height);
                    form.FormBorderStyle = FormBorderStyle.FixedDialog;
                    form.StartPosition = FormStartPosition.CenterScreen;
                    form.MinimizeBox = false;
                    form.MaximizeBox = false;
                    form.AcceptButton = buttonOk;
                    form.CancelButton = buttonCancel;

                    dialogResult = form.ShowDialog();
                    if (dialogResult == DialogResult.OK)
                    {
                        for (int i = 0; i < defaultValue.Length; i++)
                            typeCheck(ref resultValue[i], textBox[i], CheckType.Assignment);

                        return resultValue;
                    }
                }
            }

            dialogResult = DialogResult.Cancel;
            return defaultValue;
        }

        private static T[] zeroPadding<T>(T[] origin, T zero, int count)
        {
            if (origin == null || origin.Length < count)
            {
                List<T> temp = new List<T>();
                while (temp.Count < count)
                    temp.Add(zero);

                return temp.ToArray();
            }
            else return origin;
        }
    }
}
