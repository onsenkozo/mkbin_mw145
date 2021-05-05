using System;
/// <summary>
/// 
/// </summary>
namespace mkbin_mw145
{
    /// <summary>
    /// 
    /// </summary>
    class MW145 : PrinterCommon
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MW145()
        {

        }

        // ========================================================
        // 定数・Enumerate
        // ========================================================
        public const int UNDERLINE     = 128;
        public const int ITALIC        = 64;
        public const int DOUBLE_WIDTH  = 32;
        public const int DOUBLE_HEIGHT = 16;
        public const int BOLD          = 8;
        public const int HALF_WIDTH    = 4;
        public const int PROPOTIONAL   = 2;
        public const int CPI_12        = 1;

        // ========================================================
        // 制御文字
        // ========================================================

        /// <summary>
        /// 水平タブの実行
        /// </summary>
        protected void HT()
        {
            binaryWriter.Write((byte)0x09);
        }

        /// <summary>
        /// 改行
        /// </summary>
        protected void LF()
        {
            binaryWriter.Write((byte)0x0a);
        }

        /// <summary>
        /// 垂直タブの実行
        /// </summary>
        protected void VT()
        {
            binaryWriter.Write((byte)0x0b);
        }

        /// <summary>
        /// 改ページ
        /// </summary>
        protected void FF()
        {
            binaryWriter.Write((byte)0x0c);
        }

        /// <summary>
        /// 印字復帰
        /// </summary>
        protected void CR()
        {
            binaryWriter.Write((byte)0x0d);
        }

        /// <summary>
        /// 自動解除付き拡大指定
        /// </summary>
        protected void SO()
        {
            binaryWriter.Write((byte)0x0e);
        }

        /// <summary>
        /// 縮小の指定
        /// </summary>
        protected void SI()
        {
            binaryWriter.Write((byte)0x0f);
        }

        /// <summary>
        /// 縮小の解除
        /// </summary>
        protected void DC2()
        {
            binaryWriter.Write((byte)0x12);
        }

        /// <summary>
        /// 自動解除付き倍幅拡大の解除
        /// </summary>
        protected void DC4()
        {
            binaryWriter.Write((byte)0x14);
        }

        // ========================================================
        // 特殊処理
        // ========================================================

        /// <summary>
        /// スリープの実行
        /// </summary>
        protected void Sleep(int millisec)
        {
            System.Threading.Thread.Sleep(millisec);
        }

        // ========================================================
        // エスケープコマンド
        // ========================================================

        /// <summary>
        /// 初期化
        /// </summary>
        protected void Init()
        {
            binaryWriter.Write((byte)0x1b);
            binaryWriter.Write((byte)0x40);
        }

        /// <summary>
        /// 自動解除付き拡大指定
        /// </summary>
        protected void AutoResetDoubleWidthFont()
        {
            binaryWriter.Write((byte)0x1b);
            binaryWriter.Write((byte)0x0e);
        }

        /// <summary>
        /// 縮小の指定
        /// </summary>
        protected void SetHalfWidthFont()
        {
            binaryWriter.Write((byte)0x1b);
            binaryWriter.Write((byte)0x0f);
        }

        /// <summary>
        /// ANK文字のスペース量設定
        /// </summary>
        protected void SetAnkFontSpace(int value)
        {
            if (0 <= value && value <= 127)
            {
                binaryWriter.Write((byte)0x1b);
                binaryWriter.Write((byte)0x20);
                binaryWriter.Write((byte)value);
            }
            else
            {
                throw new Exception(string.Format("SetAnkFontSpace: Bad Parameter: {0:d}", value));
            }
        }

        /// <summary>
        /// 一括指定
        /// </summary>
        protected void SetFontStyle(int value)
        {
            if (0 <= value && value <= 255)
            {
                binaryWriter.Write((byte)0x1b);
                binaryWriter.Write((byte)0x21);
                binaryWriter.Write((byte)value);
            }
            else
            {
                throw new Exception(string.Format("SetFontStyle: Bad Parameter: {0:d}", value));
            }
        }

        /// <summary>
        /// 絶対水平位置指定
        /// </summary>
        /// <param name="value"></param>
        protected void SetHorizontalAbsolutePosition(int value)
        {
            if (0 <= value &&
                value < 1180)
            {
                byte lb = (byte)(value % 255);
                byte hb = (byte)(value / 256);

                binaryWriter.Write((byte)0x1b);
                binaryWriter.Write((byte)0x24);
                binaryWriter.Write(lb);
                binaryWriter.Write(hb);
            }
            else
            {
                throw new Exception(string.Format("SetAnkFontSize: Bad Parameter: {0:d}", value));
            }
        }

