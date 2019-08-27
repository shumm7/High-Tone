using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

// https://github.com/tomochan154/toy-box


/// <summary>�\�[�g�̕����B</summary>
public enum NaturalSortOrder : int
    {
        #region Enum

        /// <summary>�Ȃ��B</summary>
        None = 0,
        /// <summary>�����B</summary>
        Ascending = 1,
        /// <summary>�~���B</summary>
        Descending = 2,

        #endregion
    }

    /// <summary>���R���̔�r�I�v�V�����B</summary>
    [Flags()]
    public enum NaturalComparerOptions
    {
        #region Enum

        /// <summary>�A���r�A�����B</summary>
        Number = 0x1,
        /// <summary>ASCII���[�}�����B</summary>
        RomanNumber = 0x2,
        /// <summary>���{�ꃍ�[�}�����B</summary>
        JpRomanNumber = 0x4,
        /// <summary>���{��ې����B</summary>
        CircleNumber = 0x8,
        /// <summary>���{�ꊿ�����B</summary>
        KanjiNumber = 0x10,
        /// <summary>���ׂĂ̐����B</summary>
        NumberAll = Number | RomanNumber | JpRomanNumber | CircleNumber | KanjiNumber,

        /// <summary>�󔒕����̑��݂𖳎��B</summary>
        IgnoreSpace = 0x10000,
        /// <summary>�����\���̈Ⴂ�𖳎��B</summary>
        IgnoreNumber = 0x20000,
        /// <summary>�S�p���p�̈Ⴂ�𖳎��B</summary>
        IgnoreWide = 0x40000,
        /// <summary>�啶���������̈Ⴂ�𖳎��B</summary>
        IgnoreCase = 0x80000,
        /// <summary>�J�^�J�i�Ђ炪�Ȃ̈Ⴂ�𖳎��B</summary>
        IgnoreKana = 0x100000,
        /// <summary>���ׂĂ̖��������B</summary>
        IgnoreAll = IgnoreSpace | IgnoreNumber | IgnoreWide | IgnoreCase | IgnoreKana,

        /// <summary>����̔�r�I�v�V�����B</summary>
        Default = NumberAll | IgnoreSpace | IgnoreNumber | IgnoreWide | IgnoreCase,

        #endregion
    }

    /// <summary>���R���̔�r�@�\��񋟂��܂��B</summary>
    public class NaturalComparer : IComparer<string>, IComparer
    {
        #region Enum

        /// <summary>�����\�������̎�ށB</summary>
        private enum CharTypes : uint
        {
            /// <summary>�Ȃ��B</summary>
            None = 0x0,
            /// <summary>�A���r�A�����B</summary>
            Number = 0x1,
            /// <summary>ASCII���[�}�����B</summary>
            RomanNumber = 0x2,
            /// <summary>���{�ꃍ�[�}�����B</summary>
            JpRomanNumber = 0x4,
            /// <summary>���{��ې����B</summary>
            CircleNumber = 0x8,
            /// <summary>���{�ꊿ�����B</summary>
            KanjiNumber = 0x10,
        }

        #endregion

        #region Field

        /// <summary>�\�[�g�̕�����\�� int�B</summary>
        private int _order;
        /// <summary>���R���̔�r�I�v�V������\�� <see cref="NaturalComparerOptions"/>�B</summary>
        private NaturalComparerOptions _options;
        /// <summary>���O������\�� char[]�B</summary>
        private char[] _ignoreCharacter;
        /// <summary>��r�I�v�V������g�ݍ��킹�����O������\�� char[]�B</summary>
        private char[] _ignoreTable;

        #endregion

        #region Constructor

        /// <summary>�C���X�^���X�����������܂��B</summary>
        public NaturalComparer()
            : this(NaturalSortOrder.Ascending, NaturalComparerOptions.Default, new char[0])
        {
        }

        /// <summary>�C���X�^���X�����������܂��B</summary>
        /// <param name="order">�\�[�g������\�� <see cref="NaturalSortOrder"/>�B</param>
        public NaturalComparer(NaturalSortOrder order)
            : this(order, NaturalComparerOptions.Default, new char[0])
        {
        }

        /// <summary>�C���X�^���X�����������܂��B</summary>
        /// <param name="order">�\�[�g������\�� <see cref="NaturalSortOrder"/>�B</param>
        /// <param name="options">��r���@��\�� <see cref="NaturalComparerOptions"/>�B</param>
        public NaturalComparer(NaturalSortOrder order, NaturalComparerOptions options)
            : this(order, NaturalComparerOptions.Default, new char[0])
        {
        }

        /// <summary>�C���X�^���X�����������܂��B</summary>
        /// <param name="order">�\�[�g������\�� <see cref="NaturalSortOrder"/>�B</param>
        /// <param name="options">��r���@��\�� <see cref="NaturalComparerOptions"/>�B</param>
        /// <param name="ignoreCharacter">�������镶����\�� char[]�B</param>
        public NaturalComparer(NaturalSortOrder order, NaturalComparerOptions options, char[] ignoreCharacter)
        {
            this.SortOrder = order;
            _ignoreCharacter = ignoreCharacter;
            this.Options = options; // �Ō�Ɏ��s����K�v������
        }

        #endregion

        #region Property

        /// <summary>�\�[�g�������擾�܂��͐ݒ肵�܂��B</summary>
        public virtual NaturalSortOrder SortOrder
        {
            get
            {
                switch (_order)
                {
                case 1:
                    return NaturalSortOrder.Ascending;
                case -1:
                    return NaturalSortOrder.Descending;
                default:
                    return NaturalSortOrder.Ascending;
                }
            }
            set
            {
                switch (value)
                {
                case NaturalSortOrder.Ascending:
                    _order =  1;
                    break;
                case NaturalSortOrder.Descending:
                    _order =  -1;
                    break;
                default:
                    _order = 0;
                    break;
                }
            }
        }

        /// <summary>���R���̔�r�I�v�V�������擾�܂��͐ݒ肵�܂��B</summary>
        public virtual NaturalComparerOptions Options
        {
            get { return _options; }
            set
            {
                _options = value;
                if (this.IgnoreSpace)
                {
                    _ignoreTable = new char[_ignoreCharacter.Length + 3];
                    _ignoreCharacter.CopyTo(_ignoreTable, 0);
                    _ignoreTable[_ignoreTable.Length - 3] = ' ';
                    _ignoreTable[_ignoreTable.Length - 2] = '�@';
                    _ignoreTable[_ignoreTable.Length - 1] = '\t';
                }
                else
                {
                    _ignoreTable = _ignoreCharacter;
                }
            }
        }

        /// <summary>�󔒕����̑��݂𖳎����邩�ǂ������擾���܂��B</summary>
        protected virtual bool IgnoreSpace
        {
            get { return ((_options & NaturalComparerOptions.IgnoreSpace) == NaturalComparerOptions.IgnoreSpace); }
        }

        /// <summary>�����\���̈Ⴂ�𖳎����邩�ǂ������擾���܂��B</summary>
        protected virtual bool IgnoreNumber
        {
            get { return ((_options & NaturalComparerOptions.IgnoreNumber) == NaturalComparerOptions.IgnoreNumber); }
        }

        /// <summary>�S�p���p�̈Ⴂ�𖳎����邩�ǂ������擾���܂��B</summary>
        protected virtual bool IgnoreWide
        {
            get { return ((_options & NaturalComparerOptions.IgnoreWide) == NaturalComparerOptions.IgnoreWide); }
        }

        /// <summary>�啶���������̈Ⴂ�𖳎����邩�ǂ������擾���܂��B</summary>
        protected virtual bool IgnoreCase
        {
            get { return ((_options & NaturalComparerOptions.IgnoreCase) == NaturalComparerOptions.IgnoreCase); }
        }

        /// <summary>�J�^�J�i�Ђ炪�Ȃ̈Ⴂ�𖳎����邩�ǂ������擾���܂��B</summary>
        protected virtual bool IgnoreKana
        {
            get { return ((_options & NaturalComparerOptions.IgnoreKana) == NaturalComparerOptions.IgnoreKana); }
        }

        #endregion

        #region Method

        /// <summary>�I�u�W�F�N�g�̑召�֌W���r���܂��B</summary>
        /// <param name="s1">��r�Ώۂ̃I�u�W�F�N�g��\�� string�B</param>
        /// <param name="s2">��r�Ώۂ̃I�u�W�F�N�g��\�� string�B</param>
        /// <returns>
        /// <list type="table">
        ///   <item><term>0 ��菬����</term><description><paramref name="s1"/> �� <paramref name="s2"/> ��菬�����B</description></item>
        ///   <item><term>0</term><description><paramref name="s1"/> �� <paramref name="s2"/> �͓������B</description></item>
        ///   <item><term>0 ���傫��</term><description><paramref name="s1"/> �� <paramref name="s2"/> ���傫���B</description></item>
        /// </list>
        /// </returns>
        public int Compare(string s1, string s2)
        {
            return LocalCompare(s1, s2) * _order;
        }

        /// <summary>�I�u�W�F�N�g�̑召�֌W���r���܂��B</summary>
        /// <param name="s1">��r�Ώۂ̃I�u�W�F�N�g��\�� object�B</param>
        /// <param name="s2">��r�Ώۂ̃I�u�W�F�N�g��\�� object�B</param>
        /// <returns>
        /// <list type="table">
        ///   <item><term>0 ��菬����</term><description><paramref name="s1"/> �� <paramref name="s2"/> ��菬�����B</description></item>
        ///   <item><term>0</term><description><paramref name="s1"/> �� <paramref name="s2"/> �͓������B</description></item>
        ///   <item><term>0 ���傫��</term><description><paramref name="s1"/> �� <paramref name="s2"/> ���傫���B</description></item>
        /// </list>
        /// </returns>
        int IComparer.Compare(object s1, object s2)
        {
            return LocalCompare(s1 as string, s2 as string) * _order;
        }

        /// <summary>�I�u�W�F�N�g�̑召�֌W���r���܂��B</summary>
        /// <param name="s1">��r�Ώۂ̃I�u�W�F�N�g��\�� string�B</param>
        /// <param name="s2">��r�Ώۂ̃I�u�W�F�N�g��\�� string�B</param>
        /// <returns>
        /// <list type="table">
        ///   <item><term>0 ��菬����</term><description><paramref name="s1"/> �� <paramref name="s2"/> ��菬�����B</description></item>
        ///   <item><term>0</term><description><paramref name="s1"/> �� <paramref name="s2"/> �͓������B</description></item>
        ///   <item><term>0 ���傫��</term><description><paramref name="s1"/> �� <paramref name="s2"/> ���傫���B</description></item>
        /// </list>
        /// </returns>
        protected virtual int LocalCompare(string s1, string s2)
        {
            // �����ꂩ�� null �������͋󕶎��ł���Δ�r�I��
            if (string.IsNullOrEmpty(s1))
            {
                return string.IsNullOrEmpty(s2) ? 0 : -1;
            }
            else if (string.IsNullOrEmpty(s2))
            {
                return 1;
            }

            CharTypes filter = (CharTypes)(_options & NaturalComparerOptions.NumberAll);
            CharTypes t1 = CharTypes.None;
            CharTypes t2 = CharTypes.None;
            int p1 = 0;
            int p2 = 0;
            char c1 = char.MinValue;
            char c2 = char.MinValue;

            s1 = ConvertChar(s1);
            s2 = ConvertChar(s2);

            // ���O������ǂݔ�΂�
            if (_ignoreTable.Length > 0)
            {
                SkipIgnoreCharacter(s1, ref p1);
                SkipIgnoreCharacter(s1, ref p2);
            }

            while (p1 < s1.Length && p2 < s2.Length)
            {
                t1 = GetCharType(s1[p1], c1, t1) & filter;
                t2 = GetCharType(s2[p2], c2, t2) & filter;
                c1 = s1[p1];
                c2 = s2[p2];

                // �����Ƃ����炩�̐����̏ꍇ
                if ((this.IgnoreNumber || (this.IgnoreNumber == false && t1 == t2)) && t1 != CharTypes.None && t2 != CharTypes.None)
                {
                    int i1 = p1;
                    int i2 = p2;
                    long v1 = 0;
                    long v2 = 0;

                    bool success = GetNumber(s1, t1, ref i1, out v1) && GetNumber(s2, t2, ref i2, out v2);
                    if (success)
                    {
                        if (v1 < v2)
                        {
                            return -1;
                        }
                        else if (v1 > v2)
                        {
                            return 1;
                        }
                        p1 = i1;
                        p2 = i2;
                    }
                    else
                    {
                        int diff = CompareChar(s1[p1], s2[p2]);
                        if (diff != 0)
                        {
                            return diff;
                        }
                        p1++;
                        p2++;
                    }
                }
                // �����ꂩ�������̏ꍇ
                else if ((t1 != CharTypes.None || t2 != CharTypes.None) && t1 != CharTypes.RomanNumber && t2 != CharTypes.RomanNumber)
                {
                    return (t1 != CharTypes.None) ? 1 : -1;
                }
                // �����łȂ��ꍇ�͕����R�[�h���r����
                else
                {
                    int diff = CompareChar(s1[p1], s2[p2]);
                    if (diff != 0)
                    {
                        return diff;
                    }
                    p1++;
                    p2++;
                }

                // ���O������ǂݔ�΂�
                if (_ignoreTable.Length > 0)
                {
                    SkipIgnoreCharacter(s1, ref p1);
                    SkipIgnoreCharacter(s2, ref p2);
                }
            }

            // ���ʕ�������v���Ă���ꍇ�́A�c��̕����񒷂ő召�֌W�����߂�
            if (p1 >= s1.Length)
            {
                return (p2 >= s2.Length) ? 0 : -1;
            }
            else
            {
                return 1;
            }
        }

        /// <summary>���O������ǂݔ�΂��܂��B</summary>
        /// <param name="source">�Ώۂ̕������\�� string�B</param>
        /// <param name="pos">�J�n�ʒu��\�� int�B</param>
        private void SkipIgnoreCharacter(string source, ref int pos)
        {
            for (; pos < source.Length; pos++)
            {
                if (Array.IndexOf<char>(_ignoreTable, source[pos]) == -1)
                {
                    break;
                }
            }
        }

        /// <summary>�����̎�ނ��擾���܂��B</summary>
        /// <param name="c">�擾�Ώۂ̕�����\�� char�B</param>
        /// <param name="back">���O�̕�����\�� char�B</param>
        /// <param name="state">���O�̕����̎�ނ�\�� <see cref="CharTypes"/>�B</param>
        /// <returns>�擾���������̎�ނ�\�� <see cref="CharTypes"/>�B</returns>
        private CharTypes GetCharType(char c, char back, CharTypes state)
        {
            // ASCII�A���r�A���� (0�`9)
            if (c >= '0' && c <= '9')
            {
                return CharTypes.Number;
            }
            // ���{��A���r�A���� (�O�`�X)
            else if (c >= '�O' && c <= '�X')
            {
                return CharTypes.Number;
            }
            // ���{��ې��� (�@�`�S)
            else if (c >= '�@' && c <= '�S')
            {
                return CharTypes.CircleNumber;
            }
            // ASCII�p�啶�� (A�`Z)
            else if (c >= 'A' && c <= 'Z')
            {
                // ASCII���[�}���� (I,V,X,L,C,D,M)
                if (back < 'A' || back > 'Z')
                {
                    switch (c)
                    {
                    case 'I':
                    case 'V':
                    case 'X':
                    case 'L':
                    case 'C':
                    case 'D':
                    case 'M':
                        return CharTypes.RomanNumber;
                    }
                }
            }
            // ASCII�p������ (a�`z)
            else if (c >= 'a' && c <= 'z')
            {
                // ASCII���[�}���� (i,v,x,l,c,d,m)
                if ((back < 'A' || back > 'Z') && (back < 'a' || back > 'z'))
                {
                    switch (c)
                    {
                    case 'i':
                    case 'v':
                    case 'x':
                    case 'l':
                    case 'c':
                    case 'd':
                    case 'm':
                        return CharTypes.RomanNumber;
                    }
                }
            }
            // ���{��p�啶�� (�`�`�y)
            else if (c >= '�`' && c <= '�y')
            {
                // ���{�ꃍ�[�}���� (�h,�u,�w,�k,�b,�c,�l)
                if (back < '�`' || back > '�y')
                {
                    switch (c)
                    {
                    case '�h':
                    case '�u':
                    case '�w':
                    case '�k':
                    case '�b':
                    case '�c':
                    case '�l':
                        return CharTypes.RomanNumber;
                    }
                }
            }
            // ���{��p������ (���`��)
            else if (c >= '��' && c <= '��')
            {
                // ���{�ꃍ�[�}���� (�@,�D,�I,��,��,��,��)
                if ((back < '�`' || back > '�y') && (back < '��' || back > '��'))
                {
                    switch (c)
                    {
                    case '�@':
                    case '�D':
                    case '�I':
                    case '��':
                    case '��':
                    case '��':
                    case '��':
                        return CharTypes.RomanNumber;
                    }
                }
            }
            // ���[�}����
            else if (c >= 0x2160 && c <= 0x217F)
            {
                return CharTypes.JpRomanNumber;
            }
            else
            {
                // ���{�ꊿ����
                if (state == CharTypes.KanjiNumber)
                {
                    switch (c)
                    {
                    case '�Z':
                    case '��':
                    case '��':
                    case '�O':
                    case '�l':
                    case '��':
                    case '�Z':
                    case '��':
                    case '��':
                    case '��':
                    case '�\':
                    case '�S':
                    case '��':
                    case '��':
                    case '��':
                    case '��':
                    case '��':
                    case '��':
                    case '��':
                    case '�Q':
                    case '�E':
                        return CharTypes.KanjiNumber;
                    }
                }
                else
                {
                    switch (c)
                    {
                    case '�Z':
                    case '��':
                    case '��':
                    case '�O':
                    case '�l':
                    case '��':
                    case '�Z':
                    case '��':
                    case '��':
                    case '��':
                    case '�\':
                    case '�S':
                    case '��':
                    case '��':
                    case '��':
                    case '�Q':
                        return CharTypes.KanjiNumber;
                    }
                }
            }

            return CharTypes.None;
        }

        /// <summary>��r�I�v�V�����ɍ��킹�ĕ�����ϊ����܂��B</summary>
        /// <param name="source">�ϊ����镶����\�� string�B</param>
        /// <returns>�ϊ����ʂ̕�����\�� string�B</returns>
        private string ConvertChar(string source)
        {
            StringBuilder buffer = new StringBuilder(source);

            // �S�p���p�̈Ⴂ�𖳎�����
            if (this.IgnoreWide)
            {
                ConvertHalf(buffer);
            }

            // �啶���������̈Ⴂ�𖳎�����
            if (this.IgnoreCase)
            {
                ConvertUpperCase(buffer);
            }

            // �J�^�J�i�Ђ炪�Ȃ̈Ⴂ�𖳎�����
            if (this.IgnoreKana)
            {
                ConvertKatakana(buffer);
            }

            return buffer.ToString();
        }

        /// <summary>�S�p�𔼊p�֕ϊ����܂��B</summary>
        /// <param name="source">�ϊ����̕������\�� <see cref="StringBuilder"/>�B</param>
        private void ConvertHalf(StringBuilder source)
        {
            for (int i = 0; i < source.Length; i++)
            {
                if (source[i] >= '�I' && source[i] <= '�`')
                {
                    source[i] = (char)(source[i] - '�I' + '!');
                }
                else
                {
                    switch (source[i])
                    {
                    case '�A': source[i] = '�'; break;
                    case '�B': source[i] = '�'; break;
                    case '�q': source[i] = '<'; break;
                    case '�r': source[i] = '>'; break;
                    case '�s': source[i] = '<'; break;
                    case '�t': source[i] = '>'; break;
                    case '�u': source[i] = '�'; break;
                    case '�v': source[i] = '�'; break;
                    case '�w': source[i] = '�'; break;
                    case '�x': source[i] = '�'; break;
                    case '�y': source[i] = '['; break;
                    case '�z': source[i] = ']'; break;
                    case '�k': source[i] = '['; break;
                    case '�l': source[i] = ']'; break;
                    }
                }
            }
        }

        /// <summary>��������啶���֕ϊ����܂��B</summary>
        /// <param name="source">�ϊ����̕������\�� <see cref="StringBuilder"/>�B</param>
        private void ConvertUpperCase(StringBuilder source)
        {
            for (int i = 0; i < source.Length; i++)
            {
                if ((source[i] >= 'a' && source[i] <= 'z') && (source[i] >= '��' && source[i] <= '��'))
                {
                    source[i] = char.ToUpper(source[i], CultureInfo.InvariantCulture);
                }
            }
        }

        /// <summary>�Ђ炪�Ȃ��J�^�J�i�֕ϊ����܂��B</summary>
        /// <param name="source">�ϊ����̕������\�� <see cref="StringBuilder"/>�B</param>
        private void ConvertKatakana(StringBuilder source)
        {
            for (int i = 0; i < source.Length; i++)
            {
                if (source[i] >= '��' && source[i] <= '�U')
                {
                    source[i] = (char)(source[i] + '�@' - '��');
                }
                else if (source[i] >= '�' && source[i] <= '�')
                {
                    bool replaced = false;

                    if (i + 1 < source.Length)
                    {
                        replaced = true;

                        switch (source[i + 1])
                        {
                        case '�':
                            switch (source[i])
                            {
                            case '�': source[i] = '�K'; break;
                            case '�': source[i] = '�M'; break;
                            case '�': source[i] = '�O'; break;
                            case '�': source[i] = '�Q'; break;
                            case '�': source[i] = '�S'; break;
                            case '�': source[i] = '�U'; break;
                            case '�': source[i] = '�W'; break;
                            case '�': source[i] = '�Y'; break;
                            case '�': source[i] = '�['; break;
                            case '�': source[i] = '�]'; break;
                            case '�': source[i] = '�_'; break;
                            case '�': source[i] = '�a'; break;
                            case '�': source[i] = '�d'; break;
                            case '�': source[i] = '�f'; break;
                            case '�': source[i] = '�h'; break;
                            case '�': source[i] = '�o'; break;
                            case '�': source[i] = '�r'; break;
                            case '�': source[i] = '�u'; break;
                            case '�': source[i] = '�x'; break;
                            case '�': source[i] = '�{'; break;
                            case '�': source[i] = '��'; break;
                            default: replaced = false; break;
                            }
                            break;
                        case '�':
                            switch (source[i])
                            {
                            case '�': source[i] = '�p'; break;
                            case '�': source[i] = '�s'; break;
                            case '�': source[i] = '�v'; break;
                            case '�': source[i] = '�y'; break;
                            case '�': source[i] = '�|'; break;
                            default: replaced = false; break;
                            }
                            break;
                        default:
                            replaced = false;
                            break;
                        }

                        if (replaced)
                        {
                            source.Remove(i + 1, 1);
                        }
                    }

                    if (replaced == false)
                    {
                        switch (source[i])
                        {
                        case '�': source[i] = '��'; break;
                        case '�': source[i] = '�@'; break;
                        case '�': source[i] = '�B'; break;
                        case '�': source[i] = '�D'; break;
                        case '�': source[i] = '�F'; break;
                        case '�': source[i] = '�H'; break;
                        case '�': source[i] = '��'; break;
                        case '�': source[i] = '��'; break;
                        case '�': source[i] = '��'; break;
                        case '�': source[i] = '�b'; break;
                        case '�': source[i] = '�['; break;
                        case '�': source[i] = '�A'; break;
                        case '�': source[i] = '�C'; break;
                        case '�': source[i] = '�E'; break;
                        case '�': source[i] = '�G'; break;
                        case '�': source[i] = '�I'; break;
                        case '�': source[i] = '�J'; break;
                        case '�': source[i] = '�L'; break;
                        case '�': source[i] = '�N'; break;
                        case '�': source[i] = '�P'; break;
                        case '�': source[i] = '�R'; break;
                        case '�': source[i] = '�T'; break;
                        case '�': source[i] = '�V'; break;
                        case '�': source[i] = '�X'; break;
                        case '�': source[i] = '�Z'; break;
                        case '�': source[i] = '�\'; break;
                        case '�': source[i] = '�^'; break;
                        case '�': source[i] = '�`'; break;
                        case '�': source[i] = '�c'; break;
                        case '�': source[i] = '�e'; break;
                        case '�': source[i] = '�g'; break;
                        case '�': source[i] = '�i'; break;
                        case '�': source[i] = '�j'; break;
                        case '�': source[i] = '�k'; break;
                        case '�': source[i] = '�l'; break;
                        case '�': source[i] = '�m'; break;
                        case '�': source[i] = '�n'; break;
                        case '�': source[i] = '�q'; break;
                        case '�': source[i] = '�t'; break;
                        case '�': source[i] = '�w'; break;
                        case '�': source[i] = '�z'; break;
                        case '�': source[i] = '�}'; break;
                        case '�': source[i] = '�~'; break;
                        case '�': source[i] = '��'; break;
                        case '�': source[i] = '��'; break;
                        case '�': source[i] = '��'; break;
                        case '�': source[i] = '��'; break;
                        case '�': source[i] = '��'; break;
                        case '�': source[i] = '��'; break;
                        case '�': source[i] = '��'; break;
                        case '�': source[i] = '��'; break;
                        case '�': source[i] = '��'; break;
                        case '�': source[i] = '��'; break;
                        case '�': source[i] = '��'; break;
                        case '�': source[i] = '��'; break;
                        case '�': source[i] = '��'; break;
                        case '�': source[i] = '�J'; break;
                        case '�': source[i] = '�K'; break;
                        }
                    }
                }
            }
        }

        /// <summary>��������̐��l���擾���܂��B</summary>
        /// <param name="source">�Ώۂ̕������\�� string�B</param>
        /// <param name="type">�J�n�����̎�ނ�\�� <see cref="CharTypes"/>�B</param>
        /// <param name="pos">�J�n�ʒu��\�� int�B</param>
        /// <param name="value">�擾�������l��\�� long�B</param>
        /// <returns>���l���擾�o�����ꍇ�� <see langword="true"/>�A����ȊO�̏ꍇ�� <see langword="false"/>�B</returns>
        private bool GetNumber(string source, CharTypes type, ref int pos, out long value)
        {
            INumberComverter number = null;

            switch (type)
            {
            case CharTypes.Number: number = new NumberConverter(source[pos]); break;
            case CharTypes.RomanNumber: number = new RomanNumberConverter(source[pos]); break;
            case CharTypes.JpRomanNumber: number = new JpRomanNumberConverter(source[pos]); break;
            case CharTypes.CircleNumber: number = new CircleNumberConverter(source[pos]); break;
            case CharTypes.KanjiNumber: number = new KanjiNumberConverter(source[pos]); break;
            }

            for (int i = pos + 1; i < source.Length; i++)
            {
                if (number.AddChar(source[i]) == false)
                {
                    break;
                }
            }

            if (number.IsError == false)
            {
                value = number.Value;
                pos += number.Length;
            }
            else
            {
                value = 0;
            }

            return (number.IsError == false);
        }

        /// <summary>2�̕����R�[�h���r���܂��B</summary>
        /// <param name="c1">��r���镶����\�� char�B</param>
        /// <param name="c2">��r���镶����\�� char�B</param>
        /// <returns>2�̕����R�[�h�̑召�֌W��\�� int�B</returns>
        private int CompareChar(char c1, char c2)
        {
            // �O����A�㒆���̐�����l������
            string list = "��O������";
            int p1 = list.IndexOf(c1);
            int p2 = list.IndexOf(c2);

            if (p1 >= 0 && p2 >= 0)
            {
                return p1 - p2;
            }

            return StringComparer.CurrentCulture.Compare(c1.ToString(), c2.ToString());
        }

        #endregion

        #region INumberComverter

        /// <summary>�����𐔒l�֕ϊ�����@�\��񋟂��܂��B</summary>
        private interface INumberComverter
        {
            #region Property

            /// <summary>�G���[�������������ǂ������擾���܂��B</summary>
            bool IsError { get; }

            /// <summary>��������ϊ��������l���擾���܂��B</summary>
            long Value { get; }

            /// <summary>���l�S�̂̕��������擾���܂��B</summary>
            int Length { get; }

            #endregion

            #region Method

            /// <summary>������ǉ����܂��B</summary>
            /// <param name="number">������\�� char�B</param>
            /// <returns>�����Ƃ��Đ�������ꍇ�� <see langword="true"/>�A����ȊO�̏ꍇ�� <see langword="false"/>�B</returns>
            bool AddChar(char number);

            #endregion
        }

        #endregion

        #region NumberConverter

        /// <summary>�A���r�A�����𐔒l�֕ϊ�����@�\��񋟂��܂��B</summary>
        /// <remarks>ASCII�Ɠ��{��̍��݂͋����܂���B</remarks>
        private class NumberConverter : INumberComverter
        {
            #region Field

            /// <summary>���l�S�̂̒�����\�� int�B</summary>
            private int _length;
            /// <summary>�A���r�A������ 0 ��\�� char�B</summary>
            private char _numberZero;
            /// <summary>�A���r�A������ 9 ��\�� char�B</summary>
            private char _numberNine;
            /// <summary>�ϊ����ʂ̐��l��\�� long�B</summary>
            private long _value;
            /// <summary>�J���}��؂�̐��l���ǂ�����\�� bool�B</summary>
            private bool _isComma;
            /// <summary>�擪����A�������A���r�A�����̕�������\�� int�B</summary>
            private int _numberCount;
            /// <summary>�J���}��؂�ȍ~�̃A���r�A�����̕�������\�� int�B</summary>
            private int _commaLength;

            #endregion

            #region Constructor

            /// <summary>�C���X�^���X�����������܂��B</summary>
            /// <param name="number">�P�����ڂ̐�����\�� char�B</param>
            public NumberConverter(char number)
            {
                if (number >= '0' && number <= '9')
                {
                    _numberZero = '0';
                    _numberNine = '9';
                }
                else
                {
                    _numberZero = '�O';
                    _numberNine = '�X';
                }

                _length = 1;
                _value = number - _numberZero;
                _isComma = false;
            }

            #endregion

            #region Property

            /// <summary>�G���[�������������ǂ������擾���܂��B</summary>
            public bool IsError
            {
                get { return false; }
            }

            /// <summary>��������ϊ��������l���擾���܂��B</summary>
            public long Value
            {
                get { return _value; }
            }

            /// <summary>���l�S�̂̕��������擾���܂��B</summary>
            public int Length
            {
                get { return _length; }
            }

            #endregion

            #region Method

            /// <summary>������ǉ����܂��B</summary>
            /// <param name="number">������\�� char�B</param>
            /// <returns>�����Ƃ��Đ�������ꍇ�� <see langword="true"/>�A����ȊO�̏ꍇ�� <see langword="false"/>�B</returns>
            public bool AddChar(char number)
            {
                // 1�����ڂ̐����Ɠ���̃A���r�A�������ǂ���
                if (number >= _numberZero && number <= _numberNine)
                {
                    if (_isComma)
                    {
                        _commaLength++;
                        if (_commaLength > 3)
                        {
                            _length = _numberCount;
                            return false;
                        }
                    }
                    else
                    {
                        _numberCount++;
                    }

                    _value = _value * 10 + (number - _numberZero);
                }
                // 3����؂�̃J���}���ǂ���
                else if (_numberZero - number == 4)
                {
                    if (_isComma == false && _numberCount > 3)
                    {
                        return false;
                    }
                    _commaLength = 0;
                }
                // �A���r�A�����ȊO�̕���������������I��
                else
                {
                    return false;
                }

                _length++;
                return true;
            }

            #endregion
        }

        #endregion

        #region RomanNumberConverter

        /// <summary>�p���\���̃��[�}�����𐔒l�֕ϊ�����@�\��񋟂��܂��B</summary>
        /// <remarks>�啶���Ə������AASCII�Ɠ��{��̍��݂͋����܂���B</remarks>
        private class RomanNumberConverter : INumberComverter
        {
            #region Field

            /// <summary>���l�S�̂̒�����\�� int�B</summary>
            private int _length;
            /// <summary>�A���t�@�x�b�g�� A ��\�� char�B</summary>
            private char _alphaA;
            /// <summary>���m��̐�����\�� long�B</summary>
            private long _number;
            /// <summary>�ϊ����ʂ̐��l��\�� long�B</summary>
            private long _value;
            /// <summary>���ݒP�ʂ�\�� long�B</summary>
            private long _max;
            /// <summary>�G���[�������������ǂ�����\�� bool�B</summary>
            private bool _isError;

            #endregion

            #region Constructor

            /// <summary>�C���X�^���X�����������܂��B</summary>
            /// <param name="alpha">�P�����ڂ̉p����\�� char�B</param>
            public RomanNumberConverter(char alpha)
            {
                if (alpha >= 'A' && alpha <= 'Z')
                {
                    _alphaA = 'A';
                }
                else if (alpha >= 'a' && alpha <= 'z')
                {
                    _alphaA = 'a';
                }
                else if (alpha >= '�`' && alpha <= '�y')
                {
                    _alphaA = '�`';
                }
                else if (alpha >= '��' && alpha <= '��')
                {
                    _alphaA = '��';
                }

                _length = 1;
                _number = Parse(alpha);
                _max = _number;
            }

            #endregion

            #region Property

            /// <summary>�G���[�������������ǂ������擾���܂��B</summary>
            public bool IsError
            {
                get { return _isError; }
            }

            /// <summary>��������ϊ��������l���擾���܂��B</summary>
            public long Value
            {
                get { return _value + _number; }
            }

            /// <summary>���l�S�̂̕��������擾���܂��B</summary>
            public int Length
            {
                get { return _length; }
            }

            #endregion

            #region Method

            /// <summary>���[�}������ǉ����܂��B</summary>
            /// <param name="roman">���[�}������\�� char�B</param>
            /// <returns>�����Ƃ��Đ�������ꍇ�� <see langword="true"/>�A����ȊO�̏ꍇ�� <see langword="false"/>�B</returns>
            public bool AddChar(char roman)
            {
                long value = Parse(roman);

                // ���[�}�����ȊO�̕���������������I��
                if (value == 0)
                {
                    _isError = IsAlpha(roman);
                    return false;
                }
                // IV IX �Ȃǂ̌��Z���\�L
                else if (value > _max)
                {
                    long mag = value / _max;
                    if (mag == 5 || mag == 10)
                    {
                        _value += value - _number;
                        _number = 0;
                        _max = _max / 2;
                    }
                    else
                    {
                        _isError = IsAlpha(roman);
                        return false;
                    }
                }
                // VI XI �Ȃǉ��Z���\�L
                else if (value < _max)
                {
                    _value += _number;
                    _number = value;
                    _max = value;
                }
                // II XX �ȂǓ��������̌J��Ԃ�
                else
                {
                    _number += value;
                }

                _length++;
                return true;
            }

            /// <summary>���[�}�����𐔒l�֕ϊ����܂��B</summary>
            /// <param name="alpha">1 �����̃��[�}������\�� char�B</param>
            /// <returns>�ϊ���̐��l��\�� long�B</returns>
            protected long Parse(char alpha)
            {
                switch (alpha - _alphaA)
                {
                case 08: return 1;      // I
                case 21: return 5;      // V
                case 23: return 10;     // X
                case 11: return 50;     // L
                case 02: return 100;    // C
                case 03: return 500;    // D
                case 12: return 1000;   // M
                }

                return 0;
            }

            /// <summary>�w��̕������p�����ǂ����𔻒肵�܂��B</summary>
            /// <param name="alpha">�����Ώۂ̕�����\�� char�B</param>
            /// <returns>�w��̕������p���������ꍇ�� <see langword="true"/>�A����ȊO�̏ꍇ�� <see langword="false"/>�B</returns>
            protected bool IsAlpha(char alpha)
            {
                return ((alpha >= 'A' && alpha <= 'Z') || (alpha >= 'a' && alpha <= 'z') || (alpha >= '�`' && alpha <= '�y') || (alpha >= '��' && alpha <= '��'));
            }

            #endregion
        }

        #endregion

        #region JpRomanNumberConverter

        /// <summary>�S�p���[�}�����𐔒l�֕ϊ�����@�\��񋟂��܂��B</summary>
        private class JpRomanNumberConverter : INumberComverter
        {
            #region Field

            /// <summary>���l�S�̂̒�����\�� int�B</summary>
            private int _length;
            /// <summary>2�����ȏ�̑g�ݍ��킹���\���ǂ�����\�� bool�B</summary>
            private bool _isMultiChar;
            /// <summary>���[�}������ 1 ��\�� char�B</summary>
            private char _romanOne;
            /// <summary>���m��̐�����\�� long�B</summary>
            private long _number;
            /// <summary>�ϊ����ʂ̐��l��\�� long�B</summary>
            private long _value;
            /// <summary>���ݒP�ʂ�\�� long�B</summary>
            private long _max;

            #endregion

            #region Constructor

            /// <summary>�C���X�^���X�����������܂��B</summary>
            /// <param name="roman">�P�����ڂ̃��[�}������\�� char�B</param>
            public JpRomanNumberConverter(char roman)
            {
                _length = 1;

                // �S�p���[�}����(�T�`XII,L,C,D,M)
                if (roman >= 0x2160 && roman <= 0x216F)
                {
                    _romanOne = (char)0x2160;
                }
                // �S�p���[�}����(�@�`xii,l,c,d,m)
                else if (roman >= 0x2170 && roman <= 0x217F)
                {
                    _romanOne = (char)0x2170;
                }

                long value = Parse(roman);
                if (value == 0)
                {
                    _value = roman - _romanOne + 1;
                    _isMultiChar = false;
                }
                else
                {
                    _number = value;
                    _max = _number;
                    _isMultiChar = true;
                }
            }

            #endregion

            #region Property

            /// <summary>�G���[�������������ǂ������擾���܂��B</summary>
            public bool IsError
            {
                get { return false; }
            }

            /// <summary>��������ϊ��������l���擾���܂��B</summary>
            public long Value
            {
                get { return _value + _number; }
            }

            /// <summary>���l�S�̂̕��������擾���܂��B</summary>
            public int Length
            {
                get { return _length; }
            }

            #endregion

            #region Method

            /// <summary>������ǉ����܂��B</summary>
            /// <param name="roman">������\�� char�B</param>
            /// <returns>�����Ƃ��Đ�������ꍇ�� <see langword="true"/>�A����ȊO�̏ꍇ�� <see langword="false"/>�B</returns>
            public bool AddChar(char roman)
            {
                if (_isMultiChar == false)
                {
                    return false;
                }

                long value = Parse(roman);

                // ���[�}�����ȊO�̕���������������I��
                if (value == 0)
                {
                    return false;
                }
                // IV IX �Ȃǂ̌��Z���\�L
                else if (value > _max)
                {
                    long mag = value / _max;
                    if (mag == 5 || mag == 10)
                    {
                        _value += value - _number;
                        _number = 0;
                        _max = _max / 2;
                    }
                    else
                    {
                        return false;
                    }
                }
                // VI XI �Ȃǉ��Z���\�L
                else if (value < _max)
                {
                    _value += _number;
                    _number = value;
                    _max = value;
                }
                // II XX �ȂǓ��������̌J��Ԃ�
                else
                {
                    _number += value;
                }

                _length++;
                return true;
            }

            /// <summary>���[�}�����𐔒l�֕ϊ����܂��B</summary>
            /// <param name="roman">1 �����̃��[�}������\�� char�B</param>
            /// <returns>�ϊ���̐��l��\�� long�B</returns>
            protected long Parse(char roman)
            {
                switch (roman - _romanOne)
                {
                case 0x0: return 1;     // I
                case 0x4: return 5;     // V
                case 0x9: return 10;    // X
                case 0xC: return 50;    // L
                case 0xD: return 100;   // C
                case 0xE: return 500;   // D
                case 0xF: return 1000;  // M
                }

                return 0;
            }

            #endregion
        }

        #endregion

        #region CircleNumberConverter

        /// <summary>�ې����𐔒l�֕ϊ�����@�\��񋟂��܂��B</summary>
        private class CircleNumberConverter : INumberComverter
        {
            #region Field

            /// <summary>���݂̐�����\�� long�B</summary>
            private long _number;

            #endregion

            #region Constructor

            /// <summary>�C���X�^���X�����������܂��B</summary>
            /// <param name="number">�P�����ڂ̐�����\�� char�B</param>
            public CircleNumberConverter(char number)
            {
                // �@�`�S
                if (number >= 0x2460 && number <= 0x2473)
                {
                    _number = number - 0x2460 + 1;
                }
                // (1)�`(12)
                else if (number >= 0x2474 && number <= 0x2487)
                {
                    _number = number - 0x2474 + 1;
                }
                // 1.�`20.
                else if (number >= 0x2488 && number <= 0x249B)
                {
                    _number = number - 0x2488 + 1;
                }
                // �ەt��21�`35
                else if (number >= 0x3251 && number <= 0x325F)
                {
                    _number = number - 0x3251 + 21;
                }
                // {��}�`{�\}
                else if (number >= 0x3220 && number <= 0x3229)
                {
                    _number = number - 0x3220 + 1;
                }
                // �ەt����`�\
                else if (number >= 0x3280 && number <= 0x3289)
                {
                    _number = number - 0x3280 + 1;
                }
            }

            #endregion

            #region Property

            /// <summary>�G���[�������������ǂ������擾���܂��B</summary>
            public bool IsError
            {
                get { return false; }
            }

            /// <summary>��������ϊ��������l���擾���܂��B</summary>
            public long Value
            {
                get { return _number; }
            }

            /// <summary>���l�S�̂̕��������擾���܂��B</summary>
            public int Length
            {
                get { return 1; }
            }

            #endregion

            #region Method

            /// <summary>������ǉ����܂��B</summary>
            /// <param name="number">������\�� char�B</param>
            /// <returns>�����Ƃ��Đ�������ꍇ�� <see langword="true"/>�A����ȊO�̏ꍇ�� <see langword="false"/>�B</returns>
            public bool AddChar(char number)
            {
                return false;
            }

            #endregion
        }

        #endregion

        #region KanjiNumberConverter

        /// <summary>�������𐔒l�֕ϊ�����@�\��񋟂��܂��B</summary>
        private class KanjiNumberConverter : INumberComverter
        {
            #region Field

            /// <summary>���l�S�̂̒�����\�� int�B</summary>
            private int _length;
            /// <summary>�ʎ��L���@���ǂ�����\�� bool�B</summary>
            private bool _isNumeral;
            /// <summary>���O�̐�����\�� long�B</summary>
            private long _number;
            /// <summary>1�������̐��l��\�� long�B</summary>
            private long _value1;
            /// <summary>�ϊ����ʂ̐��l��\�� long�B</summary>
            private long _value2;
            /// <summary>1�������̌��ݒP�ʂ�\�� long�B</summary>
            private long _unit1;
            /// <summary>���l�S�̂̌��ݒP�ʂ�\�� long�B</summary>
            private long _unit2;

            #endregion

            #region Constructor

            /// <summary>�C���X�^���X�����������܂��B</summary>
            /// <param name="number">�P�����ڂ̐�����\�� char�B</param>
            public KanjiNumberConverter(char number)
            {
                _length = 1;
                long temp = Parse(number);
                if (temp < 10)
                {
                    _number = temp;
                    _unit1 = 9999;
                    _unit2 = 99999999999999999;
                }
                else
                {
                    _value1 = _unit1 = temp;
                }
            }

            #endregion

            #region Property

            /// <summary>�G���[�������������ǂ������擾���܂��B</summary>
            public bool IsError
            {
                get { return false; }
            }

            /// <summary>��������ϊ��������l���擾���܂��B</summary>
            public long Value
            {
                get { return _value2 + _value1 + _number; }
            }

            /// <summary>���l�S�̂̕��������擾���܂��B</summary>
            public int Length
            {
                get { return _length; }
            }

            #endregion

            #region Method

            /// <summary>������ǉ����܂��B</summary>
            /// <param name="kanji">������\�� char�B</param>
            /// <returns>�����Ƃ��Đ�������ꍇ�� <see langword="true"/>�A����ȊO�̏ꍇ�� <see langword="false"/>�B</returns>
            public bool AddChar(char kanji)
            {
                long value = Parse(kanji);

                // 2�����ڂ̓��e�ňʎ��L���@���ǂ��������肷��
                if (_length == 1)
                {
                    _isNumeral = (_number + _value1 < 10 && value < 10);
                    if (_isNumeral)
                    {
                        _value2 = _number;
                        _number = 0;
                    }
                }

                if (value < 0)
                {
                    return false;
                }

                if (_isNumeral)
                {
                    if (value > 10)
                    {
                        return false;
                    }

                    _value2 = _value2 * 10 + value;
                    _length++;
                    return true;
                }
                else
                {
                    if (value < 10)
                    {
                        // 9�ȉ��̊��������A��������G���[
                        if (_number > 0)
                        {
                            return false;
                        }

                        _number = value;
                    }
                    else if (value <= 1000)
                    {
                        // �O�����傫�ȒP�ʂ��o��������G���[
                        if (_unit1 <= value)
                        {
                            return false;
                        }

                        _value1 += _number * value;
                        _number = 0;
                        _unit1 = value;
                    }
                    else
                    {
                        // �O�����傫�ȒP�ʂ��o��������G���[
                        if (_unit2 <= value)
                        {
                            return false;
                        }

                        _value2 += (_value1 + _number) * value;
                        _value1 = _number = 0;
                        _unit1 = 9999;
                        _unit2 = value;
                    }

                    _length++;
                    return true;
                }
            }

            /// <summary>�������𐔒l�֕ϊ����܂��B</summary>
            /// <param name="kanji">��������\�� char�B</param>
            /// <returns>�ϊ���̐��l��\�� long�B</returns>
            protected long Parse(char kanji)
            {
                switch (kanji)
                {
                case '�Z': return 0;
                case '��': return 1;
                case '��': return 2;
                case '�O': return 3;
                case '�l': return 4;
                case '��': return 5;
                case '�Z': return 6;
                case '��': return 7;
                case '��': return 8;
                case '��': return 9;
                case '�\': return 10;
                case '�S': return 100;
                case '��': return 1000;
                case '��': return 10000;
                case '��': return 100000000;
                case '��': return 1000000000000;
                case '��': return 10000000000000000;
                case '��': return 0;
                case '��': return 1;
                case '��': return 2;
                case '�Q': return 3;
                case '�E': return 10;
                }

                return -1;
            }

            #endregion
        }

        #endregion
    }