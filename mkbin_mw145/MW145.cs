using System;
using System.Reflection;
using System.Text;
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
        /// <param name="inputFileName"></param>
        /// <param name="outputFileName"></param>
        public MW145(string inputFileName, string outputFileName)
            : base(inputFileName, outputFileName)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        // ========================================================
        // 定数・Enumerate
        // ========================================================
        public const int ANK_UNDERLINE     = 128;
        public const int ANK_ITALIC        = 64;
        public const int ANK_DOUBLE_WIDTH  = 32;
        public const int ANK_DOUBLE_HEIGHT = 16;
        public const int ANK_BOLD          = 8;
        public const int ANK_HALF_WIDTH    = 4;
        public const int ANK_PROPOTIONAL   = 2;
        public const int ANK_CPI_12        = 1;

        public const int KNJ_UNDERLINE        = 128;
        public const int KNJ_ITALIC           = 64;
        public const int KNJ_SUBSCRIPT        = 32;
        public const int KNJ_QUATER_SIZE      = 16;
        public const int KNJ_DOUBLE_HEIGHT    = 8;
        public const int KNJ_DOUBLE_WIDTH     = 4;
        public const int KNJ_HALF_WIDTH       = 2;
        public const int KNJ_VERTICAL_WRITING = 1;

        /// <summary>
        /// エンコーディング
        /// </summary>
        protected Encoding sjis = Encoding.GetEncoding("shift_jis");

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
                throw new Exception(string.Format("{0}: Bad Parameter: {1:d}", MethodBase.GetCurrentMethod().Name, value));
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
                throw new Exception(string.Format("{0}: Bad Parameter: {1:d}", MethodBase.GetCurrentMethod().Name, value));
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
                throw new Exception(string.Format("{0}: Bad Parameter: {1:d}", MethodBase.GetCurrentMethod().Name, value));
            }
        }

        /// <summary>
        /// 絶対垂直位置指定
        /// </summary>
        /// <param name="value"></param>
        protected void SetVerticalAbsolutePosition(int value)
        {
            if (0 <= value &&
                value < 1180)
            {
                byte lb = (byte)(value % 255);
                byte hb = (byte)(value / 256);

                binaryWriter.Write((byte)0x1b);
                binaryWriter.Write((byte)0x28);
                binaryWriter.Write((byte)0x56);
                binaryWriter.Write((byte)0x02);
                binaryWriter.Write((byte)0x00);
                binaryWriter.Write(lb);
                binaryWriter.Write(hb);
            }
            else
            {
                throw new Exception(string.Format("{0}: Bad Parameter: {1:d}", MethodBase.GetCurrentMethod().Name, value));
            }
        }

        /// <summary>
        /// 相対垂直位置指定
        /// </summary>
        /// <param name="value"></param>
        protected void SetVerticalReativePosition(int value)
        {
            if (0 <= value &&
                value < 1180)
            {
                byte lb = (byte)(value % 255);
                byte hb = (byte)(value / 256);

                binaryWriter.Write((byte)0x1b);
                binaryWriter.Write((byte)0x28);
                binaryWriter.Write((byte)0x76);
                binaryWriter.Write((byte)0x02);
                binaryWriter.Write((byte)0x00);
                binaryWriter.Write(lb);
                binaryWriter.Write(hb);
            }
            else
            {
                throw new Exception(string.Format("{0}: Bad Parameter: {1:d}", MethodBase.GetCurrentMethod().Name, value));
            }
        }

        /// <summary>
        /// ページフォーマット設定
        /// </summary>
        /// <param name="topMargin"></param>
        /// <param name="bottomMargin"></param>
        protected void SetPageFormat(int topMargin, int bottomMargin)
        {
            if (0 <= topMargin && 
                topMargin < 1180 &&
                0 <= bottomMargin &&
                bottomMargin < 1180 &&
                topMargin < bottomMargin)
            {
                byte tlb = (byte)(topMargin % 255);
                byte thb = (byte)(topMargin / 256);

                byte blb = (byte)(bottomMargin % 255);
                byte bhb = (byte)(bottomMargin / 256);

                binaryWriter.Write((byte)0x1b);
                binaryWriter.Write((byte)0x28);
                binaryWriter.Write((byte)0x63);
                binaryWriter.Write((byte)0x04);
                binaryWriter.Write((byte)0x00);
                binaryWriter.Write(tlb);
                binaryWriter.Write(thb);
                binaryWriter.Write(blb);
                binaryWriter.Write(bhb);
            }
            else
            {
                throw new Exception(string.Format("{0}: Bad Parameter: topMargin {1:d} smaller than bottomMargin {2:d}", MethodBase.GetCurrentMethod().Name, topMargin, bottomMargin));
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
        /// 垂直タブ設定
        /// </summary>
        /// <param name="values"></param>
        protected void SetVerticalTabStops(params int[] values)
        {
            if (values.Length < 17)
            {
                int MinValue = int.MinValue;
                foreach (int val in values)
                {
                    if (MinValue > val)
                    {
                        throw new Exception(string.Format("{0}: Bad Parameter Sequence: Current Value {1:d} is smaller than Previous Value {2:d}", MethodBase.GetCurrentMethod().Name, val, MinValue));
                    }
                }

                binaryWriter.Write((byte)0x1b);
                binaryWriter.Write((byte)0x42);

                foreach (int val in values)
                {
                    binaryWriter.Write((byte)val);
                }

                binaryWriter.Write((byte)0x0);
            }
            else
            {
                throw new Exception(string.Format("{0}: Bad Parameter Count: {1:d}", MethodBase.GetCurrentMethod().Name, values.Length));
            }
        }

        /// <summary>
        /// 水平タブ設定
        /// </summary>
        /// <param name="values"></param>
        protected void SetHorizontalTabStops(params int[] values)
        {
            if (values.Length < 33)
            {
                int MinValue = int.MinValue;
                foreach (int val in values)
                {
                    if (MinValue > val)
                    {
                        throw new Exception(string.Format("{0}: Bad Parameter Sequence: Current Value {1:d} is smaller than Previous Value {2:d}", MethodBase.GetCurrentMethod().Name, val, MinValue));
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
                throw new Exception(string.Format("{0}: Bad Parameter Count: {1:d}", MethodBase.GetCurrentMethod().Name, values.Length));
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
                throw new Exception(string.Format("{0}: Bad Parameter: {1:d}", MethodBase.GetCurrentMethod().Name, value));
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
                throw new Exception(string.Format("{0}: Bad Parameter: {1:d}", MethodBase.GetCurrentMethod().Name, value));
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
                throw new Exception(string.Format("{0}: Bad Parameter: {1:d}", MethodBase.GetCurrentMethod().Name, value));
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
                throw new Exception(string.Format("{0}: Bad Parameter: {1:d}", MethodBase.GetCurrentMethod().Name, value));
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

        /// <summary>
        /// ダウンロードデータプリント
        /// </summary>
        /// <param name="value"></param>
        protected void PrintPreDownloadedData(int value)
        {
            if (0 <= value && value <= 98)
            {
                binaryWriter.Write((byte)0x1b);
                binaryWriter.Write((byte)0x69);
                binaryWriter.Write((byte)0x46);
                binaryWriter.Write((byte)0x50);
                binaryWriter.Write((byte)value);
            }
            else
            {
                throw new Exception(string.Format("{0}: Bad Parameter: {1:d}", MethodBase.GetCurrentMethod().Name, value));
            }
        }

        /// <summary>
        /// プリンタステータス要求
        /// </summary>
        protected void RequestPrinterStatus()
        {
            binaryWriter.Write((byte)0x1b);
            binaryWriter.Write((byte)0x69);
            binaryWriter.Write((byte)0x53);
        }

        /// <summary>
        /// コマンドモード切替
        /// </summary>
        /// <param name="value"></param>
        protected void ChangeCommandMode(int value)
        {
            if (0 == value || 1 == value || 3 == value)
            {
                binaryWriter.Write((byte)0x1b);
                binaryWriter.Write((byte)0x69);
                binaryWriter.Write((byte)0x61);
                binaryWriter.Write((byte)value);
            }
            else
            {
                throw new Exception(string.Format("{0}: Bad Parameter: {1:d}", MethodBase.GetCurrentMethod().Name, value));
            }
        }

        /// <summary>
        /// ランドスケープ設定
        /// </summary>
        /// <param name="value"></param>
        protected void SetLandscapeMode(bool value)
        {
            binaryWriter.Write((byte)0x1b);
            binaryWriter.Write((byte)0x69);
            binaryWriter.Write((byte)0x4c);
            if (value)
            {
                binaryWriter.Write('1');
            }
            else
            {
                binaryWriter.Write('0');
            }
        }

        // ========================================================
        // FSコマンド
        // ========================================================

        /// <summary>
        /// 漢字モード指定
        /// </summary>
        protected void SetKanjiMode()
        {
            binaryWriter.Write((byte)0x1c);
            binaryWriter.Write((byte)0x26);
        }

        /// <summary>
        /// 漢字モード解除
        /// </summary>
        protected void UnsetKanjiMode()
        {
            binaryWriter.Write((byte)0x1c);
            binaryWriter.Write((byte)0x2e);
        }

        /// <summary>
        /// 半角文字縦書き2文字印字
        /// </summary>
        /// <param name="char1"></param>
        /// <param name="char2"></param>
        protected void Print2HalfAs1FullWidth(char char1, char char2)
        {
            byte[] sjis1 = sjis.GetBytes(char1.ToString());
            byte[] sjis2 = sjis.GetBytes(char2.ToString());

            byte llb;
            byte lhb;

            byte rlb;
            byte rhb;

            if (sjis1.Length < 2)
            {
                llb = 0x00;
                lhb = sjis1[0];
            }
            else
            {
                llb = sjis1[0];
                lhb = sjis1[1];
            }

            if (sjis2.Length < 2)
            {
                rlb = 0x00;
                rhb = sjis2[0];
            }
            else
            {
                rlb = sjis2[0];
                rhb = sjis2[1];
            }

            binaryWriter.Write((byte)0x1c);
            binaryWriter.Write((byte)0x44);
            binaryWriter.Write((byte)llb);
            binaryWriter.Write((byte)lhb);
            binaryWriter.Write((byte)rlb);
            binaryWriter.Write((byte)rhb);
        }

        /// <summary>
        /// 縦書き指定
        /// </summary>
        protected void SetVerticalWriting()
        {
            binaryWriter.Write((byte)0x1c);
            binaryWriter.Write((byte)0x4a);
        }

        /// <summary>
        /// 横書き指定
        /// </summary>
        protected void SetHorizontalWriting()
        {
            binaryWriter.Write((byte)0x1c);
            binaryWriter.Write((byte)0x4b);
        }

        /// <summary>
        /// 全角文字のスペース量設定
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        protected void SetFullWidthSpacing(int left, int right)
        {
            if (left < 0 || 128 <= left)
            {
                throw new Exception(string.Format("SetFullWidthSpacing: Bad Parameter: left {0:d}", left));
            }
            else  if (left < 0 || 128 <= left)
            {
                throw new Exception(string.Format("SetFullWidthSpacing: Bad Parameter: right {0:d}", right));
            }
            else
            {
                binaryWriter.Write((byte)0x1c);
                binaryWriter.Write((byte)0x53);
                binaryWriter.Write((byte)left);
                binaryWriter.Write((byte)right);
            }
        }

        /// <summary>
        /// 半角文字のスペース量設定
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        protected void SetHalfWidthSpacing(int left, int right)
        {
            if (left < 0 || 128 <= left)
            {
                throw new Exception(string.Format("{0}: Bad Parameter: left {1:d}", MethodBase.GetCurrentMethod().Name, left));
            }
            else if (left < 0 || 128 <= left)
            {
                throw new Exception(string.Format("{0}: Bad Parameter: right {1:d}", MethodBase.GetCurrentMethod().Name, right));
            }
            else
            {
                binaryWriter.Write((byte)0x1c);
                binaryWriter.Write((byte)0x54);
                binaryWriter.Write((byte)left);
                binaryWriter.Write((byte)right);
            }
        }

        /// <summary>
        /// 半角文字間スペース補正
        /// </summary>
        protected void SetAdjustmentHalfWidthFont()
        {
            binaryWriter.Write((byte)0x1c);
            binaryWriter.Write((byte)0x55);
        }

        /// <summary>
        /// 半角文字間スペース補正の解除
        /// </summary>
        protected void UnsetAdjustmentHalfWidthFont()
        {
            binaryWriter.Write((byte)0x1c);
            binaryWriter.Write((byte)0x56);
        }

        /// <summary>
        /// 4倍角文字選択
        /// </summary>
        /// <param name="value"></param>
        protected void SetDoubleWidthAndHeightFont(bool value)
        {
            binaryWriter.Write((byte)0x1c);
            binaryWriter.Write((byte)0x55);

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
        /// 漢字サイズ指定
        /// </summary>
        /// <param name="value"></param>
        protected void SetKanjiFontSize(int value)
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

                binaryWriter.Write((byte)0x1c);
                binaryWriter.Write((byte)0x59);
                binaryWriter.Write('0');
                binaryWriter.Write('0');
                binaryWriter.Write(lb);
                binaryWriter.Write(hb);
                binaryWriter.Write('0');
                binaryWriter.Write('0');
            }
            else
            {
                throw new Exception(string.Format("{0}: Bad Parameter: {1:d}", MethodBase.GetCurrentMethod().Name, value));
            }
        }

        /// <summary>
        /// 1/4角文字指定
        /// </summary>
        /// <param name="value"></param>
        protected void SetQuaterSquareCharacter(bool value)
        {
            binaryWriter.Write((byte)0x1c);
            binaryWriter.Write((byte)0x72);

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
        /// 漢字アンダーライン指定
        /// </summary>
        /// <param name="value"></param>
        protected void SetKaniUnderline(int value)
        {
            if (0 <= value &&
                value < 5)
            {
                binaryWriter.Write((byte)0x1c);
                binaryWriter.Write((byte)0x2d);
                binaryWriter.Write((byte)value);
            }
            else
            {
                throw new Exception(string.Format("{0}: Bad Parameter: {1:d}", MethodBase.GetCurrentMethod().Name, value));
            }
        }

        /// <summary>
        /// 漢字印字モードの設定
        /// </summary>
        protected void SetKanjiFontStyle(int value)
        {
            if (0 <= value && value <= 255)
            {
                binaryWriter.Write((byte)0x1c);
                binaryWriter.Write((byte)0x21);
                binaryWriter.Write((byte)value);
            }
            else
            {
                throw new Exception(string.Format("{0}: Bad Parameter: {1:d}", MethodBase.GetCurrentMethod().Name, value));
            }
        }

        /// <summary>
        /// 半角文字指定
        /// </summary>
        protected void SetHalfWidthKanjiFont()
        {
            binaryWriter.Write((byte)0x1c);
            binaryWriter.Write((byte)0x0f);
        }

        /// <summary>
        /// 半角文字解除
        /// </summary>
        protected void UnsetHalfWidthKanjiFont()
        {
            binaryWriter.Write((byte)0x1c);
            binaryWriter.Write((byte)0x12);
        }

        /// <summary>
        /// 自動解除付き倍角文字指定
        /// </summary>
        protected void SetAutoUnsetDoubleWidthKanjiFont()
        {
            binaryWriter.Write((byte)0x1c);
            binaryWriter.Write((byte)0x0e);
        }

        /// <summary>
        /// 自動解除付き倍角文字解除
        /// </summary>
        protected void UnsetAutoUnsetDoubleWidthKanjiFont()
        {
            binaryWriter.Write((byte)0x1c);
            binaryWriter.Write((byte)0x14);
        }

        /// <summary>
        /// 一文字出力
        /// </summary>
        /// <param name="char1"></param>
        protected override void EmitChar(char char1)
        {
            byte[] sjis1 = sjis.GetBytes(char1.ToString());

            foreach (byte aByte in sjis1)
            {
                binaryWriter.Write(aByte);
            }
        }
    }
}
