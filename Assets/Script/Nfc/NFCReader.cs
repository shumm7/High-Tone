using System;
using UnityEngine;
using SmardCard;
using System.Text;


public class NFCReader : MonoBehaviour
{
    private IntPtr hContext = IntPtr.Zero;
    private IntPtr hCard;
    private IntPtr activeProtocol;

    public string readerName;
    public string cardId;
    public bool DetectOnlyFeliCa;
    public bool ThrowExceptionLog;

    public class CardData
    {
        public string ReaderName = "";
        public string CardID = "";
    }

    public CardData ReadCardData()
    {
        try
        {
            SCardEstablishContext();
            SCardListReaders();
            SCardConnect();
            SCardStatus();
            SCardTransmit();
            SCardDisconnect();
            SCardReleaseContext();
        }
        catch(Exception e)
        {
            if (ThrowExceptionLog)
            {
                Debug.LogWarning(e);
            }
            return new CardData();
        }
        return new CardData { ReaderName = readerName, CardID = cardId };
    }

    public string GetCardReaderName()
    {
        try
        {
            SCardEstablishContext();
            SCardListReaders();
        }
        catch (Exception e)
        {
            if (ThrowExceptionLog)
            {
                Debug.LogWarning(e);
            }
            return "";
        }
        return readerName;
    }

    private uint SCardEstablishContext()
    {
        uint ret = NfcApi.SCardEstablishContext(NfcConstant.SCARD_SCOPE_USER, IntPtr.Zero, IntPtr.Zero, out hContext);
        if (ret != NfcConstant.SCARD_S_SUCCESS)
        {
            string message;
            switch (ret)
            {
                case NfcConstant.SCARD_E_NO_SERVICE:
                    message = "サービスが起動されていません。";
                    break;
                default:
                    message = "サービスに接続できません。code = " + ret;
                    break;
            }
            throw new ApplicationException(message);
        }

        if (hContext == IntPtr.Zero)
        {
            throw new ApplicationException("コンテキストの取得に失敗しました。");
            
        }
        return ret;
    }

    private void SCardListReaders()
    {
        uint pcchReaders = 0;

        // NFCリーダの文字列バッファのサイズを取得
        uint ret = NfcApi.SCardListReaders(hContext, null, null, ref pcchReaders);
        if (ret != NfcConstant.SCARD_S_SUCCESS)
        {
            // 検出失敗
            throw new ApplicationException("NFCリーダを確認できません。");
        }

        // NFCリーダの文字列を取得
        byte[] mszReaders = new byte[pcchReaders * 2]; // 1文字2byte
        ret = NfcApi.SCardListReaders(hContext, null, mszReaders, ref pcchReaders);
        if (ret != NfcConstant.SCARD_S_SUCCESS)
        {
            // 検出失敗
            throw new ApplicationException("NFCリーダの取得に失敗しました。");
        }


        UnicodeEncoding unicodeEncoding = new UnicodeEncoding();
        string readerNameMultiString = unicodeEncoding.GetString(mszReaders);

        // 認識したNDCリーダの最初の1台を使用
        int nullindex = readerNameMultiString.IndexOf((char)0);
        readerName = readerNameMultiString.Substring(0, nullindex);
    }

    private void SCardConnect()
    {
        activeProtocol = IntPtr.Zero;
        hCard = IntPtr.Zero;
        uint ret = NfcApi.SCardConnect(hContext, readerName, NfcConstant.SCARD_SHARE_SHARED, NfcConstant.SCARD_PROTOCOL_T1, ref hCard, ref activeProtocol);
        if (ret != NfcConstant.SCARD_S_SUCCESS)
        {
            throw new ApplicationException("カードに接続できません。code = " + ret);
        }
    }

    private void SCardStatus()
    {
        int dwReaderLen = readerName.Length;
        int dwState = 0;
        byte[] byAtr = new byte[64]; //ATR
        int dwAtrLen = byAtr.Length;
        long lResult = NfcApi.SCardStatus(hCard, null, ref dwReaderLen, ref dwState, ref activeProtocol, byAtr, ref dwAtrLen);
        if (lResult != NfcConstant.SCARD_S_SUCCESS)
        {
            throw new ApplicationException("");
        }

        lResult = NfcApi.SCardStatus(hCard, readerName, ref dwReaderLen, ref dwState, ref activeProtocol, byAtr, ref dwAtrLen);
        if (lResult != NfcConstant.SCARD_S_SUCCESS)
        {
            throw new ApplicationException("");
        }

        // FeliCaかどうか
        if ((byAtr[13] != 0x00 || byAtr[14] != 0x3b) && DetectOnlyFeliCa)
        {
            throw new ApplicationException("FeliCaではありません。");
        }
    }

    private void SCardTransmit()
    {
        uint maxRecvDataLen = 256;
        var recvBuffer = new byte[maxRecvDataLen + 2];
        var sendBuffer = new byte[] { 0xff, 0xca, 0x00, 0x00, 0x00 };  // ← IDmを取得するコマンド

        NfcApi.SCARD_IO_REQUEST ioRecv = new NfcApi.SCARD_IO_REQUEST();
        ioRecv.cbPciLength = 255;

        int pcbRecvLength = recvBuffer.Length;
        int cbSendLength = sendBuffer.Length;

        IntPtr handle = NfcApi.LoadLibrary("Winscard.dll");
        IntPtr pci = NfcApi.GetProcAddress(handle, "g_rgSCardT1Pci");
        NfcApi.FreeLibrary(handle);

        uint ret = NfcApi.SCardTransmit(hCard, pci, sendBuffer, cbSendLength, ioRecv, recvBuffer, ref pcbRecvLength);
        if (ret != NfcConstant.SCARD_S_SUCCESS)
        {
            throw new ApplicationException("NFCカードへの送信に失敗しました。code = " + ret);
        }

        // 受信データからIDmを抽出する
        // recvBuffer = IDm + SW1 + SW2 (SW = StatusWord)
        // SW1 = 0x90 (144) SW1 = 0x00 (0) で正常だが、ここでは見ていない
        cardId = BitConverter.ToString(recvBuffer, 0, pcbRecvLength - 2);
    }

    private void SCardDisconnect()
    {
        uint ret = NfcApi.SCardDisconnect(hCard, NfcConstant.SCARD_LEAVE_CARD);
        if (ret != NfcConstant.SCARD_S_SUCCESS)
        {
            throw new ApplicationException("NFCカードとの切断に失敗しました。code = " + ret);
        }
    }
    
    private void SCardReleaseContext()
    {

        if (hContext == IntPtr.Zero)
        {
            throw new ApplicationException("コンテキストの取得に失敗しました。");
        }

        uint ret = NfcApi.SCardReleaseContext(hContext);

        if (ret != NfcConstant.SCARD_S_SUCCESS)
        {
            throw new ApplicationException("カードリーダーとの切断に失敗しました。code = " + ret);
        }
    }
}