        /// <summary>
        /// アンダーライン指定/解除
        /// </summary>
        protected void SetUnderline(bool value)
        {
            binaryWriter.Write((byte)0x1b);
            binaryWriter.Write((byte)0x2d);
            if (value)
            {
                binaryWriter.Write('1');
            }
            else
            {
                binaryWriter.Write('0');
            }
        }

        /// <summary>
        /// 1/8インチ改行量設定
        /// </summary>
        protected void SetOneEighthLineHeight()
        {
            binaryWriter.Write((byte)0x1b);
            binaryWriter.Write((byte)0x30);
        }

        /// <summary>
        /// 1/6インチ改行量設定
        /// </summary>
        protected void SetOneSixthLineHeight()
        {
            binaryWriter.Write((byte)0x1b);
            binaryWriter.Write((byte)0x32);
        }

        /// <summary>
        /// 最小単位の改行量設定
        /// </summary>
        /// <param name="value"></param>
        protected void SetMinimumLineHeight(int value)
        {
            binaryWriter.Write((byte)0x1b);
            binaryWriter.Write((byte)0x33);
            binaryWriter.Write((byte)value);
        }

        /// <summary>
        /// イタリック文字の指定
        /// </summary>
        protected void SetItalic()
        {
            binaryWriter.Write((byte)0x1b);
            binaryWriter.Write((byte)0x34);
        }

        /// <summary>
        /// イタリック文字の解除
        /// </summary>
        protected void UnsetItalic()
        {
            binaryWriter.Write((byte)0x1b);
            binaryWriter.Write((byte)0x35);
        }

        /// <summary>
        /// n/60インチ改行量設定
        /// </summary>
        /// <param name="value"></param>
        protected void SetNSixtiethLineHeight(int value)
        {
            binaryWriter.Write((byte)0x1b);
            binaryWriter.Write((byte)0x40);
            binaryWriter.Write((byte)value);
        }

        /// <summary>
        /// 水平タブ設定
        /// </summary>
        /// <param name="values"></param>
        protected void SetTabStops(params int[] values)
        {
            if (values.Length < 33)
            {
                int MinValue = int.MinValue;
                foreach (int val in values)
                {
                    if (MinValue > val)
                    {
                        throw new Exception(string.Format("SetTabStops: Bad Parameter Sequence: Current Value {0:d} is smaller than Previous Value {1:d}", val, MinValue));
                    }
                }

                binaryWriter.Write((byte)0x1b);
                binaryWriter.Write((byte)0x44);

                foreach (int val in values)
                {
                    binaryWriter.Write((byte)val);
                }

                binaryWriter.Write((byte)0x0);
            }
            else
            {
                throw new Exception(string.Format("SetTabStops: Bad Parameter Count: {0:d}", values.Length));
            }
        }

        /// <summary>
        /// 強調指定
        /// </summary>
        protected void SetBold()
        {
            binaryWriter.Write((byte)0x1b);
            binaryWriter.Write((byte)0x45);
        }

        /// <summary>
        /// 強調解除
        /// </summary>
        protected void UnsetBold()
        {
            binaryWriter.Write((byte)0x1b);
            binaryWriter.Write((byte)0x46);
        }

        /// <summary>
        /// 二重印字指定
        /// </summary>
        protected void SetDouble()
        {
            binaryWriter.Write((byte)0x1b);
            binaryWriter.Write((byte)0x47);
        }

        /// <summary>
        /// 二重印字解除
        /// </summary>
        protected void UnsetDouble()
        {
            binaryWriter.Write((byte)0x1b);
            binaryWriter.Write((byte)0x48);
        }

        /// <summary>
        /// 順方向紙送り実行
        /// </summary>
        /// <param name="value"></param>
        protected void FeedPaper(int value)
        {
            if (0 <= value &&
                value < 256)
            {
                binaryWriter.Write((byte)0x1b);
                binaryWriter.Write((byte)0x4a);
                binaryWriter.Write((byte)value);
            }
            else
            {
                throw new Exception(string.Format("FeedPaper: Bad Parameter: {0:d}", value));
            }
        }

