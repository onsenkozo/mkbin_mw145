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
        /// 
        /// </summary>
        public MW145()
        {

        }

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
        /// パイカピッチ指定
        /// </summary>
        protected void SetPicaPitch()
        {
            binaryWriter.Write((byte)0x1b);
            binaryWriter.Write((byte)0x50);
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
