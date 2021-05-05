using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

/// <summary>
/// 
/// </summary>
namespace mkbin_mw145
{
    /// <summary>
    /// 
    /// </summary>
    abstract class PrinterCommon
    {
        /// <summary>
        /// 
        /// </summary>
        protected BinaryWriter binaryWriter;

        /// <summary>
        /// 
        /// </summary>
        protected StreamReader fileReader;

        /// <summary>
        /// 実行ステート区分
        /// </summary>
        protected enum State
        {
            PASS_THROUGH,   // パススルー部
            COMMAND,        // コマンド部
            PARAMATERS,     // パラメーター部
        }

        /// <summary>
        /// 実行ステート
        /// </summary>
        protected State execState = State.PASS_THROUGH;

        /// <summary>
        /// バックスラッシュ状態
        /// </summary>
        protected bool aEscapeState = false;

        /// <summary>
        /// コマンド文字列保持
        /// </summary>
        protected string aCommandStr = "";

        /// <summary>
        /// パラメータ文字列配列保持
        /// </summary>
        protected List<string> aParamStrs;

        /// <summary>
        /// パラメータ文字列保持
        /// </summary>
        protected string aParamStr;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="inputFileName"></param>
        /// <param name="outputFileName"></param>
        public PrinterCommon(string inputFileName, string outputFileName)
        {
            fileReader = new StreamReader(inputFileName);
            binaryWriter = new BinaryWriter(File.Open(outputFileName, FileMode.Create));
        }

        /// <summary>
        /// デストラクタ
        /// </summary>
        ~PrinterCommon()
        {
            if (binaryWriter != null)
            {
                binaryWriter.Close();
                binaryWriter = null;
            }
            if (fileReader != null)
            {
                fileReader.Close();
                fileReader = null;
            }
        }

        /// <summary>
        /// 一文字ずつ処理する
        /// </summary>
        /// <returns></returns>
        public void Exec()
        {
            int char1;
            while ((char1 = fileReader.Read()) != -1)
            {
                switch (execState)
                {
                    case State.PASS_THROUGH:    // パススルー状態
                        if (char1 == '\\')
                        {
                            // コマンド部開始（またはバックスラッシュ）
                            aEscapeState = true;
                            execState = State.COMMAND;
                        }
                        else
                        {
                            // 一文字出力
                            EmitChar((char)char1);
                        }
                        break;

                    case State.COMMAND:
                        if (char1 == '\\' && aEscapeState == true)
                        {
                            // バックスラッシュ（コマンド部の開始ではない）
                            execState = State.PASS_THROUGH;
                            binaryWriter.Write('\\');
                        }
                        else if (char1 == ';')
                        {
                            // コマンド部終了
                            aParamStrs.Clear();
                            ExecCommand();
                            execState = State.PASS_THROUGH;
                        }
                        else if (char1 == '(')
                        {
                            // コマンド・パラメータ部開始
                            execState = State.PARAMATERS;
                            aParamStrs.Clear();
                            aParamStr = "";
                        }
                        else
                        {
                            // コマンド文字列に追加
                            aCommandStr += char1;
                        }
                        aEscapeState = false;
                        break;

                    case State.PARAMATERS:
                        if (char1 == ',')
                        {
                            // コマンド・パラメータ部、次パラメータ開始
                            aParamStrs.Add(aParamStr);
                            aParamStr = "";
                        }
                        else if (char1 == ')')
                        {
                            // コマンド・パラメータ部終了（コマンド部終了）
                            aParamStrs.Add(aParamStr);
                            aParamStr = "";
                            ExecCommand();
                            execState = State.PASS_THROUGH;
                        }
                        else
                        {
                            // パラメータ文字列に追加
                            aParamStr += char1;
                        }
                        break;

                    default:
                        aEscapeState = false;
                        break;
                }
            }
        }

        /// <summary>
        /// コマンド実行
        /// </summary>
        protected void ExecCommand()
        {
            MethodInfo aMethod = this.GetType().GetMethod(aCommandStr);
            ParameterInfo[] aParams = aMethod.GetParameters();
            object[] aMethodParams = new object[aParams.Length];

            // パラメータの構築
            for(int idx = 0; idx < aParams.Length; idx++)
            {
                ParameterInfo aParam = aParams[idx];
                Type aType = aParam.ParameterType;

                // パラメータ値
                var aParamValue = Activator.CreateInstance(aType);

                string[] aOrParams = aParamStrs[idx].Split("|");

                // OR演算
                foreach(string val in aOrParams)
                {
                    // パラメータ値
                    var aTmpValue = Activator.CreateInstance(aType);

                    // 定数定義をチェックする
                    FieldInfo aFieldInfo = this.GetType().GetField(val.Trim());
                    var aResult = aFieldInfo.GetValue(this);
                    if (aResult == null)
                    {
                        // 定数定義はない→リテラル値をチェックする
                        MethodInfo aTryParseMethod = aType.GetMethod("TryParse");
                        aResult = aTryParseMethod.Invoke(aType, new object[] { val.Trim(), aTmpValue });
                        if (aResult == null)
                        {
                            throw new Exception(string.Format("{0}: Type {1} does not have a TryParse Method.", MethodBase.GetCurrentMethod().Name, aType.Name));
                        }
                        else if ((bool)aResult == false)
                        {
                            throw new Exception(string.Format("{0}: Fail to TryParse to type {1} from \"{2}\".", MethodBase.GetCurrentMethod().Name, aType.Name, val.Trim()));
                        }
                    }

                    // 算術ORオペレータ
                    MethodInfo aOrMethod = aType.GetMethod("op_BitwiseOr");
                    if (aOrMethod == null)
                    {
                        throw new Exception(string.Format("{0}: Type {1} does not have a BitwiseOR(|) Operator.", MethodBase.GetCurrentMethod().Name, aType.Name));
                    }

                    // パラメータを変換できた
                    aParamValue = aOrMethod.Invoke(aParamValue, new object[] { aParamValue, aTmpValue });
                }

                aMethodParams[idx] = aParamValue;
            }

            // コマンド実行
            aMethod.Invoke(this, aMethodParams);
        }

        /// <summary>
        /// 一文字出力
        /// </summary>
        /// <param name="char1"></param>
        protected abstract void EmitChar(char char1);
    }
}