        /// <summary>
        /// エリートピッチ指定
        /// </summary>
        protected void SetElitePitch()
        {
            binaryWriter.Write((byte)0x1b);
            binaryWriter.Write((byte)0x4d);
        }

        /// <summary>
        /// パイカピッチ指定
        /// </summary>
        protected void SetPicaPitch()
        {
            binaryWriter.Write((byte)0x1b);
            binaryWriter.Write((byte)0x50);
        }

        /// <summary>
        /// 英数カナ文字サイズ指定
        /// </summary>
        /// <param name="value"></param>
        protected void SetAnkFontSize(int value)
        {
            if (value == 16 ||
                value == 24 ||
                value == 32 ||
                value == 33 ||
                value == 38 ||
                value == 42 ||
                value == 46 ||
                value == 50 ||
                value == 58 ||
                value == 67 ||
                value == 75 ||
                value == 83 ||
                value == 92 ||
                value == 100 ||
                value == 117 ||
                value == 133 ||
                value == 150 ||
                value == 167 ||
                value == 200 ||
                value == 233 ||
                value == 267 ||
                value == 300 ||
                value == 333 ||
                value == 367 ||
                value == 400)
            {
                byte lb = (byte)(value % 255);
                byte hb = (byte)(value / 256);

                binaryWriter.Write((byte)0x1b);
                binaryWriter.Write((byte)0x58);
                binaryWriter.Write('0');
                binaryWriter.Write(lb);
                binaryWriter.Write(hb);
            }
            else
            {
                throw new Exception(string.Format("SetAnkFontSize: Bad Parameter: {0:d}", value));
            }
        }

        /// <summary>
        /// 相対水平位置指定
        /// </summary>
        /// <param name="value"></param>
        protected void SetHorizontalReativePosition(int value)
        {
            if (0 <= value &&
                value < 1180)
            {
                byte lb = (byte)(value % 255);
                byte hb = (byte)(value / 256);

                binaryWriter.Write((byte)0x1b);
                binaryWriter.Write((byte)0x5c);
                binaryWriter.Write(lb);
                binaryWriter.Write(hb);
            }
            else
            {
                throw new Exception(string.Format("SetHorizontalReativePosition: Bad Parameter: {0:d}", value));
            }
        }

        /// <summary>
        ///位置揃えの設定
        /// </summary>
        /// <param name="value"></param>
        protected void SetAlignment(int value)
        {
            if (0 <= value &&
                value < 4)
            {
                binaryWriter.Write((byte)0x1b);
                binaryWriter.Write((byte)0x61);
                binaryWriter.Write((byte)value);
            }
            else
            {
                throw new Exception(string.Format("SetAlignment: Bad Parameter: {0:d}", value));
            }
        }

        /// <summary>
        /// 左マージン設定
        /// </summary>
        /// <param name="value"></param>
        protected void SetLeftMargin(int value)
        {
            binaryWriter.Write((byte)0x1b);
            binaryWriter.Write((byte)0x6c);
            binaryWriter.Write((byte)value);
        }

        /// <summary>
        /// 右マージン設定
        /// </summary>
        /// <param name="value"></param>
        protected void SetRightMargin(int value)
        {
            binaryWriter.Write((byte)0x1b);
            binaryWriter.Write((byte)0x51);
            binaryWriter.Write((byte)value);
        }

        /// <summary>
        /// ミクロンピッチ指定
        /// </summary>
        protected void SetMicronPitch()
        {
            binaryWriter.Write((byte)0x1b);
            binaryWriter.Write((byte)0x67);
        }

        /// <summary>
        /// プロポーショナル文字の選択
        /// </summary>
        /// <param name="value"></param>
        protected void SetProportionalFont(bool value)
        {
            binaryWriter.Write((byte)0x1b);
            binaryWriter.Write((byte)0x70);
            if (value)
            {
                binaryWriter.Write('1');
            }
            else
            {
                binaryWriter.Write('0');
            }
        }

        /// <summary>
        /// 倍幅拡大文字の選択
        /// </summary>
        /// <param name="value"></param>
        protected void SetDoubleWidthFont(bool value)
        {
            binaryWriter.Write((byte)0x1b);
            binaryWriter.Write((byte)0x57);
            if (value)
            {
                binaryWriter.Write('1');
            }
            else
            {
                binaryWriter.Write('0');
            }
        }
    }
}
