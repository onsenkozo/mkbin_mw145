﻿using System;
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
        /// パラメータ文字列保持
        /// </summary>
        protected string[] aParamStrs;

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
                        }
                        else if (char1 == '(')
                        {
                            // コマンド・パラメータ部開始
                            execState = State.PARAMATERS;

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

                        }
                        else if (char1 == '|')
                        {
                            // コマンド・パラメータ部、OR演算

                        }
                        else if (char1 == ')')
                        {
                            // コマンド・パラメータ部終了（コマンド部終了）

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
        /// <returns></returns>
        protected MethodInfo ExecCommand()
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
                    MethodInfo aTryParseMethod = aType.GetMethod("TryParse");

                    // パラメータ値
                    var aTmpValue = Activator.CreateInstance(aType);

                    // 定数定義をチェックする
                    FieldInfo aFieldInfo = this.GetType().GetField(val.Trim());
                    var aResult = aFieldInfo.GetValue(this);
                    if (aResult == null)
                    {
                        // 定数定義はない→リテラル値をチェックする
                        aResult = aTryParseMethod.Invoke(aType, new object[] { val.Trim(), aTmpValue });
                        if (aResult == null || (bool)aResult == false)
                        {
                            throw new Exception(string.Format("{0}: Null Reference Excepotion", MethodBase.GetCurrentMethod().Name));
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

            return null;
        }

        /// <summary>
        /// 一文字出力
        /// </summary>
        /// <param name="char1"></param>
        protected abstract void EmitChar(char char1);
    }
}
