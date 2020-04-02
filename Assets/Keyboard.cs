using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keyboard : MonoBehaviour
{
    public GameObject Key;

    enum TextCategory
    {
        None, Number, AlphabetCapital, AlphabetSmall, Hiragana, Katakana
    };

    private char[] Number = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ' ', '?', '!', '@', '(', ')', '+', '-', '#', '~', '<', '>', '_', ',' };
    private char[] AlphabetCapital = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
    private char[] AlphabetSmall = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
    private char[,] Hiragana = { { 'あ', ' ', ' ' }, { 'い', ' ', ' ' }, { 'う', 'ゔ', ' ' }, { 'え', ' ', ' ' }, { 'お', ' ', ' ' }, { 'か', 'が', ' ' }, { 'き', 'ぎ', ' ' }, { 'く', 'ぐ', ' ' }, { 'け', 'げ', ' ' }, { 'こ', 'ご', ' ' }, { 'さ', 'ざ', ' ' }, { 'し', 'じ', ' ' }, { 'す', 'ず', ' ' }, { 'せ', 'ぜ', ' ' }, { 'そ', 'ぞ', ' ' }, { 'た', 'だ', ' ' }, { 'ち', 'ぢ', ' ' }, { 'つ', 'づ', ' ' }, { 'て', 'で', ' ' }, { 'と', 'ど', ' ' }, { 'な', ' ', ' ' }, { 'に', ' ', ' ' }, { 'ぬ', ' ', ' ' }, { 'ね', ' ', ' ' }, { 'の', ' ', ' ' }, { 'は', 'ば', 'ぱ' }, { 'ひ', 'び', 'ぴ' }, { 'ふ', 'ぶ', 'ぷ' }, { 'へ', 'べ', 'ぺ' }, { 'ほ', 'ぼ', 'ぽ' }, { 'ま', ' ', ' ' }, { 'み', ' ', ' ' }, { 'む', ' ', ' ' }, { 'め', ' ', ' ' }, { 'も', ' ', ' ' }, { 'や', ' ', ' ' }, { 'ゐ', ' ', ' ' }, { 'ゆ', ' ', ' ' }, { 'ゑ', ' ', ' ' }, { 'よ', ' ', ' ' }, { 'ら', ' ', ' ' }, { 'り', ' ', ' ' }, { 'る', ' ', ' ' }, { 'れ', ' ', ' ' }, { 'ろ', ' ', ' ' }, { 'わ', ' ', ' ' }, { 'を', ' ', ' ' }, { 'ん', ' ', ' ' } };
    private char[,] Katakana = { { 'ア', ' ', ' ' }, { 'イ', ' ', ' ' }, { 'ウ', 'ヴ', ' ' }, { 'エ', ' ', ' ' }, { 'オ', ' ', ' ' }, { 'カ', 'ガ', ' ' }, { 'キ', 'ギ', ' ' }, { 'ク', 'グ', ' ' }, { 'ケ', 'ゲ', ' ' }, { 'コ', 'ゴ', ' ' }, { 'サ', 'ザ', ' ' }, { 'シ', 'ジ', ' ' }, { 'ス', 'ズ', ' ' }, { 'セ', 'ゼ', ' ' }, { 'ソ', 'ゾ', ' ' }, { 'タ', 'ダ', ' ' }, { 'チ', 'ヂ', ' ' }, { 'ツ', 'ヅ', ' ' }, { 'テ', 'デ', ' ' }, { 'ト', 'ド', ' ' }, { 'ナ', ' ', ' ' }, { 'ニ', ' ', ' ' }, { 'ヌ', ' ', ' ' }, { 'ネ', ' ', ' ' }, { 'ノ', ' ', ' ' }, { 'ハ', 'バ', 'パ' }, { 'ヒ', 'ビ', 'ピ' }, { 'フ', 'ブ', 'プ' }, { 'ヘ', 'ベ', 'ペ' }, { 'ホ', 'ボ', 'ポ' }, { 'マ', ' ', ' ' }, { 'ミ', ' ', ' ' }, { 'ム', ' ', ' ' }, { 'メ', ' ', ' ' }, { 'モ', ' ', ' ' }, { 'ヤ', ' ', ' ' }, { 'ヰ', ' ', ' ' }, { 'ユ', ' ', ' ' }, { 'ヱ', ' ', ' ' }, { 'ヨ', ' ', ' ' }, { 'ラ', ' ', ' ' }, { 'リ', ' ', ' ' }, { 'ル', ' ', ' ' }, { 'レ', ' ', ' ' }, { 'ロ', ' ', ' ' }, { 'ワ', ' ', ' ' }, { 'ヲ', ' ', ' ' }, { 'ン', ' ', ' ' } };
    private char[,] SmallHiragana = { {'あ', 'ぁ'}, {'い', 'ぃ'}, {'う', 'ぅ'}, {'え', 'ぇ'}, {'お', 'ぉ'},{'つ', 'っ'}, {'や', 'ゃ'}, {'ゆ', 'ゅ'}, {'よ', 'ょ'} };
    private char[,] SmallKatakana = { { 'ア', 'ァ' }, { 'イ', 'ィ' }, { 'ウ', 'ゥ' }, { 'エ', 'ェ' }, { 'オ', 'ォ' }, { 'ツ', 'ッ' }, { 'ヤ', 'ャ' }, { 'ユ', 'ュ' }, { 'ヨ', 'ョ' } };

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private TextCategory checkTextCategory(char text)
    {
        for(int i=0; i<Number.Length; i++)
        {
            if (Number[i] == text)
                return TextCategory.Number;
        }
        for (int i = 0; i < AlphabetCapital.Length; i++)
        {
            if (AlphabetCapital[i] == text)
                return TextCategory.AlphabetCapital;
        }
        for (int i = 0; i < AlphabetSmall.Length; i++)
        {
            if (AlphabetSmall[i] == text)
                return TextCategory.AlphabetSmall;
        }
        for (int i = 0; i < Hiragana.Length; i++)
        {
            if (Hiragana[i,0] == text)
                return TextCategory.Hiragana;
            if (Hiragana[i, 1] == text)
                return TextCategory.Hiragana;
            if (Hiragana[i, 2] == text)
                return TextCategory.Hiragana;
        }
        for (int i = 0; i < Katakana.Length; i++)
        {
            if (Katakana[i, 0] == text)
                return TextCategory.Hiragana;
            if (Katakana[i, 1] == text)
                return TextCategory.Hiragana;
            if (Katakana[i, 2] == text)
                return TextCategory.Hiragana;
        }
        for (int i = 0; i < SmallHiragana.Length; i++)
        {
            if (SmallHiragana[i, 1] == text)
                return TextCategory.Hiragana;
        }
        for (int i = 0; i < SmallKatakana.Length; i++)
        {
            if (SmallKatakana[i, 1] == text)
                return TextCategory.Katakana;
        }

        return TextCategory.None;
    }

}
