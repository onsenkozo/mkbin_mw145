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
    abstract class PrinterCommon : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        protected string outputFileName;

        /// <summary>
        /// 
        /// </summary>
        protected BinaryWriter binaryWriter;

        /// <summary>
        /// 
        /// </summary>
        protected string inputFileName;

        /// <summary>
        /// 入力ライン番号
        /// </summary>
        protected int lineNo = 0;

        /// <summary>
        /// 入力文字位置
        /// </summary>
        protected int columnNo = 0;

        /// <summary>
        /// 実行ステート区分
        /// </summary>
        protected enum State
        {
            PASS_THROUGH,       // パススルー部
            CARRIAGE_RETURN,    // キャリッジ・リターン（復帰）
            COMMENT,            // コメント部
            COMMAND,            // コマンド部
            PARAMATERS,         // パラメーター部
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
        protected List<string> aParamStrs = new();

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
            this.inputFileName = inputFileName;
            this.outputFileName = outputFileName;
        }

        /// <summary>
        /// 一文字ずつ処理する
        /// </summary>
        /// <returns></returns>
        public void Exec()
        {
            try
            {
                using StreamReader sr = new(inputFileName);
                int char1;
                while ((char1 = sr.Read()) != -1)
                {
                    if (lineNo == 0)
                    {
                        lineNo = 1;
                    }

                    switch (execState)
                    {
                        case State.PASS_THROUGH:    // パススルー状態
                            if (char1 == '\\')
                            {
                                // コマンド部開始（またはバックスラッシュ）
                                aEscapeState = true;
                                execState = State.COMMAND;
                                columnNo++;
                            }
                            else if (char1 == '#')
                            {
                                // コメント開始
                                execState = State.COMMENT;
                                columnNo++;
                            }
                            else if (char1 == '\x0d')
                            {
                                // CR（復帰）
                                execState = State.CARRIAGE_RETURN;
                                columnNo = 0;
                                lineNo++;
                            }
                            else if (char1 == '\x0a')
                            {
                                // LF（改行）
                                columnNo = 0;
                                lineNo++;
                            }
                            else
                            {
                                // 一文字出力
                                columnNo += EmitChar((char)char1);
                            }
                            break;

                        case State.CARRIAGE_RETURN:    // キャリッジ・リターン（復帰）
                            if (char1 == '\\')
                            {
                                // コマンド部開始（またはバックスラッシュ）
                                aEscapeState = true;
                                execState = State.COMMAND;
                                columnNo++;
                            }
                            else if (char1 == '\x0d')
                            {
                                // CR（復帰）
                                execState = State.CARRIAGE_RETURN;
                                columnNo = 0;
                                lineNo++;
                            }
                            else if (char1 == '\x0a')
                            {
                                // LF（改行）
                                execState = State.PASS_THROUGH;
                            }
                            else
                            {
                                // 一文字出力
                                columnNo += EmitChar((char)char1);
                                execState = State.PASS_THROUGH;
                            }
                            break;

                        case State.COMMENT:    // コメント
                            if (char1 == '\x0d')
                            {
                                // CR（復帰）
                                execState = State.CARRIAGE_RETURN;
                                columnNo = 0;
                                lineNo++;
                            }
                            else if (char1 == '\x0a')
                            {
                                // LF（改行）
                                execState = State.PASS_THROUGH;
                                columnNo = 0;
                                lineNo++;
                            }
                            break;

                        case State.COMMAND:
                            if (char1 == '\\' && aEscapeState == true)
                            {
                                // バックスラッシュ（コマンド部の開始ではない）
                                execState = State.PASS_THROUGH;
                                columnNo += EmitChar('\\');
                            }
                            else if (char1 == '#' && aEscapeState == true)
                            {
                                // ナンバーサイン（コマンド部の開始ではない）
                                execState = State.PASS_THROUGH;
                                columnNo += EmitChar('#');
                            }
                            else if (char1 == ';')
                            {
                                // コマンド部終了
                                columnNo++;
                                aParamStrs.Clear();
                                ExecCommand();
                                execState = State.PASS_THROUGH;
                                aCommandStr = "";
                            }
                            else if (char1 == '(')
                            {
                                // コマンド・パラメータ部開始
                                columnNo++;
                                execState = State.PARAMATERS;
                                aParamStrs.Clear();
                                aParamStr = "";
                            }
                            else
                            {
                                // コマンド文字列に追加
                                aCommandStr += (char)char1;
                                columnNo++;
                            }
                            aEscapeState = false;
                            break;

                        case State.PARAMATERS:
                            if (char1 == ',')
                            {
                                // コマンド・パラメータ部、次パラメータ開始
                                aParamStrs.Add(aParamStr);
                                aParamStr = "";
                                columnNo++;
                            }
                            else if (char1 == ')')
                            {
                                // コマンド・パラメータ部終了（コマンド部終了）
                                aParamStrs.Add(aParamStr);
                                aParamStr = "";
                                columnNo++;
                                ExecCommand();
                                execState = State.PASS_THROUGH;
                                aCommandStr = "";
                            }
                            else
                            {
                                // パラメータ文字列に追加
                                aParamStr += (char)char1;
                                columnNo += CountChar((char)char1);
                            }
                            break;

                        default:
                            aEscapeState = false;
                            break;
                    }
                }

            }
            catch (Exception e)
            {
                Console.Error.WriteLine(string.Format("L{0:d} C{1:d} {2}", lineNo, columnNo, e.Message));
            }
        }

        /// <summary>
        /// コマンド実行
        /// </summary>
        protected void ExecCommand()
        {
            MethodInfo aMethod = this.GetType().GetMethod(aCommandStr, BindingFlags.Instance | BindingFlags.NonPublic);
            if (aMethod == null)
            {
                throw new Exception();
            }
            ParameterInfo[] aParams = aMethod.GetParameters();
            if (aParams == null)
            {
                throw new Exception();
            }

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
                    FieldInfo aFieldInfo = this.GetType().GetField(val.Trim(), BindingFlags.Instance | BindingFlags.NonPublic);
                    if (aFieldInfo != null)
                    {
                        aTmpValue = aFieldInfo.GetValue(this);
                    }
                    else
                    {
                        // 定数定義はない→リテラル値をチェックする
                        MethodInfo aTryParseMethod = aType.GetMethod("TryParse", new Type[] { typeof(System.String), aType.MakeByRefType() });
                        if (aTryParseMethod == null)
                        {
                            throw new Exception(string.Format("{0}: Type {1} does not have a TryParse Method.", MethodBase.GetCurrentMethod().Name, aType.Name));
                        }
                        object[] args = { val.Trim(), null };
                        var aResult = (bool)aTryParseMethod.Invoke(null, args);
                        if (aResult == false)
                        {
                            throw new Exception(string.Format("{0}: Fail to TryParse to type {1} from \"{2}\".", MethodBase.GetCurrentMethod().Name, aType.Name, val.Trim()));
                        }
                        aTmpValue = args[1];
                    }

                    switch (aType.Name)
                    {
                        case "Int64":
                            aParamValue = (Int64)aParamValue | (Int64)aTmpValue;
                            break;
                        case "Int32":
                            aParamValue = (Int32)aParamValue | (Int32)aTmpValue;
                            break;
                        case "Int16":
                            aParamValue = (Int16)aParamValue | (Int16)aTmpValue;
                            break;
                        case "Byte":
                            aParamValue = (Byte)aParamValue | (Byte)aTmpValue;
                            break;
                        case "Boolean":
                            aParamValue = (Boolean)aParamValue | (Boolean)aTmpValue;
                            break;
                        default:
                            throw new Exception(string.Format("{0}: Type {1} does not have a BitwiseOR(|) Operator.", MethodBase.GetCurrentMethod().Name, aType.Name));

                    }
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
        /// <returns>Additional Columns</returns>
        protected abstract int EmitChar(char char1);

        /// <summary>
        /// 一文字出力
        /// </summary>
        /// <param name="char1"></param>
        /// <returns>Additional Columns</returns>
        protected abstract int CountChar(char char1);

        /// <summary>
        /// 
        /// </summary>
        public abstract void Dispose();
    }
}
